<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Vauction.Models.IEvent>>" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Past Auction Bidding History - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%
        
        List<Event> lstEvent = null;
        if (ViewData["lstEvent"] != null)
        {
            lstEvent = (System.Collections.Generic.List<Vauction.Models.Event>)(ViewData["lstEvent"]);
        }
    %>
    <div class="col-md-9 col-sm-8">

        <div class="row">
            <div class="col-sm-12">
                <p class="text-primary text-weight-900 text-lg">Past Auction Bidding History</p>
                <% if (lstEvent != null && lstEvent.Count > 0)
                   {%>
                <p class="text-muted">Below is a list of auctions you have participated in. Auctions are listed in reverse order by begining date. </p>

                <%} %>
                <%else
                   { %>

                <p class="text-muted">We show no bidding history for you at this time. </p>
                <%} %>
            </div>
        </div>
        <% if (lstEvent != null && lstEvent.Count > 0)
           {%>
        <div class="row">
            <div class="col-sm-12">
                <div class="table-responsive">
                    <table class="table table-hover table-striped">
                        <thead>
                            <tr class="bg-primary">
                                <th>Auction</th>
                                <th class="text-center">Start Date</th>
                                <th class="text-center">End Date</th>
                            </tr>
                        </thead>
                        <tbody>
                            <% for (int i = 0; i < lstEvent.Count; i++)
                               {%>
                            <tr>
                                <td><b><%=lstEvent[i].EventDetailsInfoDemo %></b></td>
                                <td class="text-center"><%=lstEvent[i].DateStart.ToString("MM/dd/yyyy") %></td>
                                <td class="text-center"><%=lstEvent[i].DateEnd.ToString("MM/dd/yyyy")  %></td>
                            </tr>
                            <%} %>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>


        <% } %>
        <div class="back_link"><%=Html.ActionLink("Return to the My Account page", "MyAccount", new {controller="Account", action="MyAccount"}) %></div>

    </div>
</asp:Content>
