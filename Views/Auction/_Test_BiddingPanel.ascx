<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AuctionDetail>" %>
          
<% using (Html.BeginForm("PlaceBidTest", "Auction", FormMethod.Post)){ %>
  <%=Html.Hidden("id", Model!=null? Model.LinkParams.ID : -1) %>
  <input type="submit" value="Place Bid" id="btnPlaceBid" />
<%} %>