<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Payment processed - <%=ConfigurationManager.AppSettings["CompanyName"]%></title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <div class="col-md-9 col-sm-8">
   <p>
  Sorry, we were unable to authourize your credit card.
  </p>
  <p>
  <% if (ViewData["ErrorMessage"] != null)
     { %>
          <strong>Error:</strong>&nbsp;&nbsp;<%=ViewData["ErrorMessage"] %>
  <%} %>  
  </p> 
   <div  >
    <% if (ViewData["IsPayment"] != null && Convert.ToBoolean(ViewData["IsPayment"]))
       { %>
      <%= Html.ActionLink("Try again", "PayForItems", "Account")%>
    <%}
       else
       {%>
         <%--<%using (Html.BeginForm("PayDeposit", "Account", FormMethod.Post)){%>      --%>
         <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/PayDeposit">
          <%=Html.AntiForgeryToken() %>
          <%=Html.Hidden("id", ViewData["IdAuction"])%>          
          <%=Html.Hidden("sum", ViewData["AmountToPay"])%>          
          <%=Html.Hidden("ProxyBidding", ViewData["ProxyBidding"])%>
          <%=Html.Hidden("Quantity", ViewData["Quantity"])%>         
          <%=Html.Hidden("RealBidAmount", ViewData["RealBidAmount"])%>
          <%=Html.Hidden("BidToPlace", ViewData["BidToPlace"])%>
          
          <input id="btnSubmit" type="submit" class="btn btn-danger btn-lg" style="display:none" />
          <span id='lSubmit' style='color:Navy;cursor:default;border-bottom:solid 1px white'>Try again</span>
          </form>
      <% //}
       } %>    
   </div>
   </div>
    <script type="text/javascript">
      $(document).ready(function() {
        $("#lSubmit").click(function() {
          $("#btnSubmit").click();
        });

        $("#lSubmit").mouseout(function() {
          $(this).css("border-bottom", "solid 1px white");          
        });

        $("#lSubmit").mouseover(function() {
          $(this).css("border-bottom", "solid 1px navy");
        });
      });      
  </script>
</asp:Content>
