<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<bool>" %>
<asp:Content ID="registerHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Email confirmation was <%:Model?"successful" : "failed" %> - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <h1 class="title">Email confirmation was <%:Model?"successful" : "failed" %></h1>
    <p style="font-size:14px">
      <% if (Model) {%>
        <strong>Congratulations.</strong><br />  
        Your email was confirmed successfuly        
      <%} else { %>
        Your email wasn't confrimed.
      <%} %>
    </p>
    <%=Html.ActionLink("Return to Home page", "Index", new { controller = "Home", action = "Index" }, new { style="font-size:14px" })%> 
  </div>
</asp:Content>