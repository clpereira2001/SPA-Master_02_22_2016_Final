<%@ Control Language="C#" AutoEventWireup="true" Inherits="ViewUserControl<IPagedList<AuctionShort>>" %>

<%@ Import Namespace="Vauction.Models.CustomModels" %>
<%@ Import Namespace="Vauction.Utils.Paging" %>

<% GeneralFilterParams filterParams = (GeneralFilterParams)ViewData["filterParam"];
   bool ShowPager = (ViewData["HidePager"] == null);
   bool IsRefreshAvailable = ViewData["IsRefreshAvailable"] != null && Convert.ToBoolean(ViewData["IsRefreshAvailable"]);
   if (ShowPager && (Model.TotalItemCount > 0 || IsRefreshAvailable))
   {
%>

<%--<div class="span-14 last">
        <% if (IsRefreshAvailable)
           {%>
        <div style="width: 150px; float: left; text-align: left;">
            <span id="lnkRBR">
                <img src="<%=AppHelper.CompressImage("refresh.gif") %>" style="margin-top: 5px" />
                <u>refresh current bids</u></span>
            <span id="lnkRBR_loading">
                <img src="<%=AppHelper.CompressImage("bid_result_loader.gif") %>" alt="" />&nbsp;refreshing current bids</span>
        </div>
        <%} %>
        <% if (Model.TotalItemCount > 0)
           { %>


        <div >
            <label>View as:</label>
            <%= Html.DropDownList("ViewModeTop", BaseHelpers.GetAuctionViewModeList((Consts.AuctonViewMode)filterParams.ViewMode), new { @class="span-3 viewselect form-control" })%>
        </div>
        <%} %>
    </div>--%>

<%-- <div class="span-14 last pager">
        <%if (ViewData["IsHomePage"] == null)
          {%>
        <%= Html.Pager(Model.PageSize, Model.PageNumber, Model.TotalItemCount, CollectParameters.CollectAll())%>
        <%} %>
    </div>--%>


<div>
    <%--class="col-md-12 col-sm-12">--%>
    <%if (Model.TotalItemCount > Model.PageSize)
      { %>
    <%= Html.Pager(Model.PageSize, Model.PageNumber, Model.TotalItemCount, CollectParameters.CollectAll())%>
    <%} %>
</div>
<%-- <div class="row text-center">
        <% if (IsRefreshAvailable)
           {%>
        <div style="width: 150px; float: left; text-align: left;">
            <span id="lnkRBR_b">
                <img src="<%=AppHelper.CompressImage("refresh.gif") %>" style="margin-top: 5px" />
                <u>refresh current bids</u></span>
            <span id="lnkRBR_loading_b">
                <img src="<%=AppHelper.CompressImage("bid_result_loader.gif") %>" alt="" />&nbsp;refreshing current bids</span>
        </div>
        <%}%>
        <div style="width: 150px; float: right; text-align: right; margin-right: 5px;" class="span-3 last viewform ie_viewform">
            <label>View as:</label>
            <%= Html.DropDownList("ViewModeTop", BaseHelpers.GetAuctionViewModeList((Consts.AuctonViewMode)filterParams.ViewMode), new { @class = "span-3 viewselect form-control" })%>
        </div>
    </div>--%>

<div class="row text-center">
    <div class="col-sm-4">
        <form class="form-horizontal">
            <div class="form-group text-left">
                <div class="col-sm-12">
                    <label for="col" class="control-label text-left pull-left main-label"><%=Model.TotalItemCount %> Items</label>

                </div>
            </div>
        </form>
    </div>
    <div class="col-md-4 col-sm-2 col-xs-4">
        <%--<%= Html.DropDownList("ViewModeTop", BaseHelpers.GetAuctionViewModeList((Consts.AuctonViewMode)filterParams.ViewMode), new { @class="span-3 viewselect form-control" })%>--%>
        <ul class="list-unstyled list-inline product-view" id="uItem"  name="uItem">
            <li value="1">
                <a href="#" class="fa fa-th-large grid-product active"></a>
            </li>
            <li value="0">
                <a href="#" class="fa fa-list list-product"></a>
            </li>
        </ul>
    </div>

</div>


<%}%>

