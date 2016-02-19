<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CreditCardInfo>" %>
<%@ Import Namespace="Vauction.Utils.Html" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Pay for deposit - <%=Consts.CompanyTitleName %></title>
    <% Html.Style("jQueryCustomControls.css"); %>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">     
  <div class="center_content">
    <% PreviewBid pb = Session["PreviewBid"] as PreviewBid; %>
    <h1 class="title">Pay for deposit</h1>
<%--    <br class="clear" />--%>
    <table cellpadding="0" cellspacing="0">
      <tr class="payment_table_header">
        <th style='width: 80px'>
          Event#
        </th>
        <th style='width: 420px'>
          Title
        </th>
        <th style='width: 130px'>
          Description
        </th>
        <th style='width: 100px'>
          Total Amount
        </th>
      </tr>
      <tr class="payment_table_content3">
        <td>
          <%=pb.LinkParams.Event_ID %>
        </td>
        <td>
          <%=pb.LinkParams.EventTitle %>          
        </td>
        <td style='color: Navy'>
          Good Faith Deposit<br />Fully Refundable
        </td>
        <td>
          <%=pb.DepositNeed.GetCurrency() %>
        </td>
      </tr>
      <tr class="payment_table_footer3">
        <td colspan="2">&nbsp;</td>
        <td>Payment for deposit:</td>
        <td><%=pb.DepositNeed.GetCurrency()%></td>
      </tr>
    </table>
    
<%--    <br class="br_clear" />--%>
    
    <table id="dvChooseMethod">
      <tr>
        <td style="width: 290px;">Select payment method</td>
        <td style="width: 150px;" id="imgPayPal">
          <img src="<%=AppHelper.CompressImage("PPExpressCheckout.gif") %>"/>
        </td>
        <td>&nbsp</td>
        <td id="imgCard" style="width: 150px;">
          <img src="<%=AppHelper.CompressImage("cards.gif") %>" style="height:24px" />
        </td>
        <td style="width:10px;">&nbsp</td>
      </tr>
    </table>
    
