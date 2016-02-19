<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CreditCardInfo>" %>

<%@ Import Namespace="Vauction.Utils.Html" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Making payment -
   
        <%=Consts.CompanyTitleName %></title>
    <% Html.Style("jQueryCustomControls.css"); %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% PaymentConfirmation pc = Session["PayForItems"] as PaymentConfirmation;%>
    <div class="col-md-9 col-sm-8">

        <div class="row">
            <div class="col-sm-12">
                <div class="table-responsive">
                    <%--<h1 class="title">Make Payment</h1>--%>
                    <% if (pc == null)
                       {
                           Response.Write("Sorry, an error occurred while processing your request. Please " + Html.ActionLink("return to the Pay for Auction Items page", "PayForItems", new { controller = "Account", action = "PayForItems" }).ToSslLink() + " and try again.");
                       }
                       else if (Convert.ToBoolean(ViewData["IsBack"]))
                       {
                           Response.Write("Your account balance is <b>" + pc.Deposit.GetCurrency(false) + "</b>. The total amount for selected invoice" + (pc.Invoices.Count > 0 ? "s" : "") + " is <b>" + pc.TotalCost.GetCurrency() + "</b>. You can't continue making payment from account balance. Please select more item(s) to continue payment.");
                       }
                       else
                       {
                           if (pc.Invoices.Count > 4)
                           { %>
                    <% if (pc.Deposit - pc.TotalOrDeposit > 0)
                       { %>
                    <table id="dvChooseMethod">
                        <tr>
                            <td style="width: 290px;">Select payment method
        </td>
                            <td>&nbsp;
        </td>
                            <td style="width: 200px;" id="imgAccount">
                                <a href="#dvAccount">
                                    <img src="<%=AppHelper.CompressImage("PayFromAccount.png" ) %>" />
                                </a>
                            </td>
                        </tr>
                    </table>
                    <%}
                       else
                       { %>
                    <table id="dvChooseMethod">
                        <tr>
                            <td style="width: 290px;">Select payment method
        </td>
                            <td style="width: 150px;" id="imgPayPal">
                                <a href="#dvPayPalImportant">
                                    <img src="<%=AppHelper.CompressImage("PPExpressCheckout.gif" ) %>" alt="" />
                                </a>
                            </td>
                            <td>&nbsp
        </td>
                            <td style="width: 150px;" id="imgCardTop">
                                <a href="#CardNumber">
                                    <img src="<%=AppHelper.CompressImage("cards.gif" ) %>" style="height: 24px" alt="" />
                                </a>
                            </td>
                            <td style="width: 10px;">&nbsp
        </td>
                        </tr>
                    </table>
                    <%} %>
                    <%} %>
                    <%-- <a href="#" id="printInvoice" style="float: right; margin-right: 20px; font-size: 14px; font-weight: normal; text-decoration: underline">Print the invoice</a>--%>
                    <table id="tblInvoice" class="table table-hover table-striped margin-bottom-none">

                        <%--   <tr>
                            <th style='width: 80px'>&nbsp;
        </th>
                            <th style='width: 80px'>&nbsp;
        </th>
                            <th style='width: 340px'>&nbsp;
        </th>
                            <th style='width: 60px;'>&nbsp;
        </th>
                            <th style='width: 100px'>&nbsp;
        </th>
                        </tr>--%>
                        <thead>
                            <tr class="bg-primary">
                                <th>Lot#
        </th>
                                <th>Date
        </th>
                                <th>Description
        </th>
                                <th>Quantity
        </th>
                                <th>Amount
        </th>
                            </tr>
                        </thead>
                        <tbody>
                            <% foreach (InvoiceDetail invoice in pc.Invoices)
                               {             
      %>
                            <tr class="payment_table_line">
                                <td>
                                    <%= Html.ActionLink(invoice.LinkParams.Lot.ToString(), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = invoice.LinkParams.ID, evnt=invoice.LinkParams.EventUrl, cat=invoice.LinkParams.CategoryUrl, lot=invoice.LinkParams.LotTitleUrl }).ToSslLink() %>
        </td>
                                <td>
                                    <%= invoice.DateCreated.ToShortDateString()%>
        </td>
                                <td>
                                    <%= invoice.LinkParams.Title%>
        </td>
                                <td>
                                    <%=invoice.Quantity%>
        </td>
                                <td>
                                    <%=invoice.TotalCostWithoutCouponDiscount.GetCurrency()%>
        </td>
                            </tr>
                            <tr class="payment_table_content">
                                <td colspan="5" class="payment_table_content_td">
                                    <table class="table table-hover table-striped margin-bottom-none">
                                        <colgroup>
                                            <col width='190px' />
                                            <col width='150px' />
                                            <col width='150px' />
                                            <col width='110px' />
                                        </colgroup>
                                        <tr>
                                            <td>&nbsp;
              </td>
                                            <% if (invoice.Quantity == 1)
                                               {%>
                                            <td class="tableTL" colspan="2">Winning Bid
              </td>
                                            <% }
                                               else
                                               { %>
                                            <td class="tableTL" colspan="1">Winning Bid
              </td>
                                            <td class="tableT" style="text-align: right">
                                                <%=invoice.Quantity%>&nbsp;x&nbsp;<%=(invoice.Amount / invoice.Quantity).GetCurrency()%>&nbsp;=&nbsp;
              </td>
                                            <%} %>
                                            <td class="tableTLR">
                                                <%=invoice.Amount.GetCurrency()%>
              </td>
                                        </tr>
                                        <% if (invoice.AuctionType != (long)Consts.AuctionType.DealOfTheWeek && (invoice.BuyerPremium > 0 || invoice.RealBP.HasValue))
                                           {%>
                                        <tr>
                                            <td>&nbsp;
              </td>
                                            <td class="tableTL" colspan="2">Buyer's premium
              </td>
                                            <td class="tableTLR">
                                                <%=invoice.RealBP.GetValueOrDefault(invoice.BuyerPremium).GetCurrency()%>
              </td>
                                        </tr>
                                        <%} %>
                                        <% if (invoice.Shipping >= 0 || invoice.RealSh.HasValue)
                                           { %>
                                        <tr>
                                            <td>&nbsp;
              </td>
                                            <td class="tableTL" colspan="2">Shipping and handling
              </td>
                                            <td class="tableTLR">
                                                <span class="accordion_shipping">
                                                    <%=invoice.RealSh.GetValueOrDefault(invoice.Shipping).GetCurrency()%>
                </span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;
              </td>
                                            <td class="<%=(invoice.Tax > 0)?"tableTL":"tableTLB" %>" colspan="2">Insurance
              </td>
                                            <td class="<%=(invoice.Tax > 0)?"tableTLR":"tableTLBR" %>">
                                                <span class="accordion_insurance">
                                                    <%=invoice.Insurance.GetCurrency()%></span>
                                            </td>
                                        </tr>
                                        <%} %>
                                        <% if (invoice.Tax > 0)
                                           { %>
                                        <tr>
                                            <td>&nbsp;
              </td>
                                            <td class="tableTLB" colspan="2">Tax
              </td>
                                            <td class="tableTLBR">
                                                <%=invoice.Tax.GetCurrency()%>
              </td>
                                        </tr>
                                        <%} %>
                                        <% if (invoice.Discount > 0 && invoice.TotalOrderDiscount.GetValueOrDefault(0) != invoice.Discount)
                                           { %>
                                        <tr>
                                            <td>&nbsp;
              </td>
                                            <td class="tableBL" colspan="2">Discount
              </td>
                                            <td class="tableBLR">
                                                <%=(invoice.Discount - invoice.TotalOrderDiscount.GetValueOrDefault(0)).GetCurrency()%>
              </td>
                                        </tr>
                                        <%} %>
                                    </table>
                                </td>
                            </tr>
                            <%} %>
                            <tfoot>

                                <tr class="bg-primary">
                                    <td colspan="2">[ <a href="#" class="collapse_all_message" style='color: White;'>Collapse</a>&nbsp;&nbsp;|&nbsp;
         
                            <a href="#" class="expand_all_message" style='color: White;'>Expand</a> ]
        </td>
                                    <td style="text-align: right" colspan="2">Total Due:</td>
                                    <td><%=pc.TotalCostWithoutDiscount.GetCurrency()%></td>
                                </tr>
                            </tfoot>
                            <% if (pc.HasDiscount)
                               {%>
                            <tr class="payment_table_footer3">
                                <td colspan="4" style="text-align: right">Promo Discount:</td>
                                <td><%=pc.DiscountAmount.GetCurrency()%></td>
                            </tr>
                            <tr class="payment_table_footer2">
                                <td colspan="4" style="text-align: right">Payment Due After Discount:</td>
                                <td><%=pc.TotalCost.GetCurrency() %></td>
                            </tr>
                            <% } %>
                            <% if (pc.TotalOrDeposit > 0)
                               {%>
                            <tr class="payment_table_footer3">
                                <td colspan="4" style="text-align: right">Deposit:</td>
                                <td><%=pc.TotalOrDeposit.GetCurrency()%></td>
                            </tr>
                            <tr class="payment_table_footer2">
                                <td colspan="4" style="text-align: right"><%=(pc.TotalCost - pc.Deposit <= 0) ? "Remaining Balance" : "Balance Due:"%></td>
                                <td><%=(pc.TotalCost - pc.Deposit <= 0) ? (pc.Deposit - pc.TotalCost).GetCurrency(false) : (pc.TotalCost - pc.Deposit).GetCurrency(false)%></td>
                            </tr>
                            <%} %>
                        </tbody>
                    </table>

                    <br class="br_clear" />
                    <% if (pc.Invoices.Count != 0)
                           if (pc.TotalCost - pc.Deposit <= 0)
                           { %>
                    <table id="dvChooseMethod">
                        <tr>
                            <td style="width: 290px;">Select payment method
        </td>
                            <td>&nbsp;
        </td>
                            <td style="width: 200px;" id="imgAccount">
                                <img src="<%=AppHelper.CompressImage("PayFromAccount.png" ) %>" />
                            </td>
                        </tr>
                    </table>
                    <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/PayFromDeposit">
                        <%=Html.AntiForgeryToken() %>
                        <%--<%=Html.Hidden("Deposit", pc.TotalOrDeposit)%>--%>
                        <input type="submit" id="btnAccount" style="display: none" />
                    </form>
                    <br class="br_clear" />
                    <div id="dvAccount" style="color: rgb(180,0,0); text-align: center; display: none">
                        <strong>Please wait. This may take a couple of minutes.</strong>
                        <br />
                        <br />
                        <br />
                        <center>
        <img src="<%=AppHelper.CompressImage("throbber2.gif" ) %>" /></center>
                    </div>
                    <% }
                           else
                           { %>
                    <table id="dvChooseMethod">
                        <tr>
                            <td style="width: 290px;">Select payment method
        </td>
                            <td style="width: 150px;" id="imgPayPal2">
                                <img src="<%=AppHelper.CompressImage("PPExpressCheckout.gif" ) %>" />
                            </td>
                            <td>&nbsp
        </td>
                            <td id="imgCard" style="width: 150px;">
                                <a href="#CardNumber">
                                    <img src="<%=AppHelper.CompressImage("cards.gif" ) %>" style="height: 24px" /></a>
                            </td>
                            <td style="width: 10px;">&nbsp
        </td>
                        </tr>
                    </table>
                    <br class="br_clear" />
                    <div id="dvPayPalImportant" style="color: rgb(180,0,0); text-align: center; display: none">
                        <strong>Please wait a second, you are being redirected to PayPal.</strong>
                        <br />
                        <br />
                        <br />
                        <center>
        <img src="<%=AppHelper.CompressImage("throbber2.gif" ) %>" /></center>
                    </div>
                    <div id="dvAuthorizedNet" style="color: rgb(180,0,0); text-align: center; display: none">
                        <strong>Please wait while we authorize your credit card. This may take a couple of
        minutes.</strong>
                        <br />
                        <br />
                        <br />
                        <center>
        <img src="<%=AppHelper.CompressImage("throbber2.gif" ) %>" /></center>
                    </div>
                    <div id="dvPayPal" style='display: none'>
                        <div class="control">
                            <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/PayPalResult">
                                <%=Html.AntiForgeryToken() %>
                                <input type="submit" id="btnPayPal" />
                            </form>
                        </div>
                    </div>
                    <div id="dvCard">
                        <div class="control">
                            <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/CreditCardResult">
                                <%=Html.AntiForgeryToken() %>
                                <div class="row">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label for="CardNumber" class="text-uppercase">Card Number:</label><span class="text-danger">*</span>
                                            <%=Html.TextBox("CardNumber", Model.CardNumber, new { @maxlength = "16", @size = "16", @class="form-control",@placeholder="Card Number" })%>
                                            <%= Html.ValidationMessageArea("CardNumber")%>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-sm-4 col-xs-6">
                                        <div class="form-group">

                                            <label for="ExpDate" class="text-uppercase">Expiration Date</label><span class="text-danger">*</span>
                                            <select name="ExpirationMonth" class="form-control">
                                                <%for (int i = 1; i < 13; i++)
                                                  {%>
                                                <%if (Model.ExpirationMonth != i)
                                                  {%>
                                                <option value="<%=i %>">&nbsp;&nbsp;<%=((i < 10) ? "0" : "") + i.ToString()%></option>
                                                <%}
                                                  else
                                                  { %>
                                                <option value="<%=i %>" selected="selected">&nbsp;&nbsp;<%=((i < 10) ? "0" : "") + i.ToString()%></option>
                                                <%} %>
                                                <%} %>
                                            </select>

                                        </div>
                                    </div>

                                    <div class="col-xs-1 text-center hidden-xs">
                                        <div class="form-group">
                                            <p>&nbsp;</p>
                                            <span>/</span>
                                        </div>
                                    </div>

                                    <div class="col-sm-4 col-xs-6">
                                        <div class="form-group">
                                            <label for="date" class="text-uppercase">&nbsp;</label>
                                            <br />
                                            <select name="ExpirationYear" class="form-control">
                                                <%for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 10; i++)
                                                  {%>
                                                <%if (Model.ExpirationMonth != i)
                                                  {%>
                                                <option value="<%=i %>">&nbsp;&nbsp;<%=i.ToString()%></option>
                                                <%}
                                                  else
                                                  { %>
                                                <option value="<%=i %>" selected="selected">&nbsp;&nbsp;<%=i.ToString()%></option>
                                                <%} %>
                                                <%} %>
                                            </select>
                                        </div>
                                    </div>

                                    <div class="col-sm-3 hidden-xs">
                                        <div class="form-group">
                                            <p>&nbsp;</p>
                                            <span>( mm / yyyy )</span>
                                            <%= Html.ValidationMessageArea("ExpirationMonth")%><%= Html.ValidationMessageArea("ExpirationYear")%>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label for="ExpDate" class="text-uppercase">Card Secure Code:</label>
                                            <input type="text" name="CardCode" value="<%=Model.CardCode %>" class="form-control" placeholder="Card Secure Code">
                                            <%= Html.ValidationMessageArea("CardCode")%>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-12">
                                        <div class="form-group">
                                            <label for="address1" class="text-uppercase">Address 1</label>
                                            <span class="text-danger">*</span>

                                            <%=Html.TextBox("Address1", Model.Address1, new { @class="form-control",@placeholder="Address 1"})%>
                                            <%= Html.ValidationMessageArea("Address1")%>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-sm-12">
                                        <div class="form-group">
                                            <label for="address2" class="text-uppercase">Address 2</label>
                                            <%=Html.TextBox("Address2", Model.Address2, new { @class="form-control",@placeholder="Address 2"})%>
                                            <%= Html.ValidationMessageArea("Address2")%>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-3 col-xs-6">
                                        <div class="form-group">
                                            <label for="city" class="text-uppercase">City</label>
                                            <span class="text-danger">*</span>
                                            <%=Html.TextBox("City", Model.City, new { @class="form-control",@placeholder="City"})%>
                                            <%= Html.ValidationMessageArea("City")%>
                                        </div>
                                    </div>
                                    <div class="col-md-3 col-xs-6">
                                        <div class="form-group">
                                            <label for="state" class="text-uppercase">State</label>
                                            <span class="text-danger">*</span>
                                            <%=Html.TextBox("State", Model.State, new { @class="form-control",@placeholder="State"})%>
                                            <%= Html.ValidationMessageArea("State")%>
                                        </div>
                                    </div>
                                    <div class="col-md-3 col-xs-6">
                                        <div class="form-group">
                                            <label for="zip" class="text-uppercase">Zip</label>
                                            <span class="text-danger">*</span>
                                            <%=Html.TextBox("Zip", Model.Zip, new { @class="form-control",@placeholder="Zip"})%>
                                            <%= Html.ValidationMessageArea("Zip")%>
                                        </div>
                                    </div>
                                    <div class="col-md-3 col-xs-6">
                                        <div class="form-group">
                                            <label for="country" class="text-uppercase">Country</label>
                                            <span class="text-danger">*</span>
                                            <%= Html.DropDownList("Country", (IEnumerable<SelectListItem>)ViewData["Countries"],new { @class="form-control",@placeholder="Country"})%>
                                            <%= Html.ValidationMessageArea("Country")%>
                                            <%= Html.Hidden("CountryTitle") %>
                                        </div>
                                    </div>
                                </div>
                                <p class="text-right"><span class="text-danger">* Required field</span></p>

                                <div class="row margin-top">
                                    <div class="col-sm-12 text-right">
                                        <p id="ccSubmit" class="Submit_Payment" >
                                            <button type="submit" id="submitDeposit" class="btn btn-danger btn-lg">
                                                <span>Submit Payment</span>
                                            </button>
                                        </p>
                                    </div>
                                </div>

                            </form>
                        </div>
                    </div>
                    <%} %>
    </>
   
                <script type="text/javascript"><% if (ViewData["IsCreditCard"] == null || !Convert.ToBoolean(ViewData["IsCreditCard"]))
                                                  { %> $("#dvCard").hide();<%}
                                                  else
                                                  {%>$("#dvCard").show();<%}%></script>
                    <%} %>
                    <div class="back_link" id="linkBack">
                        <%=Html.ActionLink("Return to the Pay for Auction Items Page", "PayForItems", new { controller = "Account", action = "PayForItems" }) %>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntJS" ContentPlaceHolderID="cphScriptBottom" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $(".payment_table_line").toggle(function () {
                $(this).next(".payment_table_content:first").hide();
            }, function () {
                $(this).next(".payment_table_content:first").show();
            });

            $(".collapse_all_message").click(function () {
                $(".payment_table_content").hide();
            });

            $(".expand_all_message").click(function () {
                $(".payment_table_content").show();
            });

            $("#imgAccount").click(function () {
                $("#dvChooseMethod").hide();
                $("#btnAccount").click();
                $("#dvAccount").show();
            });

            $("#imgPayPal, #imgPayPal2").click(function () {
                $("#dvChooseMethod").hide();
                $("#btnPayPal").click();
                $("#dvPayPalImportant").show();
                $("#dvCard").hide();
            });

            $("#ccSubmit").click(function () {
                $("#dvChooseMethod").hide();
                $("#dvAuthorizedNet").show();
                $("#dvCard").hide();
            });

            $("#imgCard, #imgCardTop").click(function () {
                $("#dvCard").show();
            });

            $("#printInvoice").click(function () {
                window.showModalDialog('<%=AppHelper.GetSiteUrl("/Account/PrintInvoice/") %>', "Print", "border=thin; center=1;dialogTop=1; dialogLeft=1; dialogWidth=" + document.body.offsetWidth + "px; dialogHeight=" + document.body.offsetHeight + "px");
            });

            $("#Country").change(function () {
                $("#CountryTitle").attr("value", $("#Country option:selected").text());
            });
            $("#Country").change();
        });
  </script>
</asp:Content>
