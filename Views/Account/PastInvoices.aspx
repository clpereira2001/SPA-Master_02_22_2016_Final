<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Vauction.Models.IUserInvoice>>" %>

<%@ Import Namespace="Vauction.Utils.Helpers" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Past Auction Invoices - <%=Consts.CompanyTitleName %></title>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
        <div class="center_content">
            <h1 class="title">Past Auction Invoices</h1>
            <% if (Model == null || Model.Count() == 0)
               {
                   Response.Write("You don't have any winning invoices from our past auction(s) yet.");
               }
               else
               { %>
    Below is a list of your invoices. Invoices are listed in reverse order by begining date.
   
            <br class="clear" />
            <br class="clear" />
            <ul>
                <% List<IUserInvoice> inv = new List<IUserInvoice>(Model);
                   foreach (UserInvoice item in inv)
                   { 
      %>
                <li>
                    <%=Html.ActionLink(item.Event.Title, "InvoiceDetailed", new { controller = "Account", action = "InvoiceDetailed", id = item.ID, evnt=item.Event.UrlTitle, cat=UrlParser.TitleToUrl("Invoice# "+item.ID+" details") }, new { @style = "font-size:14px" })%>
      </li>
                <% }%>
                <%} %>
            </ul>
        </div>
        <% bool isDOW = (ViewData["isDOW"] != null) ? Convert.ToBoolean(ViewData["isDOW"]) : false;
           if (isDOW)
           {
       %>
        <ul>

            <li>
                <%=Html.ActionLink("Deal of the week invoices", "InvoiceDetailedDOW", new { controller = "Account", action = "InvoiceDetailedDOW", id = AppHelper.CurrentUser.ID, evnt=UrlParser.TitleToUrl("Deal of the week invoices") }, new { @style = "font-size:14px" })%>
      </li>
        </ul>
        <%
     }
    %>

        <%--<div class="back_link">
            <%=Html.ActionLink("Return to the My Account page", "MyAccount", new {controller="Account", action="MyAccount"}) %>
        </div>--%>
    </div>
</asp:Content>
