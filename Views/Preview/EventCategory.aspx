<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IPagedList<AuctionShort>>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>

<%@ Import Namespace="Vauction.Utils.Paging" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <% EventCategoryDetail ecd = ViewData["CurrentCategory"] as EventCategoryDetail ?? new EventCategoryDetail(); %>
  <title>PREVIEW - Category <%=ecd.CategoryTitle%> detailed - <%=Consts.CompanyTitleName%></title>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
<%  EventDetail currentEvent = ViewData["CurrentEvent"] as EventDetail ?? new EventDetail();
    EventCategoryDetail currentCategory = ViewData["CurrentCategory"] as EventCategoryDetail ?? new EventCategoryDetail();
    %>
  <div class="center_content">	
    <p class="pay_notice">
	  Do not bid unless you intend to pay. You are entering into a binding contract. 
    <br />All items must be paid in full within 2 days after the auction has ended.	
  </p>  
    <div class="span-14" id="auction_title"><%=currentCategory.CategoryTitle%> in <%=currentEvent.Title%></div>
    <br class="clear" />
    <p>All auctions end <b><%=currentEvent.EndTime%></b></p>    
    <%Html.RenderPartial("~/Views/Auction/_AuctionGrid.ascx", ViewData.Model); %>
    </div>	
</asp:Content>