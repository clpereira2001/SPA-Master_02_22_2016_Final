<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<EventDetail>" MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <%
        EventCategoryDetail evCat = ViewData["CurrentCategory"] as EventCategoryDetail;
        string title = (evCat != null) ? String.Format("{0}{1} / {2} / {3}", evCat.CategoryTitle, String.IsNullOrEmpty(evCat.CategoryDescription) ? String.Empty : " - " + evCat.CategoryDescription, Model.Title, Consts.CompanyTitleName) : Consts.CompanyTitleName;
  %>
    <title><%=title %></title>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
        <% EventCategoryDetail currentCategory = ViewData["CurrentCategory"] as EventCategoryDetail; %>
        <div class="row">
            <% if ((currentCategory != null))
               {%>
            <% if (Model.IsCurrent)
               { %>
            <p class="pay_notice">
                Do not bid unless you intend to pay. You are entering into a binding contract. 
   
                <br />
                All items must be paid in full within 2 days after the auction has ended.	
 
            </p>
            <% if (DateTime.Now >= Model.DateEnd.Subtract(Model.DateEnd.TimeOfDay).AddDays(-3))
               { %>
            <div >Place your proxy bids now. System may experience slowdowns at the end of the auction.</div>
            <%} %>
            <br />
            <%} %>
            <div id="auction_title"><%=currentCategory.CategoryTitle%> in <%=Model.Title%></div>
            <% if (Model.IsCurrent)
               { %><br />
            All auctions end <b><%=Model.EndTime %></b><br />
            <%} %>

            <%Html.RenderPartial("~/Views/Auction/_AuctionGrid.ascx", ViewData["Lots"]); %>
            <%--<% CategoryFilterParams filterParams = ViewData["filterParam"] as CategoryFilterParams;
       bool userregforevent = ViewData["UserRegisterForEvent"] != null && Convert.ToBoolean(ViewData["UserRegisterForEvent"]);
       Html.RenderAction(cuser == null ? "pEventCategoryUn" : "pEventCategory", "Auction", new { eventcategory_id = currentCategory.EventCategory_ID, event_id = Model.ID, param = filterParams, page = filterParams.page, viewmode = filterParams.ViewMode, iscurrent = Model.IsCurrent, isregistered = userregforevent });%>    --%>
            <% }
               else
               {%>No auctions found<% } %>
        </div>
    </div>
</asp:Content>
