<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master"
    Inherits="System.Web.Mvc.ViewPage<List<InvoiceDetail>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Pay for items -
   
        <%=Consts.CompanyTitleName %></title>
    <% Html.Style("jQueryCustomControls.css"); %>
    <% Html.Script("custom.js"); %>
    <% Html.Script("payment.js"); %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
        <div class="center_content">
            <div class="row">
                <div class="col-sm-12">

                    <% if (Model.Count() == 0)
                       { %>
    You do not have any invoices from our past auctions.
   
            <% }
                       else
                       { %>
                    <% decimal amount = 0; decimal value; %>
                    <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/MakePayment">
                        <div class="table-responsive">
                            <%=Html.AntiForgeryToken() %>
                            <table class="table table-hover table-striped margin-bottom-none">
                                <thead>


                                    <tr class="bg-primary">
                                        <th class="align-middle"><%=Html.CheckBox("chkHeaderTop", true)%></th>
                                        <th class="align-middle" colspan="2">[<a href="#" class="text-white colapse-btn">Collapse</a> | <a href="#" class="text-white expand-btn">Expand</a>]</th>
                                        <th class="text-center align-middle" colspan="1">Selected Invoices: <span id="spSelectedItemsTop">0</span> / <span id="spTotalItems">0</span></th>
                                        <th class="align-middle">Total Due:</th>
                                        <th class="text-center align-middle" id="lTotalAmountTop" >&nbsp; <%=amount.GetCurrency()%></th>
                                    </tr>
                                    <tr class="bg-primary">
                                        <th class="align-middle">&nbsp;</th>
                                        <th class="align-middle">Lot#</th>
                                        <th class="text-center align-middle">Date</th>
                                        <th class="align-middle">Title</th>
                                        <th class="text-center align-middle">Qality</th>
                                        <th class="text-center align-middle">Amount</th>
                                    </tr>
                                </thead>
                                 <tbody>

                                    <% foreach (InvoiceDetail invoice in Model)
                                       {
                                           value = invoice.TotalCost;
                                    %>
                                    <tr class=" payment_table_line">
                                        <td class="align-middle">
                                            <%=Html.CheckBox("sel_" + invoice.Invoice_ID.ToString(), true, new { @class = "chk", @amount = value, @withoutship = value - ((!invoice.IsConsignorShip) ? invoice.Shipping + invoice.Insurance : 0), @defamount = value, @consship = Convert.ToByte(invoice.IsConsignorShip) })%>
                                        </td>
                                        <td class="align-middle">
                                            <%= Html.ActionLink(invoice.LinkParams.Lot.ToString(), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = invoice.LinkParams.ID, evnt = invoice.LinkParams.EventUrl, cat = invoice.LinkParams.CategoryUrl, lot = invoice.LinkParams.LotTitleUrl })%>
                                        </td>
                                        <td class="text-center align-middle">
                                            <%= invoice.DateCreated.ToShortDateString()%>
                                        </td>
                                        <td class="align-middle">
                                            <%= invoice.LinkParams.Title%>
                                        </td>
                                        <td class="text-center align-middle">
                                            <%=invoice.Quantity%>
                                        </td>
                                        <td class="text-center align-middle">$<span class="accordion_amount" withoutship="<%=(value - ((!invoice.IsConsignorShip) ? invoice.Shipping + invoice.Insurance : 0)).GetPrice()%>"
                                            defamount="<%=value.GetPrice()%>"><%=value.GetPrice()%></span>
                                        </td>
                                    </tr>

                                    <tr class="table-collapse">
                                        <td>&nbsp;</td>
                                        <% if (invoice.Quantity == 1)
                                           {%>
                                        <td colspan="3" class="align-middle"><span class="visible-xs" style="white-space: nowrap;">Winning bid</span></td>
                                        <td class="align-middle"><span class="hidden-xs" style="white-space: nowrap;">Winning bid</span></td>
                                        <% }
                                           else
                                           { %>
                                        <td colspan="4" class="align-middle"><span class="visible-xs">Winning bid</span></td>
                                        <td class="align-middle"><span class="hidden-xs">Winning bid</span></td>
                                        <td class="align-middle">
                                            <%=invoice.Quantity%>&nbsp;x&nbsp;<%=(invoice.Amount / invoice.Quantity).GetCurrency()%>&nbsp;=&nbsp;
                                        </td>
                                        <%} %>
                                        <td class="text-center align-middle" >
                                            <%=invoice.Amount.GetCurrency()%>
                                        </td>
                                    </tr>

                                    <% if (invoice.AuctionType != (long)Consts.AuctionType.DealOfTheWeek && invoice.BuyerPremium > 0)
                                       {%>
                                    <tr class="table-collapse">

                                        <td colspan="4" class="align-middle"><span class="visible-xs">Buyer's premium</span></td>
                                        <td class="align-middle"><span class="hidden-xs">Buyer's premium</span></td>
                                        <td class="text-center align-middle"><%=invoice.BuyerPremium.GetCurrency()%></td>
                                    </tr>
                                    <%} %>



                                    <% if (invoice.Shipping > 0)
                                       { %>
                                    <tr class="table-collapse">

                                        <td colspan="4" class="align-middle"><span class="visible-xs">Sipping and handing </span></td>
                                        <td class="align-middle"><span class="hidden-xs">Sipping and handing <span class="pull-right text-xs visible-lg-inline">(consignor ship)</span></span></td>
                                        <td class="text-center align-middle">

                                            <%=invoice.Shipping.GetCurrency()%>
                                        </td>
                                    </tr>

                                    <tr class="table-collapse">

                                        <td colspan="4" class="align-middle"><span class="visible-xs">Insurance</span></td>
                                        <td class="align-middle"><span class="hidden-xs">Insurance</span></td>
                                        <td class="<%=(invoice.Tax > 0)?"tableTLR":"tableTLBR" %>">
                                            <span class="accordion_insurance" defamount='<%=invoice.Insurance.GetCurrency() %>'
                                                defzero="<%=(!invoice.IsConsignorShip)?"$0.00":invoice.Insurance.GetCurrency() %>">
                                                <%=invoice.Insurance.GetCurrency()%></span>
                                        </td>
                                    </tr>
                                    <%} %>

                                    <% if (invoice.Tax > 0)
                                       { %>
                                    <tr class="table-collapse">

                                        <td colspan="4" class="align-middle"><span class="visible-xs">Tax</span></td>
                                        <td class="align-middle"><span class="hidden-xs">Tax</span></td>
                                        <td class="text-center align-middle"><%=invoice.Tax.GetCurrency()%></td>
                                    </tr>
                                    <%} %>

                                    <% if (invoice.Discount > 0)
                                       { %>
                                    <tr>
                                        <td>&nbsp;
                                        </td>
                                        <td class="tableBL" colspan="2">Discount
                                        </td>
                                        <td class="tableBLR">
                                            <%=invoice.Discount.GetCurrency()%>

                                        </td>
                                    </tr>
                                    <%} %>

                                    <% amount += value; %>
                                    <%} %>
                                </tbody>

                                <tfoot>
                                    <tr class="bg-primary">
                                        <th class="align-middle"><%=Html.CheckBox("chkHeaderBottom", true)%></th>
                                        <th class="align-middle"  colspan="2"><a href="#" class="text-white colapse-btn">Collapse</a> | <a href="#" class="text-white expand-btn">Expand</a></th>
                                        <th class="text-center align-middle" colspan="1">Selected Invoices:&nbsp;&nbsp;&nbsp;&nbsp;<span id="spSelectedItemsBottom">0</span>&nbsp;/&nbsp;<%=Model.Count()%></th>
                                        <th class="align-middle">Total Due:</th>
                                        <th class="text-center align-middle" id="lTotalAmountBottom"><%=amount.GetCurrency()%></th>


                                    </tr>
                                </tfoot>
                            </table>


                            <table class="table table-hover table-striped">
                                <thead>
                                    
                                </thead>

                               
                            </table>

                            <table class="table table-hover table-striped margin-bottom-none">
                                

                            </table>



                            <p style='display: none'>
                                <%=Html.CheckBox("chkPickup", false)%>
      I will pick up my items (shipping charges will be removed)
                            </p>
                            <% if ((bool)ViewData["vDiscount"])
                               {%>
                            <table style="table-layout: fixed;">
                                <colgroup>
                                    <col width="220px" />
                                    <col width="480px" />
                                </colgroup>
                                <tbody>
                                    <tr>
                                        <td colspan="2" id="spPromoCodeTitle">ENTER A PROMO CODE OR PRESS CONTINUE</td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <span id="spPromoCode">The promo code</span>
                                            <%=Html.TextBox("tbxCouponCode", "Promo Code", new { @style = "width:115px;", @class = "promocodefield" })%>
                                        </td>
                                        <td id="tdValidationResult" rowspan="2">
                                            <img id="imgValidation" src="<%=AppHelper.CompressImage("Invalid.png") %>" alt="valid"
                                                title="Enter the Promo Code" />
                                            <div id="dsc_desc">
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="margin: 0; padding: 0">
                                            <span id="btnValidateCoupon" title="Validate the promo code">validate promo code</span>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <% } %>
                        </div>
                        <div class="row margin-top">
                            <div class="col-sm-12 text-right">
                                <button type="submit" class="btn btn-danger btn-lg">
                                    <span>Continue &raquo;</span>
                                </button>
                            </div>


                          
                        </div>
                    </form>
                    <% } %>
                    <div class="back_link">
                        <%=Html.ActionLink("Return to the My Account page", "MyAccount", new { controller = "Account", action = "MyAccount" })%>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntJ" ContentPlaceHolderID="cphScriptBottom" runat="server">
     <script type="text/javascript">
         $(document).ready(function () {
             //$(".payment_table_line .m").toggle(function () {
             //    $(this).parent(".payment_table_line:last").next(".payment_table_content:first").hide();
             //}, function () {
             //    $(this).parent(".payment_table_line:last").next(".payment_table_content:first").show();
             //});

             //$(".collapse_all_message").click(function () {
             //    $(".payment_table_content").hide();
             //});

             //$(".expand_all_message").click(function () {
             //    $(".payment_table_content").show();
             //});

             $("#tbxCouponCode").val("Promo Code").focus(function () {
                 this.value = "";
                 $(this).removeClass("promocodefield");
                 $("#imgValidation, #dsc_desc").hide();
             }).blur(function () {
                 if (this.value == "") {
                     $(this).addClass("promocodefield");
                     this.value = "Promo Code";
                 }
             });

             $("#tbxCouponCode").val("Promo Code").focus(function () {
                 this.value = "";
                 $(this).removeClass("promocodefield");
                 $("#imgValidation, #dsc_desc").hide();
             }).blur(function () {
                 if (this.value == "") {
                     $(this).addClass("promocodefield");
                     this.value = "Promo Code";
                 }
             });

             $("form").submit(function () {
                 if ($("#tbxCouponCode").val() == "Promo Code")
                     $("#tbxCouponCode").val('');
             });

             $("#btnValidateCoupon").click(function () {
                 $.post('/Account/ValidateDiscount', { discount_code: $("#tbxCouponCode").attr('value') }, function (data) {
                     if (data == null) return;
                     $("#dsc_desc").html(data.Message).show();
                     $("#imgValidation").attr("src", data.Status == 'ERROR' ? '<%=AppHelper.CompressImage("invalid.png")%>' : '<%=AppHelper.CompressImage("valid.png") %>').attr("title", data.Message).show();
        }, 'json');
      });

             $("#chkHeaderTop, #chkHeaderBottom").click(function () {
            var check = $(this).attr("checked")
            $("#chkHeaderTop").attr("checked", check);
            $("#chkHeaderBottom").attr("checked", check);
            $(".payment_table_line .chk").each(function () {
                this.checked = check
            });
            RemoveShipping($("#chkPickup").attr("checked"));
        });

        $("#chkPickup").click(function () {
            RemoveShipping($("#chkPickup").attr("checked"));
        });

        $("#spTotalItems").text($(".payment_table_line .chk").length);

        $(".payment_table_line .chk").click(function () {
            CalculateTotal();
        });

        RemoveShipping($("#chkPickup").attr("checked"));
    });
  </script>
</asp:Content>
