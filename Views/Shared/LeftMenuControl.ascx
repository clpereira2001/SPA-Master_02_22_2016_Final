<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>

<% SessionUser cuser = AppHelper.CurrentUser;
   Auction DOW = ViewData["DOW"] as Auction;
   var name = Html.ViewContext.View;

   var currentController = ViewContext.RouteData.Values["controller"] as string;

   var currentControllerAccount = "" as string;
   if (currentController == "Account")
   {
       currentControllerAccount = currentController;
   }
   var currentAction = ViewContext.RouteData.Values["action"] as string;

   bool isDemo = false; // (ViewData["DEMO_MODE"] != null) ? Convert.ToBoolean(ViewData["DEMO_MODE"]) : false;
   EventDetail currentEvent = ViewData["CurrentEvent"] as EventDetail;
   bool userregforevent = ViewData["UserRegisterForEvent"] != null && Convert.ToBoolean(ViewData["UserRegisterForEvent"]);
%>
<%if (DOW != null && DOW.StartDate < DateTime.Now && DateTime.Now < DOW.EndDate && DOW.Status == (byte)Consts.AuctionStatus.Open)
  {%>
<div>
    <%=Html.ActionLink("TEMP", "DealOfTheWeek", new { controller = "Auction", action = "DealOfTheWeek", id = DOW.ID, evnt = DOW.UrlTitle }).ToHtmlString().Replace("TEMP", "<img src='/public/images/GOLD_DOW.jpg' />")%>
</div>
<% }%>
<% if (!isDemo && ViewData["LeftUserControlVisibility"] == null)
   {%>
<aside class="left-sidebar bg-white margin-767-bottom-xl">

    <% if (!isDemo)
       {%>
    <% if (ViewData["IsHomePage"] != null && Convert.ToBoolean(ViewData["IsHomePage"]))
       { %>
<%--    <div class="row">
        <h3 class="title-sidebar bg-primary">RESOURCE CENTER<a class="toggle-sidebar open-menu fa fa-bars" href="javascript:void(0);">&nbsp;</a></h3>
    </div>
    <ul class="list-unstyled">
        <li>
            <a href="<%=(isDemo)?"#": (Url.Action("ResourceCenter", "Home")+"#1")%>">IRS</a>
        </li>

        <li>
            <a href="<%=(isDemo)?"#": (Url.Action("ResourceCenter", "Home")+"#2")%>">GSA</a>
        </li>

        <li>
            <a href="<%=(isDemo)?"#": (Url.Action("ResourceCenter", "Home")+"#3")%>">Fleet Vehicle Sales</a>
        </li>

        <li>
            <a href="<%=(isDemo)?"#": (Url.Action("ResourceCenter", "Home")+"#4")%>">GovSales.gov</a>
        </li>

        <li style="border-bottom: 0;">
            <a href="<%=(isDemo)?"#": (Url.Action("ResourceCenter", "Home")+"#5")%>">US Marshals</a>
        </li>
    </ul>--%>

    <%} %>

    <% if (ViewData["IsHomePage"] == null && (currentController == "Account") && Convert.ToBoolean(ViewData["IsHomePage"]) == false && cuser != null)
       {%>
    <div class="row">
        <h3 class="title-sidebar bg-primary">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Hello, <%: cuser.Login %>! <a class="toggle-sidebar open-menu fa fa-bars" href="javascript:void(0);">&nbsp;</a></h3>
    </div>
    <div class="row account-menu">
        <ul class="list-unstyled account-menu">
            <li>
                <%= Html.ActionLink("Update / Change Your information", "Profile", "Account").ToSslLink() %>
      </li>
            <li>
                <%= Html.ActionLink("Auctions You Have Participated In", "PastAuction", "Account")%>
      </li>
            <li>
                <%= Html.ActionLink("Pay for Auction Items", "PayForItems", "Account")%>
      </li>
            <li>
                <%= Html.ActionLink("Past Auction Invoices", "PastInvoices", "Account")%>
      </li>
            <li>
                <%= Html.ActionLink("Edit Your Mail Settings", "EditMailSettings", "Account")%>
      </li>
            <li>
                <%= Html.ActionLink("Edit Your Personal Shopper", "EditPersonalShopper", "Account") %>
      </li>
            <li>
                <%= Html.ActionLink("Receive Your Personal Shopper Update", "ReceivePersonalShopperUpdate", "Account")%>
      </li>
            <% if (cuser.IsSellerType)
               {%>
            <li>
                <div class="attention_text"><%= Html.ActionLink("Seller Tool", "Index", "Consignor").ToNonSslLink() %></div>
            </li>
            <% } %>
            <% if (cuser.IsAdminType || cuser.IsReviewer)
               {%>
            <li>
                <div class="attention_text"><%= Html.ActionLink("Auction Preview", "Index", "Consignor").ToNonSslLink() %></div>
            </li>
            <% } %>
            <li>
                <%= Html.ActionLink("View Auctions", "Index", "Event").ToNonSslLink() %>
      </li>
            <li>
                <%= Html.ActionLink("Watch list", "WatchBid", "Account").ToNonSslLink() %>
      </li>
            <li>
                <%= Html.ActionLink("Logout", "LogOff", "Account").ToNonSslLink() %>
      </li>

            <%-- <li><a href="Profile">Update / Change Your information</a></li>
            <li><a href="PastAuction">Auctions You Have Participated In</a></li>
            <li><a href="PayForItems">Pay for Auction Items</a></li>
            <li><a href="PastInvoices">Past Auction Invoices</a></li>
            <li><a href="EditMailSettings">Edit Your Mail Settings</a></li>
            <li><a href="EditPersonalShopper">Edit Your Personal Shopper</a></li>
            <li><a href="ReceivePersonalShopperUpdate">Receive Your Personal Shopper Update</a></li>--%>
            <%--<li><a href="#">Seller Tool</a></li>--%>
            <%-- <% if (cuser.IsSellerType)
                   {%>
                <li>
                    <div class="attention_text"><%= Html.ActionLink("Seller Tool", "Index", "Consignor").ToNonSslLink() %></div>
                </li>
                <% } %>
                <% if (cuser.IsAdminType || cuser.IsReviewer)
                   {%>
                <li>
                    <div class="attention_text"><%= Html.ActionLink("Auction Preview", "Index", "Consignor").ToNonSslLink() %></div>
                </li>
                <% } %>--%>
            <%-- <li><a href="#">View Auctions</a></li>
            <li><a href="#">Watch list</a></li>
            <li><a href="#">Logout</a></li>--%>
        </ul>
    </div>

    <%} %>
    <% else
       {%>

    <% if (currentEvent != null && (ViewData["IsHomePage"] == null || !Convert.ToBoolean(ViewData["IsHomePage"])))
       {%>
    <div class="row">
        <h3 class="title-sidebar bg-primary"><span>CURRENT AUCTION</span><a class="toggle-sidebar open-menu fa fa-bars" href="javascript:void(0);">&nbsp;</a></h3>
    </div>
    <ul class="list-unstyled">
        <li>
            <%=(isDemo) ? Html.ActionLink("Back to the Event", "EventDetailed", new { controller = "Preview", action = "EventDetailed", id = currentEvent.ID, evnt = currentEvent.UrlTitle }) : Html.ActionLink("Back to the Event", "EventDetailed", new { controller = "Auction", action = "EventDetailed", id = currentEvent.ID, evnt = currentEvent.UrlTitle }) %>
        </li>
        <%
       if (!isDemo && currentEvent.IsClickable && !userregforevent && currentEvent.DateEnd > DateTime.Now)
       {%>
        <li>
            <%=(isDemo) ? Html.ActionLink("Featured Items", "EventDetailed", new { controller = "Preview", action = "EventDetailed", id = currentEvent.ID, evnt = currentEvent.UrlTitle }) : Html.ActionLink("Featured Items", "FeaturedItems", new { controller = "Auction", action = "FeaturedItems", id = currentEvent.ID, evnt = currentEvent.UrlTitle })%>
        </li>
        <li style="border-bottom: 0">
            <%= Html.ActionLink("Register for the Event", "Register", new { controller = "Event", action = "Register", id = currentEvent.ID, evnt = currentEvent.UrlTitle })%>
        </li>
        <%}
           else
           {%>
        <li style="border-bottom: 0">
            <%=(isDemo) ? Html.ActionLink("Featured Items", "EventDetailed", new { controller = "Preview", action = "EventDetailed", id = currentEvent.ID, evnt = currentEvent.UrlTitle }) : Html.ActionLink("Featured Items", "FeaturedItems", new { controller = "Auction", action = "FeaturedItems", id = currentEvent.ID, evnt = currentEvent.UrlTitle })%>
        </li>
        <%} %>
    </ul>

    <%} %>


    <% if (currentEvent != null)
       {%>
    <% if (currentEvent.IsViewable || isDemo || (cuser != null && cuser.IsAccessable))
       { %>

   <%-- <div class="row">
        <h3 class="title-sidebar bg-primary">ALL CATEGORIES <a class="toggle-sidebar open-menu fa fa-bars" href="javascript:void(0);">&nbsp;</a></h3>
    </div>--%>

     <div class="row">
        <h3 class="title-sidebar bg-primary">ALL CATEGORIES <a class="categories-click-menu fa fa-bars" href="javascript:void(0);">&nbsp;</a></h3>
    </div>

    
    <div id="categories_tree" class="categories-menu-open">
        <% Html.RenderAction("GetCategoriesTreeChild", "Home", new { event_id = currentEvent.ID, demo = isDemo });%>
    </div>
  
    <br />
    <%} %>
    <% }
       else Response.Write("<br /><br />"); %>
    <% } %>
    <%} %>
</aside>
<% }%>