<%Html.RenderPartial(((Consts.AuctonViewMode)filterParams.ViewMode == Consts.AuctonViewMode.Grid) ? "~/Views/Auction/_GridView.ascx" : "~/Views/Auction/_ListView.ascx", Model); %>

<%if (ShowPager && Model.TotalItemCount > Model.PageSize)
  { %>
<div>
    <%if (Model.TotalItemCount > Model.PageSize)
      { %>
    <%= Html.Pager(Model.PageSize, Model.PageNumber, Model.TotalItemCount, CollectParameters.CollectAll())%>
    <%} %>
</div>
<%-- <div class="row text-center">
        <% if (IsRefreshAvailable)
           {%>
        <div style="width: 150px; float: left; text-align: left;">
            <span id="lnkRBR_b">
                <img src="<%=AppHelper.CompressImage("refresh.gif") %>" style="margin-top: 5px" />
                <u>refresh current bids</u></span>
            <span id="lnkRBR_loading_b">
                <img src="<%=AppHelper.CompressImage("bid_result_loader.gif") %>" alt="" />&nbsp;refreshing current bids</span>
        </div>
        <%}%>
        <div style="width: 150px; float: right; text-align: right; margin-right: 5px;" class="span-3 last viewform ie_viewform">
            <label>View as:</label>
            <%= Html.DropDownList("ViewModeTop", BaseHelpers.GetAuctionViewModeList((Consts.AuctonViewMode)filterParams.ViewMode), new { @class = "span-3 viewselect form-control" })%>
        </div>
    </div>--%>

<div class="row text-center">
    <div class="col-sm-4">
        <form class="form-horizontal">
            <div class="form-group text-left">
                <div class="col-sm-12">
                    <label for="col" class="control-label text-left pull-left main-label"><%=Model.TotalItemCount %> Items</label>

                </div>
            </div>
        </form>
    </div>
    <div class="col-md-4 col-sm-2 col-xs-4">
        <%--<%= Html.DropDownList("ViewModeTop", BaseHelpers.GetAuctionViewModeList((Consts.AuctonViewMode)filterParams.ViewMode), new { @class="span-3 viewselect form-control" })%>--%>
        <ul class="list-unstyled list-inline product-view" id="uItem1" name="uItem1">
            <li value="1">
                <a href="#" class="fa fa-th-large grid-product active"></a>
            </li>
            <li value="0">
                <a href="#" class="fa fa-list list-product"></a>
            </li>
        </ul>
    </div>

</div>

<%}%>
<script type="text/javascript">$("#lnkRBR_loading, #lnkRBR_loading_b").hide();</script>
<script type="text/javascript">
    //$('#uItem li').click(function () {
    //    var $this = $(this);
    //    alert($this.text() + ' \nIndex ' + $this.index());
    //})
</script>
<script src="<%=AppHelper.CompressScript("jquery.cookie.js") %>" type="text/javascript"></script>
<% if (IsRefreshAvailable)
   { %>
<%=Html.Hidden("hd_pgpms", ViewData["PageParams"]) %>
<%=Html.Hidden("hd_prem", ViewData["PreviewMethod"])%>
<script src="<%=AppHelper.CompressScript("table.js") %>" type="text/javascript"></script>
<%} %>
<script type="text/javascript">
    $(document).ready(function () {
        $("select.span-3.viewselect.form-control").change(function (e) {
            $.cookie("ViewMode", this.value, { path: "/" })
            location.reload();
        })

        $('#uItem li').click(function () {

            $.cookie("ViewMode", $(this).attr('value'), { path: "/" })
            location.reload();
        })

        $('#uItem1 li').click(function () {

            $.cookie("ViewMode", $(this).attr('value'), { path: "/" })
            location.reload();
        })
  <% if (IsRefreshAvailable)
     { %>
        $("#lnkRBR, #lnkRBR_b").click(function () {
            $("#lnkRBR,#lnkRBR_b").hide();
            $("#lnkRBR_loading,#lnkRBR_loading_b").show();
      <% if ((Consts.AuctonViewMode)filterParams.ViewMode == Consts.AuctonViewMode.List)
         { %>
            UpdateTableView();
      <%}
         else
         { %>
            UpdateGridView();
            <%} %>
        });
        <%} %>
    });

</script>
