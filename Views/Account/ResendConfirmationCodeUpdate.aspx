<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Resent the confirmation code - <%=Consts.CompanyTitleName %></title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div class="center_content">
	<h1 class="title">The confirmation code has been sent successfuly</h1>
    <br />
    <p>An e-mail with your confirmation information has been sent.</p>    
</div>    
</asp:Content>