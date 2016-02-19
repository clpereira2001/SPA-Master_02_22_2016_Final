<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<AuctionDetail>" MasterPageFile="~/Views/Shared/SiteWithoutLeft.Master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
  <title><%=Model != null ? String.Format("{0} - {1} (LOT#{2} ENDS {3})", Model.LinkParams.Title, Consts.CompanyTitleName, Model.LinkParams.Lot, Model.EndTime) : String.Empty %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <% Html.RenderPartial("~/Views/Auction/_Test_BiddingPanel.ascx", Model); %>
</asp:Content>
