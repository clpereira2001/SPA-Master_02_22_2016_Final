<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<AuctionDetail>" MasterPageFile="~/Views/Shared/Site.Master" %>

<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<%@ Import Namespace="Vauction.Utils.Paging" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title><%=String.Format("{0} - {1} (LOT#{2} ENDS {3})", Model.LinkParams.Title, Consts.CompanyTitleName, Model.LinkParams.Lot, Model.EndTime) %></title>
    <style type="text/css">
        .auto-style1 {
            text-decoration: underline;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container margin-bottom-xl">
    <%  AuctionResultDetail aresult = ViewData["LotResult"] as AuctionResultDetail ?? new AuctionResultDetail();
            List<Vauction.Models.Image> currentAuctionImages = ViewData["CurrentAuctionImages"] as List<Vauction.Models.Image>;
            string strDesc = "";

            IPagedList<AuctionShort> GetFeaturedList = ViewData["Lots"] as IPagedList<AuctionShort>;
        AuctionUserInfo aui = ViewData["AuctionUserInfo"] as AuctionUserInfo;
            int intMaincnt = 0;
            long auction_id = Convert.ToInt64(ViewData["auction_id"]);
            string path = "";             //List<Vauction.Models.Image> lstImage = new ImageRepository.GetAuctionImages(Model.LinkParams.ID, true);
            SessionUser cuser = AppHelper.CurrentUser;
            if (currentAuctionImages != null && currentAuctionImages.Count > 0)
            {
                foreach (Vauction.Models.Image Image in currentAuctionImages)
                {
                    path = AppHelper.AuctionImage(auction_id, Image.PicturePath);
                    if (!System.IO.File.Exists(Server.MapPath(path))) continue;
                    intMaincnt = 1;
                }
            }
        %>
        <% if (cuser != null && (cuser.IsAdminType || (((cuser.IsSellerType && Model.Owner_ID == cuser.ID) || cuser.IsReviewer) && !Model.IsClickable)) && Model.StartDate > DateTime.Now)
           {%>
      <%=Html.ActionLink("Click here to edit this lot", "ModifyAuction", "Consignor", new { controller = "Consignor", action = "ModifyAuction", id=Model.LinkParams.ID, edittype=1 }, new {@style="font-size:14px"})%>
    <%}%>
        <div class="row">
            <div class="col-sm-12">
                <p class="text-primary text-weight-900 text-md"><%=Model.EventDetailsInfo%><span style="color: #000099;">&raquo;&nbsp;<%=ViewData["FullCategoryLink"]%></span></p>
   </div>
        </div>
    
        <div class="row">

            <% if (currentAuctionImages != null && currentAuctionImages.Count > 0)
               {%>


            <div class="col-sm-6">

                <%if (intMaincnt > 0)
                  { %>
                <div id="slider" class="flexslider">

                    <div class="flex-viewport" style="overflow: hidden; position: relative;">
                        <ul class="slides" style="width: 1400%; transition-duration: 0s; transform: translate3d(-2735px, 0px, 0px);">

                            <% int intCnt1 = 0; %>
                            <% foreach (Vauction.Models.Image Image in currentAuctionImages)
                               {
                                   path = AppHelper.AuctionImage(auction_id, Image.PicturePath);
                                   if (!System.IO.File.Exists(Server.MapPath(path))) continue;
                            %>
                            <% if (intCnt1 == 0)
                               {%>
                            <li class="flex-active-slide" style="width: 547px; float: left; display: block;">
                                <%} %>
                                <%else
                               { %>
                                <li style="width: 547px; float: left; display: block;">
                                    <%} %>
                                    <img src="<%=AppHelper.CompressAuctionImage(auction_id, Image.PicturePath) %>" alt="" draggable="false" />
                                </li>
                                <%intCnt1 = intCnt1 + 1; %>
    <% } %>
                                <li style="width: 547px; float: left; display: block;">
                                    <img src="/images/img-preview.jpg" alt="" draggable="false" />
                                </li>
                        </ul>
                    </div>
                    <ul class="flex-direction-nav">
                        <li class="flex-nav-prev"><a class="flex-prev" href="#">Previous</a></li>
                        <li class="flex-nav-next"><a class="flex-next" href="#">Next</a></li>
                    </ul>
                </div>
    
                <div class="padding-horizontal-xl">
                    <div id="carousel" class="flexslider">

                        <div class="flex-viewport" style="overflow: hidden; position: relative;">
                            <ul class="slides" style="width: 1400%; transition-duration: 0.6s; transform: translate3d(0px, 0px, 0px);">

                                <% int intCnt = 0; %>
                                <% foreach (Vauction.Models.Image Image1 in currentAuctionImages)
                                   {

                                       path = AppHelper.AuctionImage(auction_id, Image1.ThumbNailPath);
                                       if (!System.IO.File.Exists(Server.MapPath(path))) continue;
                                %>
                                <% if (intCnt == 0)
                                   {%>
                                <li class="flex-active-slide" style="width: 123.75px; float: left; display: block;">
                                    <%} %>
                                    <%else
                                   { %>
                                    <li style="width: 123.75px; float: left; display: block;">
                                        <%} %>
                                        <img src="<%=AppHelper.CompressAuctionImage(auction_id, Image1.ThumbNailPath) %>" alt="" draggable="false" />
                                    </li>
                                    <%intCnt = intCnt + 1; %>
                                    <% } %>
                            </ul>
                        </div>
                        <ul class="flex-direction-nav">
                            <li class="flex-nav-prev"><a class="flex-prev" href="#">Previous</a></li>
                            <li class="flex-nav-next"><a class="flex-next" href="#">Next</a></li>
                        </ul>
                    </div>
                </div>

                <%}%>
            </div>
            <%}%>



            <div class="col-sm-6">
            <p>
                <span class="bg-color-fourth text-uppercase text-white padding-vertical-sm padding-horizontal-sm display-inline-block text-weight-700">Lot # <%= Model.LinkParams.Lot%></span> 
            </p>
                <p class="text-lg text-primary border-bottom border-weight-xs padding-bottom border-primary">
                    <%= Model.LinkParams.Title%>
            </p>

                <div class="row">
                    <div class="col-sm-5">

                        <p class="text-uppercase text-danger margin-bottom-none">current bid</p>
            <p class="text-lg text-primary text-weight-700">
                            <p class="text-lg text-primary text-weight-700">
          <% if (!aui.IsRegisterForEvent && Model.Status == (byte)Consts.AuctionStatus.Closed) Response.Write("$1.00");
             else %>
          <% if (!Request.IsAuthenticated)
             { %>
<%--              <%= Html.ActionLink("Log in", "LogOn", "Account", new { @ReturnUrl = Url.Action("AuctionDetail", "Auction", new { id = Model.LinkParams.ID }) }, null).ToSslLink()%> --%>
<%--              <a href="#" class="aLogIn" >Log In</a>--%>
          <%}
             else
             { %>                
             <span id="tbCurrentBid">
            <% if (aresult.HasBid)
               { %>
                <% =aui.IsRegisterForEvent ? aresult.CurrentBid : ((decimal)1).GetCurrency()%>
            <%}
               else
               { %> none <%} %>
               </span>
                                <% } %>
                            </p>
                        </p>
                        <p class="text-primary"><a href="#" onclick="window.location.href= window.location;"><i class="text-md fa fa-refresh"></i>Refresh Current Bid</a></p>
                        <%--<p class="text-primary">
                            <span id="lnkRBR">
                                <img src="<%=AppHelper.CompressImage("refresh.gif") %>" alt="" /><span></span></span>
                            <span id="lnkRBR_loading">
                                <img src="<%=AppHelper.CompressImage("bid_result_loader.gif") %>" alt="" class="text-md fa fa-refresh" />refreshing current bid</span>
                            <button id="btnRefresh" onclick="window.location.href= window.location;" class="text-md fa fa-refresh">refresh current bid</button>

                        </p>
                        <script type="text/javascript">$("#lnkRBR_loading").hide();</script>--%>
                    </div>
                    <div class="col-sm-7">
                        <p class="text-uppercase text-primary text-weight-700">
                            Shipping and Handling:  <%=(Model.Shipping > 0) ? Model.Shipping.GetCurrency() : (!Model.IsSpecialInstruction.GetValueOrDefault(false)?"Free shipping":String.Empty) %>
                            <% if (Model.IsSpecialInstruction.GetValueOrDefault(false))
           { %>
                            <% if (Model.Shipping > 0)
            { %>
                            <span style='float: right; margin-right: 5px'>(<a href="#auction_description">See Description</a>)</span>
        <% }
            else
                               {%>
                            <span style='float: left; margin-left: 5px'><a href="#auction_description">See Description</a></span>
          <% }
                               }%>
                        </p>
                        <p class="margin-bottom-none"><b>Category:</b> <%=ViewData["FullCategoryLink"]%></p>
                        <p class="margin-bottom-none"><b>Opening Bid:</b> <%= Request.IsAuthenticated ? Model.Price.GetCurrency() : Convert.ToDecimal(1).GetCurrency()%></p>
                        <p class="margin-bottom-none"><b>Auction Starts:</b> <%=Model.StartTime%></p>
                        <p class="margin-bottom-none"><b>Auction Ends:</b>  <%=Model.EndTime%></p>
                        <p>
        <% if (Model.AuctionType != (int)Consts.AuctionType.Dutch)
           { %>
          <b>HighBidder:</b>
          <% if (!aui.IsRegisterForEvent && Model.Status == (byte)Consts.AuctionStatus.Closed) Response.Write("");
             else %>
          <% if (!Request.IsAuthenticated)
             { %>

<%--                            <%= Html.ActionLink("Log in", "LogOn", "Account", new { @ReturnUrl = Url.Action("AuctionDetail", "Auction", new { id = Model.LinkParams.ID }) }, null).ToSslLink()%>--%>
          <% }
             else
             { %>
             <span id='tbHighBidder'><%=(aresult.HasBid && aui.IsRegisterForEvent) ? aresult.HighBidder_1 : String.Empty%></span>
          <%  } %>

        <%  }
           else
           { %>
Current Bid<%=(aresult.CurrentBid_2.HasValue && aresult.CurrentBid_1.GetValueOrDefault(-1) != aresult.CurrentBid_2.GetValueOrDefault(-1) ? " Range:" : ":")%>


          <% if (!aui.IsRegisterForEvent && Model.Status == (byte)Consts.AuctionStatus.Closed) Response.Write("$1.00");
             else %>
          <% if (!Request.IsAuthenticated)
             { %>
                <%--<%= Html.ActionLink("Log in", "LogOn", "Account", new { @ReturnUrl = Url.Action("AuctionDetail", "Auction", new { id = Model.LinkParams.ID }) }, null).ToSslLink()%>  --%>
                            <%-- <a href="#" class="aLogIn">Log In</a>--%>
                            <%= Html.ActionLink("Log in", "LogOn", "Account", new { @ReturnUrl = Url.Action("AuctionDetail", "Auction", new { id = Model.LinkParams.ID }) }, null).ToSslLink()%>
          <% }
             else { Response.Write("<span id='tbCurrentBid'>" + (!aresult.HasBid || !aui.IsRegisterForEvent ? "none" : aresult.CurrentBid) + "</span>"); } %>

                            <%} %>
                </p>
                    </div>
                </div>

                <div class="row margin-top-lg margin-bottom">
                    <% if (!Request.IsAuthenticated)
                       { %>
                    <div class="col-sm-12">
<%--                        <%= Html.ActionLink("Log in", "LogOn", "Account", new { @ReturnUrl = Url.Action("AuctionDetail", "Auction", new { id = Model.LinkParams.ID }), },  new {@class="text-uppercase btn btn-danger btn-lg full-width" }).ToSslLink()%>--%>
<%--                            <a href="<%=Consts.ProtocolSitePort %>/Account/LogOn" class="text-uppercase btn btn-danger btn-lg padding-horizontal-xl" onclick="document.forms[0].submit();">Log In</a>--%>
                        <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/LogOn">
                            <div class="text-center padding-top margin-bottom">
                                <button id="btnSignIn" name="SignIn" type="submit" class="text-uppercase btn btn-danger btn-lg padding-horizontal-xl" onclick="submit();">log in</button>
                            </div>
                        </form>
                    </div>
                    <% }
                       else
                       { %>
                    <%--<div class="col-sm-6">
                        <a href="#" class="btn btn-primary text-uppercase btn-lg full-width make-btn">make a bid</a>
                    </div>--%>
                    <div class="col-sm-6">
                        <% if (Model.StartDate > DateTime.Now)
                           {%>

                        <% if (!aui.IsRegisterForEvent)
                           { %>
                  You can register for this event now. <%= Html.ActionLink("Register for the Event", "Register", new { controller = "Event", action = "Register", id = Model.LinkParams.Event_ID, id_auction = Model.LinkParams.ID }).ToSslLink()%><br />
                        <br />
                        <% }
                           else if (Request.IsAuthenticated && !aui.IsInWatchList)
         { %>

                        <%= Html.ActionLink(" Add to watch list", "AddBidWatch", new { controller = "Auction", action = "AddBidWatch", id = Model.LinkParams.ID }, new {@class="btn btn-default text-uppercase btn-lg full-width make-btn fa fa-eye" })%>
                        <br />
                        <br />
      <%} %>
              The bidding for this lot hasn't started, please come back again on <%=Model.StartTime%>.

      <%}%>
                    </div>
                    <%  } %>
        
                    <%-- <div class="col-sm-6">
                        <% if (!Request.IsAuthenticated)
                           { %>

                        <%= Html.ActionLink("Log in", "LogOn", "Account", new { @ReturnUrl = Url.Action("AuctionDetail", "Auction", new { id = Model.LinkParams.ID }) }, null).ToSslLink()%>
                        <% }
                           else
                           { %>
                        <a href="#" class="btn btn-primary text-uppercase btn-lg full-width make-btn">make a bid</a>
                        <%  } %>
                    </div>--%>
                    <%--   <div class="col-sm-6">
      <% if (Model.StartDate > DateTime.Now)
         {%>        

            <% if (!aui.IsRegisterForEvent)
               { %>
                  You can register for this event now. <%= Html.ActionLink("Register for the Event", "Register", new { controller = "Event", action = "Register", id = Model.LinkParams.Event_ID, id_auction = Model.LinkParams.ID }).ToSslLink()%><br />
                        <br />
          <% }
               else if (Request.IsAuthenticated && !aui.IsInWatchList)
               { %>

                        <%= Html.ActionLink(" Add to watch list", "AddBidWatch", new { controller = "Auction", action = "AddBidWatch", id = Model.LinkParams.ID }, new {@class="btn btn-default text-uppercase btn-lg full-width make-btn fa fa-eye" })%>
                        <br />
                        <br />
              <%} %>
              The bidding for this lot hasn't started, please come back again on <%=Model.StartTime%>.
            
      <%}%>
                    </div>--%>
                </div>

                <%-- <div class="col-sm-6">--%>
                <div class="row margin-top-lg margin-bottom">
            <%if (!Request.IsAuthenticated)
              {%>
              <% if (Model.EndDate < DateTime.Now)
                 {%>
                  The bidding for this lot has ended, please <%= Html.ActionLink("Log in", "LogOn", "Account", new { @ReturnUrl = Url.Action("AuctionDetail", "Auction", new { id = Model.LinkParams.ID }) }, null).ToSslLink()%> to view the bidding history if you were a registered user for this event.
              <%}
                 else
                 { %>
                    <%--<%= Html.ActionLink("Log in", "LogOn", "Account", new { @ReturnUrl = Url.Action("AuctionDetail", "Auction", new { id = Model.LinkParams.ID }) }, null).ToSslLink()%>--%>
<%--                <a class="aLogIn" style="cursor:pointer">Log In</a>--%>
<%--                    <a href="<%=Consts.ProtocolSitePort %>/Account/LogOn" class="text-uppercase btn btn-danger btn-lg padding-horizontal-xl" onclick="$(this).closest('form').submit()">Log In</a>--%>
                <%} %>
                <% }
              else if (Model.StartDate <= DateTime.Now && Model.EndDate > DateTime.Now)
              {
                Html.RenderPartial((Model.AuctionType == (long)Consts.AuctionType.Normal) ? "~/Views/Auction/_BiddingPanel_Normal.ascx" : "~/Views/Auction/_BiddingPanel_Multiple.ascx", Model);
              }
              else if ((Model.EndDate < DateTime.Now || Model.Status == (byte)Consts.AuctionStatus.Closed) && (Request.IsAuthenticated) && (aui.IsRegisterForEvent))
              {%> 
                  <% if (Model.EventDateEnd > DateTime.Now && (Model.IsCurrent || Model.IsPrivate))
                         Response.Write("Sold to the \"I'll take it Now\" bidder.");
                     else
                     {
                         if (!Model.IsViewable && !Model.IsClickable)
                         {
                             if (Model.EndDate.AddMinutes(5) > DateTime.Now)
                                 Response.Write("Auction has ended. Results will be available in approximately 5 minutes.");
                             else
                                   Html.RenderAction("pBiddingHistory", "Auction", new { auction_id = Model.LinkParams.ID });
                         }
                         else
                         {
                             if (Model.EndDate.AddMinutes(25) > DateTime.Now)
                                 Response.Write("Auction has ended. Results will be available in approximately 20 to 30 minutes.");
                             else
                                   Html.RenderAction("pBiddingHistory", "Auction", new { auction_id = Model.LinkParams.ID });
                         }
                     }
              }
              else if ((Model.EndDate < DateTime.Now) && (Request.IsAuthenticated) && (!aui.IsRegisterForEvent))
                Response.Write("The bidding for this lot has ended. You are not able to view the bid history because you were not registered for the event.");
            %>
                </div>



            </div>
        </div>


        <div class="row margin-vertical-xl">
            <div class="col-sm-12">
                <h2 class="text-uppercase text-center text-primary title-featured"><span>featured products</span></h2>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-12">

                <div class="padding-horizontal-xl">
                    <div id="owl-slider">
                        <% if (GetFeaturedList != null && GetFeaturedList.Count > 0)
                           { %>
                        <%  
                           foreach (AuctionShort auctionshortdetails in GetFeaturedList)
                           {
                               
                        %>
                        <div class="item">
                            <div class="thumbnail">
                                <%--<% string address = AppHelper.AuctionImage(auctionshortdetails.LinkParams.ID, auctionshortdetails.ThumbnailPath);%>
                                    <a href="#">
                                        <img alt="" src="images/product/product-001.jpg"/>
                                    </a>--%>
                                <% string address = AppHelper.AuctionImage(auctionshortdetails.LinkParams.ID, auctionshortdetails.ThumbnailPath);
                                   bool isDemo = false;
                                   if (System.IO.File.Exists(Server.MapPath(address)))
                                   {
                                       if (auctionshortdetails.IsClickable || isDemo || auctionshortdetails.IsAccessable)
                                       {
                                           Html.RenderPartial("~/Views/Auction/_ItemView.ascx", auctionshortdetails);
                                %>
                                
<%--                                <form action="<%=Url.Action("AuctionDetail", (isDemo?"Preview":"Auction"), new CollectParameters((isDemo?"Preview":"Auction"), "AuctionDetail", auctionshortdetails.LinkParams.ID, auctionshortdetails.LinkParams.EventUrl, auctionshortdetails.LinkParams.CategoryUrl, auctionshortdetails.LinkParams.LotTitleUrl).Collect("page", "ViewMode"))%>"" method="post">                   
                                    <div>
                                    <a href="#" title="<%=auctionshortdetails.LinkParams.LotTitle %>" onclick="document.forms[0].submit();return false; ">--%>
<%--                                    <button id="btnRegister" name="Register" type="submit" onclick="submit();"><%=auctionshortdetails.LinkParams.LotTitle %>--%>
                                    <%} %>
<%--                                    <img src="<%=AppHelper.CompressImagePath(address) %>" style="max-height: 100px" alt="" />--%>
                                    <% if (auctionshortdetails.IsClickable || isDemo || auctionshortdetails.IsAccessable)
                                       { %>
<%--                                </a></div></form>--%>
                                <%}
                               }%>
                                <%else
                               {%>
                                <img src="../../public/images/no_image.gif" style="max-height: 100px" alt="" />
                                
                                
                                <% }
                                %>
                                <div class="caption">
                                    <%-- <p><a href="#"><%=auctionshortdetails.LinkParams.Title%></a></p>--%>
                                    <% if (auctionshortdetails.IsClickable || isDemo || auctionshortdetails.IsAccessable)
                                       {%>
                                    <form action="<%=Url.Action("AuctionDetail", (isDemo?"Preview":"Auction"), new CollectParameters((isDemo?"Preview":"Auction"), "AuctionDetail", auctionshortdetails.LinkParams.ID, auctionshortdetails.LinkParams.EventUrl, auctionshortdetails.LinkParams.CategoryUrl, auctionshortdetails.LinkParams.LotTitleUrl).Collect("page", "ViewMode"))%>"" method="post">                   
                                        <div>
<%--                                            <%Html.RenderPartial("~/Views/Auction/_ItemView.ascx", auctionshortdetails); %>--%>
<%--                                    <a href="#" title="<%=auctionshortdetails.LinkParams.LotTitle %>" onclick="document.forms[0].submit();return false; "><%=auctionshortdetails.LinkParams.Title%></a>--%>
                                    </div></form>
                                            <%} %>

<%--                                    <p class="text-uppercase">Opening bid <span class="text-primary"><%=auctionshortdetails.Price.GetCurrency() %></span></p>
                                    <p><a class="btn btn-info btn-square text-uppercase" href="<%= Url.Action("AuctionDetail", (isDemo?"Preview":"Auction"), new CollectParameters((isDemo?"Preview":"Auction"), "AuctionDetail", auctionshortdetails.LinkParams.ID, auctionshortdetails.LinkParams.EventUrl, auctionshortdetails.LinkParams.CategoryUrl, auctionshortdetails.LinkParams.LotTitleUrl).Collect("page", "ViewMode"))%>" >PREVIEW</a></p>--%>
                                </div>
                            </div>
                        </div>

                        <%} %>
                        <%} %>
                    </div>
                </div>



            </div>
    </div>
<%--<div>Path = <%=path %></div>--%>
  </div>  
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphScriptBottom" runat="server">  
    <script type="text/javascript">
    function swap_image(img) {      
      $("#main_image").get(0).src = img;
    }
    $(document).ready(function () {
      $("#printLot").click(function () {
        window.showModalDialog('<%=AppHelper.GetSiteUrl("/Auction/PrintAuction/"+Model.LinkParams.ID) %>', "Print", "border=thin; center=1;dialogTop=1; dialogLeft=1; dialogWidth=" + document.body.offsetWidth + "px; dialogHeight=" + document.body.offsetHeight + "px");
      });       
      $("#lnkRBR").click(function () {        
        $("#lnkRBR").hide();
        $("#lnkRBR_loading").show();
        //setTimeout(function () {
          $.post('/Auction/UpdateAuctionResult', {id:<%=Model.LinkParams.ID %>}, function (data) {            
            $("#tbCurrentBid").html(data.currentbid);
            $("#tbHighBidder").html(data.highbidder);
            $("#tbMinBid").html(data.minbid);
            $("#tbQuantity").html(data.quantity);
            $("#lnkRBR_loading").hide();
            $("#lnkRBR").show();
          }, 'json');
        //}, 5000);
      });

      $("#tblImageSchema").resize(function () {
        $(".cssbutton").css('margin-top','5px'); 
        $("#ProxyBidding").css('margin-top','-1px'); 
      });
      $("#tblImageSchema").resize();
    });    
  </script>
    <script type="text/javascript" src="https://code.jquery.com/jquery-1.11.3.min.js"></script>
    <script type="text/javascript" src="http://afarkas.github.io/webshim/js-webshim/minified/polyfiller.js"></script>
    <script type="text/javascript" src="/public/scripts/sticky.js"></script>
    <script type="text/javascript" src="/public/scripts/flexslider.js"></script>
    <script type="text/javascript" src="/public/scripts/owl.carousel.min.js"></script>
    <script type="text/javascript" src="/public/scripts/bootstrap.js"></script>
    <script type="text/javascript" src="/public/scripts/scripts.js"></script>


</asp:Content>