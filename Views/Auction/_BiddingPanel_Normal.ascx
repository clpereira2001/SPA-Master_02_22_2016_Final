<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AuctionDetail>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<%@ Import Namespace="System.Globalization" %>
<%
    BiddingResult bresult = ViewData["BiddingResult"] as BiddingResult;
    if (bresult != null)
    {
        SessionUser cuser = AppHelper.CurrentUser;

        decimal OldStartBid, StartBid, StartBidProxy;
        OldStartBid = StartBid = StartBidProxy = bresult.MinBid;
        OldStartBid += Consts.GetIncrement(bresult.MinWinBid == null ? Model.Price : StartBid);

        List<SelectListItem> BidAmountItems = new List<SelectListItem>();
        for (int i = 0; i < Consts.DropDownSize; i++)
        {
            StartBid += Consts.GetIncrement((bresult.MinWinBid == null && i == 0) ? Model.Price : StartBid);
            BidAmountItems.Add(new SelectListItem { Text = StartBid.GetCurrency(), Value = Convert.ToString(StartBid.GetPrice()) });
        }

        StartBid = (bresult.UserWinBid == null || bresult.UserWinBid.MaxBid < StartBidProxy) ? StartBidProxy : bresult.UserWinBid.MaxBid;
        List<SelectListItem> BidAmountItemsProxy = new List<SelectListItem>();
        for (int i = 0; i < Consts.DropDownSize; i++)
        {
            StartBid += Consts.GetIncrement((bresult.MinWinBid == null && i == 0) ? Model.Price : StartBid);
            BidAmountItemsProxy.Add(new SelectListItem { Text = StartBid.GetCurrency(), Value = Convert.ToString(StartBid.GetPrice()) });
        }
        List<SelectListItem> QuantityItems = new List<SelectListItem> { new SelectListItem { Text = "1", Value = "1", Selected = true } };

        AuctionUserInfo aui = ViewData["AuctionUserInfo"] as AuctionUserInfo ?? new AuctionUserInfo();

        bool IsBuyItNow = cuser != null && (aui.IsRegisterForEvent && Model.BuyPrice.HasValue && Model.BuyPrice.Value > 0 && Model.Owner_ID != cuser.ID && cuser.IsBuyerType && OldStartBid < Model.BuyPrice.Value);
%>
<% if (cuser == null)
   { %>
<br />
<%--<%= Html.ActionLink("Log in", "LogOn", "Account", new { @ReturnUrl = Url.Action("AuctionDetail", "Auction", new { id = Model.LinkParams.ID, evnt=Model.LinkParams.EventUrl, cat=Model.LinkParams.CategoryUrl, lot=Model.LinkParams.LotTitleUrl }) }, null).ToSslLink() %>--%>
<a href="#" class="aLogIn">Log In</a>
<% } %>
<% if (!aui.IsRegisterForEvent)
   { %>
<br />
<br />
You must be registered for this event before you can bid.
<br />
<%= Html.ActionLink("Register for the Event", "Register", new { controller = "Event", action = "Register", id = Model.LinkParams.Event_ID, id_auction = Model.LinkParams.ID }).ToSslLink() %>
<% }
   else
   { %>
<% 
       //Model.IsCurrent = true;

       if (Model.IsCurrent || Model.IsPrivate)
       { %>
<% if (cuser != null)
   {%>
<% using (Html.BeginForm("PreviewBid", "Auction", FormMethod.Post))
   { %>
<%=Html.Hidden("id", Model.LinkParams.ID) %>

<div class="row margin-top-lg margin-bottom">
    <div class="col-sm-6">
        <%--<a href="#" class="btn btn-primary text-uppercase btn-lg full-width make-btn">make a bid</a>--%>
<%--        <button type="button" class="btn btn-primary text-uppercase btn-lg full-width make-btn" onclick="window.location='<%= Url.Action ("PayForItems", "Account", new { controller = "Account", action = "PayForItems" })%>'">
                            <span>Buy Now</span>
                        </button>--%>
    </div>
    <div class="col-sm-6">
        <%if (!aui.IsInWatchList)
          { %>
        <%= Html.ActionLink(" Add to watch list", "AddBidWatch", new { controller = "Auction", action = "AddBidWatch", id = Model.LinkParams.ID }, new {@class="btn btn-default text-uppercase btn-lg full-width make-btn fa fa-eye" })%>
        <%} %>
    </div>
</div>




<div class="bg-white padding-vertical-sm padding-horizontal-sm">
    <p class="text-uppercase text-weight-700 border-bottom border-weight-xs border-primary padding-bottom-xs">minimum bid <span class="text-weight-300 pull-right" id='tbMinBid'><%=OldStartBid.GetCurrency() %></span></p>

    <div class="border-bottom border-weight-xs border-primary overflow-hidden margin-bottom-sm padding-right-sm">
        <p class="text-uppercase text-weight-700 pull-left">use proxy bidding <a href="#" class="text-dark"><i class="fa fa-question-circle"></i></a></p>


        <div class="alt-check pull-right">
            <label class="text-uppercase text-weight-700">
                <%= Html.CheckBox("ProxyBidding", true)%>
                <span class="check-icon"></span>
            </label>
        </div>
    </div>
    <!--</span>-->

    <form class="">
        <div class="row">
            <div class="col-sm-4">
                <div class="form-group">
                    <label for="Qty">Qty</label>

                    <%= Html.DropDownList("Quantity", QuantityItems, new { style = "//height:22px;//line-height:22px;//width:70px",@class="form-control text-center" })%>
                </div>
            </div>

            <div class="col-sm-8">
                <div class="form-group">
                    <label for="Bid">Max Bid</label>
                    <div class="input-group">
                        <div class="input-group-addon">$</div>
                        <%= Html.DropDownList("ba1", BidAmountItemsProxy, new { style = "//height:22px;//line-height:22px;//width:140px",@class="form-control currency" })%>
                        <%= Html.DropDownList("ba2", BidAmountItems, new { style = "//height:22px;//line-height:22px;//width:140px" })%>
                        <%= Html.Hidden("BidAmount", OldStartBid.GetPrice())%>
                        <script type="text/javascript">$("#ba2").hide(); $("#ProxyBidding").attr("checked", "true");</script>
                        <%--<input type="number" value="1000" min="0" step="0.01" data-number-to-fixed="2" data-number-stepfactor="100" class="form-control currency" style="-webkit-appearance: none; display: none;" />
                        <input class="ws-number ws-inputreplace form-control currency wsshadow-1454570958724 has-input-buttons" type="text" placeholder="" value="1,000" aria-required="false" inputmode="numeric" aria-labelledby="" style="margin-left: 0px; margin-right: 0px; padding-right: 27px; width: 312.2px;" /><span class="input-buttons number-input-buttons input-button-size-1 wsshadow-1454570958724" style="margin-right: 3px;"><span unselectable="on" class="step-controls"><span class="step-up step-control"></span><span class="step-down step-control"></span></span></span>--%>
                    </div>
                </div>
            </div>

        </div>
    </form>

    <div class="row margin-top-lg">
        <div class="col-sm-12">
            <% if (Model.Quantity > 0)
               { %>
            <%if (cuser.IsBidder && cuser.ID != Model.Owner_ID)
              {%>
            <button type="submit" name="btnPreview" class="text-uppercase btn btn-danger btn-lg full-width" onclick="submit()">
                <span>Preview Bid</span>
            </button>
            <%}
              else
              {%>
            <center>
                              <%if (cuser.IsAdmin)
                                {%>        
                                <span style="padding:5px;color:#008000;border:dotted 1px #008000; margin:5px">Administrator can't place bid on the items.</span><br /><br />
                              <%}
                                else if (cuser.IsSeller)
                                {%>        
                                <span style="padding:5px;color:#008000;border:dotted 1px #008000; margin:5px">Seller can't place bid on the items.</span><br /><br />
                              <%}
                                else if (cuser.IsReviewer)
                                {%>        
                                <span style="padding:5px;color:#008000;border:dotted 1px #008000; margin:5px">Reviewer can't place bid on the items.</span><br /><br />
                              <%}
                                else if ((cuser.IsSellerBuyer) && (cuser.ID == Model.Owner_ID))
                                {%>        
                                <span style="padding:5px;color:#008000;border:dotted 1px #008000; margin:5px">You can't place bid on your own items.</span><br /><br />
                              <%}
              } %> </center>
            <%}
               else
               { %>
            <b>This auction is not available.</b>
            <%} %>
        </div>
    </div>

    <div class="row margin-top-lg">
        <div class="col-sm-12">
            <p class="text-uppercase text-weight-700">how to bid</p>
            <ul class="list list-decimal">
                <li>Select your maximum bid amount</li>
                <li>Preview your bid</li>
                <li>Submit your final bid</li>
            </ul>


            <hr />

            <p>Contact Federal Asset Recovery Services if you have any questions before bidding. By bidding you agree to all Terms and Conditions and are entering into a legally binding Contract.</p>

            <p>
                <b>15% buyers premium will be added to all winning bids for this auction. Please bid accordingly.</b>
            </p>
        </div>
    </div>

</div>

<%} %>
<%} %>

<%}
       else
       {
           if (Model.EventDateEnd < DateTime.Now)
           { 
%>
      The auction has already ended.      
    <%}
           else
               if (Model.EventDateStart > DateTime.Now)
               { 
    %>
         The auction hasn't yet started.
      <%}
               else
               {%>
          Please try to refresh the page again.
        <%}
       }
   } %>
<%}%>

<script type="text/javascript">
    function InitBiddingDropDowns(IsProxy) {
        if (!IsProxy) {
            $("#ba1").hide();
            $("#ba2").show();
            $("#ba2").val($("#ba1").val());
            $("#ba2").change();
        } else {
            $("#ba2").hide();
            $("#ba1").show();
            $("#ba1").val($("#ba2").val());
            $("#ba1").change();
        }
    }
    $("#ProxyBidding").click(function () {
        InitBiddingDropDowns($("#ProxyBidding").attr("checked"));
    });
    InitBiddingDropDowns(true);
    $("#ba1").change(function () {
        $("#BidAmount").attr("value", $("#ba1").val());
    });
    $("#ba2").change(function () {
        $("#BidAmount").attr("value", $("#ba2").val());
    });
    $("#ba1").change();
</script>
