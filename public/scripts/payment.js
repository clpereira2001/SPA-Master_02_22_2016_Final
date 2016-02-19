function CalculateTotal() {
  var amount = 0;
  $(".payment_table_line .chk:checked").each(function () {
    amount = amount + parseFloat($(this).attr("amount"));
  });
  var txt = FloatToCurrency(amount);
  $("#lTotalAmountTop, #lTotalAmountBottom").text(txt);
  $("#spSelectedItemsTop, #spSelectedItemsBottom").text($(".payment_table_line .chk:checked").length);
  if ($(".payment_table_line .chk:checked").length == 0)
    $("#chkHeaderTop, #chkHeaderBottom").removeAttr("checked");
  else if ($(".payment_table_line .chk:checked").length == $(".payment_table_line .chk").length)
    $("#chkHeaderTop, #chkHeaderBottom").attr("checked", "true");
}
function InvertCheckBoxes(container, check) {
  $("#" + container + " .chk").each(function () {
    this.checked = check
  });
  CalculateTotal();
}
function RemoveShipping(checked) {
  $(".payment_table_content_td .accordion_shipping, .payment_table_content_td .accordion_insurance").each(function () {
    $(this).text($(this).attr((checked) ? "defzero" : "defamount"));
  });
  $(".payment_table_line .accordion_amount").each(function () {
    $(this).text($(this).attr((checked) ? "withoutship" : "defamount"));
  });
  $(".payment_table_line .chk").each(function () {
    $(this).attr("amount", $(this).attr((checked) ? "withoutship" : "defamount"));
    if (checked && $(this).attr("consship") == 1) {
      $(this).attr("disabled", "true");
      $(this).removeAttr("checked");
    } else {
      $(this).removeAttr("disabled");
    }
  });
  CalculateTotal();
}