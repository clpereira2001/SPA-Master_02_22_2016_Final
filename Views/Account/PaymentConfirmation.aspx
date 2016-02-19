<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PaymentConfirmation>" %>

<%@ Import Namespace="Vauction.Utils.Autorization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Payment receipt - <%=Consts.CompanyTitleName %></title>
    <% Html.Style("jQueryCustomControls.css"); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% SessionUser cuser = AppHelper.CurrentUser; %>
    <div class="col-md-9 col-sm-8">
        <div class="row">
            <div class="col-sm-12">
                <%-- <h1 class="title">Payment Confirmation</h1>--%>
                <p>
                    Congratulations and thank you for your prompt payment.<br />
                    <%if (Model.IsShipping)
                      { %>
                We will be shipping out your item<%=(Model.Quantity > 1) ? "s" : ""%> and sending you a shipping confirmation email within 8 business days.
          
                    <%}
                      else
                      {                
                %>Your item<%=(Model.Quantity > 1) ? "s" : ""%> will be held at will call and will be available Monday - Friday, 8:00AM - 4:30PM ET<br />
                    <b><%=Consts.CompanyTitleName %></b><br />
                    <b><%=Consts.CompanyAddress %></b><br />
                    <%} %>
                </p>
                <p>
                    <% if (!String.IsNullOrEmpty(Model.TransactionID))
                       { %>

                    <span class="text-primary text-weight-700">Transaction information </span>
                    <br />
                    <%=(Model.PaymentType == Consts.PaymentType.Paypal) ? "Express Checkout Payment " : "Paid by Credit Card, "%>Transaction ID# <strong><%=Model.TransactionID%></strong>
                    <%}
                       else
                       { %>
                    <span class="text-primary text-weight-700"><strong>Paid from the Account Balance</strong></span>
                    <%} %>
                </p>
                <br />
                <br />
                <p class="text-primary text-weight-700">
                    Billing and Shipping information
                </p>
                <div class="row">
                    <div class="col-sm-12">
                        <div>
                            <table class="table table-hover table-striped">

                                <thead>
                                    <tr class="bg-primary">
                                        <th class="align-middle">Billing information</th>
                                        <th class="align-middle">Shipping information</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>

                                        <td class="align-middle"><%=String.Format("{0} (<b>{1}</b>)", Model.Address_Billing.FullName, Html.Encode(cuser.Login.ToUpper()))%></td>
                                        <% if (Model.IsShipping) { Response.Write("<td style='border-top: solid 1px #DDDDDD;border-right:solid 1px #DDDDDD; padding-left: 10px; background-color: White;'>" + String.Format("{0} (<b>{1}</b>)", Model.Address_Shipping.FullName, Html.Encode(cuser.Login.ToUpper())) + "&nbsp;</td>"); }
                                           else
                                           {%>
                                        <td class="align-middle" rowspan="<%=((!String.IsNullOrEmpty(Model.Address_Billing.WorkPhone) || (Model.Address_Shipping!=null && !String.IsNullOrEmpty(Model.Address_Shipping.WorkPhone))))?"8":"7" %>">Your item<%=(Model.Quantity > 1) ? "s" : ""%> will be held at will call and will be available Monday - Friday, 8AM - 4:30PM ET<br />
                                            <b><%=Consts.CompanyTitleName %></b><br />
                                            <b><%=Consts.CompanyAddress %></b>
                                        </td>
                                        <%} %>
                                    </tr>
                                    <tr>
                                        <td class="align-middle"><%=Model.Address_Billing.Address_1 %></td>
                                        <%=(Model.IsShipping) ? "<td class='align-middle'>" + Model.Address_Shipping.Address_1 + "&nbsp;</td>" : ""%>
                                    </tr>
                                    <tr>
                                        <td class="align-middle"><%=Model.Address_Billing.Address_2 %></td>
                                        <%=(Model.IsShipping) ? "<td class='align-middle'>" + Model.Address_Shipping.Address_2 + "&nbsp;</td>" : ""%>
                                    </tr>
                                    <tr>
                                        <td class="align-middle"><%=Model.Address_Billing.City %>, <%=Model.Address_Billing.State %></td>
                                        <%=(Model.IsShipping) ? "<td class='align-middle'>" + Model.Address_Shipping.City + ", " + Model.Address_Shipping.State + "&nbsp;</td>" : ""%>
                                    </tr>
                                    <tr>
                                        <td class="align-middle"><%=Model.Address_Billing.Zip %></td>
                                        <%=(Model.IsShipping) ? "<td class='align-middle'>" + Model.Address_Shipping.Zip + "&nbsp;</td>" : ""%>
                                    </tr>
                                    <tr>
                                        <td class="align-middle"><%=Model.Address_Billing.Country %></td>
                                        <%=(Model.IsShipping) ? "<td class='align-middle'>" + Model.Address_Shipping.Country + "&nbsp;</td>" : ""%>
                                    </tr>
                                    <% if (!String.IsNullOrEmpty(Model.Address_Billing.WorkPhone) || (Model.Address_Shipping != null && !String.IsNullOrEmpty(Model.Address_Shipping.WorkPhone)))
                                       { %>
                                    <tr>
                                        <td class="align-middle"><%=Model.Address_Billing.HomePhone%></td>
                                        <%=(Model.IsShipping) ? "<td class='align-middle'>" + Model.Address_Shipping.HomePhone + "&nbsp;</td>" : ""%>
                                    </tr>
                                    <% }
                                       else
                                       { %>
                                    <tr>
                                        <td class="align-middle"><%=Model.Address_Billing.HomePhone%></td>
                                        <%=(Model.IsShipping) ? "<td class='align-middle'>" + Model.Address_Shipping.HomePhone + "&nbsp;</td>" : ""%>
                                    </tr>
                                    <% } %>
                                    <% if (!String.IsNullOrEmpty(Model.Address_Billing.WorkPhone) || (Model.Address_Shipping != null && !String.IsNullOrEmpty(Model.Address_Shipping.WorkPhone)))
                                       { %>
                                    <tr>
                                        <td class="align-middle"><%=Model.Address_Billing.WorkPhone%></td>
                                        <%=(Model.IsShipping) ? "<td class='align-middle'>" + Model.Address_Shipping.WorkPhone + "&nbsp;</td>" : ""%>
                                    </tr>
                                    <% } %>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <br />
                <br />
                <p class="text-primary text-weight-700">Invoices</p>

                <div class="table-responsive">
                    <table class="table table-hover table-striped">
                        <thead>
                            <tr class="bg-primary">
                                <th class="align-middle">Lot#</th>
                                <th class="text-center align-middle">Date</th>
                                <th class="text-center align-middle">Description</th>
                                <th class="text-center align-middle">Quantity</th>
                                <th class="text-center align-middle">Amount</th>
                            </tr>
                        </thead>
                        <tbody>
                            <% foreach (InvoiceDetail invoice in Model.Invoices)
                               {             
           %>
                            <tr >
                                <td class="align-middle text-center">
                                    <%= Html.ActionLink(invoice.LinkParams.Lot.ToString(), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = invoice.LinkParams.ID, evnt=invoice.LinkParams.EventUrl, cat=invoice.LinkParams.CategoryUrl, lot=invoice.LinkParams.LotTitleUrl })%>
            </td>
                                <td class="text-center align-middle">
                                    <%= invoice.DateCreated.ToShortDateString()%>
            </td>
                                <td class="align-middle text-center">
                                    <%= invoice.LinkParams.Title%>
            </td>
                                <td class="text-center align-middle">
                                    <%= invoice.Quantity %>             
            </td>
                                <td class="text-center align-middle">
                                    <%= invoice.TotalCostWithoutCouponDiscount.GetCurrency()%>              
            </td>
                            </tr>
                            <tr class="bg-primary">
                                <td colspan="5" class="payment_table_content_td">
                                    <table style="margin: 0px; table-layout: fixed; padding-left: 20px; padding-right: 20px;" cellpadding="0" cellspacing="0">
                                        
                                        <tr>
                                            <td>&nbsp;</td>
                                            <% if (invoice.Quantity == 1)
                                               {%>
                                            <td class="align-middle" colspan="2">Winning Bid
                     </td>
                                            <% }
                                               else
                                               { %>
                                            <td class="align-middle" colspan="1">Winning Bid</td>
                                            <td class="text-center align-middle" style="text-align: right"><%=invoice.Quantity%>&nbsp;x&nbsp;<%=(invoice.Amount / invoice.Quantity).GetCurrency()%>&nbsp;=&nbsp;
                     </td>
                                            <%} %>
                                            <td class="text-center align-middle">
                                                <%=invoice.Amount.GetCurrency() %>
                    </td>
                                        </tr>
                                        <% if (invoice.AuctionType != (long)Consts.AuctionType.DealOfTheWeek && (invoice.BuyerPremium > 0 || invoice.RealBP.HasValue))
                                           {%>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td class="align-middle" colspan="2">Buyer's premium
                    </td>
                                            <td class="text-center align-middle">
                                                <%=invoice.RealBP.GetValueOrDefault(invoice.BuyerPremium).GetCurrency()%>
                    </td>
                                        </tr>
                                        <%} %>
                                        <% if (invoice.Shipping > 0 || invoice.RealSh.HasValue)
                                           { %>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td class="align-middle" colspan="2">Shipping and handling
                    </td>
                                            <td class="text-center align-middle">
                                                <%=invoice.RealSh.GetValueOrDefault(invoice.Shipping).GetCurrency(false)%>
                    </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td class="<%=(invoice.Tax > 0)?"tableTL":"tableTLB" %>" colspan="2">Insurance
                    </td>
                                            <td class="<%=(invoice.Tax > 0)?"tableTLR":"tableTLBR" %>">
                                                <span class="accordion_insurance"><%=invoice.Insurance.GetCurrency(false)%></span>
                                            </td>
                                        </tr>
                                        <%} %>
                                        <% if (invoice.Tax > 0)
                                           { %>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td class="tableTLB" colspan="2">Tax
                      </td>
                                            <td class="tableTLBR">
                                                <%=invoice.Tax.GetCurrency() %>
                      </td>
                                        </tr>
                                        <%} %>
                                        <% if (invoice.Discount > 0 && invoice.TotalOrderDiscount.GetValueOrDefault(0) != invoice.Discount)
                                           { %>
                                        <tr>
                                            <td>&nbsp;</td>
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
                            <% if (Model.HasDiscount)
                               {%>
                            <tr class="payment_table_header2">
                                <td colspan="4" style="text-align: right">Subtotal:</td>
                                <td><%=Model.TotalCostWithoutDiscount.GetCurrency() %></td>
                            </tr>
                            <tr class="payment_table_footer3">
                                <td colspan="4" style="text-align: right">Promo Discount:</td>
                                <td><%=Model.DiscountAmount.GetCurrency()%></td>
                            </tr>
                            <% } %>
                            <tr class="<%=Model.HasDiscount?"payment_table_footer2":"payment_table_header2" %>">
                                <td style="text-align: right" colspan="4">Total Paid:</td>
                                <td><%=Model.TotalCost.GetCurrency() %></td>
                            </tr>
                            <% if (Model.Deposit > 0)
                               {%>
                            <tr class="payment_table_footer3">
                                <td colspan="4" style="text-align: right"><%=(Model.TotalCost - Model.Deposit > 0)?"Deposit":"Paid from Account Balance" %>:</td>
                                <td><%=Model.Deposit.GetCurrency(false)%></td>
                            </tr>
                            <% if (Model.IsDepositRefunded)
                               {%>
                            <tr class="payment_table_footer2">
                                <td colspan="4" style="text-align: right">Refunded Amount: </td>
                                <td><%=Model.DepositLimit.GetCurrency(false) %></td>
                            </tr>
                            <%}
                               else
                               { %>
                            <tr class="payment_table_footer2">
                                <td colspan="4" style="text-align: right"><%=(Model.TotalCost - Model.Deposit <= 0) ? "Account Balance" : "Balance Paid:"%></td>
                                <td><%=(Model.Deposit-Model.TotalCost>=0)? Model.DepositLimit.GetCurrency(false) : (Model.TotalCost - Model.Deposit).GetCurrency(false) %></td>
                            </tr>
                            <%} %>
                            <%} %>
                        </tbody>
                    </table>
                </div>
                <br />
                <br />
                <p>
                    Thank you for bidding on <b style='color: Navy'><%=Consts.CompanyTitleName %></b> and we hope you return to bid with us in the future. If you have questions or need assistance, please <%=Html.ActionLink("contact us", "ContactUs", new { controller = "Home", action = "ContactUs" }) %>.
         
                    <br />
                    <br />
                    Sincerely,
                    <br />
                    <b style='color: Navy'><%=Consts.CompanyTitleName %></b>
                </p>
                <br />
            </div>
            <div class="back_link">
                <%
                    switch (Model.PaymentConfirmationType)
                    {
                        case PaymentConfirmationType.ITEMS:
                            Response.Write(Html.ActionLink("Return to the Pay for Auction Items Page", "PayForItems", new { controller = "Account", action = "PayForItems" }));
                            break;
                        case PaymentConfirmationType.DOW:
                            Response.Write(Html.ActionLink("Return to the Home page", "Index", new { controller = "Home", action = "Index" }));
                            break;
                        default:
                            LinkParams lp = Model.Invoices.First().LinkParams;
                            Response.Write(Html.ActionLink("Back to Lot", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = lp.ID, evnt = lp.EventUrl, cat = lp.CategoryUrl, lot = lp.LotTitleUrl }).ToNonSslLink() + " | " + Html.ActionLink("Back to Event", "EventDetailed", new { controller = "Auction", action = "EventDetailed", id = lp.Event_ID, evnt = lp.EventUrl }));
                            break;
                    }%>
            </div>
        </div>
    </div>
</asp:Content>
