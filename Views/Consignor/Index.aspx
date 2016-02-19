<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<Event>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Seller Tool - <%=Consts.CompanyTitleName %> </title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="col-md-9 col-sm-8">
        <div class="center_content">
            <div class="row">
                <div class="col-sm-12">
                    <p class="text-primary text-weight-900 text-lg">Seller Tools</p>
                </div>
            </div>
            <div class="row">
                <div class="col-sm-12">
                    <% if (Model.Count > 0)
                       { %>
                    <% foreach (Event evnt in Model)
                       { %>

                    <p class="text-uppercase text-primary text-weight-600">List an Auction</p>
                    <ul class="list list-arrow margin-bottom-xl padding-left-xl">
                        <li>
                            <%= Html.ActionLink(String.Format("{0}   {1}", evnt.Title, evnt.StartEndTime), "AddAuction", new { controller = "Consignor", action = "AddAuction", id = evnt.ID })%>
                        </li>
                    </ul>

                    <p class="text-uppercase text-primary text-weight-600">Preview an Auction</p>
                    <ul class="list list-arrow margin-bottom-lg margin-bottom-xl padding-left-xl">
                        <li>
                            <%=Html.ActionLink(String.Format("{0}   {1}", evnt.Title, evnt.StartEndTime), "Index", new { controller = "Preview", action = "Index", event_id = evnt.ID.ToString() }, new { @target = "_blank" }) %>
                        </li>
                    </ul>
                    <%} %>


                    <%} %>

                    <p class="text-uppercase text-primary text-weight-600">Auction Management</p>
                    <ul class="list list-arrow margin-bottom-lg margin-bottom-xl padding-left-xl">
                        <li><%= Html.ActionLink("Edit a Current Auction", "EditAuction", "Consignor")%></li>
                        <li><%= Html.ActionLink("Batch Image Upload", "BatchImageUpload", "Consignor")%></li>
                    </ul>
                </div>
            </div>
            <div class="back_link">
                <%=Html.ActionLink("Return to My Account page", "MyAccount", new {controller="Account", action="MyAccount"}).ToSslLink() %>
            </div>
        </div>
    </div>
</asp:Content>
