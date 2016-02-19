<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Email Alerts Unsubscribe Success - <%=Consts.CompanyTitleName%></title>
</asp:Content>
<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">    
    <div class="control congratutation">
	    <h2>Email Alerts Unsubscribe</h2>
	    <div class="blue_box">
          <p>
           Your e-mail address <%=(ViewData["Email"] as string) %> has been successfuly unsubscribed.           
          </p>
      </div>
    </div>    
</asp:Content>