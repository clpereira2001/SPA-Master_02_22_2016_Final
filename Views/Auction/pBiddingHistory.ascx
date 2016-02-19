<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<BiddingHistory>>" %>

<span id="biddingTitle">The bidding for this lot has ended<%=(!Model.Any()) ? ". This item has no succesfull bids." : ",&nbsp;<span id=\"spLink\" style=\"color:#6990AF; font-size:16px; cursor:pointer\"><U>click to</U></span>&nbsp;view bidding history."%></span>
<%
  if (Model.Any())
  {
    bool line = false;    
%>
<div id="BHBlock" style="display: none">
  <b>Bidding History</b><br />
  <table cellpadding="0" cellspacing="0">
    <tr style="background-color: #CCC">
      <td class="bordered"><b>User</b></td>
      <td class="bordered"><b>Bid</b></td>
      <td class="bordered"><b>Quantity</b></td>
      <td class="bordered"><b>Time</b></td>
    </tr>
    <% 
      foreach (BiddingHistory o in Model)
      {
        line = !line;%>
      <%=(o.IsWinner) ? "<tr style=\"background-color:#9ADB8E;\">" : ((line) ? "<tr style=\"background-color:#EFEFEF\">" : "<tr style=\"background-color:#FFF\">")%>
      <td class="bordered"><%=(o.IsWinner) ? "<b>" : String.Empty%><%=o.Login%><%=(o.IsWinner) ? "</b>" : String.Empty%></td>
      <td class="bordered"><%=(o.IsWinner) ? "<b>" : String.Empty%><%=o.Amount.GetCurrency()%><%=(o.IsWinner) ? "</b>" : String.Empty%></td>
      <td class="bordered"><%=(o.IsWinner) ? "<b>" : String.Empty%>1<%=(o.IsWinner)?"</b>":String.Empty %></td>
      <td class="bordered"><%=(o.IsWinner) ? "<b>" : String.Empty%><%=o.DateMade.ToString("T")%><%=(o.IsWinner) ? "</b>" : String.Empty%></td>
    </tr>
    <%}%>
  </table>  

  <script type="text/javascript">
    $(document).ready(function () {
      $("#spLink").click(function () {
        $("#BHBlock").show();
        $("#biddingTitle").hide();
      });
    });
  </script>
</div>
<% } %>