<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Vauction.Models.RegisterInfo>" %>

<%@ Import Namespace="Vauction.Utils.Html" %>



<asp:Content ID="registerHead" ContentPlaceHolderID="HTMLhead" runat="server">

    <title>Registration - <%=Consts.CompanyTitleName %></title>
    <% 
        Html.Script("hashtable.js");
        Html.Script("validation.js");
        Html.Script("jquery.maskedinput.min.js"); 
    %>

</asp:Content>

<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
    <%
        bool EmailExistsConfirmed = ViewData["EmailExistsConfirmed"] != null && Convert.ToBoolean(ViewData["EmailExistsConfirmed"]);
        bool EmailExistsNonConfirmed = ViewData["EmailExistsNonConfirmed"] != null && Convert.ToBoolean(ViewData["EmailExistsNonConfirmed"]); 
    %>

    <%=Html.AntiForgeryToken() %>
    
    <%--<div class="container margin-top-xs">
	<div class="row">
		<div class="col-sm-6">
			<div class="row">
				<ol class="breadcrumb">
					<li><a href="index.html">Home</a></li>
					<li class="active">Login</li>
				</ol>
			</div>
		</div>
		<div class="col-sm-6 hidden-xs">
			<ul class="list-unstyled list-inline pull-right select-top">
				<li>
					<a href="#">
						<span class="fa-stack fa-lg">
							<i class="fa fa-circle fa-stack-2x"></i>
							<i class="fa fa-bookmark-o fa-stack-1x fa-inverse"></i>
						</span>
					</a>
				</li>
				<li>
					<a href="#">
						<span class="fa-stack fa-lg">
							<i class="fa fa-circle fa-stack-2x"></i>
							<i class="fa fa-envelope fa-stack-1x fa-inverse"></i>
						</span>
					</a>
				</li>
				<li>
					<a href="#">
						<span class="fa-stack fa-lg">
							<i class="fa fa-circle fa-stack-2x"></i>
							<i class="fa fa-print fa-stack-1x fa-inverse"></i>
						</span>
					</a>
				</li>
			</ul>
		</div>
	</div>
