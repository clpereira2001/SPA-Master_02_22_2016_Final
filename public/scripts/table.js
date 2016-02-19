function UpdateTableView() {
  $.each($(".tbl_res_row"), function () {
    $(this).css("background-color", $(this).attr("defcolor"));
  });
  $.post('/Auction/UpdatePageResult', { method: $("#hd_prem").attr("value"), prms: $("#hd_pgpms").attr("value") }, function (data) {
    if (data != "null") {
      $.each(data, function (i, item) {        
        if ($("#cv_cb_" + item.id).text() != item.col1)
          $("#tbl_res_row_" + item.id).css("background-color", $("#tbl_res_row_" + item.id).attr("defchanged"));
        $("#cv_cb_" + item.id).html(item.col1);
      });
    }
    $("#lnkRBR,#lnkRBR_b").show();
    $("#lnkRBR_loading,#lnkRBR_loading_b").hide();
  }, 'json');
}
function UpdateGridView() {
  $.each($(".tbl_res_row"), function () {
    $(this).css("background-color", "#FFF");
  });
  $.post('/Auction/UpdatePageResult', { method: $("#hd_prem").attr("value"), prms: $("#hd_pgpms").attr("value") }, function (data) {
    if (data != "null") {
      $.each(data, function (i, item) {        
        if ($("#cv_cb_" + item.id).text() != item.col2.replace("<br/>", ""))
          $("#tbl_res_row_" + item.id).css("background-color", $("#tbl_res_row_" + item.id).attr("defchanged"));
        $("#cv_cb_" + item.id).html(item.col2);
      });
    }
    $("#lnkRBR,#lnkRBR_b").show();
    $("#lnkRBR_loading,#lnkRBR_loading_b").hide();
  }, 'json'); 
}