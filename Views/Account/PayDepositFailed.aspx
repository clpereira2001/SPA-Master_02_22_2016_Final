<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<int>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Pay for deposit failed - <%=Consts.CompanyTitleName %></title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <div class="col-md-9 col-sm-8">
     <p class="text-danger text-md">Pay for deposit failed.</p>
    <p><%:Model == 0 ? String.Empty : "Sorry, we were unable to authourize your credit card."%></p>
    <p><% if (ViewData["ErrorMessage"] != null){ %><strong>Error:</strong>&nbsp;&nbsp;<%=ViewData["ErrorMessage"] %><%} %></p>
    <div style='width:100%;text-align:right'>    
      <%=Html.ActionLink("Try Again", "PayDeposit", "Account").ToSslLink() %>
    </div>
  </div>
</asp:Content>


			 