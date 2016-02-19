<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Vauction.Utils.Autorization" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>My Account - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% SessionUser cuser = AppHelper.CurrentUser; %>
    <div class="col-md-9 col-sm-8">
        <% EventDetail currentEvent = ViewData["CurrentEvent"] as EventDetail;
            %>
        <div class="row">
            <div class="col-sm-12">
                <div >
                    <% if (currentEvent != null && currentEvent.IsCurrent)
                       { %>
                    <br />
                     
                    <h4><%= Html.ActionLink(currentEvent.Title, "EventDetailed", new { controller = "Auction", action = "EventDetailed", id = currentEvent.ID, evnt=currentEvent.UrlTitle })%></h4>
                    <div style="clear: both; text-align: justify;">
                        <p><strong>When:</strong><%=currentEvent.StartEndTime %></p>
                        <%= currentEvent.Description%><br />
                        <%= Html.ActionLink("Click here to enter auction", "EventDetailed", new { controller = "Auction", action = "EventDetailed", id = currentEvent.ID, evnt=currentEvent.UrlTitle }, new {@class="event_click_here" })%>
                    </div>
                    <br />
                    <% } %>
                    <% Html.RenderAction("pIndex", "Event", new { event_id = currentEvent.ID, isa = (cuser != null && cuser.IsAccessable), iscurrent = currentEvent.IsCurrent });%>
                </div>
            </div>

        </div>
    </div>
   
</asp:Content>
