<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<h2>Test</h2>
  
  SessionID: <%=Session.SessionID %>
  VIdentity: <%=ViewData["VIdentity"] %>
  
  <% foreach (var header in Request.Headers)
     {%>
    <%=header %> - <%=Request.Headers[header.ToString()] %><br />
     <%} %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="subMenu" runat="server">
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="rightCategories" runat="server">
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="cphScriptBottom" runat="server">
</asp:Content>
