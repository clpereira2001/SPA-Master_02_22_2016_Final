<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Advanced Search - <%=ConfigurationManager.AppSettings["CompanyName"]%></title>
    <% Html.Script("hashtable.js"); %>
    <% Html.Script("validation.js"); %>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
        <div id="adv_search" class="control">
<%--            <h2>Advanced Search</h2>--%>
            <%= Html.Encode(ViewData["Message"]) %>
            <% Html.RenderPartial("~/Views/Auction/_AdvancedSearch.ascx", new Vauction.Models.CustomModels.AuctionFilterParams(), ViewData); %>
        </div>
    </div>
</asp:Content>
