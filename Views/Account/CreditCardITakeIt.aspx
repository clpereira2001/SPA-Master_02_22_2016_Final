<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Payment processed - <%=ConfigurationManager.AppSettings["CompanyName"]%></title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <div class="col-md-9 col-sm-8">
    <%-- <h1 class="title">Error</h1>--%>
     <p>
     Sorry, we were unable to authourize your credit card.
     </p>  
  <p>
  <% if (ViewData["ErrorMessage"] != null)
     { %>
          <strong>Error:</strong>&nbsp;&nbsp;<%=ViewData["ErrorMessage"] %>
  <%} %>  
  </p> 
  
   <div >
   <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/ITakeIt">
    <%=Html.AntiForgeryToken() %>
            <%= Html.Hidden("auction_id", ViewData["Auction_ID"])%>
            <%= Html.Hidden("isDOW", ViewData["IsDOW"])%>
            <%= Html.Hidden("quantity", ViewData["Quantity"])%>
            <button type="submit" style="" class="btn btn-danger btn-lg" id="btnPay">
               <span>Try Again</span>
            </button>
        </form>
   </div> 
   </div>
</asp:Content>