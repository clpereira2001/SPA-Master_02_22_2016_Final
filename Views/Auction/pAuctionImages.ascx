<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Vauction.Models.Image>>" %>
<%if (Model.Count()>0){
  long auction_id = Convert.ToInt64(ViewData["auction_id"]);
  string path = AppHelper.AuctionImage(auction_id, Model[0].PicturePath); %>
  <table cellspacing="0" cellpadding="0" border="0" id="tblImageSchema">
    <tr>
      <td class="image_td" style="width:100%;vertical-align:top">    
        <% if (System.IO.File.Exists(Server.MapPath(path))){ %>        
          <img style="max-width:200px;" id="main_image" src="<%=AppHelper.CompressImagePath(path) %>" alt="" />
        <%} else { %>
          <img style="max-width:200px;" id="main_image" alt="" />
        <%} %>
      </td>
<%--    </tr>
      <tr>--%>
      <td align="center" class="img_scroller">
        <ul id="mycarousel" class="jcarousel-skin-tango" style="list-style:none">
          <% foreach (Vauction.Models.Image Image in Model)
             {
               path = AppHelper.AuctionImage(auction_id, Image.PicturePath);
               if (!System.IO.File.Exists(Server.MapPath(path))) continue;
           %>
              <li name="decide"><a href="javascript:swap_image('<%=AppHelper.CompressImagePath(path)%>');">
              <img src="<%=AppHelper.CompressAuctionImage(auction_id, Image.ThumbNailPath) %>" width="80" alt="" /></a></li>
          <% } %>
        </ul>
      </td>
    </tr>
  </table>
<%} %>