<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<UserBidWatch>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Watch List - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">

        <div class="row">
            <div class="col-sm-12">
                <p class="text-primary text-weight-900 text-lg">Watch list</p>
            </div>
        </div>
        <%
            if (Model == null || !Model.Any())
            { %>
        <p style="font-weight: bold; color: #000">There are currently no items in your Watch List.</p>
        <p style="font-weight: normal; color: #000">If you would like to add items to your Watch List, view an item and select the "Add this item to your Watch list" link or make a bid.</p>
        <%}
            else
            { %>
    Your Watch List shows items you are watching.<br />

        <div class="col-xs-12 col-sm-12 col-md-12 col-lg-12">
            <div class="main-table">
                <table class="table table-hover table-striped">
                    <tbody>
                        <tr>
                            <td align="left">
                                <font color="black">Black</font>
                            </td>
                            <td>- you are watching this item
          </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <font color="red">Red</font>
                            </td>
                            <td>- you are not currently the high bidder
          </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <font color="green">Green</font>
                            </td>
                            <td>- you are currently the high bidder
          </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>


        <div style="width: 150px; float: right; text-align: right; margin-bottom: 10px">
            <span id="lnkRBR">
                <img src="<%=AppHelper.CompressImage("refresh.gif") %>" style="margin-top: 5px" />
                <u>refresh current bids</u></span>
            <span id="lnkRBR_loading">
                <img src="<%=AppHelper.CompressImage("bid_result_loader.gif") %>" alt="" />&nbsp;refreshing current bids</span>
            <script type="text/javascript">$("#lnkRBR_loading").hide();</script>
        </div>

        <%--<div class="row">--%>
        <div >
            <div class="main-table">
                <table class="table table-hover table-striped">

                    <thead>

                        <tr class="bg-primary">

                            <th class="align-middle">Lot
        </th>
                            <th class="text-center align-middle">Title</th>
                            <% if (AppHelper.CurrentUser.IsHouseBidder)
                               { %>
                            <th class="text-center align-middle">&nbsp;</th>
                            <% } %>
                            <th class="text-center align-middle">Highbidder
        </th>
                            <th class="text-center align-middle">Current Bid
        </th>
                            <th class="text-center align-middle">Your Bid
        </th>
                            <th class="text-center align-middle">Your Max Bid
        </th>
                            <th class="text-center align-middle">QTY
        </th>
                            <th>&nbsp;
        </th>
                        </tr>
                    </thead>
                    <tbody>
                        <%      
                    foreach (UserBidWatch item in Model)
                    {
        %>
                        <tr>
                            <td>
                                <%= Html.ActionLink(item.LinkParams.Lot.ToString(), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = item.LinkParams.ID }) %>
      </td>
                            <td>
                                <%= Html.ActionLink(item.LinkParams.Title, "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = item.LinkParams.ID})%>
      </td>
                            <% if (AppHelper.CurrentUser.IsHouseBidder)
                               { %>
                            <td><%=(int)item.Cost %></td>
                            <% } %>

                            <td>
                                <%
                        switch (item.Option)
                        {
                            case 0:
                                Response.Write("<span id='col1_" + item.LinkParams.ID + "' style=\"color:red\">" + item.HighBidder + "</span>");
                                break;
                            case 1:
                                Response.Write("<span id='col1_" + item.LinkParams.ID + "' style=\"color:green\">" + item.HighBidder + "</span>");
                                break;
                            default:
                                Response.Write("<span id='col1_" + item.LinkParams.ID + "'>" + item.HighBidder + "</span>");
                                break;
                        }
       %>
      </td>
                            <td>
                                <%switch (item.Option)
                                  {
                                      case 0:
                                          Response.Write("<span id='col2_" + item.LinkParams.ID + "' style=\"color:red\">" + item.CurrentBid + "</span>");
                                          break;
                                      case 1:
                                          Response.Write("<span id='col2_" + item.LinkParams.ID + "' style=\"color:green\">" + item.CurrentBid + "</span>");
                                          break;
                                      default:
                                          Response.Write("<span id='col2_" + item.LinkParams.ID + "' >" + item.CurrentBid + "</td>");
                                          break;
                                  }%>
       </td>
                            <td>
                                <%
                        switch (item.Option)
                        {
                            case 0:
                                Response.Write("<span id='col3_" + item.LinkParams.ID + "' style=\"color:red\">" + item.Amount.GetCurrency() + "</span>");
                                break;
                            case 1:
                                Response.Write("<span id='col3_" + item.LinkParams.ID + "' style=\"color:green\">" + item.Amount.GetCurrency() + "</span>");
                                break;
                            default:
                                Response.Write("<span id='col3_" + item.LinkParams.ID + "' >&nbsp;</span>");
                                break;
                        }
       %>
       </td>
                            <td>
                                <%switch (item.Option)
                                  {
                                      case 0:
                                          Response.Write("<span id='col4_" + item.LinkParams.ID + "' style=\"color:red\">" + item.MaxBid.GetCurrency() + "</span>");
                                          break;
                                      case 1:
                                          Response.Write("<span id='col4_" + item.LinkParams.ID + "' style=\"color:green\">" + item.MaxBid.GetCurrency() + "</span>");
                                          break;
                                      default:
                                          Response.Write("<span id='col4_" + item.LinkParams.ID + "' >&nbsp;</span>");
                                          break;
                                  }%>
      </td>
                            <td>
                                <span id='col5_<%=item.LinkParams.ID%>'><%=item.Quantity.HasValue?item.Quantity.Value.ToString():String.Empty %></span>
                            </td>
                            <td>
                                <span id='col6_<%=item.LinkParams.ID%>'><%=(item.Option != 2) ? "&nbsp;" : Html.ActionLink("Remove", "RemoveBid", new { controller = "Account", action = "RemoveBid", id = item.LinkParams.ID }, new { @onclick = "return confirm('Do you really want to remove this item from your Watch list?')" }).ToHtmlString()%></span>
                            </td>
                            <%}%>
                        </tr>
                    </tbody>
                </table>

                <% }%>
            </div>
        </div>
        <%-- </div>--%>
    </div>

    <script type="text/javascript">
        $("#bids_table tr:even").css("background-color", "#d9e9f0");
</script>
</asp:Content>

<asp:Content runat="server" ID="jsC" ContentPlaceHolderID="cphScriptBottom">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#lnkRBR").click(function () {
                $("#lnkRBR").hide();
                $("#lnkRBR_loading").show();
                $.post('/Account/UpdateWatchListPage', function (data) {
                    if (data != "null") {
                        $.each(data, function (i, item) {
                            $("#col1_" + item.id).html(item.col1);
                            $("#col2_" + item.id).html(item.col2);
                            $("#col3_" + item.id).html(item.col3);
                            $("#col4_" + item.id).html(item.col4);
                            $("#col5_" + item.id).html(item.col5);
                            $("#col6_" + item.id).html(item.col6);
                        });
                    }
                    $("#lnkRBR").show();
                    $("#lnkRBR_loading").hide();
                });
            });
        });
</script>
</asp:Content>
