<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Vauction.Models.RegisterInfo>" %>

<%@ Import Namespace="Vauction.Utils.Html" %>

<asp:Content ID="registerHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Personal Information updated - <%=Consts.CompanyTitleName %></title>    
</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="center_content">
	<h1 class="title">Personal Information</h1>
    <p style="font-size:14px">Your account has been updated.</p>
    <%=Html.ActionLink("Return to Home page", "Index", new { controller = "Home", action = "Index" }, new { style="font-size:14px" })%>  
</div>    
</asp:Content>