<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<AuctionShort>>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<%
    SessionUser cuser = AppHelper.CurrentUser;
    bool isUserRegisteredForEvent = (cuser != null && ViewData["IsRegisteredForEvent"] == null) || (Convert.ToBoolean(ViewData["IsRegisteredForEvent"]) && ViewData["IsRegisteredForEvent"] != null);

    if (!Model.Any())
    {%>
<div class="grid-col-block list-col-view">
    
     
         
            <div>&nbsp;</div>
    <% }
    else
    {%>
    <div id="item_view" class="grid-col product-box list-view">

       
            <%                 
        bool isDemo = false; // (ViewData["DEMO_MODE"] != null) ? Convert.ToBoolean(ViewData["DEMO_MODE"]) : false;
        int featuredCount = 0;
        string address, strPrice;
        bool line = true;
        foreach (AuctionShort item in Model)
        {
            if (item.IsFeatured) featuredCount++;
            else if (featuredCount != 0)
            {
                Response.Write("<tr><td colspan='3' style='background-color:#888; height:3px; font-size:1px;padding:0'>&nbsp;</td></tr>");
                featuredCount = 0;
            }
            strPrice = (Consts.IsShownOpenBidOne) ? (!isUserRegisteredForEvent ? "$1.00" : item.Price.GetCurrency()) : item.Price.GetCurrency();
            %>

            <div class="thumbnail">


                <% address = AppHelper.AuctionImage(item.LinkParams.ID, item.ThumbnailPath);
                   if (System.IO.File.Exists(Server.MapPath(address)))
                   {
                       if (item.IsClickable || isDemo || item.IsAccessable)
                       {%>
                <a href="<%= Url.Action ("AuctionDetail", (isDemo?"Preview":"Auction"), new CollectParameters((isDemo?"Preview":"Auction"), "AuctionDetail", item.LinkParams.ID, item.LinkParams.EventUrl, item.LinkParams.CategoryUrl, item.LinkParams.LotTitleUrl).Collect("page", "ViewMode"))%>" title="<%=item.LinkParams.LotTitle %>">
                    <%} %>
                    <img src="<%=AppHelper.CompressImagePath(address)%>" alt="" />
                    <%if (item.IsClickable || isDemo || item.IsAccessable)
                      {%>
                </a>
                <%}
                   }
                   else Response.Write("&nbsp;"); 
                %>

                <div class="caption">
                    <div class="list-visible">
                        <%=item.IsBold?"<b>":String.Empty %>
                        <%=(!item.IsClickable && !isDemo && !item.IsAccessable) ? item.LinkParams.Title : Html.ActionLink(item.LinkParams.Title, "AuctionDetail", new { controller = (isDemo ? "Preview" : "Auction"), action = "AuctionDetail", id = item.LinkParams.ID, evnt = item.LinkParams.EventUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.LotTitleUrl }).ToHtmlString() %>
                        <%=item.IsBold?"</b>":String.Empty %>
                    </div>
                </div>
                <div class="list-price" id='tbl_res_row_<%=item.LinkParams.ID %>'>
                    <%=item.IsBold?"<b>":String.Empty %>
                    <p class="text-uppercase">
                        Opening bid
                        <br>
                        <span id="cv_cb_<%=item.LinkParams.ID %>" class="text-primary text-weight-700 text-md"><%= (isUserRegisteredForEvent && item.IsUserRegisteredForEvent) ? ((!item.HasBid) ? item.Price.GetCurrency() : item.CurrentBid) : strPrice %></span>
                        <%=item.IsBold?"</b>":String.Empty %>
                    </p>

                    <p>
                        <% if (item.IsClickable || isDemo || item.IsAccessable)
                           {%>
                        <button id="add_item" type="button" class="btn btn-info btn-square text-uppercase" onclick="javascript:location='<%= Url.Action ("AuctionDetail", (isDemo?"Preview":"Auction"), new CollectParameters((isDemo?"Preview":"Auction"), "AuctionDetail", item.LinkParams.ID, item.LinkParams.EventUrl, item.LinkParams.CategoryUrl, item.LinkParams.LotTitleUrl).Collect("page", "ViewMode"))%>'">

                            <span><%= (cuser != null && (item.StartDate<=DateTime.Now&&DateTime.Now<=item.EndDate)) ? "Bid Now" : "Preview"%></span>
                        </button>
                        <%} %>
                    </p>
                </div>
            </div>

            <%--   <tr>

                <td>&nbsp;</td>
                <td style='text-align: center'>
                    <% if (item.IsClickable || isDemo || item.IsAccessable)
                       {%>
                    <button id="add_item" type="button" class="btn btn-danger btn-lg" onclick="javascript:location='<%= Url.Action ("AuctionDetail", (isDemo?"Preview":"Auction"), new CollectParameters((isDemo?"Preview":"Auction"), "AuctionDetail", item.LinkParams.ID, item.LinkParams.EventUrl, item.LinkParams.CategoryUrl, item.LinkParams.LotTitleUrl).Collect("page", "ViewMode"))%>'">

                        <span><%= (cuser != null && (item.StartDate<=DateTime.Now&&DateTime.Now<=item.EndDate)) ? "Bid Now" : "Preview"%></span>
                    </button>
                    <%} %>
                </td>
            </tr>--%>
            <% line = !line; %>
            <%}%>
        </div>
    </div>



<%} %>
