<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PreviewBid>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Preview Bid - <%=Consts.CompanyTitleName%></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%  int intCnt = 0;
        bool yes = true;
        List<Vauction.Models.Image> currentAuctionImages = ViewData["CurrentAuctionImages"] as List<Vauction.Models.Image>;
        long auction_id = Convert.ToInt64(ViewData["auction_id"]);
        string path = "";             //List<Vauction.Models.Image> lstImage = new ImageRepository.GetAuctionImages(Model.LinkParams.ID, true);


        if (currentAuctionImages != null && currentAuctionImages.Count > 0)
        {
            foreach (Vauction.Models.Image Image in currentAuctionImages)
            {
                path = AppHelper.AuctionImage(auction_id, currentAuctionImages[0].PicturePath);
                //path = AppHelper.AuctionImage(auction_id, Image.PicturePath);
                //if (!System.IO.File.Exists(Server.MapPath(path))) continue;

            }
        }
    %>
    <% using (Html.BeginForm("PlaceBid", "Auction", FormMethod.Post))
       { %>
    <%=Html.AntiForgeryToken() %>
    <div class="container margin-bottom-xl">
        <div class="row">
            <div class="col-sm-12">
            </div>
        </div>

        <div class="row">
            <div class="col-sm-6">

                <%-- <% Html.RenderAction("pAuctionImages", "Auction", new { auction_id = Model.LinkParams.ID }); %>--%>
                <img src="<%=AppHelper.CompressImagePath(AppHelper.AuctionImage(Model.LinkParams.ID,currentAuctionImages.FirstOrDefault().PicturePath)) %>" class="img-responsive" alt="" draggable="false" />
            </div>
            <div class="col-sm-6">
                <p>
                    <span class="bg-color-fourth text-uppercase text-white padding-vertical-sm padding-horizontal-sm display-inline-block text-weight-700">Lot # <%=Model.LinkParams.Lot %></span>
                </p>
                <p class="text-lg text-primary">
                    <%=Model.LinkParams.Title %>
                </p>

                <div class="bg-white padding-vertical-sm padding-horizontal-sm">
                    <p class="padding-bottom-xs text-uppercase border-bottom border-weight-xs border-success text-weight-700">Your Maximum Bid <span class="text-weight-300 pull-right"><%=Model.Amount.GetCurrency() %></span></p>
                    <p class="padding-bottom-xs text-uppercase border-bottom border-weight-xs border-success text-weight-700">Bid Amount <span class="text-weight-300 pull-right"><%=Model.RealAmount.GetCurrency() %></span></p>
                    <p class="padding-bottom-xs text-uppercase border-bottom border-weight-xs border-success text-weight-700">Quantity <span class="text-weight-300 pull-right"><%=Model.Quantity %></span></p>
                    <p class="padding-bottom-xs text-uppercase border-bottom border-weight-xs border-success text-weight-700">Use Proxy Bidding <span class="text-weight-300 pull-right"><%=Model.IsProxy?"Yes":"No" %></span></p>
                    <p class="padding-top-sm padding-bottom text-uppercase border-bottom border-weight-xs border-success text-weight-700 text-danger overflow-hidden"><span class="margin-top-sm display-inline-block">Total Bid Amount</span> <span class="pull-right text-primary text-lg"><%=Model.TotalRealAmount.GetCurrency()%></span></p>
                    <% =Html.Hidden("BidAmount", Model.Amount)%>
                    <% =Html.Hidden("id", Model.LinkParams.ID) %>
                    <% =Html.Hidden("ProxyBidding", Model.IsProxy )%>
                    <% =Html.Hidden("Quantity", Model.Quantity )%>
                    <% =Html.Hidden("RealBidAmount", Model.RealAmount )%>

                    <% if (Model.Amount < Model.RealAmount || Model.IsOutBid)
                       { %>
       Sorry, but you are not allowed to make this bid, because the minimum bid amount is greater than yours bid amount.
                    <br />
                    <br />
                    <%}
                       else %>
                    <% if (!Model.IsQuantityBig)
                       {%>
                    <% if (Model.QuantityType != 255)
                       {
                           Response.Write(String.Format("Your previous quantity was {0} and you are currently winning {3} of {4}. You are choosing to {1} your quantity to {2}.<br/><br />", Model.PreviousQuantity, (Model.QuantityType == 0) ? "decrease" : "increase", Model.Quantity, Model.WinnerBidsCount, Model.WinnerBidsCount + Model.LoserBidsCount));
                       }
                    %>
                    <% if (!Model.IsUpdate)
                       {%>
            If the information above is not correct, use the "CANCEL BID" button to return to the item and correct your bid. If you hit "PLACE BID" the bid becomes final. Once placed, you can not retract the bid.
            <br />
                    <br />
                    <% }
                       else
                       {
                    %>

                    <p class="text-xs">If the information above is not correct, use the "CANCEL BID" button to return to the item and correct your bid. If you hit "PLACE BID" the bid becomes final. Once placed, you can not retract the bid.</p>

                    <br />
                    <br />
                    You are currently the high bidder for this item with:
      <br />
                    <br />
                    Your Current Bid: <%=Model.PreviousAmount.GetCurrency() %>
                    <% if (Model.PreviousMaxBid > 0)
                       { %>
                    <br />
                    Maximum bid of <%=Model.PreviousMaxBid.GetCurrency()%>
                    <% }
                       else
                       {%>
                    <br />
                    Maximum bid of <%=Model.PreviousAmount.GetCurrency()%>
                    <%} %>
                    <br />
                    <br />
                    <% if ((Model.Amount != Model.PreviousMaxBid || Model.RealAmount != Model.PreviousAmount))
                       {%>
                    <% if (Model.Amount > Model.PreviousMaxBid)
                       {%> Please confirm if you would like to raise your maximum bid to <%=Model.Amount.GetCurrency() %><%}
                       else
                       { %>
                    <b>Note: </b>Placing this bid does not change your maximum bid of <%=Model.PreviousMaxBid.GetCurrency() %>, it will only increase your current bid from <%=Model.PreviousAmount.GetCurrency() %> to <%=Model.RealAmount.GetCurrency() %>. 
             <br />
                    <br />
                    Please confirm if you would like to raise your bid amount to <%=Model.RealAmount.GetCurrency() %><%} %>
                    <%=(Model.QuantityType!=255)?String.Format(" and to {0} the quantity",((Model.QuantityType == 0) ? "decrease" : "increase")):"" %>
                    <%}
                       else
                       {
                           yes = false; %> Sorry, but you are not allowed to make this bid, because it will not raise your maximum bid or bid amount. <%}
                       } %>

                    <% if (yes)
                       { %>
                    <p class="margin-top-lg">
                        <%intCnt = 1; %>
                        <%-- <a class="text-uppercase btn btn-danger btn-lg" href="#">Place Bid</a>--%>
                        <button type="button" class="text-uppercase btn btn-danger btn-lg" id="btnPlace" onclick="submit();">
                            <span>Place Bid</span>
                        </button>
                        <input type="submit" id="btnSubmit" style="display: none" />
                           <button type="button" class="text-uppercase btn btn-success btn-lg" onclick="window.location='<%= Url.Action ("AuctionDetail", "Auction", new { controller = "Auction", action = "AuctionDetail", id = Model.LinkParams.ID, evnt=Model.LinkParams.EventUrl, cat=Model.LinkParams.CategoryUrl, lot=Model.LinkParams.LotTitleUrl })%>'">
                            <span>Cancel Bid</span>
                        </button>
                        <div id="dvAnimation" style="color: rgb(180,0,0); text-align: center; display: none">
                            <strong>Please wait for a while, the request is processed.</strong>
                            <br />
                            <br />
                            <br />
                            <img src="<%=AppHelper.CompressImage("throbber2.gif") %>" alt="" />
                        </div>
                        <%} %>
                        <% }
                       else
                       { %>
         Sorry, but you are not allowed to lower your quantity with a new bid as long as you have a winning bid on the auction.<br />
                        <%--<br />--%>
                        <%} %>
                        <%if (intCnt==1){ %>
                        <%} %>
                        <%
                        else
                        {%>
                        <button type="button" class="text-uppercase btn btn-success btn-lg" onclick="window.location='<%= Url.Action ("AuctionDetail", "Auction", new { controller = "Auction", action = "AuctionDetail", id = Model.LinkParams.ID, evnt=Model.LinkParams.EventUrl, cat=Model.LinkParams.CategoryUrl, lot=Model.LinkParams.LotTitleUrl })%>'">
                            <span>Cancel Bid</span>
                        </button>
                      
                         <% } %>
                    </p>
                </div>
            </div>
        </div>
        <%
       }            
        %>
    </div>

    <script type="text/javascript">
        $("#btnPlace").click(function () {
            $(".cssbutton").hide();
            $("#dvAnimation").show();
            $("#btnSubmit").click();
        });
    </script>
</asp:Content>
