<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Vauction.Utils.Paging.IPagedList<AuctionShort>>" %>
<%--<%=AppHelper.CacheLabel %>--%>
<%Html.RenderPartial("~/Views/Auction/_AuctionGrid.ascx", Model ); %>
<%--<%=Html.Hidden("cch_ParticalAuctionGrid", DateTime.Now.ToString()) %>--%>