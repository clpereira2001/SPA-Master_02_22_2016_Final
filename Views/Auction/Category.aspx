<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<EventDetail>" MasterPageFile="~/Views/Shared/Site.Master" %>

<%@ Import Namespace="Vauction.Utils.Helpers" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <%
        CategoryMapDetail cat = ViewData["CurrentCategory"] as CategoryMapDetail;
        string title = (cat != null) ? String.Format("{0}{1} / {2} / {3}", cat.CategoryTitle, String.IsNullOrEmpty(cat.CategoryDescription) ? String.Empty : " - " + cat.CategoryDescription, Model.Title, Consts.CompanyTitleName) : Consts.CompanyTitleName;
  %>
    <title><%=title %></title>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">

    <% CategoryMapDetail currentCategory = ViewData["CurrentCategory"] as CategoryMapDetail; %>
    <div class="col-md-9 col-sm-8">
        <div class="center_content">
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
            <div style="padding: 5px; color: #000; border: dotted 1px #000; margin: 5px; font-family: Arial, Helvetica, sans-serif; font-size: 13px;">Place your proxy bids now. System may experience slowdowns at the end of the auction.</div>
            <%} %>
            <br />
            <%} %>

            <div class="span-14" id="auction_title"><%=currentCategory.CategoryTitle%> in <%=Model.Title%></div>
            <% if (Model.IsCurrent)
               { %><br />
            All auctions end <b><%=Model.EndTime %></b><br />
            <%} %>
            <%Html.RenderPartial("~/Views/Auction/_AuctionGrid.ascx", ViewData["Lots"]); %>
            <%--<% CategoryFilterParams filterParams = ViewData["filterParam"] as CategoryFilterParams;
       bool userregforevent = ViewData["UserRegisterForEvent"] != null && Convert.ToBoolean(ViewData["UserRegisterForEvent"]);
       Html.RenderAction(cuser == null ? "pCategoryUn" : "pCategory", "Auction", new { category_id = currentCategory.CategoryMap_ID, event_id = Model.ID, param = filterParams, page = filterParams.page, viewmode = filterParams.ViewMode, iscurrent = Model.IsCurrent, isregistered = userregforevent });%>--%>
            <% }
               else
               {%>No auctions found<% } %>
        </div>
    </div>
</asp:Content>
