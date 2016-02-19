<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Vauction.Utils" %>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title><%=ViewData["Title"] %></title>
<%--  <% Html.Style("combinedstyle_20140216.css"); %>--%>
  <script type="text/javascript" src="<%=Context.Request.Url.Scheme.ToLower() + "://" %>ajax.googleapis.com/ajax/libs/jquery/1.4.4/jquery.min.js"></script>
  <%=Html.CompressJs(Url) %>
  <%=Html.CompressCss(Url) %>
  <% Html.Clear(); %>
    <style type="text/css">
        .auto-style1 {
            font-size: large;
        }
        .auto-style2 {
            font-size: large;
            width: 205px;
        }
        .bid_details {
            font-size: large;
            text-align: center;
        }
        .auto-style3 {
            width: 86px;
        }
        .auto-style4 {
            width: 7px;
        }
        .auto-style5 {
            width: 222px;
        }
        </style>
</head>
<body style=" font-family: Arial, Helvetica, sans-serif; font-size:12px">  
    <% 
      AuctionDetail currentAuction = ViewData["CurrentAuction"] as AuctionDetail;
      AuctionResultDetail aresult = ViewData["LotResult"] as AuctionResultDetail;
      if (currentAuction == null || aresult == null) { %> Auction Item cannot be found <% } else 
      {
        bool IsAuthenticated = Convert.ToBoolean(ViewData["IsAuthenticated"]);
        Boolean IsRegisteredForTheEvent = Convert.ToBoolean(ViewData["IsRegisteredForTheEvent"]);
        List<Vauction.Models.Image> currentAuctionImages = ViewData["CurrentAuctionImages"] as List<Vauction.Models.Image>;
    %>       
    <table id="auction_detailed" cellspacing="0" cellpadding="0" border="0" style="border-collapse: collapse;table-layout: fixed;font-size:12px; width: 1133px;">
      <colgroup>
        <col />
        <col />
        <col />
        <col width="170px" />
      </colgroup>
      <tr>
        <td colspan="4" class="auto-style1" style="background-color:navy;color:white; text-align: center; font-weight: 700;">
          Lot:
          <%= currentAuction.LinkParams.Lot%>: <span style="text-transform: uppercase; text-align: center;">
            <%= currentAuction.LinkParams.Title%></span>
        </td>
      </tr>
      <tr>
        <td class="auto-style2">
            &nbsp;</td>
        <td class="bordered" colspan="3">
            &nbsp;</td>
      </tr>
      <tr>
        <td class="auto-style2">
          <b>Event:</b>
        </td>
        <td class="bordered" colspan="3">
          <%=currentAuction.LinkParams.EventTitle%>
          <%=currentAuction.StartEndTime%>
        </td>
      </tr>
      <tr>
        <td class="auto-style2">
          <b>Category:</b>
        </td>
        <td colspan="3" class="bordered">
          <%=ViewData["FullCategoryLink"]%>
        </td>
      </tr>
      <tr>
        <td class="auto-style2">
          <b>Opening Price:</b>
        </td>
        <td class="auto-style3">
          <%= IsAuthenticated ? currentAuction.Price.GetCurrency() : Convert.ToDecimal(1).GetCurrency()%>
        </td>
        <td class="auto-style5">
          <b style="font-size: large">Shipping and Handling:</b>
        </td>
        <td class="auto-style4">
          <%=IsAuthenticated ? (currentAuction.Shipping > 0) ? currentAuction.Shipping.GetCurrency() : "Free shipping" : Convert.ToDecimal(1).GetCurrency()%>
        </td>
      </tr>
      <tr>
        <td class="auto-style2">
          <b>Auction Starts:</b>
        </td>
        <td class="auto-style3">
          <%=currentAuction.StartTime%>
        </td>
<%--        <% if (currentAuction.AuctionType == (int)Consts.AuctionType.Normal)
           { %>
        <td class="auto-style6">
          <b>Current Bid:</b>
        </td>
        <td class="auto-style4">
          <% if (!IsRegisteredForTheEvent && currentAuction.Status == (byte)Consts.AuctionStatus.Closed) Response.Write("$1.00");
             else %>
          <% if (!IsAuthenticated)
             { %> &nbsp; <%  }
             else
             { %>
          <% if (aresult.HasBid != null)
             { %>
                <%=IsRegisteredForTheEvent ? aresult.CurrentBid : ((decimal)1).GetCurrency()%>
          <%}
             else
             { %>
          none
          <%} %>
          <% } %>
        </td>
        <%}
           else
           { %>
        <%  if (!IsAuthenticated && currentAuction.Status != (byte)Consts.AuctionStatus.Closed)
            { %>
        <%--<td class="auto-style1">
          <b>Quantity:</b>
        </td>
        <td class="bordered">
          &nbsp;
        </td>--%>
<%--        <% }
            else
            { %>
        <% if (currentAuction.Quantity <= 1) { }
           else
           { %>--%>
<%--        <td class="auto-style1">
          <b>Quantity:</b>
        </td>--%>
<%--        <td class="bordered">
          <%=(currentAuction.Quantity == 0 || currentAuction.Status == (byte)Consts.AuctionStatus.Closed) ? currentAuction.IQuantity.ToString(CultureInfo.InvariantCulture) : currentAuction.Quantity.ToString(CultureInfo.InvariantCulture)%>
        </td>--%>
<%--        <% } %>
        <% } %>
        <%  }   %>--%>
        <td class="auto-style2">
          <b>Auction Ends:</b>
        </td>
        <td class="auto-style3">
          <%=currentAuction.EndTime%>
        </td>
      </tr>
<%--      <tr>
        
        <% if (currentAuction.AuctionType != (int)Consts.AuctionType.Dutch)
           { %>
        <td class="auto-style6">
          <b>HighBidder:</b>
        </td>
        <td class="auto-style4">
          <% if (!IsRegisteredForTheEvent && currentAuction.Status == (byte)Consts.AuctionStatus.Closed) Response.Write("");
             else %>
          <% if (!IsAuthenticated)
             { %>
          &nbsp;
          <% }
             else
             {     %>
          <%=(aresult.HasBid && IsRegisteredForTheEvent) ? aresult.HighBidder_1 : String.Empty%>
          <%  } %>
        </td>
        <%  }
           else
           { %>
        <%--<td class="auto-style1">
           <b>Current Bid<%=(aresult.CurrentBid_2.HasValue ? " Range:" : ":")%></b>
        </td>
        <td class="bordered">
          <% if (!IsRegisteredForTheEvent && currentAuction.Status == (byte)Consts.AuctionStatus.Closed) Response.Write("$1.00");
             else %>
          <% if (!IsAuthenticated)
             { %>
          &nbsp;--%>
         <%--<% }
             else { Response.Write(!aresult.HasBid ? "none" : aresult.CurrentBid); } %>--%>
        <%--</td>--%>
<%--        <%} %>
      </tr>--%>
      <tr>
        <td class="auto-style2">
            &nbsp;</td>
        <td class="auto-style3">
            &nbsp;</td>
        <td class="auto-style2">
            &nbsp;</td>
        <td class="auto-style3">
            &nbsp;</td>
      </tr>
    </table>    
    <table id="auction_description" cellspacing="0" cellpadding="0" border="0" style="border-collapse: collapse; width: 1131px">
      <tr>
        <td class="bordered bid_details" style="background-color:navy;color:white; text-align: center; font-weight: 700;">
          Item Description
        </td>
      </tr>
      <tr>
        <td class="bordered">
          <div>
            <% if (currentAuction.Description != "")
               { %>
 <%--           <div class="description_text">
              <%= currentAuction.Description%>
            </div>--%>
            <% } %>
            <% if (currentAuctionImages.Any())
               {
                 long id = Convert.ToInt64(currentAuction.LinkParams.ID);
                 string path;
            %>
            <table cellspacing="0" cellpadding="0" border="0" style="margin:5px">
              <%
                int i = 0;
                foreach (Vauction.Models.Image img in currentAuctionImages)
                {
                  path = AppHelper.AuctionImage(id, img.PicturePath);
                  if (!System.IO.File.Exists(Server.MapPath(path))) continue;
                  if (i % 2 == 0)
                    Response.Write("<tr>");
                  i++;       
              %>
              <td class="image_td" style="text-align:center; width:330px">
                <img style="max-width: 300px; max-height:300px" id="main_image" src="<%=AppHelper.CompressImagePath(path) %>"
                  alt="" />
              </td>
              <%                
                if (i % 2 == 2)
                {
                  Response.Write("</tr>");
                  i = 0;
                }
           } %>
            </table>
            <% } %>
          </div>
        </td>
      </tr>
    </table>    
  <br class="clear" />
<%--  <br /><b>URL</b>: <%=AppHelper.GetSiteUrl(String.Format("/Auction/AuctionDetail/{0}/{1}/{2}/{3}", currentAuction.LinkParams.ID, currentAuction.LinkParams.EventUrl, currentAuction.LinkParams.CategoryUrl, currentAuction.LinkParams.LotTitleUrl)) %>--%>
  <% } %>
<%--  <br /><b>Printed date</b>: <%=DateTime.Now %>--%>
  <script type="text/javascript">
    $(document).ready(function() {
      window.print();
    });
  </script>
</body>
</html>