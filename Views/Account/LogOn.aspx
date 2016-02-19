<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="loginHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Login - <%=Consts.CompanyTitleName %></title>
</asp:Content>

<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
   <%-- <div class="center_content">--%>
        <% Html.RenderPartial("_LogOn");%>
    <%--</div>--%>
</asp:Content>
