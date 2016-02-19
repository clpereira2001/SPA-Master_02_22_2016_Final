<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IPagedList<MyBid>>" %>
<%@ Import Namespace="Vauction.Models.CustomModels" %>
<%@ Import Namespace="Vauction.Utils.Paging" %>
<br class="clear" />
<br class="clear" />
Note: If the price is <span style="color: green; font-weight: bold">green</span> then you are the high bidder of this item.
<br class="clear" />
<br class="clear" />
<% AuctionFilterParams filterParams = (AuctionFilterParams)ViewData["filterParam"];%>
<div class="pager_pos" style="vertical-align: middle">
    <%if (Model.TotalItemCount > Model.PageSize)
      { %>
    <%= Html.Pager(Model.PageSize, Model.PageNumber, Model.TotalItemCount, CollectParameters.CollectAll())%>
    <%} %>
</div>
<br class="clear" />
<%  
    if (Model.Count() == 0)
        Response.Write("You have no bids in this auction.");
    else
    {%>
<div class="row">
    <div class="col-sm-12">
        <div class="table-responsive">
            <table id="item_view" class="table table-hover table-striped margin-bottom-none">
                <thead>
                    <tr class="bg-primary">
                        <th>Lot</th>
                        <th>Title</th>
                        <th>Date</th>
                        <th>Bid Amount</th>
                        <th>Max. Bid</th>
                        <th>Winning Bid</th>
                    </tr>
                </thead>
                <tbody>
                    <%
        string address;
        int count = 0;
        foreach (var item in Model)
        {
            count++;          
    %>

                    <% if (count % 2 == 1)
                       {%>
                    <tr >
                        <%}
                       else
                       {%>
                        <tr>
                            <%}%>
                            <td class="align-middle" data-label="lot">
                                <%=Html.ActionLink(item.LinkParams.Lot.ToString(), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = item.LinkParams.ID, evnt = item.LinkParams.EventUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.LotTitleUrl })%>
        </td>

                            <td>
                                <%=Html.ActionLink(item.LinkParams.Title, "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = item.LinkParams.ID, evnt = item.LinkParams.EventUrl, cat = item.LinkParams.CategoryUrl, lot = item.LinkParams.LotTitleUrl })%>
        </td>
                            <%
                       Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner ? "style=\"color:green\"" : String.Empty, item.DateMade));
                       Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner ? "style=\"color:green\"" : String.Empty, item.Amount.GetCurrency()));
                       Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner ? "style=\"color:green\"" : String.Empty, item.MaxBid.GetCurrency()));
                       Response.Write(String.Format("<td {0}>{1}</td>", item.IsWinner ? "style=\"color:green\"" : String.Empty, item.CurrentBid));
       %>
                        </tr>
                        <%
                   address = AppHelper.AuctionImage(item.LinkParams.ID, item.ThubnailImage);
                   if (!System.IO.File.Exists(Server.MapPath(address))) continue;
    %>
                        <%=((count % 2 == 1) ? "<tr style=\"background-color:#EFEFEF\">" : "<tr>")%>
                        <td colspan="6">
                            <a href="<%= Url.Action ("AuctionDetail", "Auction", new CollectParameters("Auction", "AuctionDetail", item.LinkParams.ID).Collect("page", "ViewMode"))%>">
                                <img src="<%=AppHelper.CompressImagePath(address) %>" alt="" />
                            </a>
                        </td>
                    </tr>
                    <%     
        }
    }
   %>
                </tbody>
            </table>
        </div>
    </div>
</div>
<%if (Model.TotalItemCount > Model.PageSize)
  { %>
<div class="pager_pos" style="vertical-align: middle">
    <%= Html.Pager(Model.PageSize, Model.PageNumber, Model.TotalItemCount, CollectParameters.CollectAll())%>
</div>
<%} %>

<script src="<%=AppHelper.ScriptUrl("jquery.cookie.js") %>" type="text/javascript"></script>
<script type="text/javascript">
    $(document).ready(function () {
        $("select.span-3.viewselect").change(function (e) {
            $.cookie("ViewMode", this.value, { path: "/" })
            location.reload();
        })
    });
</script>
