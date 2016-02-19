<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Vauction.Utils.Autorization" %>

<asp:Content ID="cntHead" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Upcoming Auctions - <%=Consts.CompanyTitleName %></title>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
        <% EventDetail currentEvent = ViewData["CurrentEvent"] as EventDetail;
           SessionUser cuser = AppHelper.CurrentUser;%>
        <div class="row">
            <div class="col-sm-12">
                <div class="well well-lg bg-white">
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
<%--                    <% Html.RenderAction("pIndex", "Event", new { event_id = currentEvent.ID, isa = (cuser != null && cuser.IsAccessable), iscurrent = currentEvent.IsCurrent });%>--%>
                <%  if (currentEvent != null)
                            { %>
                        <% if (currentEvent.IsClickable && currentEvent.IsViewable)
                           {
                               %>
                                    <p class="text-primary text-uppercase text-weight-700"><%=currentEvent.Title%> <span class="pull-right text-weight-700"><%=currentEvent.StartTime%></span></p>
                                    <% if (currentEvent.DateEnd < DateTime.Now)
                                   { %>
                                        <p class="text-uppercase margin-bottom-none text-md"><%=currentEvent.Title.ToUpper() %> HAS ENDED -- THANK YOU</p>
                                    <%} %>
                            <%}
                           else
                           { %>                
                                <p class="text-primary text-uppercase text-weight-700"><%=currentEvent.Title%> <span class="pull-right text-weight-700"><%=currentEvent.StartTime%></span></p>
                            <%} %>
                        <p>
                            <%=currentEvent.Description%>
                        </p>
                        
                    <form id="frmEnterAuction" action="<%=this.ResolveUrl(String.Format("/Auction/EventDetailed/{0}/{1}", currentEvent.ID, currentEvent.UrlTitle)) %>" method="post">
                        <div class="text-center padding-top margin-bottom">
                            <p><button id="btnSignIn" name="SignIn" type="submit" class="text-uppercase btn btn-danger btn-lg padding-horizontal-xl" onclick="submit();">enter auction</button>
                            </p>
                        </div>
                    </form>
<%--                            <a href="<%=this.ResolveUrl(String.Format("/Auction/EventDetailed/{0}/{1}", currentEvent.ID, currentEvent.UrlTitle)) %>" class="btn btn-danger text-uppercase btn-lg" onclick="$(this).closest('form').submit()">enter auction</a>--%>
                        <% }%>
                </div>
            </div>

        </div>
    </div>
</asp:Content>
