<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Vauction.Utils.Paging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Search Results - <%=Consts.CompanyTitleName %></title>
    <% if (!((bool?)ViewData["HideAdvancedSearchBlock"] ?? false))
       {
           Html.Script("hashtable.js");
           Html.Script("validation.js");
       } %>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
        <div id="adv_search" class="control">
            <% Vauction.Models.CustomModels.AuctionFilterParams filterParams = (Vauction.Models.CustomModels.AuctionFilterParams)ViewData["filterParam"];%>
            <h2>Search Result</h2>
            <%-- <h4>
          <strong>Search by:</strong>            
          <%:(ViewData["HideAdvancedSearchBlock"]!=null) ? filterParams.ShortSimpleFilterString() : filterParams.ShortFilterString()%>
      </h4> --%>
            <%
                if (!((bool?)ViewData["HideAdvancedSearchBlock"] ?? false))
                    Html.RenderPartial("~/Views/Auction/_AdvancedSearch.ascx", filterParams, ViewData);
      %>
            <% Html.RenderAction((!Request.IsAuthenticated || AppHelper.CurrentUser == null) ? "pSearchUn" : "pSearch", "Home", new { method = ViewData["Method"], param = filterParams, page = filterParams.page });%>
        </div>
    </div>
</asp:Content>
