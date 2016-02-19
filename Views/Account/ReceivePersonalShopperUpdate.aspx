<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IPagedList<AuctionShort>>" %>
<%@ Import Namespace="Vauction.Utils.Helpers" %>
<%@ Import Namespace="Vauction.Utils.Paging" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Personal Shopper Result - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">    
  <div class="center_content">    
    <h1 class="title">Personal Shopper Result</h1>    
    <% if (Model.Count == 0)
          Response.Write("There are not items that matching your Personal Shopper criteria.");
        else
        {
          Response.Write("Listed below are items matching your Personal Shopper criteria:<br />");
          Html.RenderPartial("~/Views/Auction/_AuctionGrid.ascx", ViewData.Model);
        }
    %>
    <br class="clear" />
    <div class="back_link"><%=Html.ActionLink("Return to My Account Page", "MyAccount", new {controller="Account", action="MyAccount"}) %></div>
  </div>
</asp:Content>