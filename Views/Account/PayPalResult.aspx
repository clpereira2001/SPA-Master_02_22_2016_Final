<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Vauction.Models.Auction>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Redirecting to paypal - <%=ConfigurationManager.AppSettings["CompanyName"]%></title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <p class="pay_notice sized" >  <%=ViewData["Msg"] %>.</p>
       <div style='width:100%;text-align:right'>
    <% if (ViewData["IsDOW"]!=null && Convert.ToBoolean(ViewData["IsDOW"])) { %>
        <%=Html.ActionLink("Return to the Home page", "Index", new { controller = "Home", action = "Index" })%>
     <% } else { %>     
    <% if (ViewData["IsPayment"] != null && Convert.ToBoolean(ViewData["IsPayment"]))
       { %>
        <%= Html.ActionLink("Try again", "PayForItems", "Account")%>
     <% } else {%>
          <span id='lSubmit' style='color:Navy;cursor:default;border-bottom:solid 0px transparant;'>Try again</span>
         <%}%>
         
         <%} %>
         </div>
        <%--<% using (Html.BeginForm("PayDeposit", "Account", FormMethod.Post)) {%>--%>
        <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/PayDeposit">
          <%=Html.AntiForgeryToken() %>
        <%=Html.Hidden("id", ViewData["IdAuction"])%>
        <%=Html.Hidden("sum", ViewData["AmountToPay"])%>
        <%=Html.Hidden("ProxyBidding", ViewData["ProxyBidding"])%>
        <%=Html.Hidden("Quantity", ViewData["Quantity"])%>
        <%=Html.Hidden("RealBidAmount", ViewData["RealBidAmount"])%>
        <%=Html.Hidden("BidToPlace", ViewData["BidToPlace"])%>
        <input id="btnSubmit" type="submit" style="display: none" />
        </form>
      <%--<%} %>--%>
      <%--<%= Html.ActionLink("Try again", "PayDeposit", new { @id = ViewData["Auction_ID"], @sum = ViewData["AmountToPay"] })%>--%>
  
   <script type="text/javascript">
     $(document).ready(function() {
       $("#lSubmit").click(function() {
         $("#btnSubmit").click();
       });

       $("#lSubmit").mouseout(function() {
         $(this).css("border-bottom", "solid 0px white");
       });

       $("#lSubmit").mouseover(function() {
         $(this).css("border-bottom", "solid 1px navy");
       });
     });      
  </script>
  
</asp:Content>
