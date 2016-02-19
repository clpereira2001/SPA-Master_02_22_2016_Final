<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <% Html.Style("jQueryCustomControls.css"); %>
    <title>Deal of the week invoice details - <%=Consts.CompanyTitleName %></title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">

        <div class="row">
            <div class="col-sm-12">
                <div class="center_content">
                    <p>Below is a list of your invoices. Invoices are listed in reverce order by begining date.</p>
                    <%
                        List<Invoice> invoices = ViewData["UserInvoice"] as List<Invoice>;
                        if (invoices == null) { Response.Write("There are currently no invoices."); }
                        else
                        {
                            decimal Total = 0;        
    %>
                    <br />
                    <div class="table-responsive">
                        <table class="table table-hover table-striped table-down">
                            <thead>
                                <tr class="bg-primary">

                                    <th class="align-middle">Lot#</th>
                                    <th class="text-center align-middle">Date</th>
                                    <th class="align-middle">Title</th>
                                    <th class="text-center align-middle">Qality</th>
                                    <th class="text-center align-middle">Amount</th>
                                </tr>
                            </thead>

                            <tbody>
                                <% foreach (Invoice invoice in invoices)
                                   {             
           %>

                                <tr class="payment_table_line">
                                    <td>
                                        <%=invoice.Auction.Lot.ToString()%>
            </td>
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
                                                <td class="tableTL" colspan="2">Shipping and handling
                    </td>
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
                                    <td colspan="3">&nbsp;</td>
                                    <td style="text-align: right">Total Due:
          </td>
                                    <td>
                                        <%=Total.GetCurrency()%>
          </td>
                                </tr>
                            </tbody>
                        </table>
                        <%} %>
                    </div>
                    <div class="back_link"><%=Html.ActionLink("Return to the Past Auction Invoices page", "PastInvoices", new { controller = "Account", action = "PastInvoices" })%></div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
