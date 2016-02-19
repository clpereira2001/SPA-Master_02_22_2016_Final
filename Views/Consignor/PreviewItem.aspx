<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Vauction.Models.AuctionListing>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">  
  <title>Preview auction detailes - <%=Consts.CompanyTitleName%></title>  
    <style type="text/css">
        .bid_details {
            text-align: center;
        }
        .auto-style1 {
            width: 648px;
            text-align: center;
        }
        .auto-style2 {
            font-size: large;
        }
        .auto-style3 {
            font-size: medium;
            font-weight: bold;
        }
        .auto-style5 {
            font-size: medium;
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <% List<AuctionListingImage> images = ViewData["ALImages"] as List<AuctionListingImage>; %>
  <div class="center_content">      
     <table id="auction_detailed" cellspacing="0" cellpadding="0" border="0" style="border-collapse:collapse; table-layout:fixed; width: 644px;">
          <colgroup><col width="140px" /><col /><col width="160px" /><col width="180px" /></colgroup>
            <tr>
              <td colspan="4" class="bordered bid_details" style="background-color:navy;color:white; text-align: center; font-weight: 700;">
                  Lot: <span style="text-transform: uppercase; text-align: center; font-size: large;"><%:Model.Title %></span>
              </td>
            </tr>
            <tr>
              <td class="auto-style5"><span class="auto-style3">Event:</span><span class="auto-style5"> </span>
              </td>
              <td class="bordered" colspan="3">
                  <span class="auto-style5">
                <%: Model.Event.Title %></span>&nbsp;<span class="auto-style5"><%: Model.Event.StartEndTime %>
              </span>
              </td>
            </tr>
            <tr>
              <td class="auto-style5">
                  <span class="auto-style3">Category:</span><span class="auto-style5"> </span>
              </td>
              <td colspan="3" class="auto-style5">
                  <%:Model.EventCategory.FullCategory %>
              </td>              
            </tr>
            <tr>
              <td class="auto-style5">
                  <span class="auto-style3">Opening Price:</span><span class="auto-style5"> </span>
              </td>
              <td class="auto-style5">
                <%:Model.Price.GetCurrency() %>
              </td>
              <td class="auto-style2">
                  <span class="auto-style3">Shipping and Handling:</span><span class="auto-style5"> </span>
              </td>
              <td class="auto-style5">
                <%:Model.Shipping.GetCurrency() %>
              </td>
            </tr>
            <tr>
              <td class="auto-style5">
                  <span class="auto-style3">Auction Starts:</span><span class="auto-style5"> </span>
              </td>
              <td class="auto-style5">                
                  <%:Model.Event.StartTime%>
              </td>            
              <td class="auto-style2">
                  <span class="auto-style3">Auction Ends:</span><span class="auto-style5"> </span>
              </td>
              <td class="auto-style5">                
                  <%:Model.Event.EndTime%>
              </td>              
            </tr>
          </table>
      
     <table id="auction_description" cellspacing="0" cellpadding="0" border="0" style="border-collapse:collapse;">
     <tr>
        <td class="auto-style1" style="background-color:navy;color:white; text-align: center; font-weight: 700; font-size: large;">Item Description</td>
      </tr>
      <tr>
        <td class="auto-style1">
          <div>
      <% if (!String.IsNullOrEmpty(Model.Descr))
      { %>
        <div class="description_text"><%:Model.Descr %></div>
      <% } %>
      <% if (images != null && images.Count > 0)
      {
        string path = AppHelper.UserImageFolder(Model.Owner_ID, Model.ID) + "/"; %>
        <table cellspacing="0" cellpadding="0" align="left" border="0" width="100%">
          <tr>
            <td class="image_td" >
              <img style="max-width: 600px;" id="main_image" src="<%=(path+ images.First().PicturePath) %>" alt="" />
            </td>
            <td align="center" class="img_scroller">
              <ul id="mycarousel" class="jcarousel-skin-tango" style="margin:0;padding:0;list-style:none">
                <% foreach (AuctionListingImage img in images) {%>
                <li name="decide" style="line-height:120px; clear:both;"><a href="javascript:swap_image('<%=(path+ img.PicturePath)%>');"><img src="<%=(path+img.ThumbNailPath)%>" width="80px" alt="" /></a></li>
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
  <% byte edttype = Convert.ToByte(ViewData["edittype"]); %>
  <button type="submit" style="width: 65px!important;" class="btn btn-primary text-uppercase btn-lg" onclick="window.location='<%= Url.Action("SaveAuction", "Consignor", new {id = Model.ID, Auction_ID = Model.Auction_ID, edittype=edttype}) %>'">
    <span>Save</span>
  </button> 
  <button type="submit" style="width: 65px!important;" class="btn btn-info text-uppercase btn-lg" onclick="window.location='<%= Url.Action("UpdateAuction", "Consignor", new {id = Model.ID, edittype=edttype }) %>'">
    <span>Edit</span>
  </button> 
  <button type="button" style="width: 65px!important;" class="btn btn-danger pull-right text-uppercase btn-lg" onclick="window.location='<%= Url.Action("CancelListing", "Consignor", new {id = Model.ID, Auction_ID = Model.Auction_ID, edittype=edttype}) %>'">
    <span>Cancel</span>
  </button>
</asp:Content>

<asp:Content ID="Content5" ContentPlaceHolderID="cphScriptBottom" runat="server">
 <script type="text/javascript">
   function swap_image(img) {
     $("#main_image").get(0).src = img;
   }
  </script>
</asp:Content>