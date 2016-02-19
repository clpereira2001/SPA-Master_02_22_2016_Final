<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AuctionDetail>" %>

<%@ Import Namespace="Vauction.Utils.Autorization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title><%=Model.LinkParams.LotTitle %> - <%=Consts.CompanyTitleName %></title>
    <% Html.ScriptUrl("/public/carusel/jquery.jcarousel.pack.js"); %>
    <% Html.StyleUrl("/public/carusel/jquery.jcarousel.css"); %>
    <% Html.StyleUrl("/public/carusel/skins/tango/skin.css"); %>
    <% Html.Script("dow.js"); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="deal_of_week" class="container margin-bottom-xl">
        <div class="row">
            <div class="col-sm-12">
                <p class="text-primary text-weight-900 text-md">Deal of the Week</p>
            </div>
            <div class="col-sm-12">
                <p class="text-weight-900 text-md">Closeouts, Overstocks, Bankruptcies, Inventory, Liquidations & More...</p>
                <p class="text-primary">The deal of week features special items offered at a pre-set rock bottom price. Quantites are limited and these items will be sold on a first come first-served basis.</p>
                <p class="text-danger">These deals are hot once they're gone, they're gone.</p>
            </div>
        </div>
        <%
            List<Vauction.Models.Image> currentAuctionImages = ViewData["CurrentAuctionImages"] as List<Vauction.Models.Image>;
            bool isShipping = Model.Shipping > 0;
            decimal insurance = Convert.ToDecimal(ViewData["Insurance"]);
            insurance = (isShipping) ? ((int)(Model.Price / 100) * insurance + insurance) : 0;
            SessionUser cuser = AppHelper.CurrentUser;
  %>
        <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/ITakeIt">
            <%=Html.AntiForgeryToken() %>
            <%= Html.Hidden("auction_id", Model.LinkParams.ID)%>
            <%= Html.Hidden("isDOW", true)%>

            <div class="row">
                <div class="col-sm-6">
                    <div id="slider" class="flexslider">
                        <div class="flex-viewport" style="overflow: hidden; position: relative;"></div>
                        <div class="flex-viewport" style="overflow: hidden; position: relative;">
                            <ul class="slides" style="width: 1400%; transition-duration: 0s; transform: translate3d(0px, 0px, 0px);">
                                <li class="flex-active-slide" style="width: 547px; float: left; display: block;">
                                    <img src="../../public/images/_item2.png" alt="" draggable="false" />
                                </li>
                                <li style="width: 547px; float: left; display: block;" class="">
                                    <img src="../../public/images/_item2.png" alt="" draggable="false" />
                                </li>
                                <li style="width: 547px; float: left; display: block;" class="">
                                    <img src="../../public/images/_item2.png" alt="" draggable="false" />
                                </li>
                                <li style="width: 547px; float: left; display: block;" class="">
                                    <img src="../../public/images/_item2.png" alt="" draggable="false" />
                                </li>
                                <li style="width: 547px; float: left; display: block;" class="">
                                    <img src="../../public/images/_item2.png" alt="" draggable="false" />
                                </li>
                                <li style="width: 547px; float: left; display: block;" class="">
                                    <img src="../../public/images/_item2.png" alt="" draggable="false" />
                                </li>
                                <li style="width: 547px; float: left; display: block;" class="">
                                    <img src="../../public/images/_item2.png" alt="" draggable="false" />
                                </li>
                            </ul>
                        </div>
                        <ul class="flex-direction-nav">
                            <li class="flex-nav-prev"><a class="flex-prev flex-disabled" href="#" tabindex="-1">Previous</a></li>
                            <li class="flex-nav-next"><a class="flex-next" href="#" tabindex="-1">Next</a></li>
                        </ul>
                    </div>

                    <div class="padding-horizontal-xl">
                        <div id="carousel" class="flexslider">

                            <div class="flex-viewport" style="overflow: hidden; position: relative;">
                                <ul class="slides" style="width: 1400%; transition-duration: 0s; transform: translate3d(-371.25px, 0px, 0px);">
                                    <li class="flex-active-slide" style="width: 123.75px; float: left; display: block;">
                                        <img src="../../public/images/_item2.png" alt="" draggable="false" />
                                    </li>
                                    <li style="width: 123.75px; float: left; display: block;" class="">
                                        <img src="../../public/images/_item2.png" alt="" draggable="false" />
                                    </li>
                                    <li style="width: 123.75px; float: left; display: block;" class="">
                                        <img src="../../public/images/_item2.png" alt="" draggable="false" />
                                    </li>
                                    <li style="width: 123.75px; float: left; display: block;" class="">
                                        <img src="../../public/images/_item2.png" alt="" draggable="false" />
                                    </li>
                                    <li style="width: 123.75px; float: left; display: block;" class="">
                                        <img src="../../public/images/_item2.png" alt="" draggable="false" />
                                    </li>
                                    <li style="width: 123.75px; float: left; display: block;" class="">
                                        <img src="../../public/images/_item2.png" alt="" draggable="false" />
                                    </li>
                                    <li style="width: 123.75px; float: left; display: block;" class="">
                                        <img src="../../public/images/_item2.png" alt="" draggable="false" />
                                    </li>
                                </ul>
                            </div>
                            <ul class="flex-direction-nav">
                                <li class="flex-nav-prev"><a class="flex-prev" href="#">Previous</a></li>
                                <li class="flex-nav-next"><a class="flex-next" href="#">Next</a></li>
                            </ul>
                        </div>
                    </div>

                </div>
                <div class="col-sm-6">
                    <p>
                        <span class="bg-color-fourth text-uppercase text-white padding-vertical-sm padding-horizontal-sm display-inline-block text-weight-700">Lot # <%= Model.LinkParams.Lot%></span>
                    </p>
                    <p class="text-lg text-primary padding-bottom">
                        <%= Model.LinkParams.Title.ToUpper()%>
                    </p>

                    <div class="bg-white padding-vertical-sm padding-horizontal-sm">
                        <p class="text-uppercase text-weight-700 border-bottom border-weight-xs border-primary padding-bottom-xs">Item price <span class="text-weight-300 pull-right"><%=Model.Price.GetCurrency() %></span></p>
                        <p class="text-uppercase text-weight-700 border-bottom border-weight-xs border-primary padding-bottom-xs">Shipping &amp; handing <span class="text-weight-300 pull-right"><%=isShipping?"+"+(Model.Shipping + insurance).GetCurrency() : "&nbsp;Free shipping"%></span></p>
                        <% if (cuser == null)
                           { %>
                        <div class="row margin-top-lg">
                            <div class="col-sm-12">
                                <p class="text-success text-weight-700">Please <%= Html.ActionLink("Log in", "LogOn", "Account", new { @ReturnUrl = Url.Action("DealOfTheWeek", "Auction", new { id = Model.LinkParams.ID }) }, null).ToSslLink() %> or <%= Html.ActionLink("Register", "Register", "Account").ToSslLink() %> before checking out.</p>
                            </div>
                        </div>
                        <% }%>

                        <% else
                           {
                               if (Model.Quantity > 0)
                               { %>
                        <div class="row">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label for="Qty">Qty</label>
                                    <input id="quantity" name="quantity" type="number" min="1" size="5" maxlength="10" class="form-control text-center currency" value="1" onkeypress="return validateNumeric(event)" onkeyup="updateTotal(this.value)" onclick="return false" />
                                </div>
                            </div>
                        </div>
                        <p class="text-uppercase text-weight-700 border-bottom border-weight-xs border-primary padding-bottom-xs">Total <span class="text-weight-300 pull-right" id="total">$205.60</span></p>

                        <% if (Model.Owner_ID != cuser.ID)
                           {%>
                        <div class="row margin-top-lg">
                            <div class="col-sm-12">


                                <button type="submit" class="text-uppercase btn btn-danger btn-lg full-width" id="ItakeiT" name="Itakeit">
                                    <span>I'll take it</span></button>
                                <br />
                                <span style="text-align: center;" id="error">&nbsp;</span>
                            </div>
                        </div>
                        <% }%>
                        <%else
                           {%>
                        <div class="row">
                            <div class="col-sm-12">
                                <p class="text-primary text-weight-900 text-md">
                                    You can't bid on this item.<br />
                                    Possible reasons:
                                </p>
                            </div>
                            <div class="col-sm-12">

                                <p class="text-danger">
                                    1. This is Your own item<br />
                                    2. Your account can only place items to sell.
                                </p>
                            </div>
                        </div>





                        <% }%>
                        <% }%>
                        <% }%>
                        <div class="row margin-top-lg">
                            <div class="col-sm-12">

                                <p>Description Federal Asset Recovery Services if you have any questions before bidding. By bidding you agree to all Terms and Conditions and are entering into a legally binding Contract.</p>
                            </div>
                        </div>


                    </div>
                </div>
            </div>
            <%--  <table class="jsborder" style="padding: 0px;">
                <tr>
                    <td colspan="2" style="padding: 5px;" class="dealblueContent">LOT# <%= Model.LinkParams.Lot%>
            : <%= Model.LinkParams.Title.ToUpper()%>
       </td>
                </tr>
                <tr>
                    <td colspan="2"><%= Model.Description%></td>
                </tr>
                <tr>
                    <td style="padding: 5px;">
                        <table cellspacing="0" cellpadding="0" border="0">
                            <tr>
                                <td class="image_td" align="center" valign="top" style="width: 500px; height: 510px; vertical-align: top;">
                                    <% if (currentAuctionImages.Count() > 0)
                                       { %>
                                    <br />
                                    <img id="main_image" class="bigImg" name="main_image" src="<%=AppHelper.CompressAuctionImage(Model.LinkParams.ID, currentAuctionImages.First().PicturePath)%>" style="max-width: 500px; border: solid 1px #a5a6a6; vertical-align: top" alt="" />
                                    <% } %>
            </td>
                                <td style="vertical-align: top;">
                                    <table style="padding: 5px;" cellpadding="0" cellspacing="0">
                                        <colgroup>
                                            <col width="70px" />
                                            <col width="120px" />
                                        </colgroup>
                                        <tr class="price">
                                            <td>
                                                <label>Item Price:</label></td>
                                            <td>&nbsp;<span><%=Model.Price.GetCurrency() %></span></td>
                                        </tr>
                                        <tr class="price">
                                            <td>
                                                <label>Shipping & handling:</label></td>
                                            <td><span><%=isShipping?"+"+(Model.Shipping + insurance).GetCurrency() : "&nbsp;Free shipping"%></span></td>
                                        </tr>
                                        <% if (cuser == null)
                                           { %>
                                        <tr>
                                            <td colspan="2" style="text-align: center">
                                                <h5 class="item_info">Please <%= Html.ActionLink("Log in", "LogOn", "Account", new { @ReturnUrl = Url.Action("DealOfTheWeek", "Auction", new { id = Model.LinkParams.ID }) }, null).ToSslLink() %> or <%= Html.ActionLink("Register", "Register", "Account").ToSslLink() %> before checking out.
                      </h5>
                                            </td>
                                        </tr>
                                        <% }
                                           else
                                           {
                                               if (Model.Quantity > 0)
                                               { %>
                                        <tr class="price" style="vertical-align: middle">
                                            <td>
                                                <label>Quantityqq:</label></td>
                                            <td>
                                                <input id="qq" name="qq" size="5" maxlength="10" style="position: relative;" value="" onkeypress="return validateNumeric(event)" onkeyup="updateTotal(this.value)" onclick="return false" type="text" /></td>
                                        </tr>
                                        <tr>
                                            <td colspan="2">
                                                <hr />
                                            </td>
                                        </tr>
                                        <tr class="price" style='height: 40px'>
                                            <td>
                                                <label>Total:</label></td>
                                            <td><span><strong id="totalqqq">Enter quantity</strong></span></td>
                                        </tr>
                                        <% if (Model.Owner_ID != cuser.ID)
                                           {%>
                                        <tr>
                                            <td colspan="2" style="text-align: center">
                                                <button type="submit" style="width: 95px;" class="cssbutton small white" id="Itakeivvvt" name="Itcccakeit">
                                                    <span>I'll take it</span></button>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="2" style='height: 50px;'>
                                                <span style="text-align: center;" id="error">&nbsp;</span>
                                            </td>
                                        </tr>
                                        <% }
                                           else
                                           {%>
                                        <tr>
                                            <td colspan="2">
                                                <h5 class="item_info">You can't bid on this item.<br />
                                                    Possible reasons:
                                                    <br />
                                                    1. This is Your own item<br />
                                                    2. Your account can only place items to sell.
                            </h5>
                                            </td>
                                        </tr>
                                        <% }
                                               }
                                               else
                                               { %>
                                        <tr>
                                            <td>
                                                <div class="sold">SOLD OUT</div>
                                            </td>
                                        </tr>
                                        <% }
                                           }%>
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <%--   <div class="blue-bg" style="vertical-align: middle">
                            <% if (currentAuctionImages.Count() > 0)
                               {
                                   string imagePath = string.Empty;
          %>
                            <div class="arrowlft">
                                <img src="<%=AppHelper.CompressImage("arrowlft_1.jpg") %>" alt="" onclick="javascript:displayImage('previous','<%=Model.LinkParams.ID %>');" />
                            </div>
                            <div id="imagesDiv" class="imgsDiv">
                                <div id="imagesContainer" class="imgsContainer" style="line-height: 98px; vertical-align: middle">
                                    <table>
                                        <tr>
                                            <% 
                                   foreach (var Image in currentAuctionImages)
                                   {
                                       if (!string.IsNullOrEmpty(imagePath))
                                       {
                                           imagePath = imagePath + "|" + Image.PicturePath;
                                       }
                                       else
                                       {
                                           imagePath = Image.PicturePath;
                                       }

                                       if (Image.Default)
                                       {
                      %>
                                            <td style="padding-left: 4px; padding-right: 4px; vertical-align: middle">
                                                <a href="javascript:swap_image('<%= AppHelper.CompressAuctionImage(Model.LinkParams.ID, Image.PicturePath)%>','<%= string.Format("{0}", Image.PicturePath)%>');">
                                                    <img class="imagesPadg" src="<%=AppHelper.CompressAuctionImage(Model.LinkParams.ID, Image.ThumbNailPath)%>" id="<%=Image.PicturePath %>" style="border: 2px solid #FFFFFF; max-width: 80px; max-height: 80px" alt="" />
                                                </a>
                                            </td>
                                            <% }
                                       else
                                       { %>
                                            <td style="padding-left: 4px; padding-right: 4px; vertical-align: middle">
                                                <a href="javascript:swap_image('<%=AppHelper.CompressAuctionImage(Model.LinkParams.ID, Image.PicturePath) %>','<%= string.Format("{0}", Image.PicturePath)%>');">
                                                    <img class="imagesPadg" src="<%=AppHelper.CompressAuctionImage(Model.LinkParams.ID, Image.ThumbNailPath)%>" id="<%=Image.PicturePath %>" alt="" style="max-width: 80px; max-height: 80px" />
                                                </a>
                                            </td>
                                            <% }
                                   }%>
                                            <td style="width: 80%"></td>
                                        </tr>
                                    </table>
                                </div>
                            </div>
                            <div class="arrowrgt">
                                <img src="<%=AppHelper.CompressImage("arrowrgt_1.jpg") %>" onclick="javascript:displayImage('next','<%=Model.LinkParams.ID %>');" alt="" />
                            </div>
                            <div class="clr"></div>
                            <input type="hidden" id="imagepathList" name="imagepathList" value="<%=imagePath %>" />
                            <input type="hidden" id="isDefaultImagePresent" name="isDefaultImagePresent" value="<%=currentAuctionImages.First().Default %>" />
                            <%
                               }
         
                        </div> 
                    </td>
                </tr>
            </table>--%>
        </form>
    </div>
    <script type="text/javascript">
        var totalprice = parseFloat("<%=Convert.ToDecimal(Model.Price+Model.Shipping + insurance).GetPrice() %>");
        var totalquantity = parseInt("<%=Model.Quantity %>");
  </script>
</asp:Content>
<asp:Content runat="server" ID="jsc" ContentPlaceHolderID="cphScriptBottom">
    <script type="text/javascript">
        if (document.getElementById('Itakeit') != null)
            document.getElementById('Itakeit').disabled = true;
        if (document.getElementById("isDefaultImagePresent") != null)
            if (document.getElementById("imagepathList") != null) {
                var imagePathList = document.getElementById("imagepathList").value.split("|");
                document.getElementById(imagePathList[0]).style.border = "2px solid #FFFFFF";
            }

  </script>

    <script type="text/javascript" src="https://code.jquery.com/jquery-1.11.3.min.js"></script>
    <script type="text/javascript" src="http://afarkas.github.io/webshim/js-webshim/minified/polyfiller.js"></script>
    <script type="text/javascript" src="../../public/scripts/sticky.js"></script>
    <script type="text/javascript" src="../../public/scripts/flexslider.js"></script>
    <script type="text/javascript" src="../../public/scripts/owl.carousel.min.js"></script>
    <script type="text/javascript" src="../../public/scripts/bootstrap.js"></script>
    <script type="text/javascript" src="../../public/scripts/scripts.js"></script>

</asp:Content>
