<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AuctionDetail>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>

<%
  List<SelectListItem> BidAmountItems = new List<SelectListItem>();
  BiddingResult bresult = ViewData["BiddingResult"] as BiddingResult;
   
  decimal Increment,StartBid;
  StartBid = bresult.MinBid;
//  Increment = Consts.GetIncrement(StartBid);  
  for (int i = 0; i < Consts.DropDownSize; i++)
  {
    Increment = Consts.GetIncrement(StartBid);
    BidAmountItems.Add(new SelectListItem { Text = StartBid.GetCurrency(), Value=StartBid.GetPrice().ToString() });
    StartBid += Increment;
  }
  List<SelectListItem> QuantityItems = new List<SelectListItem>();
  for (int i = 1; i <= Model.Quantity; i++)
    QuantityItems.Add(new SelectListItem { Text = String.Format(" {0} ", i), Value=i.ToString(), Selected = i==1 });

  AuctionUserInfo aui = ViewData["AuctionUserInfo"] as AuctionUserInfo;

  SessionUser cuser = AppHelper.CurrentUser;
  bool IsBuyItNow = cuser != null && (aui.IsRegisterForEvent && Model.BuyPrice.HasValue && Model.BuyPrice.Value > 0 && Model.Owner_ID != cuser.ID && cuser.IsBuyerType && Model.Quantity>0);
  %>
  <table cellpadding="0" cellspacing="0" border="1" id="bid" style="margin-bottom: 0px !important;">
  <colgroup><col width="250px" /><col width="250px" /><col width="230px" /></colgroup>
  <tr>
