<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Email Notification Signup Complete - <%=ConfigurationManager.AppSettings["CompanyName"]%></title>
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="control congratutation">
	    <h2>Email Notification Signup Complete</h2>
	    
	    <div class="blue_box">
           <p><b>Congratulations, you are now signed up for our free email notifications.</b><br />
           We are sending a confirmation e-mail to you the email address you provided. Once you have received your confirmation message, you must activate your notifications by clicking the activation link in the email. You will not receive our notifications until you click the activation link.
<br />
Thank you,
<br />
<br />

<%=ConfigurationManager.AppSettings["CompanyName"]%>
          
 </p>        
        </div>
    </div>    
</asp:Content>
