<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
  <% Html.Script("general.js"); %>
  <title>Consign Now - <%=Consts.CompanyTitleName %> </title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content left">
<%--      <h1 class="title">Thank you for your consignment inquiry.</h1>--%>
      <p>
          Your inquiry has been received and someone from our office will be reviewing the submitted information and contacting you within three business days.
      </p>
      <p>
          If you have any questions or problems in the meantime, please contact us via email (admin@seizedpropertyauctions.com) or phone our customer support line at <a class="aLogIn" style="cursor:pointer">1-800-735-2719</a> ext. 103 Monday through Friday 8:30am-4:30pm Eastern Time.
      </p>
  </div>
</asp:Content>
