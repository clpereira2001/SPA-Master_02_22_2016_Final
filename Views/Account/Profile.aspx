<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<RegisterInfo>" %>

<%@ Import Namespace="Vauction.Utils.Html" %>

<asp:Content ID="registerHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Personal Information - <%=Consts.CompanyTitleName %></title>
    <% 
        Html.Script("hashtable.js");
        
        //Html.Script("../../jquery.maskedinput.min.js");
        
        //Html.Script("validation.js");
    %>

    
    <%--  --%>
     
        <script type= 'text/javascript' src="../../public/scripts/validation.js" ></script>
        <script type= 'text/javascript' src="../../public/scripts/jquery.maskedinput.min.js" ></script>
</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
        <%-- <h2>Personal Information</h2>--%>
        <div id="registration" class="control">
            <% if (!Model.IsModifyed || Model.NotPasswordReset)
               { %>
            <div style="padding: 5px; color: #800000; border: dotted 1px #800000; margin: 10px 0px 20px 0px; font-family: Arial, Helvetica, sans-serif; font-size: 14px;"><%=(!Model.IsModifyed) ? "Please verify your personal information and update it." : "Please enter your new password and update it."%></div>
            <%} %>

            <% using (Html.BeginForm())
               { %>
            <%=Html.AntiForgeryToken() %>

            <div class="row">
                <div class="col-sm-6">
                    <div class="form-group">
                        <label class="text-uppercase" for="email">email</label>
                        <span>Format: name@domain.com</span>
                        <%--<input type="Email" placeholder="Email" id="email" class="form-control">--%>


                        <%= Html.TextBox("Email", ViewData.Model.Email, new { @class = "form-control", @size = "40", @maxlength = "60" })%>
                        <br />
                        <%= Html.ValidationMessageArea("Email")%>
                    </div>
                </div>
                <div class="col-sm-6">
                    <div class="form-group">
                        <label class="text-uppercase hidden-xs">&nbsp;</label>
                        <br class="hidden-xs" />
                        <%--<input type="email" placeholder="Confirm Email" class="form-control">--%>
                        <%= Html.TextBox("ConfirmEmail", ViewData.Model.ConfirmEmail, new { @class = "form-control", @size = "40", @maxlength = "60" })%>
                        <br />
                        <%= Html.ValidationMessageArea("ConfirmEmail")%>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-6">
                    <div class="form-group">
                        <label class="text-uppercase" for="password">Password</label>
                        <span>(not less than 6 letters)</span>
                        <%--<input type="password" placeholder="Password" id="password" class="form-control">--%>

                        <%= Html.Password("Password",ViewData.Model.Password, new { @class = "form-control",@size = "40", @maxlength = "20" })%>
                        <br />
                        <%= Html.ValidationMessageArea("Password")%>
                    </div>
                </div>
                <div class="col-sm-6">
                    <div class="form-group">
                        <label class="text-uppercase hidden-xs">&nbsp;</label>
                        <br class="hidden-xs" />
                        <%--<input type="password" placeholder="Confirm Password" class="form-control">--%>
                        <%= Html.Password("ConfirmPassword", ViewData.Model.Password, new {@class = "form-control", @size = "40", @maxlength = "20" })%>
                        <br />
                        <%= Html.ValidationMessageArea("ConfirmPassword")%>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12">
                    <div class="checkbox alt-check margin-bottom-lg">
                        <label class="text-uppercase text-weight-700">
                            <%--<input type="checkbox" checked>
                        <span class="check-icon"></span>
                        Please add me to the private mailing list for news and updates--%>


                            <%= Html.CheckBox("RecieveNewsUpdates", Model.RecieveNewsUpdates, new { @class = "chk" })%> <span class="check-icon"></span>Please add me to the private mailing list for news and updates    
                        </label>
                    </div>

                    <div class="checkbox alt-check margin-bottom-lg">
                        <label class="text-uppercase text-weight-700">
                            <%--<input type="checkbox" checked>
                        <span class="check-icon"></span>
                        Please add me to the private mailing list for weekly specials--%>
                            <%= Html.CheckBox("RecieveWeeklySpecials", Model.RecieveWeeklySpecials, new { @class = "chk" })%> <span class="check-icon"></span>Please add me to the private mailing list for weekly specials   
                        </label>
                    </div>

                </div>
            </div>

            <div class="row margin-top">
                <div class="col-sm-12">
                    <p class="text-primary text-weight-900 text-lg">Billing Information</p>
                    <p>(Address must match the address on your credit card for this purchase.)</p>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-5 col-xs-8">
                    <div class="form-group">
                        <label class="text-uppercase" for="name">Name</label>
                        <%-- <input type="text" placeholder="First Name" id="name" class="form-control">--%>
                        <%= Html.TextBox("BillingFirstName", Model.BillingFirstName, new {@class="form-control", @maxlength = "20" })%>
                        <br />
                        <%= Html.ValidationMessageArea("BillingFirstName")%>
                    </div>
                </div>
                <div class="col-sm-2 col-xs-4">
                    <div class="form-group">
                        <label class="text-uppercase">&nbsp;</label>
                        <br />
                        <%-- <input type="text" placeholder="M.I." class="form-control">--%>
                        <%= Html.TextBox("BillingMIName",Model.BillingMIName, new { @class = "form-control", @size = "2", @maxlength = "2" })%>
                        <br />
                        <%= Html.ValidationMessageArea("BillingMIName")%>
                    </div>
                </div>
                <div class="col-sm-5 col-xs-12">
                    <div class="form-group">
                        <label class="text-uppercase hidden-xs">&nbsp;</label>
                        <br class="hidden-xs" />
                        <%--<input type="text" placeholder="Last Name" class="form-control">--%>
                        <%= Html.TextBox("BillingLastName",Model.BillingLastName, new {@class = "form-control", @maxlength = "30" })%>
                        <br />
                        <%= Html.ValidationMessageArea("BillingLastName")%>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12">
                    <div class="form-group">
                        <label class="text-uppercase" for="address">Address (line 1):</label>
                        <%--<input type="text" placeholder="Address" id="address" class="form-control">--%>
                        <%= Html.TextBox("BillingAddress1", Model.BillingAddress1, new {@class = "form-control", @maxlength = "40", @size = "60" })%>
                        <br />
                        <%= Html.ValidationMessageArea("BillingAddress1")%>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12">
                    <div class="form-group">
                        <label class="text-uppercase" for="address">Address (line 2):</label>
                        <%--<input type="text" placeholder="Address" id="address" class="form-control">--%>
                        <%= Html.TextBox("BillingAddress2", Model.BillingAddress2, new { @class = "form-control",@maxlength = "40", @size = "60" })%>
                        <br />
                        <%= Html.ValidationMessageArea("BillingAddress2")%>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-6 col-xs-12">
                    <div class="form-group">
                        <%-- <input type="text" placeholder="City" id="city" class="form-control">--%>
                        <%= Html.TextBox("BillingCity", Model.BillingCity, new { @class = "form-control", @maxlength = "100", @size = "60",@placeholder = "City" })%>
                        <br />
                        <%= Html.ValidationMessageArea("BillingCity")%>
                    </div>
                </div>
                <div class="col-sm-3 col-xs-6">
                    <div class="form-group">
                        <%--<select class="form-control" name="">
                        <option value="">Pennsylvania</option>
                    </select>--%>
                        <%= Html.DropDownList("BillingState", (IEnumerable<SelectListItem>)ViewData["States"], "[ - Select - ]",new { @class = "form-control",@placeholder = "State"})%>
                        <br />
                        <%= Html.ValidationMessageArea("BillingState")%>
                    </div>
                </div>
                <div class="col-sm-3 col-xs-6">
                    <div class="form-group">
                        <%= Html.DropDownList("BillingCountry", (IEnumerable<SelectListItem>)ViewData["Countries"], "[ - Select - ]",new { @class = "form-control",@placeholder = "Country"})%>
                        <br />
                        <%= Html.ValidationMessageArea("BillingCountry")%>
                    </div>
                </div>

                <div class="col-sm-3 col-xs-6">
                    <div class="form-group">
                        <%--<input type="text" placeholder="Zip Code" class="form-control">--%>
                        <%= Html.TextBox("BillingZip", Model.BillingZip, new { @class = "form-control", @maxlength = "20", @size = "11",@placeholder = "Zip Code" })%>
                        <br />
                        <%= Html.ValidationMessageArea("BillingZip")%>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-6">
                    <div class="form-group">
                        <label class="text-uppercase" for="phone">Phone Numbers</label>
                        <span>Format: (XXX-XXX-XXXX)</span>
                        <%-- <input type="tel" placeholder="Mobile" id="phone" class="form-control">--%>
                        <%=Html.TextBox("BillingWorkPhone", Model.BillingWorkPhone,new { @class = "form-control",@placeholder = "Mobile"})%>
                        <br />
                        <%= Html.ValidationMessageArea("BillingWorkPhone")%>
                    </div>
                </div>
                <div class="col-sm-3 col-xs-6">
                    <div class="form-group">
                        <label class="text-uppercase hidden-xs">&nbsp;</label>
                        <br class="hidden-xs" />
                        <%--<input type="text" placeholder="Other" class="form-control">--%>
                        <%= Html.TextBox("BillingFax", Model.BillingFax, new { @class = "form-control",@placeholder = "Fax" ,@maxlength = "20", @size = "30" })%>
                        <br />
                        <%= Html.ValidationMessageArea("BillingFax")%>
                    </div>
                </div>
                <div class="col-sm-3 col-xs-6">
                    <div class="form-group">
                        <label class="text-uppercase hidden-xs">&nbsp;</label>
                        <br class="hidden-xs" />
                        <%= Html.TextBox("Reference", Model.Reference, new {  @class = "form-control",@placeholder = "Reference" ,@maxlength = "20", @size = "30" })%>
                        <br />
                        <%= Html.ValidationMessageArea("Reference")%>
                    </div>
                </div>
            </div>



            <div class="row margin-top">
                <div class="col-sm-12">
                    <p class="text-primary text-weight-900 text-lg">Shipping Information</p>
                    <div class="checkbox alt-check margin-bottom-lg">
                        <label class="text-weight-700">
                            <%-- <input type="checkbox" checked="">
                        <span class="check-icon"></span>
                        My billing and shipping addresses are identical--%>
                            <%= Html.CheckBox("BillingLikeShipping", Model.BillingLikeShipping, new { @class = "chk", @onclick = "CheckShippingInformation()" }) %>
                            <span class="check-icon"></span>My billing and shipping addresses are identical
                        </label>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-5 col-xs-8">
                    <div class="form-group">
                        <label class="text-uppercase" for="name">Name</label>
                        <%--<input type="text" placeholder="First Name" id="name" class="form-control">--%>
                        <%= Html.TextBox("ShippingFirstName", Model.ShippingFirstName, new { @class = "form-control", @placeholder = "First Name", @maxlength = "20" })%>
                        <br />
                        <%= Html.ValidationMessageArea("ShippingFirstName")%>
                    </div>
                </div>
                <div class="col-sm-2 col-xs-4">
                    <div class="form-group">
                        <label class="text-uppercase">&nbsp;</label>
                        <br />
                        <%-- <input type="text" placeholder="M.I." class="form-control">--%>
                        <%= Html.TextBox("ShippingMIName", Model.ShippingMIName, new { @class = "form-control", @placeholder = "M.I.", @size = "2", @maxlength = "2" })%>
                        <%= Html.ValidationMessageArea("ShippingMIName")%>
                    </div>
                </div>
                <div class="col-sm-5 col-xs-12">
                    <div class="form-group">
                        <label class="text-uppercase hidden-xs">&nbsp;</label>
                        <br class="hidden-xs" />
                        <%--<input type="text" placeholder="Last Name" class="form-control">--%>
                        <%= Html.TextBox("ShippingLastName", Model.ShippingLastName, new {@class = "form-control", @placeholder = "Last Name", @maxlength = "30" })%>
                        <br />
                        <%= Html.ValidationMessageArea("ShippingLastName")%>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-12">
                    <div class="form-group">
                        <label class="text-uppercase" for="address">Address (line 1):</label>
                        <%--<input type="text" placeholder="Address" id="address" class="form-control">--%>
                        <%= Html.TextBox("ShippingAddress1", Model.ShippingAddress1, new {@class = "form-control", @placeholder = "Address",  @maxlength = "60", @size = "60" })%>
                        <br />
                        <%= Html.ValidationMessageArea("ShippingAddress1")%>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <div class="form-group">
                        <label class="text-uppercase" for="address">Address (line 2):</label>
                        <%= Html.TextBox("ShippingAddress2", Model.ShippingAddress2, new {@class = "form-control", @placeholder = "Address2", @maxlength = "100", @size = "60" })%>
                        <br />
                        <%= Html.ValidationMessageArea("ShippingAddress2")%>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-6 col-xs-12">
                    <div class="form-group">
                        <%--  <input type="text" placeholder="City" id="city" class="form-control">--%>

                        <%= Html.TextBox("ShippingCity", Model.ShippingCity, new { @class = "form-control", @placeholder = "City",@maxlength = "100", @size = "60" })%>
                        <br />
                        <%= Html.ValidationMessageArea("ShippingCity")%>
                    </div>
                </div>
                <div class="col-sm-3 col-xs-6">
                    <div class="form-group">
                        <%= Html.DropDownList("ShippingState", (IEnumerable<SelectListItem>)ViewData["States"], "[ - Select - ]", new { @class = "form-control", @placeholder = "States"})%>
                        <br />
                        <%= Html.ValidationMessageArea("ShippingState")%>
                    </div>
                </div>
                <div class="col-sm-3 col-xs-6">
                    <div class="form-group">
                        <%= Html.DropDownList("ShippingCountry", (IEnumerable<SelectListItem>)ViewData["Countries"], "[ - Select - ]", new { @class = "form-control", @placeholder = "Country"})%>
                        <br />
                        <%= Html.ValidationMessageArea("ShippingCountry")%>
                    </div>
                </div>
                <div class="col-sm-3 col-xs-6">
                    <div class="form-group">
                        <%= Html.TextBox("ShippingZip", Model.ShippingZip, new { @class = "form-control", @placeholder = "Zip Code", @maxlength = "100", @size = "11" })%>
                        <br />
                        <%= Html.ValidationMessageArea("ShippingZip")%>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-6">
                    <div class="form-group">
                        <label class="text-uppercase" for="phone">Phone Numbers</label>
                        <span>Format: (XXX-XXX-XXXX)</span>
                        <%-- <input type="tel" placeholder="Mobile" id="phone" class="form-control">--%>
                        <%=Html.TextBox("ShippingPhone", Model.ShippingPhone,new { @class = "form-control",@placeholder = "Mobile"})%>
                        <br />
                        <%= Html.ValidationMessageArea("ShippingPhone")%>
                    </div>
                </div>
                <div class="col-sm-3 col-xs-6">
                    <div class="form-group">
                        <br class="hidden-xs" />
                        <%=Html.TextBox("ShippingWorkPhone", Model.ShippingWorkPhone,new { @class = "form-control",@placeholder = "Shipping Work Phone"})%>
                        <br />
                        <%= Html.ValidationMessageArea("ShippingWorkPhone")%>
                    </div>
                </div>
                <div class="col-sm-3 col-xs-6">
                    <div class="form-group">
                        <label class="text-uppercase hidden-xs">&nbsp;</label>
                        <br class="hidden-xs" />
                        <%--<input type="text" placeholder="Other" class="form-control">--%>
                        <%= Html.TextBox("ShippingFax", Model.ShippingFax, new { @class = "form-control",@placeholder = "Fax" ,@maxlength = "20", @size = "30" })%>
                        <br />
                        <%= Html.ValidationMessageArea("ShippingFax")%>
                    </div>
                </div>
            </div>
            <%= Html.Hidden("Agree",true)%>
            <%= Html.ValidationMessageArea("Agree")%>
            <p class="info">* Required field</p>
            <div class="row text-center margin-top-lg">

              
               <%= Html.SubmitWithClientValidation("Update","btn btn-danger btn-lg")%>
                <% } %>
            </div>


        </div>
    </div>
