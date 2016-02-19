<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<List<Event>>" %>

<asp:Content ID="Content7" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Auction Preview - <%=Consts.CompanyTitleName %> </title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
       <%-- <div class="center_content">
            <h1 class="title">Auction Preview</h1>--%>
            <% if (Model.Count > 0)
               {%>


            <div class="row">
                <div class="col-sm-12">
                    <div class="table-responsive">
                        <table class="table table-hover table-striped">
                            <thead>
                                <tr class="bg-primary">
                                    <th class="text-center align-middle">Auction</th>
                                    <th class="text-center">Start Date</th>
                                    <th class="text-center">End Date</th>
                                </tr>
                            </thead>
                            <tbody>
                                <% foreach (Event evnt in Model)
                                   {%>
                                <tr>
                                    <td class="align-middle"><%=evnt.EventDetailsInfoDemo %></td>
                                    <td class="text-center"><%=evnt.DateStart.ToString("MM/dd/yyyy") %></td>
                                    <td class="text-center"><%=evnt.DateEnd.ToString("MM/dd/yyyy")  %></td>
                                </tr>
                                <% }%>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>



            <% }
               else
               {%>
      There is no pending event in the system.
   
            <% }%>
        </div>
   <%-- </div>--%>
</asp:Content>
