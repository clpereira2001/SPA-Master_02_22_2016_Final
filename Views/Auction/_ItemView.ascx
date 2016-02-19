<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AuctionShort>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<% AuctionShort categoryItem = ViewData.Model as AuctionShort;
   bool HidePreview = (ViewData["HidePreview"] == null);
   bool isDemo = false; // (ViewData["DEMO_MODE"] != null) ? Convert.ToBoolean(ViewData["DEMO_MODE"]) : false;
   SessionUser cuser = AppHelper.CurrentUser;
   bool isUserRegisteredForEvent = cuser != null && categoryItem.IsUserRegisteredForEvent;
   string strPrice = (Consts.IsShownOpenBidOne) ? (!isUserRegisteredForEvent ? "$1.00" : categoryItem.Price.GetCurrency()) : categoryItem.Price.GetCurrency();
%>
<div class="grid-col product-box">
    <div class="thumbnail" style="max-height: 900px!important">
        <% string address = AppHelper.AuctionImage(categoryItem.LinkParams.ID, categoryItem.ThumbnailPath);
           if (System.IO.File.Exists(Server.MapPath(address)))
           {
               if (categoryItem.IsClickable || isDemo || categoryItem.IsAccessable)
               {
        %>
        <a href="<%= Url.Action("AuctionDetail", (isDemo?"Preview":"Auction"), new CollectParameters((isDemo?"Preview":"Auction"), "AuctionDetail", categoryItem.LinkParams.ID, categoryItem.LinkParams.EventUrl, categoryItem.LinkParams.CategoryUrl, categoryItem.LinkParams.LotTitleUrl).Collect("page", "ViewMode"))%>" title="<%=categoryItem.LinkParams.LotTitle %>">
            <%} %>
            <img src="<%=AppHelper.CompressImagePath(address) %>" style="max-height: 100px!important" alt="" />
            <% if (categoryItem.IsClickable || isDemo || categoryItem.IsAccessable)
               { %>
        </a>
        <%}
           }%>
        <%else
           {%>
        <img src="../../public/images/no_image.gif" style="max-height: 100px!important" alt="" />
        </a>
        <% }
        %>
        <div class="caption">
            <div class="list-visible">
                <p class="title-product">
                    <%=(!categoryItem.IsClickable && !isDemo && !categoryItem.IsAccessable) ? categoryItem.LinkParams.Title : Html.ActionLink(categoryItem.LinkParams.Title, "AuctionDetail", new { controller = (isDemo ? "Preview" : "Auction"), action="AuctionDetail", id=categoryItem.LinkParams.ID, evnt=categoryItem.LinkParams.EventUrl, cat=categoryItem.LinkParams.CategoryUrl, lot=categoryItem.LinkParams.LotTitleUrl }).ToHtmlString()%>               
                </p>            
            </div>
            <div class="list-price">
                <p class="text-uppercase">
                    <%=((isUserRegisteredForEvent && categoryItem.HasBid) ? "Current" : "Opening")%> Bid
                    <span class="text-primary text-weight-700 text-md" id="cv_cb_<%=categoryItem.LinkParams.ID %>"><%=(categoryItem.HasBid && categoryItem.CurrentBid_2.HasValue && isUserRegisteredForEvent) ? " Range:  " : ": "%><%= (isUserRegisteredForEvent) ? (!categoryItem.HasBid ? categoryItem.Price.GetCurrency(): categoryItem.CurrentBid) : strPrice%></span>
                </p>
                <p>
                    <button id="add_item" type="button" class="btn btn-info btn-square text-uppercase" onclick="javascript:location='<%= Url.Action ("AuctionDetail", (isDemo?"Preview":"Auction"), new CollectParameters((isDemo?"Preview":"Auction"), "AuctionDetail", categoryItem.LinkParams.ID, categoryItem.LinkParams.EventUrl, categoryItem.LinkParams.CategoryUrl, categoryItem.LinkParams.LotTitleUrl).Collect("page", "ViewMode"))%>'">
                        <span><%=(cuser != null && (categoryItem.StartDate <= DateTime.Now && DateTime.Now < categoryItem.EndDate)) ? "Bid Now" : "Preview"%></span>
                    </button>
                </p>
                <%=(categoryItem.IsBold)? "</b>":String.Empty %>
            </div>

        </div>
    </div>
</div>




























