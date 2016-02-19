<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Vauction.Utils.Html" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Email settings - <%=Consts.CompanyTitleName %></title>
    <% Html.Script("hashtable.js"); %>
    <% Html.Script("validation.js"); %>
   <% Html.Script("jquery.maskedinput.min.js"); %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
        <div class="row">
            <div class="col-sm-12">
                <p class="text-primary text-weight-900 text-lg">Email Settings</p>
            </div>
        </div>
        <% using (Html.BeginForm())
           { %>
        <%=Html.AntiForgeryToken() %>
        <div class="row">
            <div class="col-sm-12">



                <div class="checkbox alt-check margin-bottom-lg">
                    <label class="text-uppercase text-weight-700">
                        <%= Html.CheckBox("IsRecievingWeeklySpecials", Convert.ToBoolean(ViewData["IsRecievingWeeklySpecials"]))%>
                        <span class="check-icon"></span>
                        Sign me up for new Deal of the Week notifications:*
                    </label>
                </div>
                <div class="checkbox alt-check margin-bottom-lg">
                    <label class="text-uppercase text-weight-700">
                        <%= Html.CheckBox("IsRecievingNewsUpdates", Convert.ToBoolean(ViewData["IsRecievingNewsUpdates"]))%>
                        <span class="check-icon"></span>
                        Sign me up for new Event notifications:*
                    </label>
                </div>
                <div class="checkbox alt-check margin-bottom-lg">
                    <label class="text-uppercase text-weight-700">
                        <%= Html.CheckBox("IsRecievingBidConfirmation", Convert.ToBoolean(ViewData["IsRecievingBidConfirmation"]))%>
                        <span class="check-icon"></span>
                        Sign me up for Bid Confirmations:* 
                    </label>
                </div>

                <div class="checkbox alt-check margin-bottom-lg">
                    <label class="text-uppercase text-weight-700">
                        <%= Html.CheckBox("IsRecievingOutBidNotice", Convert.ToBoolean(ViewData["IsRecievingOutBidNotice"]))%>
                        <span class="check-icon"></span>
                        Sign me up for OutBid notices:*
                    </label>
                </div>

                <div class="checkbox alt-check margin-bottom-lg">
                    <label class="text-uppercase text-weight-700">
                        <%= Html.CheckBox("IsRecievingLotSoldNotice", Convert.ToBoolean(ViewData["IsRecievingLotSoldNotice"]))%>
                        <span class="check-icon"></span>
                        Sign me up for Lot Sold notices:*
                    </label>
                </div>

                <div class="checkbox alt-check margin-bottom-lg">
                    <label class="text-uppercase text-weight-700">
                        <%= Html.CheckBox("IsRecievingLotClosedNotice", Convert.ToBoolean(ViewData["IsRecievingLotClosedNotice"]))%>
                        <span class="check-icon"></span>
                        Sign me up for Lot Closed notices:*
                    </label>
                </div>


                <br />
                <p>
                    <% if (ViewData["DataSaved"] != null && Convert.ToBoolean(ViewData["DataSaved"]))
                       { %>
                    <strong>The settings were saved successfully.</strong><br />
                    <br />
                    <%} %>

                   
                </p>
                <% } %>
                 <div class="row text-center margin-top-lg">
                      <%= Html.SubmitWithClientValidation("Update","text-uppercase text-weight-600 btn btn-danger btn-lg padding-horizontal-xl")%>
                    </div>

              <%--  <div class="back_link">
                    <%=Html.ActionLink("Return to the My Account page", "MyAccount", new {controller="Account", action="MyAccount"}) %>
                </div>--%>
            </div>
        </div>
    </div>
</asp:Content>
