<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
 <%=(Request.IsAuthenticated)?"<ul class='bottom_registred'>":"<ul class='bottom_unregistred'>" %>
  <li class="firstitem"><%=Html.ActionLink("Home", "Index", "Home").ToNonSslLink() %></li>
  <% if (Request.IsAuthenticated) { %>
  <li><%= Html.ActionLink("My Account", "MyAccount", "Account").ToSslLink() %></li>
  <% } %>
  <li><%= Html.ActionLink("Auction Events", "Index", "Event").ToNonSslLink() %> </li>
  <li><%= Html.ActionLink("Products and Services", "Product", "Home").ToNonSslLink() %></li>					
  <li><%= Html.ActionLink("FAQs", "FAQs", "Home").ToNonSslLink() %> </li>
  <li><%= Html.ActionLink("Contact Us", "ContactUs", "Home").ToNonSslLink() %></li>
    
  <% if (Request.IsAuthenticated) { %>
  <li><%= Html.ActionLink("Register", "Register", "Account").ToSslLink() %></li>
  <% } %>
  <li><%= Html.ActionLink("Site Map", "SiteMap", "Home").ToNonSslLink() %></li>
</ul>