<%  
  if (!aui.IsRegisterForEvent)
  { 
%>
  <td class="bordered" colspan="2" style="border-top-width:0px; border-left-width:0px; border-bottom-width:0px; vertical-align:top; padding-right:5px; padding-left:0px">
<% if (cuser == null)
   {
%>
<br />
<%--<%= Html.ActionLink("Log in", "LogOn", "Account", new { @ReturnUrl = Url.Action("AuctionDetail", "Auction", new { id = Model.LinkParams.ID }) }, null).ToSslLink() %>--%>
<a href="#" class="aLogIn" >Log In</a>
<% }
%>
<br />
<br />
You should be registered for the event<br />
<%= Html.ActionLink("Register for the Event", "Register", new { controller = "Event", action = "Register", id = Model.LinkParams.Event_ID, id_auction = Model.LinkParams.ID }).ToSslLink() %>
  </td>
<%        
  }
    else
    {
      if (Model.IsCurrent || Model.IsPrivate)
      { 
%>
    <td class="bordered" style="border-top-width:0px; border-left-width:0px; border-bottom-width:0px; vertical-align:top; padding-right:5px; padding-left:0px">
      <div class="winnerTableMotto" style="font-weight:bold">Place bid</div>
      <% using (Html.BeginForm("PreviewBid", "Auction", FormMethod.Post))
         { %>
        <%--<%=Html.AntiForgeryToken() %>--%>
      <%=Html.Hidden("id", Model.LinkParams.ID) %>
         <br />
          <%if (!aui.IsInWatchList) { %>
          <span style="margin-left:10px">
            Add this item to your  <b><%= Html.ActionLink("Watch list", "AddBidWatch", new { controller = "Auction", action = "AddBidWatch", id = Model.LinkParams.ID }).ToNonSslLink()%></b>
            </span><br />
         <%} %>
         
      <%=Html.Hidden("ProxyBidding", false)%>      
      <br />      
      <% if (Model.Quantity <= 0)
         { %>
      <span style="border: 1px dotted green; color: green; height: 100%; line-height: 33px; text-indent: 15px; margin-right: 30px;">This auction is not available.</span>
      <% }  %>      
      <% if (Model.Quantity > 0)
         {
           %>
      <br />
      <span style="margin-left: 10px;margin-right: 30px">Quantity:</span>
      <%=Html.DropDownList("Quantity", QuantityItems, new { style = "//height:22px;//line-height:22px;//width:60px" })%>
      <br />
      <span style="margin-left: 10px;margin-right: 10px;">Bid Amount:</span>
          <%=Html.DropDownList("BidAmount", BidAmountItems, new { style = "//height:22px;//line-height:22px;//width:120px" })%>
      <br class="clear" /><br class="clear" />
      <center>
      <%if (cuser.IsBidder && cuser.ID != Model.Owner_ID) { %>
          <button type="submit" style="border: 0 none; width: 150px !important; display: block;" class="text-uppercase btn btn-danger btn-lg full-width">
            <span>Preview Bid</span>
          </button>
        <%} else { %>
        <div style="padding:5px;color:#008000;border:dotted 1px #008000; margin:5px; text-align:justify">
          You can't bid on this item.<br />
          Possible reasons:<br />
          1. You are a seller and this is your own item<br />
          2. You are not a buyer</div>
        <% } %>
        </center>
         <%}%>         
      <% }%>
    </td>
    <td class="bordered" style="border-top-width:0px; border-left-width:0px; border-bottom-width:0px; vertical-align:top" rowspan='<%=IsBuyItNow?"2":"1" %>'>
      <div class="winnerTableMotto"><b>Enter <span id='tbMinBid'><%=bresult.MinBid.GetCurrency()%></span> or more to be a winner</b></div>
      <div class="WinnerTable">        
        <div class="winnerTableTitle">
          Current winning bids:
        </div>
        <%
          if (bresult.WinTable.Any())
          {
            int i = 0;
            foreach (BidCurrent oneBid in bresult.WinTable)
            {
              i++; %>
              <div class="winnerTableItem"><%=i%>.<%=oneBid.Bidder %></div>
              <div class="winnerTableItemComment">items:&nbsp;<%=oneBid.Comments %></div>
              <div class="winnerTableItemPrice"><%=oneBid.Amount.GetCurrency() %></div>
              <%
              }
                if (bresult.LoserTable.Any())
            {%>
              <hr />
              <div class="winnerTableTitle">Highest unsuccessful bids:</div>
              <%  
                foreach (BidCurrent oneBid in bresult.LoserTable)
                {
                  i++;
                %>
                  <div class="winnerTableItem" ><%=i%>.<%=oneBid.Bidder %></div>
                  <div class="winnerTableItemComment">items:&nbsp;<%=oneBid.Comments%></div>
                  <div class="winnerTableItemPrice"><%=oneBid.Amount.GetCurrency() %></div>
                <%
                }
              }
     }
     else
     {
        %>
        <div class="winnerTableItem">
          None
        </div>
        <%} %>
      </div>
    </td>
    <%} else { %>
    <%
        if (Model.EventDateEnd < DateTime.Now)
          Response.Write("The auction has already ended.");
        else if (Model.EventDateStart > DateTime.Now)
               Response.Write("The auction hasn't yet started.");
        else Response.Write("Event is not available");
      } 
   }%>
    <td valign="top" style="vertical-align: top; border-bottom: none; border-top: none; border-right: none"  rowspan='<%=IsBuyItNow?"2":"1" %>'>
      <p style="padding-left:5px">
        <b>How to bid:</b><br />
        1. Select your maximum bid amount<br />
        2. Select quantity<br />
        3. Preview your bid<br />
        4. Submit your final bid
      </p>
    </td>
  </tr>
  <%
    if (IsBuyItNow)
     { %>
     <tr>
      <td class="bordered" style="border-top-width:0px; border-left-width:0px; border-bottom-width:0px; text-align:center">      
      <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/ITakeIt">
        <%=Html.AntiForgeryToken() %>
            <%=Html.Hidden("auction_id", Model.LinkParams.ID) %>
            <%=Html.Hidden("isDOW", "false") %>           
            <b>Don't want to wait?</b><br />Take it now for: <%=Model.BuyPrice.Value.GetCurrency()%>&nbsp;/ ea.<br />
            Quantity: <%=Html.DropDownList("quantity", QuantityItems)%><br />           
            <button type="submit" style="border: 0 none; width: 115px !important;" class="cssbutton small white">
                <span>I'll take it!</span>
            </button>
      </form>
      </td>
    </tr>
     <% } %>
</table>

<script type="text/javascript">
  $(document).ready(function () {
    $("#Quantity").val(1);
  });
</script>