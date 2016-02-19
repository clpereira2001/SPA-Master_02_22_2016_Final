<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<EventDetail>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title><%=Model.Title%> / Event Detailed / <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
    <% SessionUser cuser = AppHelper.CurrentUser; %>
    <div class="col-md-9 col-sm-8">
        <div class="center_content">
            <% if (Model != null)
               {
                   if (Model.IsCurrent)
                   {%>
            <p class="pay_notice">
                Do not bid unless you intend to pay. You are entering into a binding contract. 
             
                <br />
                All items must be paid in full within 2 days after the auction has ended.
         
            </p>
            <% if (DateTime.Now >= Model.DateEnd.Subtract(Model.DateEnd.TimeOfDay).AddDays(-3))
               { %>
            <div style="padding: 5px; color: #000; border: dotted 1px #000; margin: 5px; font-family: Arial, Helvetica, sans-serif; font-size: 13px;">Place your proxy bids now. System may experience slowdowns at the end of the auction.</div>
            <%} %>
            <br />
            <%} %>
            <div id="featured_title"><%= Model.Title%> event Featured items:</div>
            <% if (Model.IsCurrent)
               { %><br />
            All auctions end <b><%=Model.EndTime %></b><br />
            <%} %>
            <%Html.RenderPartial("~/Views/Auction/_AuctionGrid.ascx", ViewData["Lots"]); %>
            <%--<% Vauction.Models.CustomModels.AuctionFilterParams filterParams = (Vauction.Models.CustomModels.AuctionFilterParams)ViewData["filterParam"];
         bool userregforevent = ViewData["UserRegisterForEvent"] != null && Convert.ToBoolean(ViewData["UserRegisterForEvent"]);%>
      <% Html.RenderAction(cuser == null ? "pEventDetailedUn" : "pEventDetailed", "Auction", new { event_id = Model.ID, param = filterParams, page = filterParams.page, viewmode = filterParams.ViewMode, iscurrent = Model.IsCurrent, isregistered = userregforevent });%>--%>
            <%}
               else
               {%> There is no such event.<%} %>
        </div>
    </div>

</asp:Content>
