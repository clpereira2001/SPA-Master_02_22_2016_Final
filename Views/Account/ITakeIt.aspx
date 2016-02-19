<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CreditCardInfo>" %>

<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="Vauction.Utils.Html" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Making payment - <%=Consts.CompanyTitleName %></title>
    <% Html.Style("jQueryCustomControls.css"); %>
</asp:Content>
<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
    <%
        long auction_id = Convert.ToInt64(ViewData["auction_id"]);
        bool ishipping = (ViewData["ishipping"] != null && Convert.ToBoolean(ViewData["ishipping"]));
        Dow dow = Session["DOW_" + auction_id.ToString()] as Dow;
   %>
    <div class="col-md-9 col-sm-8">

        <div class="row">
            <div class="col-sm-12">
                <div class="table-responsive">
                    <%--<h1 class="title">Make Payment</h1>--%>
                    <% if (dow != null)
                       { %>
                    <table id="tblInvoice" class="table table-hover table-striped">
                        <thead>

                            <tr class="bg-primary">
                                <th class="align-middle">Lot#</th>
                                <th class="text-center align-middle">Date</th>
                                <th class="align-middle">Description</th>
                                <th class="text-center align-middle">Quantity</th>
                                <th class="text-center align-middle">Amount</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td class="align-middle">
                                    <%= Html.ActionLink(dow.LinkParams.Lot.ToString(), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = dow.LinkParams.ID, evnt=dow.LinkParams.EventUrl, cat=dow.LinkParams.CategoryUrl, lot=dow.LinkParams.LotTitleUrl })%>
          </td>
                                <td class="text-center align-middle">
                                    <%= DateTime.Now.ToShortDateString()%>
          </td>
                                <td class="align-middle">
                                    <%=dow.LinkParams.Title%>
          </td>
                                <td class="text-center align-middle">
                                    <%=dow.Quantity %>
                                    <% if (dow.Quantity != dow.WQuantity)
                                       { %>            
                (<strike> <%=dow.WQuantity%> </strike>)<strong>*</strong>
                                    <%} %>
          </td>
                                <td class="text-center align-middle">
                                    <% if (dow.Quantity > 0)
                                       { %>
                                    <span id="total" def="<%=dow.TotalCost.GetCurrency()%>" def2="<%=(dow.TotalCost - dow.Shipping - dow.Insurance).GetCurrency()%>"><%=dow.TotalCost.GetCurrency()%></span> <%}
                                       else
                                       {%>
                                    <strong style='color: Maroon'>SOLD OUT</strong>
                                    <%} %>
          </td>
                            </tr>
                            <tr class="payment_table_content">
                                <td colspan="5" class="payment_table_content_td">
                                    <table style="margin: 0px; table-layout: fixed; padding-left: 20px; padding-right: 20px;" cellpadding="0" cellspacing="0">
                                        <colgroup>
                                            <col width='190px' />
                                            <col width='150px' />
                                            <col width='150px' />
                                            <col width='110px' />
                                        </colgroup>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <% if (dow.Quantity == 1)
                                               {%>
                                            <td class="tableTL" colspan="2">Price
                   </td>
                                            <% }
                                               else
                                               { %>
                                            <td class="tableTL" colspan="1">Price</td>
                                            <td class="tableT" style="text-align: right"><%=dow.Quantity%>&nbsp;x&nbsp;<%=(!dow.IsDOW)?dow.BuyPrice.Value.GetCurrency():dow.Price.GetCurrency()%>&nbsp;=&nbsp;
                   </td>
                                            <%} %>
                                            <td class="tableTLR">
                                                <%=dow.Amount.GetCurrency() %>
                  </td>
                                        </tr>
                                        <% if (dow.BuyerPremium > 0)
                                           { %>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td class="tableTL" colspan="2">Buyer's premium
                  </td>
                                            <td class="tableTLR">
                                                <%=dow.BuyerPremium.GetCurrency()%>
                  </td>
                                        </tr>
                                        <%} %>
                                        <% if (dow.Shipping > 0)
                                           { %>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td class="tableTL" colspan="2">Shipping and handling
                  </td>
                                            <td class="tableTLR">
                                                <span class="accordion_shipping" id="shipping" def="<%=dow.Shipping.GetCurrency() %>"><%=dow.Shipping.GetCurrency()%></span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td class="<%=(dow.Tax > 0)?"tableTL":"tableTLB" %>" colspan="2">Insurance
                  </td>
                                            <td class="<%=(dow.Tax > 0)?"tableTLR":"tableTLBR" %>">
                                                <span class="accordion_insurance" id="insurance" def="<%=dow.Insurance.GetCurrency() %>"><%=dow.Insurance.GetCurrency()%></span>
                                            </td>
                                        </tr>
                                        <%} %>
                                        <% if (dow.Tax > 0)
                                           { %>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td class="tableTLB" colspan="2">Tax
                    </td>
                                            <td class="tableTLBR">
                                                <%=dow.Tax.GetCurrency()%>
                    </td>
                                        </tr>
                                        <%} %>
                                    </table>
                                </td>
                            </tr>
                            <tr class="payment_table_header2">
                                <td colspan="3">&nbsp;</td>
                                <td>Total Due:
          </td>
                                <td id="lTotalAmountBottom">
                                    <%=dow.TotalCost.GetCurrency()%>
          </td>
                            </tr>
                            <% if (dow.Quantity != dow.WQuantity)
                               { %>
                            <tr>
                                <td colspan="5"><b>* Lot's item quantity was changed</b></td>
                            </tr>
                            <% } %>
                        </tbody>
                    </table>
                </div>
                <%--<% if (!dow.IsConsignorShip && dow.Shipping > 0 && dow.Insurance > 0) {%>
        <p><%=Html.CheckBox("chkPickup", ishipping)%> I will pick up my items (shipping charges will be removed)</p>
      <%} %> --%>
                <% if (dow.Quantity > 0)
                   { %>
                <br class="br_clear" />
                <table id="dvChooseMethod">
                    <tr>
                        <td style="width: 290px;">Select payment method</td>
                        <td style="width: 150px;" id="imgPayPal">
                            <img src="<%=AppHelper.CompressImage("PPExpressCheckout.gif") %>" />
                        </td>
                        <td>&nbsp</td>
                        <td id="imgCard" style="width: 150px;">
                            <img src="<%=AppHelper.CompressImage("cards.gif") %>" style="height: 24px" />
                        </td>
                        <td style="width: 10px;">&nbsp</td>
                    </tr>
                </table>

                <br class="br_clear" />
                <div id="dvPayPalImportant" style="color: rgb(180,0,0); text-align: center; display: none">
                    <strong>Please wait a second, you are being redirected to PayPal.</strong>
                    <br />
                    <br />
                    <br />
                    <center><img src="<%=AppHelper.CompressImage("throbber2.gif") %>" /></center>
                </div>

                <div id="dvAuthorizedNet" style="color: rgb(180,0,0); text-align: center; display: none">
                    <strong>Please wait while we authorize your credit card. This may take a couple of minutes.</strong>
                    <br />
                    <br />
                    <br />
                    <center><img src="<%=AppHelper.CompressImage("throbber2.gif") %>" /></center>
                </div>

                <div id="dvPayPal" style='display: none'>
                    <div class="control">
                        <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/PayPalITakeIt">
                            <%=Html.AntiForgeryToken() %>
                            <%=Html.Hidden("auction_id", auction_id)%>
                            <%=Html.Hidden("ishipping", true, new { @class = "hidden_shipping" })%>
                            <input type="submit" id="btnPayPal" />
                        </form>
                    </div>
                </div>

                <div id="dvCard">
                    <div class="control" style="width: 730px;">
                        <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/CreditCardITakeIt">
                            <%=Html.AntiForgeryToken() %>
                            <%=Html.Hidden("auction_id", auction_id)%>
                            <%=Html.Hidden("ishipping", true, new { @class = "hidden_shipping" })%>
                            <%-- <center>
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
                    &nbsp;&nbsp;/
                    <select name="ExpirationYear" style="width:68px">
                      <%for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 10; i++)
                        {%>
                      <%if (Model.ExpirationMonth != i)
                        {%>
                            <option value="<%=i %>">&nbsp;&nbsp;<%=i.ToString()%></option>
                        <%}
                        else
                        { %>
                            <option value="<%=i %>" selected="selected" >&nbsp;&nbsp;<%=i.ToString()%></option>
                        <%} %>
                      <%} %>
                    </select>
                    &nbsp;&nbsp;&nbsp;( mm / yyyy )                  
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
                  <td style='text-align:right'>
                    <label>Address 1:</label><em>*</em>
                  </td>
                  <td >
                    <p style='text-align:left'><%=Html.TextBox("Address1", Model.Address1)%></p>
                    <%= Html.ValidationMessageArea("Address1")%>
                  </td>
                </tr>
                <tr>
                  <td style='text-align:right'>
                    <label>Address 2:</label>
                  </td>
                  <td>
                    <p style='text-align:left'><%=Html.TextBox("Address2", Model.Address2)%></p>
                    <%= Html.ValidationMessageArea("Address2")%>
                  </td>
                </tr>
                 <tr>
                  <td style='text-align:right'>
                    <label>City:</label><em>*</em>
                  </td>
                  <td>
                    <p style='text-align:left'><%=Html.TextBox("City", Model.City)%></p>
                    <%= Html.ValidationMessageArea("City")%>
                  </td>
                </tr>              
                <tr>
                  <td style='text-align:right'>
                    <label>State:</label><em>*</em>
                  </td>
                  <td>
                    <p style='text-align:left'><%=Html.TextBox("State", Model.State)%></p>
                    <%= Html.ValidationMessageArea("State")%>
                  </td>
                </tr>
                <tr>
                  <td style='text-align:right'>
                    <label>Zip:</label><em>*</em>
                  </td>
                  <td >
                    <p style='text-align:left'><%=Html.TextBox("Zip", Model.Zip)%></p>
                    <%= Html.ValidationMessageArea("Zip")%>
                  </td>
                </tr>
                <tr>
                  <td style='text-align:right'>
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
            </center>--%>
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
                                    <p id="ccSubmit" class="Submit_Payment">
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
                <%} %>
                <br class="br_clear" />
                <div class="back_link">
                    <%=(!dow.IsDOW) ? Html.ActionLink(String.Format("Return to the LOT#{0}", dow.LinkParams.Lot.ToString(CultureInfo.InvariantCulture)), "AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = dow.LinkParams.ID, evnt=dow.LinkParams.EventUrl, cat=dow.LinkParams.CategoryUrl, lot=dow.LinkParams.LotTitleUrl }) : Html.ActionLink("Return to the Deal of the Week", "DealOfTheWeek", new { controller = "Auction", action = "DealOfTheWeek", id = dow.LinkParams.ID })%>
                </div>
                <script type="text/javascript">
        <% if (ViewData["IsCreditCard"] == null || !Convert.ToBoolean(ViewData["IsCreditCard"]))
           {%>
                    $("#dvCard").hide();
        <%}
           else
           {%>
                    $("#dvCard").show();
                    <%}%>
      </script>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="cntJS" ContentPlaceHolderID="cphScriptBottom" runat="server">
    <script type="text/javascript">
        function changeCheck() {
            var checked = $("#chkPickup").attr("checked");
            $("#total").text($("#total").attr(checked ? "def2" : "def"));
            $("#lTotalAmountBottom").text($("#total").attr(checked ? "def2" : "def"));
            $("#shipping").text(checked ? "$0.00" : $("#shipping").attr("def"));
            $("#insurance").text(checked ? "$0.00" : $("#insurance").attr("def"));
            $(".hidden_shipping").attr("value", !checked);
        }

        $(document).ready(function () {
            $("#chkPickup").click(function () {
                changeCheck();
            });

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

            changeCheck();
        });
  </script>
</asp:Content>