<%--    <br class="clear" />--%>
    <div id="dvPayPalImportant" style="color:rgb(180,0,0);text-align:center; display:none">
      <strong>Please wait a second, you are being redirected to PayPal.</strong>
      <br /><br /><br />
      <center><img src="<%=AppHelper.CompressImage("throbber2.gif") %>" /></center>
    </div>
    
    <div id="dvAuthorizedNet" style="color:rgb(180,0,0);text-align:center; display:none">
      <strong>Please wait while we authorize your credit card. This may take a couple of minutes.</strong>
      <br /><br /><br />
      <center><img src="<%=AppHelper.CompressImage("throbber2.gif") %>" /></center>
    </div>
    <div id="dvPayPal" style='display:none' >      
      <div class="control" >
        <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/PayPalDeposit">
          <%=Html.AntiForgeryToken() %>
          <input type="submit" id="btnPayPal" />
        </form>
      </div>
    </div>
    <div id="dvCard" style='display:none'>    
      <div class="control" style="width:730px;">      
        <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/CreditCardDeposit">
          <%=Html.AntiForgeryToken() %>
          <center>
          <fieldset style="padding:5px;width:600px;"  class="blue_box">          
            <table style="table-layout:fixed;" >
              <tr>
                <td style='text-align:right'>
                  <label for="CardNumber">Card Number:</label><em>*</em>
                </td>
                <td>
                  <p style='text-align:left'><%=Html.TextBox("CardNumber", Model.CardNumber, new { @maxlength = "16", @size = "16" })%></p>
                  <%= Html.ValidationMessageArea("CardNumber")%>
                </td>
              </tr>
              <tr>
                <td style='text-align:right'>
                  <label for="ExpDate">Expiration Date:</label><em>*</em>
                </td>
                <td>
                  <p style='text-align:left'>
                  <select name="ExpirationMonth" style="width:60px">
                    <%for (int i = 1; i < 13; i++){%>
                      <%if (Model.ExpirationMonth != i) {%>
                          <option value="<%=i %>">&nbsp;&nbsp;<%=((i < 10) ? "0" : "") + i.ToString()%></option>
                      <%} else { %>
                          <option value="<%=i %>" selected="selected">&nbsp;&nbsp;<%=((i < 10) ? "0" : "") + i.ToString()%></option>
                      <%} %>
                    <%} %>
                  </select>
                  &nbsp;&nbsp;/
                  <select name="ExpirationYear" style="width:68px">
                    <%for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 10; i++){%>
                      <%if (Model.ExpirationMonth != i) {%>
                          <option value="<%=i %>">&nbsp;&nbsp;<%=i.ToString() %></option>
                      <%} else { %>
                          <option value="<%=i %>" selected="selected">&nbsp;&nbsp;<%=i.ToString() %></option>
                      <%} %>
                    <%} %>
                  </select>&nbsp;&nbsp;&nbsp;( mm / yyyy )                  
                  <%= Html.ValidationMessageArea("ExpirationMonth")%><%= Html.ValidationMessageArea("ExpirationYear")%>
                  </p>
                </td>
              </tr> 
              <tr>
                <td style='text-align:right'>
                  <label for="ExpDate">Card Secure Code:</label><em>*</em>
                </td>
                <td>
                <p style='text-align:left'>
                  <input type="text" size="4" maxlength="4" style="width:60px" name="CardCode" value="<%=Model.CardCode %>" />                  
                </p>
                  <%= Html.ValidationMessageArea("CardCode")%>                  
                </td>
              </tr>
              <tr>
                <td style=';text-align:right'>
                  <label>Address 1:</label><em>*</em>
                </td>
                <td >
                  <p style='text-align:left'><%=Html.TextBox("Address1", Model.Address1)%></p>
                  <%= Html.ValidationMessageArea("Address1")%>
                </td>
              </tr>
              <tr>
                <td style=';text-align:right'>
                  <label>Address 2:</label>
                </td>
                <td>
                  <p style='text-align:left'><%=Html.TextBox("Address2", Model.Address2)%></p>
                  <%= Html.ValidationMessageArea("Address2")%>
                </td>
              </tr>
               <tr>
                <td style=';text-align:right'>
                  <label>City:</label><em>*</em>
                </td>
                <td>
                  <p style='text-align:left'><%=Html.TextBox("City", Model.City)%></p>
                  <%= Html.ValidationMessageArea("City")%>
                </td>
              </tr>              
              <tr>
                <td style=';text-align:right'>
                  <label>State:</label><em>*</em>
                </td>
                <td>
                  <p style='text-align:left'><%=Html.TextBox("State", Model.State)%></p>
                  <%= Html.ValidationMessageArea("State")%>
                </td>
              </tr>
              <tr>
                <td style=';text-align:right'>
                  <label>Zip:</label><em>*</em>
                </td>
                <td >
                  <p style='text-align:left'><%=Html.TextBox("Zip", Model.Zip)%></p>
                  <%= Html.ValidationMessageArea("Zip")%>
                </td>
              </tr>
              <tr>
                <td style=';text-align:right'>
                  <label>Country:</label><em>*</em>
                </td>
                <td >
                    <p style='text-align:left'><%= Html.DropDownList("Country", (IEnumerable<SelectListItem>)ViewData["Countries"])%></p>
                    <%= Html.ValidationMessageArea("Country")%>
                    <%= Html.Hidden("CountryTitle") %>
                  </td>                
              </tr>
              <tr>
                <td colspan="2">
                  <p class="info" style='padding-right:20px'>* Required field</p>
                </td>                                
              </tr>
            </table>            
          </fieldset>      
          </center>              
          <p id="ccSubmit" class="Submit_Payment" style="padding-right:105px">
            <%--<%= Html.SubmitWithClientValidation("Submit Payment")%>--%>
            <button type="submit" style="width: 115px!important;" id="submitDeposit" class="cssbutton small white">
              <span>Submit Payment</span>
            </button>
          </p>
        </form>
      </div>      
    </div>
  </div> 
  <div class="back_link">
    <%=Html.ActionLink("Back to lot", "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = pb.LinkParams.ID, evnt=pb.LinkParams.EventUrl, cat=pb.LinkParams.CategoryUrl, lot=pb.LinkParams.LotTitleUrl }).ToNonSslLink() %>
  </div>
  <script type="text/javascript"><% if (ViewData["IsCreditCard"] == null || !Convert.ToBoolean(ViewData["IsCreditCard"])) {%> $("#dvCard").hide();<%} else {%>$("#dvCard").show();<%}%></script>
</asp:Content>

<asp:Content ID="cnt" ContentPlaceHolderID="cphScriptBottom" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {
      $("#dvPayPalImportant").hide();
      $("#dvAuthorizedNet").hide();

      $("#imgPayPal").click(function () {
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

      $("#imgCard").click(function () {
        $("#dvCard").show();
      });

      $("#Country").change(function () {
        $("#CountryTitle").attr("value", $("#Country option:selected").text());
      });

      $("#Country").change();
    });
  </script>
</asp:Content>