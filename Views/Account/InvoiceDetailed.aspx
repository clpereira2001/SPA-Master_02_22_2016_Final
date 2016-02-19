<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
   <%-- <% Html.Style("jQueryCustomControls.css"); %>--%>
    <% UserInvoice userinvoice = ViewData["UserInvoice"] as UserInvoice; %>
    <title>Invoice#<%=(userinvoice==null)?String.Empty:userinvoice.ID.ToString() %> Details - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <% SessionUser cuser = AppHelper.CurrentUser; %>
    <div class="col-md-9 col-sm-8">


        
             
                <% UserInvoice userinvoice = ViewData["UserInvoice"] as UserInvoice; %>
                <h3 style='color: navy; font-weight: bold'>Invoice# <%=(userinvoice==null)?String.Empty:userinvoice.ID.ToString() %> details</h3>
                <% 
                    if (userinvoice == null)
                    { Response.Write("There is no information for this invoice."); }
                    else if (userinvoice.Event.DateEnd.AddMinutes(25) > DateTime.Now)
                    {
                        Response.Write("Auction has ended. Results will be available in approximately 20 to 30 minutes.");
                    }
                    else
                    {
                        List<Invoice> invoices = userinvoice.Invoices.Where(I => I.Amount > 0).ToList();
                        bool paid = invoices.Where(I => I.IsPaid && (I.Payment_ID.HasValue || I.PaymentDeposit_Id.HasValue)).Count() > 0;
                        decimal Total = 0;
                        decimal Subtotal = 0;
                        decimal CouponDiscount = 0;
                        decimal TotalLocal = 0;
                        var paymnets = invoices.Where(I => I.IsPaid && I.Payment_ID.HasValue).GroupBy(I2 => I2.Payment);
                        Payment pm;
                        if (paid)
                        { 
    %>
                <br />
                <p class="ttle">PAID LOTS</p>
                
                <table class="table table-hover table-striped">
                    <thead>
                        <tr class="bg-primary">
                            <th>Method</th>
                            <th>&nbsp</th>
                            <th>Paid Date</th>
                            <th>Note</th>
                            <th>&nbsp;</th>
                            <th>Amount</th>
                            <th>&nbsp;</th>
                        </tr>
                    </thead>
                    <% foreach (var payment in paymnets)
                       {             
           %>
                    <tr >
                        <td colspan="2">
                            <%=payment.Key.PaymentType.Title%>
            </td>
                        <td>
                            <%=payment.Key.PaidDate.Value.ToShortDateString()%>
            </td>
                        <td colspan="2">
                            <%=String.IsNullOrEmpty(payment.Key.Notes)?String.Empty:payment.Key.Notes.Replace(",", ", ") %>
            </td>
                        <td style="font-weight: bold" colspan="2">
                            <%=(payment.Key.Amount + (payment.Key.Discount.HasValue ? payment.Key.Discount.Value : 0)).GetCurrency()%>
            </td>
                    </tr>
                    <tr class="payment_table_line" style="font-weight: normal">
                        <td>&nbsp;</td>
                        <td colspan="5">
                            <table cellpadding="0" cellspacing="0">
                                <%--<colgroup>
                                    <col width="100px" />
                                    <col width="300px" />
                                </colgroup>--%>
                                <tr>
                                    <td style='background-color: #002868; color: #FFFFFF; font-weight: bold; border: 0px solid black; padding-left: 10px'>Payment address</td>
                                    <td class="tableTLBR">
                                        <% if (payment.Key.PaymentType_ID != (byte)Consts.PaymentType.Paypal && payment.Key.PaymentType_ID != (byte)Consts.PaymentType.W_PayPal)
                                           { %>
                                        <%=payment.Key.Address%>, <%=payment.Key.City%>, <%=payment.Key.State%>, <%=payment.Key.Zip%>
                                        <%}
                                           else
                                           { %>
                                        <%=(payment.Key.IsPickedUp) ? String.Format("{0}, {1}, {2}, {3}",payment.Key.Address,payment.Key.City,payment.Key.State, payment.Key.Zip) : payment.Key.ShippingAddressComma %>
                                        <% } %>
                  </td>
                                </tr>
                            </table>
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                    <% Total += payment.Key.Amount + (payment.Key.Discount.HasValue ? payment.Key.Discount.Value : 0); %>
                    <% if ((pm = payment.FirstOrDefault().DPayment) != null)
                       { %>
                    <tr class="paymnet_table_line_deposit" style="font-weight: bold">
                        <td colspan="2">
                            <%=pm.PaymentType.Title%>
              </td>
                        <td>
                            <%=pm.PaidDate.Value.ToShortDateString()%>
              </td>
                        <td colspan="2">
                            <%=pm.Notes%>
              </td>
                        <td style="font-weight: bold" colspan="2">
                            <%=(pm.Amount + (pm.Discount.HasValue ? pm.Discount.Value : 0)).GetCurrency()%>
              </td>
                    </tr>
                    <tr class="paymnet_table_line_deposit">
                        <td>&nbsp;</td>
                        <td colspan="5">
                            <table style="table-layout: fixed;" cellpadding="0" cellspacing="0">
                                <%--<colgroup>
                                    <col width="100px" />
                                    <col width="300px" />
                                </colgroup>--%>
                                <tr>
                                    <td style='background-color: #002868; color: #FFFFFF; font-weight: bold; border: 0px solid black; padding-left: 10px'>Payment address</td>
                                    <td class="tableTLBR">
                                        <% if (payment.Key.PaymentType_ID != (byte)Consts.PaymentType.Paypal && payment.Key.PaymentType_ID != (byte)Consts.PaymentType.W_PayPal)
                                           { %>
                                        <%=payment.Key.Address%>, <%=payment.Key.City%>, <%=payment.Key.State%>, <%=payment.Key.Zip%>
                                        <%}
                                           else
                                           { %>
                                        <%=(payment.Key.IsPickedUp) ? String.Format("{0}, {1}, {2}, {3}",payment.Key.Address,payment.Key.City,payment.Key.State, payment.Key.Zip) : payment.Key.ShippingAddressComma %>
                                        <% } %>
                  </td>
                                </tr>
                            </table>
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                    <% Total += pm.Amount + (pm.Discount.HasValue ? pm.Discount.Value : 0); %>
                    <%} %>

                    <tr class="payment_table_content">
                        <td colspan="7" class="payment_table_content_td">
                            <table >
                                <tr class="bg-primary">
                                    <td  >Lot#</td>
                                    <td  >Date</td>
                                    <td  >Description</td>
                                    <td  >Quantity</td>
                                    <td  >Amount</td>
                                </tr>
                                <% foreach (Invoice invoice in payment.ToList())
                                   {
                                       Subtotal += invoice.TotalCostWithoutCouponDiscount;
                                       CouponDiscount += (invoice.Real_BP.HasValue ? invoice.Real_BP.GetValueOrDefault(0) - invoice.BuyerPremium.GetValueOrDefault(0) : 0) +
                                                          (invoice.Real_Sh.HasValue ? invoice.Real_Sh.GetValueOrDefault(0) - invoice.Shipping.GetValueOrDefault(0) : 0) +
                                                          (invoice.Real_Discount.HasValue ? invoice.Discount.GetValueOrDefault(0) - invoice.Real_Discount.GetValueOrDefault(0) : 0);
                                       TotalLocal += invoice.Total;    
                  %>
                                <tr class="payment_table_line2">
                                    <td><%=Html.ActionLink(invoice.Auction.Lot.ToString(), "AuctionDetail", new { controller="Auction", action="AuctionDetail", id=invoice.Auction_ID, evnt=invoice.Auction.Event.UrlTitle, cat=invoice.Auction.UrlCategory, lot=invoice.Auction.UrlTitle }) %></td>
                                    <td><%=invoice.DateCreated.ToShortDateString()%></td>
                                    <td><%=invoice.Auction.Title%></td>
                                    <td><%=invoice.Quantity%></td>
                                    <td><%=invoice.TotalCostWithoutCouponDiscount.GetCurrency()%></td>
                                </tr>
                                <%} %>
                                <% if (CouponDiscount > 0)
                                   { %>
                                <tr class="payment_table_line">
                                    <td colspan="4" style="text-align: right">Subtotal:</td>
                                    <td><%=Subtotal.GetCurrency()%></td>
                                </tr>
                                <tr class="payment_table_line">
                                    <td colspan="4" style="text-align: right">Promo Discount:</td>
                                    <td><%=CouponDiscount.GetCurrency()%></td>
                                </tr>
                                <tr class="payment_table_header2">
                                    <td colspan="4" style="text-align: right">Total Paid:</td>
                                    <td><%=TotalLocal.GetCurrency()%></td>
                                </tr>
                                <%} %>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <hr />
                        </td>
                    </tr>
                    <%} %>

                    <% paymnets = userinvoice.Invoices.Where(I => I.IsPaid && I.PaymentDeposit_Id.HasValue && !I.Payment_ID.HasValue).GroupBy(I2 => I2.DPayment); %>
                    <% foreach (var payment in paymnets)
                       {             
           %>
                    <tr class="paymnet_table_line_deposit">
                        <td colspan="2">
                            <%=payment.Key.PaymentType.Title%>
            </td>
                        <td>
                            <%=payment.Key.PaidDate.Value.ToShortDateString()%>
            </td>
                        <td colspan="2">
                            <%=payment.Key.Notes.Replace(",", ", ") %>
            </td>
                        <td style="font-weight: bold">
                            <%=(payment.Key.Amount + (payment.Key.Discount.HasValue ? payment.Key.Discount.Value : 0)).GetCurrency()%>
            </td>
                    </tr>
                    <tr class="paymnet_table_line_deposit">
                        <td>&nbsp;</td>
                        <td colspan="3">
                            <%=payment.Key.FullAddress%>
            </td>
                        <td colspan="2">
                            <%=payment.Key.ShippingAddress%>
            </td>
                    </tr>
                    <% Total += payment.Key.Amount + (payment.Key.Discount.HasValue ? payment.Key.Discount.Value : 0); %>
                    <tr class="payment_table_content">
                        <td colspan="6" class="payment_table_content_td">
                            <table style="margin: 0px; table-layout: fixed; padding-left: 20px; padding-right: 20px;" cellpadding="0" cellspacing="0">
                                <tr class="payment_table_header3">
                                    <td style='width: 80px'>Lot#</td>
                                    <td style='width: 80px'>Date</td>
                                    <td style='width: 370px'>Description</td>
                                    <td style='width: 50px;'>Quantity</td>
                                    <td style='width: 100px'>Amount</td>
                                </tr>
                                <%  Subtotal = CouponDiscount = TotalLocal = 0;
                                    foreach (Invoice invoice in payment.ToList())
                                    {
                                        Subtotal += invoice.TotalCostWithoutCouponDiscount;
                                        CouponDiscount += (invoice.Real_BP.HasValue ? invoice.Real_BP.GetValueOrDefault(0) - invoice.BuyerPremium.GetValueOrDefault(0) : 0) +
                                                           (invoice.Real_Sh.HasValue ? invoice.Real_Sh.GetValueOrDefault(0) - invoice.Shipping.GetValueOrDefault(0) : 0) +
                                                           (invoice.Real_Discount.HasValue ? invoice.Discount.GetValueOrDefault(0) - invoice.Real_Discount.GetValueOrDefault(0) : 0);
                                        TotalLocal += invoice.Total;
                 %>
                                <tr class="payment_table_line2">
                                    <td><%=Html.ActionLink(invoice.Auction.Lot.ToString(), "AuctionDetail", new { controller="Auction", action="AuctionDetail", id=invoice.Auction_ID, evnt=invoice.Auction.Event.UrlTitle, cat=invoice.Auction.UrlCategory, lot=invoice.Auction.UrlTitle }) %></td>
                                    <td><%=invoice.DateCreated.ToShortDateString()%></td>
                                    <td><%=invoice.Auction.Title%></td>
                                    <td><%=invoice.Quantity%></td>
                                    <td><%=invoice.Total.GetCurrency()%></td>
                                </tr>
                                <%} %>
                                <% if (CouponDiscount > 0)
                                   { %>
                                <tr class="payment_table_line">
                                    <td colspan="4" style="text-align: right">Subtotal:</td>
                                    <td><%=Subtotal.GetCurrency()%></td>
                                </tr>
                                <tr class="payment_table_line">
                                    <td colspan="4" style="text-align: right">Promo Discount:</td>
                                    <td><%=CouponDiscount.GetCurrency()%></td>
                                </tr>
                                <tr class="payment_table_header2">
                                    <td colspan="4" style="text-align: right">Total Paid:</td>
                                    <td><%=TotalLocal.GetCurrency()%></td>
                                </tr>
                                <%} %>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <hr />
                        </td>
                    </tr>
                    <%} %>

                    <tr class="payment_table_header2">
                        <td colspan="5" style="text-align: right">Total Paid:
          </td>
                        <td>
                            <%=Total.GetCurrency()%>
          </td>
                    </tr>
                </table>
                <%}%>
                <%
                                invoices = invoices.Where(I => !I.IsPaid).ToList();
                                if (invoices.Count() > 0)
                                {
                                    Total = 0;        
    %>
                <br />
                <p class="ttle">UNPAID LOTS</p>
                <table cellpadding="0" cellspacing="0" style="table-layout: fixed;">
                    <tr class="payment_table_header">
                        <td style='width: 80px'>Lot#</td>
                        <td style='width: 80px'>Date</td>
                        <td style='width: 350px'>Description</td>
                        <td style='width: 80px;'>Quantity</td>
                        <td style='width: 100px'>Amount</td>
                    </tr>
                    <% foreach (Vauction.Models.Invoice invoice in invoices)
                       {             
           %>
                    <tr class="payment_table_line">
                        <td><%=Html.ActionLink(invoice.Auction.Lot.ToString(), "AuctionDetail", new { controller="Auction", action="AuctionDetail", id=invoice.Auction_ID, evnt=invoice.Auction.Event.UrlTitle, cat=invoice.Auction.UrlCategory, lot=invoice.Auction.UrlTitle }) %></td>
                        <td>
                            <%=invoice.DateCreated.ToShortDateString()%>
            </td>
                        <td>
                            <%=invoice.Auction.Title%>
            </td>
                        <td>
                            <%=invoice.Quantity %>              
            </td>
                        <td>
                            <%=invoice.Total.GetCurrency()%>
            </td>
                    </tr>
                    <% Total += invoice.Total; %>
                    <tr class="payment_table_content">
                        <td colspan="5" class="payment_table_content_td">
                            <table style="margin: 0px; table-layout: fixed; padding-left: 20px; padding-right: 20px;" cellpadding="0" cellspacing="0">
                                <colgroup>
                                    <col width='280px' />
                                    <col width='160px' />
                                    <col width='160px' />
                                    <col width='110px' />
                                </colgroup>
                                <tr>
                                    <td>&nbsp;</td>
                                    <% if (invoice.Quantity == 1)
                                       {%>
                                    <td class="tableTL" colspan="2">Winning Bid
                     </td>
                                    <% }
                                       else
                                       { %>
                                    <td class="tableTL" colspan="1">Winning Bid</td>
                                    <td class="tableT" style="text-align: right"><%=invoice.Quantity%>&nbsp;x&nbsp;<%=(invoice.Amount / invoice.Quantity).GetCurrency()%>&nbsp;=&nbsp;
                     </td>
                                    <%} %>
                                    <td class="tableTLR">
                                        <%=invoice.Amount.GetCurrency()%>
                    </td>
                                </tr>
                                <% if (invoice.Auction.AuctionType_ID != (long)Consts.AuctionType.DealOfTheWeek && invoice.BuyerPremium.HasValue)
                                   {%>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td class="tableTL" colspan="2">Buyer's premium
                    </td>
                                    <td class="tableTLR">
                                        <%=invoice.BuyerPremium.Value.GetCurrency()%>
                    </td>
                                </tr>
                                <%} %>
                                <% if (invoice.Shipping.HasValue && invoice.Shipping.Value > 0)
                                   { %>
                                <tr>
                                    <td>&nbsp;</td>
                                    <% if (!invoice.Auction.IsConsignorShip)
                                       { %>
                                    <td class="tableTL" colspan="2">Shipping and handling
                    </td>
                                    <%}
                                       else
                                       { %>
                                    <td class="tableTL">Shipping and handling</td>
                                    <td class="tableT" style="text-align: right">(consignor ship)</td>
                                    <%} %>
                                    <td class="tableTLR">
                                        <span class="accordion_shipping"><%=invoice.Shipping.GetCurrency()%></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td class="<%=(invoice.Tax!= null && invoice.Tax.Value > 0)?"tableTL":"tableTLB" %>" colspan="2">Insurance
                    </td>
                                    <td class="<%=(invoice.Tax != null && invoice.Tax.Value > 0)?"tableTLR":"tableTLBR" %>">
                                        <span class="accordion_insurance"><%=invoice.Insurance.GetCurrency()%></span>
                                    </td>
                                </tr>
                                <%} %>
                                <% if (invoice.Tax != null && invoice.Tax.Value > 0)
                                   { %>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td class="tableTLB" colspan="2">Tax
                      </td>
                                    <td class="tableTLBR">
                                        <%=invoice.Tax.GetCurrency()%>
                      </td>
                                </tr>
                                <%} %>
                                <% if (invoice.Discount.HasValue && invoice.Discount.Value > 0)
                                   { %>
                                <tr>
                                    <td>&nbsp;</td>
                                    <td class="tableBL" colspan="2">Discount
                      </td>
                                    <td class="tableBLR">
                                        <%=invoice.Discount.GetCurrency()%>
                      </td>
                                </tr>
                                <%} %>
                            </table>
                        </td>
                    </tr>
                    <%} %>
                    <tr class="payment_table_header2">
                        <td colspan="4">&nbsp;</td>
                        <td style="text-align: right">Total Due:
          </td>
                        <td>
                            <%=Total.GetCurrency()%>
          </td>
                    </tr>
                </table>
                <%} %>
                <%} %>
                <div class="back_link"><%=Html.ActionLink("Return to the Past Auction Invoices page", "PastInvoices", new { controller = "Account", action = "PastInvoices" })%></div>
            </div>
        

    
</asp:Content>
