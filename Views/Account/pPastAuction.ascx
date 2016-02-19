<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Vauction.Models.Event>>" %>
<% if (Model == null || Model.Count() == 0) { %> We show no bidding history for you at this time. <%}else{ %>
  Below is a list of auctions you have participated in. Auctions are listed in reverse order by begining date.
  <br class="clear" /><br class="clear" />      
  <ul>
  <% foreach (Event item in Model){%>
    <li><%=(item.DateEnd >= DateTime.Now.AddDays(-45)) ? Html.ActionLink(item.Title + " " + item.StartEndTime, "AuctionsParticipated", new { controller = "Account", action = "AuctionsParticipated", id = item.ID, evnt = item.UrlTitle }, new { @style = "font-size:14px" }).ToHtmlString() : item.Title + " " + item.StartEndTime %></li>
  <%}%>
  </ul>
<%} %>