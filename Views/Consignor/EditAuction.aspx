<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<string>" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Consigned Items - <%=Consts.CompanyTitleName %> </title>
<link href="<%= this.ResolveUrl("~/public/css/custom-gray/jquery-ui-1.7.2.css" ) %>" rel="stylesheet" type="text/css" />
<% Html.Style("ui.jqgrid.css"); %>
<% Html.Script("jquery-ui-1.7.2.custom.min.js"); %>
<% Html.Script("grid.locale-en.js"); %>
<% Html.Script("jquery.jqGrid.min.js"); %>
<% Html.Script("general.js"); %>
</asp:Content>
<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="center_content">
<%--<h1 class="text-primary text-weight-900 text-lg border-bottom border-weight-xs border-primary">Seller Tools Consigned Items</h1>--%>
<br />
<table id="c_list"></table>
<div id="c_pager"></div>
</div>
<div class="back_link">
<%=Html.ActionLink("Return to Seller Tool page", "Index", new {controller="Consignor", action="Index"}) %>
</div>
<% using (Html.BeginForm("ExportConsignorItemsToExcel", "Consignor", FormMethod.Post))
   { %>
<%=Html.Hidden("lot", "")%>
<%=Html.Hidden("Event", "")%>
<%=Html.Hidden("auction", "")%>
<%=Html.Hidden("category", "")%>
<%=Html.Hidden("price", "")%>
<%=Html.Hidden("cost", "")%>
<%=Html.Hidden("shipping", "")%>
<%=Html.Hidden("internalID", "")%>
<button id="btnExport" type="submit" value="Export" style="display: none"></button>
<%} %>
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="cphScriptBottom" runat="server">
    <script type="text/javascript">
  $(document).ready(function () {
    var grid = $("#c_list").jqGrid({
      mtype: 'GET',
      url: '/Consignor/GetConsignedItems/',
      datatype: "json",
      height: 600,
      width: 950,
      colNames: ['#', 'Lot#', 'Event', 'Auction', 'Category', 'Price', 'Cost', 'Shipping', 'Internal ID', 'C-Ship', 'Start Date', 'End Date', 'Status'],
      colModel: [
         { name: 'Auction_ID', index: 'Auction_ID', width: 50, key: true },
         { name: 'Lot', index: 'Lot', width: 50, sortable: false },
         { name: 'Event', index: 'Event', width: 120, sortable: false, stype: 'select', editoptions: { value: "<%=ViewData["Events"] %>" } },
 		      { name: 'Auction', index: 'Auction', width: 100, sortable: false },
 		      { name: 'Category', index: 'Category', width: 140, sortable: false },
 		      { name: 'Price', index: 'Price', width: 90, sortable: false, align: 'right' },
 		      { name: 'Cost', index: 'Cost', width: 90, sortable: false, align: 'right' },
 		      { name: 'Shipping', index: 'Shipping', width: 60, sortable: false, align: 'right' },
 		      { name: 'InternalID', index: 'InternalID', width: 90, sortable: false },
 		      { name: 'CShip', index: 'CShip', width: 30, align: 'center', sortable: false, formatter: 'checkbox', editable: false, edittype: "checkbox", editoptions: { value: "True:False" }, formoptions: { elmprefix: "    ", elmsuffix: "" }, search: false },
 		      { name: 'DateStart', index: 'DateStart', width: 90, align: 'center', sortable: false, search: false },
 		      { name: 'DateEnd', index: 'DateEnd', width: 75, align: 'center', sortable: false, search: false },
 		      { hidden: true, name: 'Status', index: 'Status', width: 50, sortable: false, search: false }
       ],
       loadtext: 'Loading ...',
       rowNum: 30,
       rowList: [20, 25, 30, 35, 40, 45, 50],
       pager: 'c_pager',

       viewrecords: true,
       shrinkToFit: false,
       loadComplete: function () {
         $("#c_list a").css({ "color": "navy", "text-decoration": "underline" });
       }
     });
     grid.jqGrid('navGrid', '#c_pager', { edit: false, add: false, del: true, deltext: 'Delete lot', search: false, refreshtext: "Refresh", afterRefresh: function () { grid[0].clearToolbar(); } }, {}, {},
     { jqModal: false, closeOnEscape: true, closeAfterAdd: true, bSubmit: "Delete", afterSubmit: AfterSubmitFunction, url: "/Consignor/DeletePendingAuction/" }, {}).navSeparatorAdd('#c_pager');
     grid.jqGrid('navButtonAdd', '#c_pager', { caption: "Export", buttonicon: 'ui-icon-contact', title: 'Export lots to excel', onClickButton: function () { ExportItemsGridToExcel(); } }).navSeparatorAdd('#c_pager');
     grid.jqGrid('filterToolbar');
   });

      function ExportItemsGridToExcel() {
        var sr = $("#c_list").jqGrid('getPostData', 'Auction_ID');
        $("#lot").attr('value', '');
        $("#Event").attr('value', '');
        $("#auction").attr('value', '');
        $("#category").attr('value', '');
        $("#price").attr('value', '');
        $("#cost").attr('value', '');
        $("#shipping").attr('value', '');
        $("#internalID").attr('value', '');

        $("#lot").attr('value', sr.Lot);
        $("#Event").attr('value', sr.Event);
        $("#auction").attr('value', sr.Auction);
        $("#category").attr('value', sr.Category);
        $("#price").attr('value', sr.Price);
        $("#cost").attr('value', sr.Cost);
        $("#shipping").attr('value', sr.Shipping);
        $("#internalID").attr('value', sr.InternalID);

        $("#btnExport").click();
      }
</script>
</asp:Content>
