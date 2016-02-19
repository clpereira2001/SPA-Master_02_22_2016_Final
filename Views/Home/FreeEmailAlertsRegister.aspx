<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Vauction.Utils.Html" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Register For Free Email - <%=Consts.CompanyTitleName %></title>
    <% Html.Script("hashtable.js"); %>
    <% Html.Script("validation.js"); %>
</asp:Content>

<asp:Content ID="indexContent2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
        <div id="email_registration" class="control">
           <%-- <h2 class="title">Register for free email alerts</h2>--%>
            <%--<% using (Html.BeginForm("FreeEmailAlertsRegistrationSuccess", "Home")) { %>--%>
            <form method="post" action="<%=Consts.ProtocolSitePort %>/Home/FreeEmailAlertsRegistrationSuccess">
                <%=Html.AntiForgeryToken() %>
                <fieldset class="blue_box">
                    <p>
                        <label>First Name:</label><em>*</em>
                        <%= Html.TextBox("FirstName", "", new { @maxlength = "20" })%>
                        <br />
                        <%= Html.ValidationMessageArea("FirstName")%>
                    </p>

                    <p>
                        <label>Last Name:</label><em>*</em>
                        <%= Html.TextBox("LastName", "", new { @maxlength = "30" })%>
                        <br />
                        <%= Html.ValidationMessageArea("LastName")%>
                    </p>

                    <p>
                        <label>Email:</label><em>*</em>
                        <%= Html.TextBox("Email", "", new { @size = "40", @maxlength = "60" })%>
                        <br />
                        <%= Html.ValidationMessageArea("Email")%>
                    </p>
                    <p>
                        <label>Confirm email:</label><em>*</em>
                        <%= Html.TextBox("EmailConfirm", "", new { @size = "40", @maxlength = "60" })%>
                        <br />
                        <%= Html.ValidationMessageArea("EmailConfirm")%>
                    </p>
                    <p>
                        <label>State/Province</label><em>*</em>
                        <%= Html.DropDownList("State", (IEnumerable<SelectListItem>)ViewData["States"], "[ - Select - ]")%>
                        <br />
                        <%= Html.ValidationMessageArea("State")%>
                    </p>

                    <p>
                        <label>Country</label><em>*</em>
                        <%= Html.DropDownList("Country", (IEnumerable<SelectListItem>)ViewData["Countries"], "[ - Select - ]")%>
                        <br />
                        <%= Html.ValidationMessageArea("Country")%>
                    </p>
                    <p class="left">
                        <%= Html.CheckBox("IsRecievingUpdates", false, new { @class = "chk" })%> Please add me to the <strong><span>private</span></strong> mailing list for news and updates    
           
                    </p>

                    <p class="left" style="padding-right: 165px;">
                        <%= Html.CheckBox("IsRecievingWeeklySpecials", false, new { @class = "chk" })%> Please add me to the <strong><span>private</span></strong> mailing list for weekly specials   
           
                    </p>
                    <p class="info">* Required field</p>
                    
                    <p>
                        <%= Html.SubmitWithClientValidation("Send")%>
                    </p>
                </fieldset>
                <%--<% }%>--%>
            </form>
        </div>

        <div id="email_registration" class="control">
            <h2 class="title">Unsubscribe from free email alerts</h2>
            <%--<% using (Html.BeginForm("FreeEmailAlertsUnsubscribeSuccess", "Home", FormMethod.Post)){%>--%>
            <form method="post" action="<%=Consts.ProtocolSitePort %>/Home/FreeEmailAlertsUnsubscribeSuccess">
                <%=Html.AntiForgeryToken() %>
                <fieldset class="blue_box">
                    <p class="left">Enter your e-mail address below.</p>
                    <p style="height: 20px;">
                        <label for="email">E-mail address:</label><em>*</em>
                        <%= Html.TextBox("Email", "", new { @size = "40" })%><%= Html.ValidationMessageArea("Email")%>
                    </p>
                    <p class="info">* Required field</p>
                    <p>
                        <%= Html.SubmitWithClientValidation("Send")%>
                    </p>
                </fieldset>
                <%--<%} %>--%>
            </form>
        </div>
    </div>
</asp:Content>
