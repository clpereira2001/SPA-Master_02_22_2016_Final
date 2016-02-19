<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="registerHead" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Registration success - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <h1 class="title">Registration success</h1>
    <p style="font-size:14px">
      <strong>Congratulations.</strong><br />  
      Your account has been activated and now you can <a class="aLogIn" style="font-weight:bold;text-decoration:underline" >Log In</a> using your user id and password.  Welcome to the Auction!
    </p>
     <%=Html.ActionLink("Return to Home page", "Index", new { controller = "Home", action = "Index" }, new { style="font-size:14px" })%> 
  </div>
</asp:Content>



<asp:Content ID="registerFooter" ContentPlaceHolderID="cphScriptBottom" runat="server">
<!-- Google Code for Exults - Registration Completed Conversion Page -->
<script type="text/javascript">
/* <![CDATA[ */
var google_conversion_id = 1071640055;
var google_conversion_language = "en";
var google_conversion_format = "3";
var google_conversion_color = "ffffff";
var google_conversion_label = "Be6lCMGrml0Q99v__gM";
var google_remarketing_only = false;
/* ]]> */
</script>
<script type="text/javascript" src="//www.googleadservices.com/pagead/conversion.js">
</script>
<noscript>
<div style="display:inline;">
<img height="1" width="1" style="border-style:none;" alt="" src="//www.googleadservices.com/pagead/conversion/1071640055/?label=Be6lCMGrml0Q99v__gM&amp;guid=ON&amp;script=0"/>
</div>
</noscript>
</asp:Content>
