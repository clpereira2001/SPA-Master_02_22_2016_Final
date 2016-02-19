<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
  <title><%=ViewData["title"] %> - <%=ConfigurationManager.AppSettings["CompanyName"]%></title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content" style='margin-left:-20px'>
		<%=ViewData["html_email"]%>
  </div>
  <div class="back_link">
    <%=Html.ActionLink("Return to Home page", "Index", new {controller="Home", action="Index"}).ToNonSslLink() %>
  </div>
</asp:Content>