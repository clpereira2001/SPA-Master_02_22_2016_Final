<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Pay for item(s) failed - <%=Consts.CompanyTitleName %></title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <h1 class="title">Pay for item(s) failed</h1>
    <p>Sorry, we were unable to authourize your credit card.</p>
    <p>
      <% if (ViewData["ErrorMessage"] != null){ %><strong>Error:</strong>&nbsp;&nbsp;<%=ViewData["ErrorMessage"] %><%} %> 
      </p> 
      <div class="back_link"><%=Html.ActionLink("Return to the Pay for Auction Items Page", "PayForItems", new { controller = "Account", action = "PayForItems" })%></div>    
  </div>
</asp:Content>