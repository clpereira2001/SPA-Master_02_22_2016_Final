<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="registerHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Registration confirmation failed - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
        <p>Registration confirmation failed. Try again.</p>
    </div>
</asp:Content>
