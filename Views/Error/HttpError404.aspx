<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Can't locate the page you're looking for -
    <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <% SessionUser cuser = AppHelper.CurrentUser; %>
  <div class="center_content">
    <div style="padding:5px;color:#a82824;border:dotted 1px #800000; margin:5px; font-family: Arial, Helvetica, sans-serif;font-size: 14px; border-color: #eba5a3; background-color: #f2c6c4">Can't locate the page you're looking for. The folowwing links might be useful.</div><br />
    <div style="width:260px;padding:5px;float:left">
	    <%=Html.ActionLink("HOME", "Index", "Home") %>
	    <br class="clear" /><br class="clear" />
	    <%List<Event> upcomingEvents = ViewData["UpcomingEvents"] as List<Event>;
       EventDetail currentEvent = ViewData["CurrentEvent"] as EventDetail; 
        if (currentEvent != null)
        {
      %>
	      <b>AUCTION EVENTS</b><br />  	    
	      <ul>            
	        <li><%=currentEvent.IsClickable ? Html.ActionLink(currentEvent.Title, "EventDetailed", new { controller = "Auction", action = "EventDetailed", id = currentEvent.ID, evnt = currentEvent.UrlTitle }).ToHtmlString() : currentEvent.Title%></li>
	        <% foreach(Event evnt in upcomingEvents){
              if (evnt.IsCurrent) continue;%>
	          <li><%=evnt.IsClickable?Html.ActionLink(evnt.Title, "EventDetailed", new { controller = "Auction", action = "EventDetailed", id = evnt.ID, evnt=evnt.UrlTitle }).ToHtmlString() : evnt.Title %></li>
	        <%} %>
	      </ul>      
	    <%} %>	   
	    <%=Html.ActionLink("PRODUCTS AND SERVICES", "Product", "Home") %>
	    <br class="clear" /><br class="clear" />
	    <%=Html.ActionLink("FAQs", "FAQs", "Home")%>
	    <br class="clear" /><br class="clear" />
	    <%=Html.ActionLink("CONTACT US", "ContactUs", "Home") %>
	    <br class="clear" /><br class="clear" />
	    <%=Html.ActionLink("TERMS AND CONDITIONS", "Terms", "Home")%>
	    <br class="clear" /><br class="clear" />
	    <%=Html.ActionLink("PRIVACY STATEMENT", "Privacy", "Home")%>
	    <br class="clear" /><br class="clear" />
	  </div>
	  <div style="width:260px;padding:5px;float:left;margin-left:5px">
	    <% if (Request.IsAuthenticated && cuser != null) { %>
        <%=Html.ActionLink("PROFILE", "Profile", "Account") %>
        <br class="clear" /><br class="clear" />
        <% if (cuser.UserType == (byte)Consts.UserTypes.Seller) { %>
          <%=Html.ActionLink("Seller Tools", "Index", "Consignor") %>
        <%} else { %>
          <b>MY ACCOUNT</b><br />  	    
	        <ul>
            <li>
                <%= Html.ActionLink("Update / Change Your information", "Profile", "Account").ToSslLink() %>
            </li>
            <li>                    
                <%= Html.ActionLink("Auctions You Have Participated In", "PastAuction", "Account").ToSslLink() %>
            </li>                
            <li>
                <%= Html.ActionLink("Pay for Auction Items", "PayForItems", "Account").ToSslLink() %>                                        
            </li>
            <li>
              <%= Html.ActionLink("Past Auction Invoices", "PastInvoices", "Account").ToSslLink()%>
            </li>
            <li>
                <%= Html.ActionLink("Edit Your Mail Settings", "EditMailSettings", "Account").ToSslLink() %>
            </li>
            <li>
                <%= Html.ActionLink("Edit Your Personal Shopper", "EditPersonalShopper", "Account").ToSslLink()%>
            </li>
            <li>
                <%= Html.ActionLink("Receive Your Personal Shopper Update", "ReceivePersonalShopperUpdate", "Account").ToSslLink() %>
            </li>                
            <%
                if (cuser.IsSellerBuyer || cuser.IsSeller)
                {
                 %>
            <li>
                <div class="attention_text"><%= Html.ActionLink("Seller Tool", "Index", "Consignor").ToNonSslLink() %></div>
            </li>                
            <%  } %>                         
          </ul>
          <%= Html.ActionLink("LOG OUT", "LogOff", "Account").ToNonSslLink() %>
        <%} %>
      <%} else { %>
        <%=Html.ActionLink("REGISTER", "Register", "Account").ToSslLink()%>
        <br class="clear" /><br class="clear" />
        <%=Html.ActionLink("LOG IN", "LogOn", "Account").ToSslLink() %>
      <%} %>
	  </div>
  </div>
</asp:Content>
