<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Forgot Password - <%=Consts.CompanyTitleName%></title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
<%--	  <h1 class="title">Forgot Password</h1><br />--%>
    <p class="text-primary">An e-mail with registration information has been sent.</p>
<%--    An e-mail with your registration information has been sent.--%>
<%--    <br />--%>
    <%=Html.ActionLink("Return to Home page", "Index", new { controller = "Home", action = "Index" }, new { style="font-size:14px" })%> 
  </div>    
</asp:Content>