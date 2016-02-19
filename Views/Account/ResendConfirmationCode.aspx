<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ResendConfirmationCode>" %>

<%@ Import Namespace="Vauction.Utils.Html" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Resend Confirmation Link - <%=Consts.CompanyTitleName %></title>
    <% Html.Script("hashtable.js"); %>
    <% Html.Script("validation.js"); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div id="registration" class="col-md-9 col-sm-8">
        <%-- <h2>Resend Confirmation Link</h2>--%>
        <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/ResendConfirmationCode">
            <%=Html.AntiForgeryToken() %>
            <fieldset class="blue_box">
                <p class="text-primary">
                    Enter your <i>registred</i> e-mail address below.
                    
                </p>
                <p>
                    Please add <b>admin@siezedropertyauction.com</b> to you safe senders list
                </p>
                <div class="form-group">
                    <label for="email" class="text-uppercase">E-mail address</label><span class="text-danger">*</span>

                    <%= Html.TextBox("Email", ViewData.Model.Email, new { @size = "40",@class="form-control" })%>
                    <br />
                    <%= Html.ValidationMessageArea("Email")%>
                </div>


                <div class="text-center padding-top margin-bottom">
                    <%= Html.SubmitWithClientValidation("Send Confirmation","text-uppercase btn btn-danger btn-lg padding-horizontal-xl")%>
                </div>
            </fieldset>
        </form>
    </div>
</asp:Content>