</asp:Content>

<asp:Content ID="jsC" ContentPlaceHolderID="cphScriptBottom" runat="server">
    <script type="text/javascript">
        var firstload = true;

        function DiableControls(check) {
            $("#ShippingFirstName, #ShippingFirstName, #ShippingMIName, #ShippingLastName, #ShippingAddress1, #ShippingAddress2, #ShippingCity, #ShippingState, #ShippingZip, #ShippingCountry, #ShippingPhone, #ShippingWorkPhone, #ShippingFax").attr("disabled", check);
        }

        function CheckShippingInformation() {
            var check = document.getElementById("BillingLikeShipping").checked;
            DiableControls(check);
            $("#ShippingFirstName").attr("value", check ? $("#BillingFirstName").attr("value") : "");
            $("#ShippingMIName").attr("value", check ? $("#BillingMIName").attr("value") : "");
            $("#ShippingLastName").attr("value", check ? $("#BillingLastName").attr("value") : "");
            $("#ShippingAddress").attr("value", check ? $("#BillingAddress").attr("value") : "");
            $("#ShippingCity").attr("value", check ? $("#BillingCity").attr("value") : "");
            $("#ShippingState").attr("value", check ? $("#BillingState").attr("value") : "");
            $("#ShippingZip").attr("value", check ? $("#BillingZip").attr("value") : "");
            $("#ShippingPhone").attr("value", check ? $("#BillingPhone").attr("value") : "");
            $("#ShippingWorkPhone").attr("value", check ? $("#BillingWorkPhone").attr("value") : "");
            $("#ShippingFax").attr("value", check ? $("#BillingFax").attr("value") : "");
            $("#ShippingCountry").val(check ? $("#BillingCountry").val() : 1);

            firstload = false;
        }

        $(document).ready(function () {

            $("#BillingPhone,#BillingWorkPhone,#ShippingPhone,#ShippingWorkPhone").mask("(999) 999-9999? x999999");

            if (!firstload) CheckShippingInformation(); else DiableControls($("#BillingLikeShipping").attr("checked"));
        });
    </script>

</asp:Content>