</div>--%>
    <%--<div id="registration" class="control">
        <p class="text-primary text-weight-900 text-lg border-bottom border-weight-xs border-primary">Registration</p>
    </div>--%>

    <div class="row">
        <div class="col-sm-12">
            <p><span class="text-uppercase">TO BID ON AN ITEM YOU MUST REGISTER HERE FIRST.</span> Your e-mail address is mandatory, and your registration will not be validated if your e-mail address cannot be confirmed. We use many safeguards to protect your information and privacy including the use of secure servers, the encryption of sensitive data, and other electronic protection measures as appropriate.</p>

            <p>Please add <a href="mailto:admin@SeizedPropertyAuctions.com">admin@SeizedPropertyAuctions.com</a> to your safe senders list for bidding notifications and <a href="mailto:info@SeizedPropertyAuctions.com">info@SeizedPropertyAuctions.com</a> for auction announcements</p>
        </div>
    </div>
    <% if (EmailExistsConfirmed)
       {%>
    <p>
       <b> This email address is already registered with our system and has been confirmed.  
            Please click here to restore your username and reset password.</b>
        <%= Html.ActionLink("Forgot your password?", "ForgotPassword").ToSslLink() %>
    </p>
    <%} %>

    <% if (EmailExistsNonConfirmed)
       {%>
    <p>
        <b>Your email address is registered with our system but, you have not confirmed your email address with us.  
            Please click the "Confirm Email" address button.</b> <%= Html.ActionLink("Confirm Email", "ResendConfirmationCode", "Account")%>
    </p>
    <%} %>

    <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/Register">

        <div class="row">
            <div class="col-sm-6">
                <div class="form-group">
                    <label for="uname" class="text-uppercase">Username <span class="text-danger">*</span></label>
                    <%= Html.TextBox("Login", "", new { @size = "10", @maxlength = "20" , @class="form-control",@placeholder="UserName"})%>
                    <%= Html.ValidationMessageArea("Login") %>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-6">
                <div class="form-group">
                    <label class="text-uppercase" for="email">email</label>
                    <span>Format: name@domain.com</span> <span class="text-danger">*</span>
                    <%= Html.TextBox("Email", "", new { @size = "40", @maxlength = "60",@class="form-control",@placeholder="Email" })%>
                    <%= Html.ValidationMessageArea("Email") %>
                </div>
            </div>

            <div class="col-sm-6">
                <div class="form-group">
                    <label class="text-uppercase hidden-xs">&nbsp;</label>
                    <br class="hidden-xs" />
                    <%= Html.TextBox("ConfirmEmail", "", new { @size = "40", @maxlength = "60",@class="form-control",@placeholder="Confirm Email" }) %>
                    <%= Html.ValidationMessageArea("ConfirmEmail")%>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-6">
                <div class="form-group">
                    <label class="text-uppercase" for="password">Password</label>
                    <span>(not less than 6 letters)</span> <span class="text-danger">*</span>
                    <%= Html.Password("Password", "", new { @size = "40", @maxlength = "20",@class="form-control",@placeholder="Password" })%>
                    <%= Html.ValidationMessageArea("Password")%>
                </div>
            </div>
            <div class="col-sm-6">
                <div class="form-group">
                    <label class="text-uppercase hidden-xs">&nbsp;</label>
                    <br class="hidden-xs">
                    <%= Html.Password("ConfirmPassword", "", new { @size = "40", @maxlength = "20",@class="form-control",@placeholder="Confirm Password" })%>
                    <%= Html.ValidationMessageArea("ConfirmPassword")%>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-12">
                <div class="checkbox alt-check margin-bottom-lg">
                    <label class="text-uppercase text-weight-700">
                        <%= Html.CheckBox("RecieveNewsUpdates", false)%>
                        <span class="check-icon"></span>
                        Please add me to the private email list for Auction Announcements
                    </label>
                </div>

                <div class="checkbox alt-check margin-bottom-lg">
                    <label class="text-uppercase text-weight-700">
                        <%= Html.CheckBox("RecieveWeeklySpecials", false)%>
                        <span class="check-icon"></span>
                        Please add me to the private email list for Deal of the Week specials
                    </label>
                </div>

            </div>
        </div>
        <p class="text-right"><span class="text-danger">* Required field</span></p>

        <div class="row margin-top">
            <div class="col-sm-12">
                <p class="text-primary text-weight-900 text-lg">Billing Information</p>
                <p>(Address must match the address on your credit card for this purchase.)</p>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-5 col-xs-8">
                <div class="form-group">
                    <label class="text-uppercase" for="name">Name  <span class="text-danger">*</span></label>
                    <%= Html.TextBox("BillingFirstName", "", new { @maxlength = "20",@class="form-control",@placeholder="First Name" })%>
                    <%= Html.ValidationMessageArea("BillingFirstName")%>
                </div>
            </div>
            <div class="col-sm-2 col-xs-4">
                <div class="form-group">
                    <label class="text-uppercase">&nbsp;</label>
                    <br />
                    <%= Html.TextBox("BillingMIName", "", new { @placeholder= "M.I.", @size = "2", @maxlength = "2",@class = "form-control" })%>
                    <%= Html.ValidationMessageArea("BillingMIName")%>
                </div>
            </div>
            <div class="col-sm-5 col-xs-12">
                <div class="form-group">
                    <label class="text-uppercase hidden-xs">&nbsp;</label>
                    <br class="hidden-xs" />
                    <%= Html.TextBox("BillingLastName", "", new { @maxlength = "30",@placeholder="Last Name",@class = "form-control" })%>
                    <%= Html.ValidationMessageArea("BillingLastName")%>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-12">
                <div class="form-group">
                    <label class="text-uppercase" for="address1">Address 1  <span class="text-danger">*</span></label>
                    <%= Html.TextBox("BillingAddress1", "", new { @maxlength = "40", @size = "60",@placeholder="Address1",@class = "form-control" })%>
                    <%= Html.ValidationMessageArea("BillingAddress1")%>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-12">
                <div class="form-group">
                    <label class="text-uppercase" for="address2">Address 2</label>
                    <%= Html.TextBox("BillingAddress2", "", new { @maxlength = "100", @size = "60",@placeholder="Address2",@class = "form-control" })%>
                    <%= Html.ValidationMessageArea("BillingAddress2")%>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-3 col-xs-6">
                <div class="form-group">
                    <%= Html.TextBox("BillingCity", "", new { @maxlength = "100", @size = "60",@placeholder="Billing City",@class = "form-control" })%>
                    <%= Html.ValidationMessageArea("BillingCity")%>
                </div>
            </div>
            <div class="col-sm-3 col-xs-6">
                <div class="form-group">
                    <%= Html.DropDownList("BillingState", (IEnumerable<SelectListItem>)ViewData["States"], "Select State", new { @class = "form-control" })%>
                    <%= Html.ValidationMessageArea("BillingState")%>
                </div>
            </div>
            <div class="col-sm-3 col-xs-6">
                <div class="form-group">
                    <%= Html.TextBox("BillingZip", "", new { @maxlength = "10",@placeholder="Zip Code",@class = "form-control" })%>
                    <%= Html.ValidationMessageArea("BillingZip")%>
                </div>
            </div>
            <div class="col-sm-3 col-xs-6">
                <div class="form-group">
                    <%= Html.DropDownList("BillingCountry", (IEnumerable<SelectListItem>)ViewData["Countries"], "Select Country",new { @class = "form-control" })%>
                    <%= Html.ValidationMessageArea("BillingCountry")%>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-6">
                <div class="form-group">
                    <label class="text-uppercase" for="phone">Phone Numbers</label>
                    <span>Format: (XXX-XXX-XXXX)</span> <span class="text-danger">*</span>
                    <%=Html.TextBox("BillingPhone", Model.BillingPhone,new { @class = "form-control",@placeholder="Mobile" })%>
                    <%= Html.ValidationMessageArea("BillingWorkPhone")%>
                </div>
            </div>
            <div class="col-sm-6">
                <div class="form-group">
                    <label class="text-uppercase hidden-xs">&nbsp;</label>
                    <br class="hidden-xs" />
                    <%=Html.TextBox("BillingWorkPhone", Model.BillingWorkPhone,new { @class = "form-control",@placeholder="Work Phone" })%>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-6">
                <div class="form-group">
                    <label class="text-uppercase" for="Fax">Fax</label>
                    <%= Html.TextBox("BillingFax", "", new { @maxlength = "20", @size = "30",@class = "form-control",@placeholder="Fax" })%>
                    <%= Html.ValidationMessageArea("BillingFax")%>
                </div>
            </div>
            <div class="col-sm-6">
                <div class="form-group">
                    <label class="text-uppercase hidden-xs">&nbsp;</label>
                    <br class="hidden-xs" />
                    <%= Html.TextBox("Reference", "", new { @maxlength = "20", @size = "30",@class = "form-control",@placeholder="Reference" })%>
                    <%= Html.ValidationMessageArea("Reference")%>
                </div>
            </div>
        </div>

        <div class="row margin-top">
            <div class="col-sm-12">
                <p class="text-primary text-weight-900 text-lg">Shipping Information</p>
                <div class="checkbox alt-check margin-bottom-lg">
                    <label class="text-weight-700">
                        <%= Html.CheckBox("BillingLikeShipping", new { @onclick = "CheckShippingInformation()" }) %>
                        <span class="check-icon"></span>
                        My billing and shipping addresses are identical
                    </label>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-5 col-xs-8">
                <div class="form-group">
                    <label class="text-uppercase" for="name">Name <span class="text-danger">*</span></label>
                    <%= Html.TextBox("ShippingFirstName", "", new { @maxlength = "20",@class = "form-control",@placeholder="First Name" })%>
                    <%= Html.ValidationMessageArea("ShippingFirstName")%>
                </div>
            </div>
            <div class="col-sm-2 col-xs-4">
                <div class="form-group">
                    <label class="text-uppercase">&nbsp;</label>
                    <br />
                    <%= Html.TextBox("ShippingMIName", "", new { @class = "form-control", @size = "2", @maxlength = "2",@placeholder="M.I." })%>
                    <%= Html.ValidationMessageArea("ShippingMIName")%>
                </div>
            </div>
            <div class="col-sm-5 col-xs-12">
                <div class="form-group">
                    <label class="text-uppercase hidden-xs">&nbsp;</label>
                    <br class="hidden-xs" />
                    <%= Html.TextBox("ShippingLastName", "", new { @class = "form-control", @maxlength = "30",@placeholder="Last Name"  })%>
                    <%= Html.ValidationMessageArea("ShippingLastName")%>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-12">
                <div class="form-group">
                    <label class="text-uppercase" for="address1">Address 1 <span class="text-danger">*</span></label>
                    <%= Html.TextBox("ShippingAddress1", "", new { @class = "form-control",@maxlength = "60", @size = "60",@placeholder="Address1" })%>
                    <%= Html.ValidationMessageArea("ShippingAddress1")%>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-12">
                <div class="form-group">
                    <label class="text-uppercase" for="address2">Address 2</label>
                    <%= Html.TextBox("ShippingAddress2", "", new { @class = "form-control",@maxlength = "100", @size = "60",@placeholder="Address2" })%>
                    <%= Html.ValidationMessageArea("ShippingAddress2")%>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-3 col-xs-6">
                <div class="form-group">
                    <%= Html.TextBox("ShippingCity", "", new { @maxlength = "100", @size = "60",@placeholder="City",@class = "form-control" })%>
                    <%= Html.ValidationMessageArea("ShippingCity")%>
                </div>
            </div>
            <div class="col-sm-3 col-xs-6">
                <div class="form-group">
                    <%= Html.DropDownList("ShippingState", (IEnumerable<SelectListItem>)ViewData["States"], "Select State", new { @class = "form-control"})%>
                    <%= Html.ValidationMessageArea("ShippingState")%>
                </div>
            </div>
            <div class="col-sm-3 col-xs-6">
                <div class="form-group">
                    <%= Html.DropDownList("ShippingCountry", (IEnumerable<SelectListItem>)ViewData["Countries"], "Select Country", new { @class = "form-control"})%>
                    <%= Html.ValidationMessageArea("ShippingCountry")%>
                </div>
            </div>
            <div class="col-sm-3 col-xs-6">
                <div class="form-group">
                    <%= Html.TextBox("ShippingZip", "", new { @maxlength = "10", @size = "10",@class = "form-control",@placeholder="Zip Code" })%>
                    <%= Html.ValidationMessageArea("ShippingZip")%>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-6">
                <div class="form-group">
                    <label class="text-uppercase" for="phone">Phone Numbers</label>
                    <span>Format: (XXX-XXX-XXXX) <span class="text-danger">*</span></span>
                    <%=Html.TextBox("ShippingPhone", Model.ShippingPhone,new {@class = "form-control",@placeholder="Mobile" })%>
                    <%= Html.ValidationMessageArea("ShippingPhone")%>
                </div>
            </div>
            <div class="col-sm-6">
                <div class="form-group">
                    <label class="text-uppercase hidden-xs">&nbsp;</label>
                    <br class="hidden-xs">
                    <%=Html.TextBox("ShippingWorkPhone", Model.ShippingWorkPhone,new {@class = "form-control",@placeholder="Work Phone" })%>
                    <%= Html.ValidationMessageArea("ShippingWorkPhone")%>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col-sm-6">
                <div class="form-group">
                    <label class="text-uppercase" for="Fax">Fax</label>
                    <%= Html.TextBox("ShippingFax", "", new { @maxlength = "20", @size = "30",@class = "form-control",@placeholder="Fax" })%>
                    <%= Html.ValidationMessageArea("ShippingFax")%>
                </div>
            </div>
        </div>

        <p>
            I agree that by submitting this registration that a) the information provided by me is complete and accurate to the best of my knowledge, and that b) purposefully providing false information in order to wrongly participate in an auction could be considered criminal activity, and c) should the situation arise, The Webmaster is authorized to release relevant information to involved parties for settlement of dispute. I also understand and agree that by registering I am stating that I will participate in auctions according to the rules and regulations set forth by this site. 
        </p>
        <p>
            <b>Please verify the above information is correct before submitting!</b> By submitting this form you are agreeing to our <a href='<%= Url.Action("Terms", "Home")%>' target="_blank">TERMS AND CONDITIONS</a> and <a href='<%= Url.Action("Privacy", "Home") %>' target="_blank">PRIVACY STATEMENT</a>
        </p>

        <hr class="border-primary" />

        <div class="checkbox alt-check margin-bottom-lg">
            <label class="text-weight-700">
                <%=Html.CheckBox("Agree", false)%>
                <span class="check-icon"></span>
                I have read and understood the terms of this website
            </label>
        </div>

        <div class="row margin-top-lg">
            <div class="col-sm-12">
                <%= Html.SubmitWithClientValidation("Create My Account","hvr-wobble-bottom text-uppercase text-weight-600 btn btn-danger btn-lg padding-horizontal-xl")%>
            </div>
        </div>

    </form>
    <script type="text/javascript">
        function CheckShippingInformation() {
            var check = document.getElementById("BillingLikeShipping").checked;
            $("#ShippingFirstName, #ShippingFirstName, #ShippingMIName, #ShippingLastName, #ShippingAddress1, #ShippingAddress2, #ShippingCity, #ShippingState, #ShippingZip, #ShippingCountry, #ShippingPhone, #ShippingWorkPhone, #ShippingFax").attr("disabled", check);

            $("#ShippingFirstName").attr("value", check ? $("#BillingFirstName").attr("value") : "");
            $("#ShippingMIName").attr("value", check ? $("#BillingMIName").attr("value") : "");
            $("#ShippingLastName").attr("value", check ? $("#BillingLastName").attr("value") : "");
            $("#ShippingAddress1").attr("value", check ? $("#BillingAddress1").attr("value") : "");
            $("#ShippingAddress2").attr("value", check ? $("#BillingAddress2").attr("value") : "");
            $("#ShippingCity").attr("value", check ? $("#BillingCity").attr("value") : "");
            $("#ShippingState").attr("value", check ? $("#BillingState").attr("value") : "");
            $("#ShippingZip").attr("value", check ? $("#BillingZip").attr("value") : "");
            $("#ShippingPhone").attr("value", check ? $("#BillingPhone").attr("value") : "");
            $("#ShippingWorkPhone").attr("value", check ? $("#BillingWorkPhone").attr("value") : "");
            $("#ShippingFax").attr("value", check ? $("#BillingFax").attr("value") : "");
            $("#ShippingCountry").val(check ? $("#BillingCountry").val() : 1);
        }

        CheckShippingInformation();

        $(document).ready(function () {
            $("#BillingPhone,#BillingWorkPhone,#ShippingPhone,#ShippingWorkPhone").mask("(999) 999-9999? x999999");

            $("#BillingCountry").val(1);
            $("#ShippingCountry").val(1);
        });
    </script>
        </div>
</asp:Content>
