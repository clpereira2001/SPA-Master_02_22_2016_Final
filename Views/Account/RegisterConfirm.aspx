<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="registerHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Registration Confirmation- <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
      <h1 class="title">Registration Confirm</h1>        
      <p><b>Congratulations, you are now registered with our site.</b><br />
          We are sending a confirmation e-mail to the email address you provided.  
  Once you have received your confirmation message, you must activate your account by clicking the activation link in the email.  You will not be able to login until your account is activated. 
    <br />
    If you don't receive your confirmation link, please <%=Html.ActionLink("click here", "ResendConfirmationCode", new {controller="Account", action="ResendConfirmationCode"}, new {@style="font-weight:bold;color:navy" }).ToSslLink() %> to resend.
    <br /><br />
    Thank You.
    <br />
    <br />
    <%=AppHelper.GetSiteUrl() %>
  </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphScriptBottom" >
  <!-- Google Code for Registration Conversion Page --> 
  <script type="text/javascript">
    /* <![CDATA[ */
    var google_conversion_id = 1071640055;
    var google_conversion_language = "en";
    var google_conversion_format = "2";
    var google_conversion_color = "ffffff";
    var google_conversion_label = "Ph_lCJud1wIQ99v__gM"; var google_conversion_value = 7.00;
    /* ]]> */
  </script>
  <script type="text/javascript" src="//www.googleadservices.com/pagead/conversion.js"></script>
</asp:Content>