<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AuctionListing>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
  <% Html.Style("ui.jqgrid.css"); %>
  <% Html.Script("MicrosoftAjax.js"); %>
  <% Html.Script("MicrosoftMvcAjax.js"); %>
  <% Html.Script("MicrosoftMvcValidation.js"); %>
  <% Html.Script("jquery-ui-1.7.2.custom.min.js"); %>
  <% Html.Script("grid.locale-en.js"); %>
  <% Html.Script("jquery.jqGrid.min.js"); %>
  <% Html.Script("jquery.uploadify.js"); %>
  <% Html.Script("general.js"); %>

  <link href="<%= this.ResolveUrl("~/public/css/custom-gray/jquery-ui-1.7.2.css" ) %>" rel="stylesheet" type="text/css" />
  <script src="<%= this.ResolveUrl("~/public/tinymce/tinymce.min.js" ) %>" type="text/javascript"></script>
  <script src="<%= this.ResolveUrl("~/public/tiny_box.js") %>" type="text/javascript"></script>

  <title><%=!Model.Auction_ID.HasValue?"New Lot":("Edit Lot# "+ Model.Auction_ID.Value.ToString())%> - <%=Consts.CompanyTitleName %> </title>
  <style type="text/css">
    .message_list { list-style: none; margin: 0; padding: 0; width: 100%; }
      .message_list li { padding: 0; margin: 0; }
    .message_head { background-color: #002868; color: #FFFFFF; padding: 5px; cursor: default; font-weight: bold; width: 100%; }
    .message_body { padding: 0px 0px 5px 0px; cursor: default; margin: 0px; }
    .dialog_form { padding: 0px; }
  </style>
  <% Html.EnableClientValidation(); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <%
    List<SelectListItem> CategoryList = new List<SelectListItem>();
    SortedList<string, long> cats = new SortedList<string, long>();
    string cat;
    var catlist = ViewData["AllowedCategories"] as List<EventCategoryDetail> ?? new List<EventCategoryDetail>();
    foreach (EventCategoryDetail runner in catlist)
    {
      cat = runner.CategoryDescription;
      if (!cats.ContainsKey(cat)) cats.Add(cat, runner.EventCategory_ID);
    }
    CategoryList.Add(new SelectListItem { Selected = Model.Category_ID == 0, Text = "", Value = "-1" });
    foreach (KeyValuePair<string, long> runner in cats)
      CategoryList.Add(new SelectListItem { Value = runner.Value.ToString(), Text = runner.Key, Selected = Model.Category_ID == runner.Value });
    byte edttype = Convert.ToByte(ViewData["edittype"]);
  %>
  <div id="auction_update_form">
    <% using (Html.BeginForm("PreviewItem", "Consignor", FormMethod.Post))
       {%>
    <%=Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <input name="ID" id="ID" type="hidden" value="<%:Model.ID %>" />
    <%: Html.HiddenFor(model => model.DateIN)%>
    <%: Html.HiddenFor(model => model.Owner_ID)%>
    <%: Html.HiddenFor(model => model.Auction_ID)%>
    <%: Html.HiddenFor(model => model.Event_ID)%>
    <%: Html.Hidden("edittype", edttype)%>
    <table border="0" cellpadding="5">
      <tr>
        <td>
          <b>Event:</b>
        </td>
        <td>
          <%=String.Format("{0}   {1}", Model.Event.Title, Model.Event.StartEndTime)%>                
        </td>
      </tr>
      <tr>
        <td>
          <b>Title:</b>
        </td>
        <td>
          <%: Html.TextBoxFor(model => model.Title, new { @style = "width:600px", @tabindex = "1" }) %>
          <br />
          <%: Html.ValidationMessageFor(model => model.Title) %>
        </td>
      </tr>
      <tr>
        <td>
          <b>Category:</b>
        </td>
        <td>
          <%: Html.DropDownListFor(model => model.Category_ID, CategoryList, new { @style = "width:610px", @tabindex = "2" })%>
          <br />
          <%: Html.ValidationMessageFor(model => model.Category_ID) %>
        </td>
      </tr>
      <tr>
        <td>
          <b>Quantity:</b>
        </td>
        <td>
          <%: Html.TextBoxFor(model => model.Quantity, new { @tabindex = "3" })%>
          <%: Html.ValidationMessageFor(model => model.Quantity) %>                
        </td>
      </tr>
      <tr>
        <td>
          <b>Opening Bid:</b>
        </td>
        <td>
          <%: Html.TextBox("Price", Model.Price.GetPrice(), new { @tabindex = "4"})%>
          <%: Html.ValidationMessageFor(model => model.Price) %>
        </td>
      </tr>
      <tr>
        <td>
          <b>Reserve Price:</b>
        </td>
        <td>
          <%: Html.TextBox("Reserve", Model.Reserve.GetPrice(), new { @tabindex = "5" })%>
          <%: Html.ValidationMessageFor(model => model.Reserve) %>
        </td>
      </tr>
      <tr>
        <td>
          <b>Taxable:</b>
        </td>
        <td>
          <%: Html.CheckBoxFor(model => model.IsTaxable, new { @tabindex = "6" })%>
          <%: Html.ValidationMessageFor(model => model.IsTaxable) %>
        </td>
      </tr>
      <tr>
        <td>
          <b>Cost:</b>
        </td>
        <td>
          <%: Html.TextBox("Cost", Model.Cost.GetPrice(), new { @tabindex = "7"})%>
          <%: Html.ValidationMessageFor(model => model.Cost) %>                
        </td>
      </tr>
      <tr>
        <td>
          <b>Location:</b>
        </td>
        <td>
          <%: Html.TextBoxFor(model => model.Location, new { @tabindex = "8" }) %>
          <%: Html.ValidationMessageFor(model => model.Location) %>                
        </td>
      </tr>
      <tr>
        <td>
          <b>Featured:</b>
        </td>
        <td>
          <%= Html.CheckBox("IsF", Model.IsFeatured, new { @disabled = true })%>
          <%: Html.CheckBoxFor(model => model.IsFeatured, new { @style="display:none" })%>
          <%: Html.ValidationMessageFor(model => model.IsFeatured) %>                
        </td>
      </tr>
      <tr>
        <td>
          <b>Bold:</b>
        </td>
        <td>
          <%: Html.CheckBoxFor(model => model.IsBold, new { @tabindex = "9" })%>
          <%: Html.ValidationMessageFor(model => model.IsBold) %>              
        </td>
      </tr>
      <tr>
        <td>
          <b>Internal Id #:</b>
        </td>
        <td>
          <%: Html.TextBoxFor(model => model.InternalID, new { @tabindex = "10" })%>
          <%: Html.ValidationMessageFor(model => model.InternalID) %>                
        </td>
      </tr>
      <tr>
        <td>
          <b>Shipping Cost:</b>
        </td>
        <td>
          <%: Html.TextBox("Shipping", Model.Shipping.GetPrice(), new { @tabindex = "11"})%>
          <%: Html.ValidationMessageFor(model => model.Shipping ) %>
        </td>
      </tr>
      <tr id="trMShipping">
        <td>
          <b>Charge Multiple Shipping:</b>
        </td>
        <td>
          <%: Html.CheckBoxFor(model => model.IsMultipleShipping, new { @tabindex = "12" })%>
        </td>
      </tr>
      <tr>
        <td>
          <b>Consignor Shipped:</b>
        </td>
        <td>
          <%: Html.CheckBoxFor(model => model.IsConsignorShip, new { @tabindex = "13" })%>
          <%: Html.ValidationMessageFor(model => model.IsConsignorShip) %>                
        </td>
      </tr>
    </table>

    <ol class="message_list">
      <li>
        <p class="message_head">Images</p>
        <div class="message_body">
          <table cellpadding="0" style="padding: 0px">
            <tr>
              <td>Use the upload fields to upload up to <b>5</b> images from your computer. When the
                description of your item is displayed, the photos will <i>automatically</i> be inserted
                for you.
              </td>
            </tr>
            <tr>
              <td>
                <table id="img_list"></table>
                <div id="img_pager"></div>
                <div id="form_iupload" title="Batch image upload" class="dialog_form">
                  <input type="file" id="biuFile" />
                </div>
              </td>
            </tr>
          </table>
        </div>
      </li>
      <li>
        <p class="message_head">Description</p>
        <div class="message_body">
          <table border="0" cellpadding="7" width="95%">
            <tr>
              <td>Enter a description of the item(s) for auction. Please be as detailed as possible.
                      You may include HTML tags.
              </td>
            </tr>
            <tr>
              <td>
                <%: Html.TextAreaFor(model=>model.Descr, new {@rows="15",@cols="80", @style="width: 730px", @tabindex = "14" })%>                      
              </td>
            </tr>
          </table>
          <!-- TinyMCE -->
          <script type="text/javascript">
            // Default skin
            tinyMCE.init({
              // General options
              mode: "exact",
              elements: "Descr",
              theme: "modern",
              plugins: "pagebreak,layer,table,save,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,template,anchor,charmap,hr,image,link,emoticons,code,textcolor",
              fontsize_formats: "8pt 9pt 10pt 11pt 12pt 14pt 16pt 18pt 20pt 22pt 26pt 36pt",
              toolbar: "undo redo pastetext | bold,italic,underline,strikethrough | formatselect | styleselect | fontselect | fontsizeselect",
              theme_advanced_toolbar_location: "top",
              theme_advanced_toolbar_align: "left",
              theme_advanced_statusbar_location: "bottom",
              theme_advanced_resizing: true,
              content_css: "/public/css/content.css", 
              template_replace_values: {
                username: "Some User",
                staffid: "991234"
              }
            });
          </script>
        </div>
      </li>
    </ol>

    <table border="0" cellpadding="5">
      <tr>
        <td align="center">
          <button type="submit" style="width: 135px!important;" class="cssbutton small white" style="//width: 130px">
            <span>Preview Listing</span>
          </button>
        </td>
        <td align="center" style="text-align: right; //padding-right: 50px">
          <button type="button" style="width: 115px!important;" class="cssbutton small white" onclick="window.location='<%= Url.Action("CancelListing", "Consignor", new {id = Model.ID, Auction_ID = Model.Auction_ID, edittype = edttype}) %>'">
            <span>Cancel</span>
          </button>
        </td>
      </tr>
    </table>
    <% } %>
  </div>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="cphScriptBottom" runat="server">
  <script type="text/javascript">
  <%    
    List<EventCategoryDetail> AllAllowedCategories = ViewData["AllowedCategories"] as List<EventCategoryDetail> ?? new List<EventCategoryDetail>();
    int i = 0;
   %>
    var sDataArray=new Array(<%=AllAllowedCategories.Count %>);  
  <% foreach (EventCategoryDetail runner in AllAllowedCategories)
     { %>
    sDataArray[<%=i%>] = new Array(2);
    sDataArray[<%=i%>][0] = <%=runner.EventCategory_ID %>;
    sDataArray[<%=i%>][1] = <%=Convert.ToInt32(runner.IsTaxable)%>;
  <% i++;
     }%>
    function ChangeTax(){
      var SelValue = $("#Category_ID").val();      
      for(var i=0;i<sDataArray.length;i++)
      {     
        if(sDataArray[i][0]!=SelValue) continue;
        $("#IsTaxable").attr("checked", sDataArray[i][1]);
        break;
      }    
    }
    function InitUploadForm(){    
      $('#form_iupload').dialog('open');
    }
    function DeleteImageByID(id) {    
      if (id == null) { MessageBox('Deleting the image', 'Please select the image.', '', 'info'); return; }
      ConfirmBox('Deleting the image', 'Do you really want to delete selected image?', function() {  
        LoadingFormOpen();
        $.post('/Consignor/DeleteImage', { image_id: id, auctionlisting_id : <%=Model.ID %> }, function(data) {
          LoadingFormClose();
          switch (data.Status) {
            case "ERROR":
              MessageBox("Deleting the image", data.Message, '', "error");
              break;
            case "SUCCESS":
              $('#img_list').jqGrid('clearGridData');
              $("#img_list").trigger('reloadGrid');            
              break;
          }      
        }, 'json');
      });
    }
    function MoveImage(id, up) {
      if (id == null) { MessageBox('Moving the image', 'Please select the image.', '', 'info'); return; }
      LoadingFormOpen();
      $.post('/Consignor/MoveImage', { image_id: id, isup:up, auctionlisting_id : <%=Model.ID %> }, function(data) {
        LoadingFormClose();
        switch (data.Status) {
          case "ERROR":
            MessageBox("Moving the image", data.Message, '', "error");
            break;
          case "SUCCESS":            
            $("#img_list").trigger('reloadGrid');            
            break;
        }      
      }, 'json');    
    }
    function SetDefault(id) {    
      if (id == null) { MessageBox('Setting as default image', 'Please select the image.', '', 'info'); return; }
      ConfirmBox('Setting as default image', 'Do you really want to set this image as a default image?', function() {  
        LoadingFormOpen();
        $.post('/Consignor/SetDefaultImage', { image_id: id, auctionlisting_id : <%=Model.ID %> }, function(data) {
          LoadingFormClose();
          switch (data.Status) {
            case "ERROR":
              MessageBox("Setting as default image", data.Message, '', "error");
              break;
            case "SUCCESS":            
              $("#img_list").trigger('reloadGrid');            
              break;
          }      
        }, 'json');
      });
    }
    var image_rowID = null;
    // loading page  
    $(document).ready(function() {
      $("#Title").focus();    
      $("#Category_ID").change(function(){    
        ChangeTax();
      });
      $("#Category_ID").keyup(function(){    
        ChangeTax();
      });
      $("#Category_ID").change();

      $("#Quantity").blur(function(){
        if (parseInt($(this).val())>1) $("#IsMultipleShipping").removeAttr("disabled"); else $("#IsMultipleShipping").attr("disabled", "true");
      });
      $("#Quantity").blur();

      $("#img_list").jqGrid({
        url: '/Consignor/GetAuctionLisingImages/',
        mtype: 'GET',
        datatype: "json",
        height: '100%',
        width: '730px',
        colNames: ['#', 'Image', 'File Name', 'Order'],
        colModel: [
          { name: 'Image_ID', index: 'Image_ID', width: 60, key: true, search: false },
          { name: 'Image', index: 'Image', width: 140, search: false, sortable: false, align: 'center' },
          { name: 'Title', index: 'Title', width: 450, search: false, sortable: false },
          { name: 'Order', index: 'Order', width: 60, search: false, sortable: false, align: 'center' }
        ],
        loadtext: 'Loading ...',           
        pager: '#img_pager',
        postData: { auctionlisting_id:<%=Model.ID %>, auction_id: '<%=Model.Auction_ID %>' },
      onSelectRow: function(id) { image_rowID = id; }
    });
    $("#img_list").jqGrid('navGrid', '#img_pager', { edit: false, add: false, del: false, search: false, refreshtext: "", afterRefresh: function() { } }).navSeparatorAdd('#img_pager');
    //btnAdd
    $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "", buttonicon: 'ui-icon-plus', title: 'Add new image', onClickButton: function() { InitUploadForm(); } }).navSeparatorAdd('#img_pager');
    //btnDel
    $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "", buttonicon: 'ui-icon-trash', title: 'Delete image', onClickButton: function() { DeleteImageByID(image_rowID); } }).navSeparatorAdd('#img_pager');
    //btnUp
    $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "", buttonicon: 'ui-icon-arrowthick-1-n', title: 'Move up  selected image', onClickButton: function() { MoveImage(image_rowID, true); } }).navSeparatorAdd('#img_pager');
    //btnDown
    $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "", buttonicon: 'ui-icon-arrowthick-1-s', title: 'Move down selected image', onClickButton: function() { MoveImage(image_rowID, false); } }).navSeparatorAdd('#img_pager');
    //btnSetDefault
    $("#img_list").jqGrid('navButtonAdd', '#img_pager', { caption: "", buttonicon: 'ui-icon-arrowthickstop-1-n', title: 'Set as a default image', onClickButton: function() { SetDefault(image_rowID); } }).navSeparatorAdd('#img_pager');
    $("#form_iupload").dialog({
      bgiframe: true,
      autoOpen: false,
      height: 300,
      width: 370,
      modal: true,
      gridview: true,    
      close: function() { $.post('/Consignor/ResortImages', { auctionlisting_id : <%=Model.ID %> }, function(data) {$("#img_list").trigger('reloadGrid');});  }
      });
    $('#biuFile').fileUpload({
      'uploader': '/public/uploader.swf',
      'script': '/Consignor/UploadPicture',
      'scriptData' : { 'user_id':'<%=Model.Owner_ID %>', 'auctionlisting_id':'<%=Model.ID %>', 'auction_id': '<%=Model.Auction_ID %>' },
        'cancelImg': '/public/cancel.png',
        'multi': 'true',
        'auto': 'false',
        'fileDesc': 'Image',
        'fileExt': '*.jpg;*.jpeg;*.png;*.gif',
        'method': 'POST',
        'queueSizeLimit' : '5'
      });
  });
  </script>
</asp:Content>
