<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<PaymentConfirmation>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <title>Printing invoices -
    <%=Consts.CompanyTitleName%></title>
  <script type="text/javascript" src="<%=Context.Request.Url.Scheme.ToLower() + "://" %>ajax.googleapis.com/ajax/libs/jquery/1.4.4/jquery.min.js"></script>
  <% Html.Style("combinedstyle_20140216.css"); %>
  <% Html.Style("jQueryCustomControls.css"); %>
  <%= Html.CompressJs(Url) %>
  <%= Html.CompressCss(Url) %>
  <% Html.Clear(); %>
</head>
<body style="font-family: Arial, Helvetica, sans-serif; font-size: 12px">
  <%    
    if (Model != null && Model.Invoices != null)
    {
  %>
  <div class="center_content">
    <div id="header" style="width: 660px">
      <div style="clear: both; width: 100%; background: #123e6b; float: left;">
        <div class="span-4" style="padding: 10px 0 10px 0">
          <img src="<%=AppHelper.CompressImage("logo_1.jpg") %>" alt="" />
        </div>
        <div id="middleheader" style="float: left; color: #FFF; margin-top: 30px; margin-right: 10px;
          margin-left: -10px">
        <div style="font-family: Arial; font-size: 20px">
            OFFICIAL WEBSITE OF FEDERAL ASSET RECOVERY</div>
          <div style="letter-spacing: 0.059em; font-family: Arial; font-size: 25px; font-weight: bold;">
            SEIZED PROPERTY AUCTIONS.COM</div>
<%--          <div style="font-family: 'Century Gothic'; font-size: 10px;">
            A PRIVATE CORPORATION SERVING GOVERNMENT, BUSINESS, AND PRIVATE INDIVIDUALS SINCE
            2002.</div>--%>
        </div>
      </div>
    </div>
    <br class="clear" />
    <span style="color: #000099; font-size: 14px; font-weight: bold">Invoices</span>
    <br class="clear" />
    <table cellpadding="0" cellspacing="0" style="table-layout: fixed; margin-bottom:0px;">
      <tr>
        <th style='width: 80px'>
          &nbsp;
        </th>
        <th style='width: 80px'>
          &nbsp;
        </th>
        <th style='width: 320px'>
          &nbsp;
        </th>
        <th style='width: 80px;'>
          &nbsp;
        </th>
        <th style='width: 100px'>
          &nbsp;
        </th>
      </tr>
      <tr class="payment_table_header">
        <td>
          Lot#
        </td>
        <td>
          Date
        </td>
        <td>
          Description
        </td>
        <td>
          Quantity
        </td>
        <td>
          Amount
        </td>
      </tr>
      <% foreach (InvoiceDetail invoice in Model.Invoices)
         {             
      %>
      <tr class="payment_table_line">
        <td>
          <%=invoice.LinkParams.Lot.ToString() %>
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
          <table style="margin: 0px; table-layout: fixed; padding-left: 20px; padding-right: 20px;"
            cellpadding="0" cellspacing="0">
            <colgroup>
              <col width='190px' />
              <col width='150px' />
              <col width='150px' />
              <col width='110px' />
            </colgroup>
            <tr>
              <td>
                &nbsp;
              </td>
              <% if (invoice.Quantity == 1)
                 {%>
              <td class="tableTL" colspan="2">
                Winning Bid
              </td>
              <% }
                 else
                 { %>
              <td class="tableTL" colspan="1">
                Winning Bid
              </td>
              <td class="tableT" style="text-align: right">
                <%=invoice.Quantity%>&nbsp;x&nbsp;<%=(invoice.Amount / invoice.Quantity).GetCurrency()%>&nbsp;=&nbsp;
              </td>
              <%} %>
              <td class="tableTLR">
                <%=invoice.Amount.GetCurrency()%>
              </td>
            </tr>
            <% if (invoice.AuctionType != (long)Consts.AuctionType.DealOfTheWeek && (invoice.BuyerPremium>0 || invoice.RealBP.HasValue))
               {%>
            <tr>
              <td>
                &nbsp;
              </td>
              <td class="tableTL" colspan="2">
                Buyer's premium
              </td>
              <td class="tableTLR">
                <%=invoice.RealBP.GetValueOrDefault(invoice.BuyerPremium).GetCurrency()%>
              </td>
            </tr>
            <%} %>
            <% if (invoice.Shipping > 0 || invoice.RealSh.HasValue)
               { %>
            <tr>
              <td>
                &nbsp;
              </td>
              <td class="tableTL" colspan="2">
                Shipping and handling
              </td>
              <td class="tableTLR">
                  <%=invoice.RealSh.GetValueOrDefault(invoice.Shipping).GetCurrency(false)%>
              </td>
            </tr>
            <tr>
              <td>
                &nbsp;
              </td>
              <td class="<%=(invoice.Tax > 0)?"tableTL":"tableTLB" %>" colspan="2">
                Insurance
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
              <td>
                &nbsp;
              </td>
              <td class="tableTLB" colspan="2">
                Tax
              </td>
              <td class="tableTLBR">
                <%=invoice.Tax.GetCurrency()%>
              </td>
            </tr>
            <%} %>
             <% if (invoice.Discount > 0 && invoice.TotalOrderDiscount.GetValueOrDefault(0) != invoice.Discount)
               { %>
            <tr>
              <td>
                &nbsp;
              </td>
              <td class="tableBL" colspan="2">
                Discount
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
      <tr class="payment_table_header2">
        <td style="text-align: right" colspan="4">Total Due:</td>
        <td><%=(Model.TotalCostWithoutDiscount).GetCurrency()%></td>
      </tr>
      <% if (Model.HasDiscount){%>
        <tr class="payment_table_footer3">
          <td colspan="4" style="text-align: right">Promo Discount:</td>
          <td><%=Model.DiscountAmount.GetCurrency()%></td>
        </tr>
        <tr class="payment_table_footer2">          
          <td colspan="4" style="text-align: right">Payment Due After Discount:</td>
          <td><%=Model.TotalCost.GetCurrency()%></td>
        </tr>
      <% } %>
      <% if (Model.TotalOrDeposit > 0){%>
      <tr class="payment_table_footer3">
        <td colspan="4" style="text-align: right">Deposit:</td>
        <td><%=Model.TotalOrDeposit.GetCurrency()%></td>
      </tr>
      <tr class="payment_table_footer2">
        <td colspan="4" style="text-align: right"><%=(Model.TotalCost - Model.Deposit <= 0) ? "Remaining Balance" : "Balance Due:"%></td>
        <td><%=(Model.TotalCost - Model.Deposit <= 0) ? (Model.Deposit - Model.TotalCost).GetCurrency(false) : (Model.TotalCost - Model.Deposit).GetCurrency(false)%></td>
      </tr>
      <%} %>
    </table> 
    <br class="clear" />
    <b>Printed date</b>:
    <%=DateTime.Now%>
    <%} %>
  </div>
  <script type="text/javascript">
    $(document).ready(function () { window.print(); });
  </script>
</body>
</html>
