<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
  <% Auction currentAuction = ViewData["CurrentAuction"] as Auction ?? new Auction(); %>
  <title>PREVIEW - Auction# (<%=(ViewData["CurrentAuction"] as Auction)==null?"":currentAuction.ID.ToString()%>) detailed - <%=Consts.CompanyTitleName %></title>  
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<% 
  Auction currentAuction = ViewData["CurrentAuction"] as Auction;
  SessionUser cuser = AppHelper.CurrentUser;
  if (currentAuction != null)
  {
      EventDetail currentEvent = ViewData["CurrentEvent"] as EventDetail ?? new EventDetail();
      EventCategory currentCategory = ViewData["CurrentCategory"] as EventCategory ?? new EventCategory();
      List<Vauction.Models.Image> currentAuctionImages = ViewData["CurrentAuctionImages"] as List<Vauction.Models.Image>;
%>
  <div class="center_content"> 
  <% if (cuser!=null && (cuser.IsAdminType || (((cuser.IsSellerType && currentAuction.Owner_ID==cuser.ID) || cuser.IsReviewer) && !currentEvent.IsClickable)) && currentAuction.StartDate>DateTime.Now) {%>
      <%=Html.ActionLink("Click here to edit this lot", "ModifyAuction", "Consignor", new { controller = "Consignor", action = "ModifyAuction", id=currentAuction.ID, edittype=2 }, new {@style="font-size:14px"})%>
    <%}%>   
    <div>
      <h4 class="breadcrumb">
       <span style="float:left">
        <%=currentEvent.EventDetailsInfoDemo %><span style="color: #000099;">&raquo;&nbsp;<%=currentCategory.FullCategoryLinkDemo %></span>
      </span>          
      </h4>
    </div>
    <br class="clear" />
    <table id="auction_detailed" cellspacing="0" cellpadding="0" border="0" style="border-collapse:collapse; table-layout:fixed">
          <colgroup>
            <col width="140px" /><col width="190px" /><col width="160px" /><col width="170px" />
          </colgroup>
            <tr>
              <td colspan="4" class="bordered bid_details" >
                  Lot# <%= currentAuction.Lot%>: <span style="text-transform: uppercase"><%= currentAuction.Title%></span>
              </td>
            </tr>
            <tr>
              <td class="bordered">
                <b>Event:</b>
              </td>
              <td class="bordered" colspan="3">
                  <%=currentEvent.EventDetailsInfoDemo %>
                  <%=currentEvent.StartEndTime %>
              </td>
            </tr>
            <tr>
              <td class="bordered">
                <b>Category:</b>
              </td>
              <td colspan="3" class="bordered">
                <%=currentCategory.FullCategoryLinkDemo %>
              </td>              
            </tr>
            <tr>
              <td class="bordered">
                <b>Opening Price:</b>
              </td>
              <td class="bordered">
                <%=currentAuction.Price.GetCurrency() %>
              </td>
              <td class="bordered">
                <b>Shipping and Handling:</b>
              </td>
              <td class="bordered">
                <%=currentAuction.Shipping.GetCurrency()%>
              </td>
            </tr>
            <tr>
              <td class="bordered">
                <b>Auction Starts:</b>
              </td>
              <td class="bordered">                
                  <%=currentEvent.StartTime%>
              </td>
              <% if (currentAuction.AuctionType_ID == (int)Consts.AuctionType.Normal)
                   { %>
              <td class="bordered">
                <b>Current Bid:</b>
              </td>
              <td class="bordered">
                &nbsp;
              </td>
              <%} else { %>
              <td class="bordered">
                <b>Quantity:</b>
              </td>
              <td class="bordered">
                <%= currentAuction.Quantity.ToString()%>
              </td>
              <% } %>              
            </tr>
            <tr>
              <td class="bordered">
                <b>Auction Ends:</b>
              </td>
              <td class="bordered">                
                  <%=currentEvent.EndTime%>
              </td>
              <% if (currentAuction.AuctionType_ID != (int)Consts.AuctionType.Dutch)
               { %>
              <td class="bordered">
                <b>HighBidder:</b>
              </td>
              <td class="bordered">
               &nbsp;
              </td>
              <%  }   else { %>
              <td class="bordered">
                Current Bid
              </td>
              <td class="bordered">
                &nbsp;
              </td>
              <%} %>
            </tr>
          </table>
    
    <table id="auction_description" cellspacing="0" cellpadding="0" border="0" style="border-collapse:collapse;">      
      <tr>
        <td class="bordered bid_details" >
            Item Description
        </td>
      </tr>
      <tr>
        <td class="bordered">
          <div>
      <% if (currentAuction.Description != "")
   { %>
      <div class="description_text">
        <%= currentAuction.Description%>
      </div>
      <% } %>
      <% if (currentAuctionImages !=null && currentAuctionImages.Count() > 0)
      {
          long id = Convert.ToInt64(currentAuction.ID);
           %>
      <table cellspacing="0" cellpadding="0" border="0">
        <tr>
          <td class="image_td" style="width: 100%">
            <img style="max-width:600px;" id="main_image" src="<%=AppHelper.CompressImagePath(AppHelper.AuctionImage(id,currentAuctionImages.FirstOrDefault().PicturePath)) %>"
              alt="" />
          </td>
          <td align="center" class="img_scroller">
            <ul id="mycarousel" class="jcarousel-skin-tango" style="list-style:none">
              <% foreach (Vauction.Models.Image image in currentAuctionImages)
                   {   %>
                    <li name="decide"><a href="javascript:swap_image('<%=AppHelper.CompressImagePath(AppHelper.AuctionImage(id,image.PicturePath))%>');">
                      <img src="<%=AppHelper.CompressImagePath(AppHelper.AuctionImage(id, image.ThumbNailPath))%>" width="80px" alt="" /></a></li>
              <% } %>
            </ul>
          </td>
        </tr>
      </table>
      <% } %>
    </div>
        </td>
      </tr>
    </table>
    </div>
  <%
    }
    else
    { %>
      Auction Item cannot be found
  <%} %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="cphScriptBottom" runat="server">
  <script type="text/javascript">
    function swap_image(img) {      
      $("#main_image").get(0).src = img;
    }
  </script>
</asp:Content>