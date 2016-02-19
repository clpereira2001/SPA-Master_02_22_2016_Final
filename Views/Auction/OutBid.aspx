<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PreviewBid>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Sorry, you have been out bid- <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
    <h1 class="title">Sorry, you have been out bid.</h1>
    <%  List<SelectListItem> BidAmountItems = new List<SelectListItem>();
        decimal StartBid = Model.Amount;
        for (int i = 0; i < Consts.DropDownSize; i++)
        {
          StartBid += Consts.GetIncrement(StartBid);          
          BidAmountItems.Add(new SelectListItem { Text = StartBid.GetCurrency(), Value = StartBid.GetPrice().ToString()  });
        } 
    %>
    <% using (Html.BeginForm("PreviewBid", "Auction")){ %>
      <%=Html.AntiForgeryToken() %>
    Sorry, you have been out bid. This is due to someone placing a higher maximum bid than
    yours, or a prior bid of the same amount, which takes precedence. If you would like
    to bid again, please use the form below:
    <table cellpadding="3" border="0" id="bids_table">
      <tr>
        <td>
          Current Bid:          
        </td>
        <td>
          <%=Model.Amount.GetCurrency() %>
        </td>
      </tr>            
        <tr>
          <td>
            Your New Bid:
          </td>
          <td>
            <%=Html.DropDownList("BidAmount",BidAmountItems) %>
          </td>
        </tr>
        <tr>
          <td>
            Quantity:
          </td>
          <td>
            <%=Model.Quantity %>
          </td>
        </tr>
        <tr>
          <td>
            Proxy Bidding:
          </td>
          <td>
            <%:Model.IsProxy?"Yes":"No" %>
          </td>
        </tr>
    </table>
    <% =Html.Hidden("id",Model.LinkParams.ID) %>
    <% =Html.Hidden("ProxyBidding", Model.IsProxy) %>
    <% =Html.Hidden("Quantity", Model.Quantity )%>
    <% =Html.Hidden("OutBidAmount", Model.Amount )%>
    <button type="submit" class="cssbutton small white place_bid">
      <span>Preview Bid</span>
    </button>
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
    <button type="button" class="cssbutton small white" onclick="window.location='<%= Url.Action ("AuctionDetail", "Auction", new { controller = "Auction", action = "AuctionDetail", id = Model.LinkParams.ID, evnt=Model.LinkParams.EventUrl, cat=Model.LinkParams.CategoryUrl, lot=Model.LinkParams.LotTitleUrl })%>'">
      <span>Cancel Bid</span>
    </button>
    <%
      }     
    %>
      <br /> <br /> <br />
     <center>
      <%= Html.ActionLink("Back to Lot", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = Model.LinkParams.ID, evnt = Model.LinkParams.EventUrl, cat = Model.LinkParams.CategoryUrl, lot = Model.LinkParams.LotTitleUrl })%>
      |
      <%= Html.ActionLink("Back to Event", "EventDetailed", new { controller = "Auction", action = "EventDetailed", id = Model.LinkParams.Event_ID, evnt=Model.LinkParams.EventUrl })%>
    </center>    
  </div>
</asp:Content>
