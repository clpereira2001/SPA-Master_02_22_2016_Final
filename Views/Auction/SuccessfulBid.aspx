<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AuctionDetail>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Successful bid - <%=Consts.CompanyTitleName %></title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="center_content">    
<%--    <h1 class="title">Congratulations, <b><%=AppHelper.CurrentUser.Login %></b>!</h1>--%>
    <p>
      You are currently the high bidder for <b>Lot#<%=Model.LinkParams.Lot%>
        <%= Html.ActionLink(Model.LinkParams.Title, "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Model.LinkParams.ID, evnt=Model.LinkParams.EventUrl, cat=Model.LinkParams.CategoryUrl, lot=Model.LinkParams.LotTitleUrl })%></b>.
      Should you be outbid or win the lot, you will be notified via e-mail.
    </p>
    <center>
      <%= Html.ActionLink("Back to Lot", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Model.LinkParams.ID, evnt = Model.LinkParams.EventUrl, cat = Model.LinkParams.CategoryUrl, lot = Model.LinkParams.LotTitleUrl })%>
      |
      <%= Html.ActionLink("Back to Event", "EventDetailed", new { controller = "Auction", action = "EventDetailed", id = Model.LinkParams.Event_ID, evnt=Model.LinkParams.EventUrl })%>
    </center>
  </div>
</asp:Content>
