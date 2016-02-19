<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Auctions participated - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% Event evnt = ViewData["Event"] as Event; %>
    <div class="col-md-9 col-sm-8">
        <div class="center_content">
            <div class="page_title"><%= Html.ActionLink("Past Auction Bidding History", "PastAuction", "Account", new { controller = "Account", action = "PastAuction" }, new { @style = "font-size:16px" })%> > <span style="font-size: 14px; font-weight: bold"><%=evnt.Title %></span></div>
            <% if (evnt.DateEnd.AddMinutes(25) > DateTime.Now)
               {
                   Response.Write("Auction has ended. Results will be available in approximately 20 to 30 minutes.");
               }
               else
               {%>
            <% Vauction.Models.CustomModels.AuctionFilterParams filterParams = ViewData["filterParam"] as Vauction.Models.CustomModels.AuctionFilterParams;%>
            <% Html.RenderAction("pAuctionsParticipated", "Account", new { event_id = evnt.ID, user_id = AppHelper.CurrentUser.ID, param = filterParams, page = filterParams.page });%>
            <%} %>
            <div class="back_link">
                <%=Html.ActionLink("Return to the My Account page", "MyAccount", new {controller="Account", action="MyAccount"}) %>
            </div>
        </div>
    </div>
</asp:Content>
