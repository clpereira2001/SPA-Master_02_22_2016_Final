<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Pay for item failed - <%=Consts.CompanyTitleName %></title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class="col-md-9 col-sm-8">
   <p class="text-danger text-md">Pay for item failed.</p>
    <p>Sorry, we were unable to authourize your credit card.</p>
    <p>
      <% if (ViewData["ErrorMessage"] != null){ %><strong>Error:</strong>&nbsp;&nbsp;<%=ViewData["ErrorMessage"] %><%} %> 
      </p> 
      <div class="back_link">
        <% Dow dow = (ViewData["auction_id"] != null ? Session["DOW_" + (ViewData["auction_id"]).ToString()] : Session["DOW"]) as Dow;
          if (dow!=null){ %>
        <%=(!dow.IsDOW) ? Html.ActionLink(String.Format("Return to the LOT#{0}", dow.LinkParams.Lot.ToString()), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = dow.LinkParams.ID, evnt=dow.LinkParams.EventUrl, cat=dow.LinkParams.CategoryUrl, lot=dow.LinkParams.LotTitleUrl }) : Html.ActionLink("Return to the Deal of the Week", "DealOfTheWeek", new { controller = "Auction", action = "DealOfTheWeek", id = dow.LinkParams.ID })%>
        <%} %>
      </div>    
  </div>
</asp:Content>


				
				 
 