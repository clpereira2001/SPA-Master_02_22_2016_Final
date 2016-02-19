<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Register for the event - <%=Consts.CompanyTitleName %></title> 
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <h1 class="title">Registering for the event</h1>
    You have successfully registered for the event!&nbsp;            
    <% if (ViewData["auction_id"] != null)
       { %>
    <%= Html.ActionLink("Back to the lot", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Convert.ToInt32(ViewData["auction_id"]) }, null).ToNonSslLink() %>
    <%} %>
  </div>
</asp:Content>