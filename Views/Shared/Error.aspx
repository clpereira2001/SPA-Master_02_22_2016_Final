<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>

<asp:Content ID="hdr" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Error - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="errorContent" ContentPlaceHolderID="MainContent" runat="server">
  <br class="clear" />
  <div style="padding:5px;color:#800000;border:dotted 1px #800000; margin:5px; font-family: Arial, Helvetica, sans-serif;font-size: 14px;"><%=ViewData["Title"]!=null?ViewData["Title"]:"Sorry, an error occurred while processing your request." %></div>
</asp:Content>
