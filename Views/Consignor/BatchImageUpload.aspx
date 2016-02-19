<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
  <% Html.Style("ui.jqgrid.css");
     Html.Script("jquery-ui-1.7.2.custom.min.js");
     Html.Script("grid.locale-en.js");
     Html.Script("jquery.jqGrid.min.js");
     Html.Script("jquery.uploadify.js");
     //Html.Script("general.js");
   %>
  <link href="<%=ResolveUrl("~/public/css/custom-gray/jquery-ui-1.7.2.css" ) %>" rel="stylesheet" type="text/css" />
  <script src="<%=ResolveUrl("~/public/scripts/general.js" ) %>" type="text/javascript"></script>
  <title>Batch Image Upload</title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">    
<%--    <h1 class="title">Batch Image Upload</h1>--%>
    <table id="img_list">
    </table>
    <div id="img_pager">
    </div>
    <div id="form_iupload" title="Batch image upload" class="dialog_form">
      <input type="file" id="biuFile" />
    </div>
  </div>
    <div class="back_link">
    <%=Html.ActionLink("Return to Seller Tool page", "Index", new {controller="Consignor", action="Index"}) %>
  </div>  
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="cphScriptBottom" runat="server">
  <script type="text/javascript">
    function InitUploadForm() {
      $('#form_iupload').dialog('open');
    }
    function AsignImages() {
      LoadingFormOpen();
      var ids = $("#img_list").jqGrid('getGridParam', 'selarrrow');
      $.post('/Consignor/AsignImages', { images: JSON.stringify(ids, null, 1) }, function (data) {
        LoadingFormClose();
        $("#img_list").trigger('reloadGrid');
        switch (data.Status) {
          case "ERROR":
            MessageBox("Assign Images", data.Message, '', "error");
            break;
          case "SUCCESS":
            MessageBox("Assign Images", "The images were assigned successfully.", '', "info");
            break;
        }
      }, 'json');
    }
    function DeleteImages() {
      var ids = $("#img_list").jqGrid('getGridParam', 'selarrrow');
      confirm("Delete Images", "Do you really want to delete selected images", function () {
        LoadingFormOpen();
        $.post('/Consignor/DeleteBatchImages', { images: JSON.stringify(ids, null, 1) }, function (data) {
          LoadingFormClose();
          $("#img_list").trigger('reloadGrid');
          switch (data.Status) {
          case "ERROR":
            MessageBox("Delete Images", data.Message, '', "error");
            break;
          case "SUCCESS":
            MessageBox("Delete Images", "The images were deleted successfully.", '', "info");
            break;
          }
        }, 'json');
      });
    }
    $(document).ready(function() {
      $("#img_list").jqGrid({
        url: '/Consignor/GetBatchImages/',
        mtype: 'GET',
        datatype: "json",
        height: '100%',
        width: '730px',
        colNames: ['#', 'Image', 'File Name', 'Date'],
        colModel: [
          { name: 'Image_ID', index: 'Image_ID', width: 60, key: true, search: false, hidden: true },
          { name: 'Image', index: 'Image', width: 150, search: false, sortable: false, align: 'center', editable: false },
          { name: 'Title', index: 'Title', width: 450, search: false, sortable: false, editable: true, editrules: { required: true }, formoptions: { elmprefix: "(*)", elmsuffix: ""} },
          { name: 'Date', index: 'Date', width: 140, search: false, sortable: false, align: 'center', editable: false }
        ],
        pager: '#img_pager',
        rowNum: 10,
        rowList: [5, 10, 15, 20, 25, 30, 35, 40, 60, 80, 100, 1000],
        viewrecords: true,
        multiselect: true,
        editurl: "/Consignor/EditImageName/"
      });
        //, afterSubmit: AfterSubmitFunction removed below
      $("#img_list").jqGrid('navGrid', '#img_pager', { edit: true, add: false, del: false, search: false, refreshtext: "", afterRefresh: function() { } },
        { jqModal: false, closeOnEscape: true, bottominfo: "Fields marked with (*) are required", closeAfterEdit: true, width: 320, bSubmit: "Save", editCaption: "Edit File Name" }, {}, {}, {}, {}).navSeparatorAdd('#img_pager');
      //btnAdd
      $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "Upload ", buttonicon: 'ui-icon-plus', title: 'Upload Image(s)', onClickButton: function () { InitUploadForm(); } }).navSeparatorAdd('#img_pager');
      //btnDel
      $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "Delete ", buttonicon: 'ui-icon-trash', title: 'Delete Image(s)', onClickButton: function () { DeleteImages(); } }).navSeparatorAdd('#img_pager');
      //btnAsign
      $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "Assign", buttonicon: 'ui-icon-shuffle', title: 'Assign Images to Lots', onClickButton: function() { AsignImages(); } }).navSeparatorAdd('#img_pager');
      
      $("#form_iupload").dialog({
        bgiframe: true,
        autoOpen: false,
        height: 300,
        width: 370,
        modal: true,
        gridview: true,    
        close: function() { $("#img_list").trigger('reloadGrid'); }
      });
      $('#biuFile').fileUpload({
        'uploader': '/public/uploader.swf',
        'script': '/Consignor/UploadBatchPicture',
        'scriptData': { 'user_id': '<%=AppHelper.CurrentUser.ID %>' },
        'cancelImg': '/public/cancel.png',
        'multi': 'true',
        'auto': 'false',
        'fileDesc': 'Image',
        'fileExt': '*.jpg;*.jpeg;*.png;*.gif',
        'method': 'POST'
      });
    });
  </script>
</asp:Content>
