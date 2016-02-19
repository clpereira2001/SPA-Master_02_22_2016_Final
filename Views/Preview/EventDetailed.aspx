<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>PREVIEW - Event #<%=(ViewData["CurrentEvent"] as EventDetail ?? new EventDetail()).ID%> detailed - <%=Consts.CompanyTitleName %> </title>
</asp:Content>
<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
        <div class="center_content">
            <% EventDetail currentEvent = ViewData["CurrentEvent"] as EventDetail ?? new EventDetail();
               if (currentEvent.IsCurrent)
               {%>
            <p class="pay_notice">
                Do not bid unless you intend to pay. You are entering into a binding contract. 
             
                <br />
                All items must be paid in full within 2 days after the auction has ended.
         
            </p>
            <%} %>
            <% if (currentEvent.ID > 0)
               {%>
            <div id="featured_title"><%= currentEvent.Title%> event Featured items:</div>
            <%Html.RenderPartial("~/Views/Auction/_AuctionGrid.ascx", ViewData.Model);%>
            <%}
               else
               {%>
        There is no such event.
   
            <%} %>
        </div>
    </div>
</asp:Content>
