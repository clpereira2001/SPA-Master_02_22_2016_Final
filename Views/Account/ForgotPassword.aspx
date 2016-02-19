<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ForgotPassword>" %>
<%@ Import Namespace="Vauction.Utils.Html" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Forgot Password - <%=Consts.CompanyTitleName%></title>
  <% Html.Script("hashtable.js"); %>
  <% Html.Script("validation.js"); %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 <div id="registration" class="control">
<%--	<h2>Forgot Password</h2>--%>
    <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/ForgotPassword">
      <%=Html.AntiForgeryToken() %>
      <fieldset class="blue_box">    
        <p class="left" style="color:#337ab7;">Enter your <i>registered</i> e-mail address below.</p>
        <p style="height:30px;">
          <label for="email">E-mail address:</label><em>*</em>
          <%= Html.TextBox("Email", ViewData.Model.Email, new { @size = "40" })%><br />
          <%= Html.ValidationMessageArea("Email")%>
        </p>
        <p style="text-align: center">
          <br /><%= Html.SubmitWithClientValidationForgotPW("Send Password")%>
        </p>
      </fieldset> 
    </form>
  </div>
</asp:Content>