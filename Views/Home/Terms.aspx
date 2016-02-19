<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Terms &amp; Conditions - <%=Consts.CompanyTitleName%></title>
  <% SessionUser cuser = AppHelper.CurrentUser; 
    if (cuser!=null && cuser.IsAdminType) {%>
    <% Html.Script("general.js"); %>
    <script src="<%=ResolveUrl("~/public/tinymce/tinymce.min.js" ) %>" type="text/javascript"></script>
    <script src="<%=ResolveUrl("~/public/tiny_box.js")%>" type="text/javascript"></script>
  <%}%>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
<div class="col-md-9 col-sm-8">
    <div class="center_content">
      <% Html.RenderPartial("EditContent", new EditContentParams{Height = 500, Width=530, File="_Terms"}); %>
    </div>
</div>
</asp:Content>