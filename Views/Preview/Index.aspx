<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IPagedList<AuctionShort>>" %>
<%@ Import Namespace="Vauction.Utils.Paging" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>

<asp:Content ID="sdffsd" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>PREVIEW - <%=Consts.CompanyTitleName %></title>   
</asp:Content>			
<asp:Content ID="subMenuContent" ContentPlaceHolderID="subMenu" runat="server">
  <% EventDetail currentEvent = ViewData["CurrentEvent"] as EventDetail ?? new EventDetail(); %>
  <div id="submenuitems" class="span-19 last submenuitems">			
    <div id="submenuitems_grad" class="span-19 last submenuitems">						
       <%for (int i = 1; i < 4; i++){ %>
	      <div class="span-3 push-1"><%=Html.ActionLink(" ", "EventDetailed", "Preview", new { controller = "Preview", action = "EventDetailed", id = currentEvent.ID }, new { @class = "category" + i.ToString() })%></div>
	    <%} %>
	    <div class="span-3 push-2"><%=Html.ActionLink(" ", "EventDetailed", "Preview", new { controller = "Preview", action = "EventDetailed", id = currentEvent.ID }, new { @class = "category4" })%></div>
    </div>
</div>    
</asp:Content>
<asp:Content ID="rightCategoriesContent" ContentPlaceHolderID="rightCategories" runat="server">
  <% EventDetail currentEvent = ViewData["CurrentEvent"] as EventDetail ?? new EventDetail(); %>
  <div id="right_sidebar_grad" class="span-5 last">
  <%for (int i = 5; i < 10; i++) {
      Response.Write(Html.ActionLink(" ", "EventDetailed", "Preview", new { controller = "Preview", action = "EventDetailed", id = currentEvent.ID }, new { @class = "category" + i.ToString() }));
      Response.Write("<br />");
    }%>      
  </div>
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">    
    <% EventDetail currentEvent = ViewData["CurrentEvent"] as EventDetail ?? new EventDetail();%>
    <% if (!Request.IsAuthenticated || AppHelper.CurrentUser==null ) { %>        
        <div id="welcome" class="span-14 ie_btn_fix last">           
            <div class="span-4">Welcome!</div>            
		    <div class="prepend-4 span-2">
		        <button class="cssbutton small white" type="button">
		            <span>Login</span>
		        </button>		                
		        <br />
		    </div>    			
		    <div class="span-2 last">		
		        <button class="cssbutton small white" type="button">
		            <span>Register</span>
		        </button>		                
	        </div>	    
        </div>    
    <% }%>    
    <p class="content_text span-14" style="text-indent:0px">    
		<strong>SeizedPropertyAuctions.com</strong> is the premier auction company for liquidating property seized by police and federal agencies, property from abandoned safe deposit boxes, seized bank assets, bankruptcies, financial institutions, business inventory liquidations, and other consigners.
        Our firm conducts traditional live auctions throughout the year at various locations across the country, as well as auctions on the Internet. Please see our schedule of upcoming auctions for further details.
	</p>
    <%  if (currentEvent.ID > 0) { %>    
        <div id="featured_title_doubleheight" class="span-14 last" >
            <%=currentEvent.Title%><br />
            <div style="font-size:14px; color:#DDDDDD;"><%=currentEvent.StartEndTime %></div>
        </div>
        <div class="center_content span-14 last">
          <p class="content_text"><%=currentEvent.Description%></p>    
        </div>    
    <% }%>    
    <%Html.RenderPartial("~/Views/Auction/_AuctionGrid.ascx", ViewData.Model); %>    
</asp:Content>