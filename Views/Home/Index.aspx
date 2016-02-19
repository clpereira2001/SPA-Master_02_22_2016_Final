<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Vauction.Utils.Autorization" %>

<asp:Content ID="sdffsd" ContentPlaceHolderID="HTMLhead" runat="server">
    <title><%=Consts.CompanyTitleName %> : rare art, jewelry, gold and silver coins, paper currency, fine watches, rugs, sports memorabilia, electronics and much more</title>
    <% SessionUser cuser = AppHelper.CurrentUser; %>
    <% if (cuser != null && cuser.IsAdminType)
       {%>
    <% Html.Script("general.js"); %>
    <script src="<%=ResolveUrl("~/public/tinymce/tinymce.min.js" ) %>" type="text/javascript"></script>
    <script src="<%=ResolveUrl("~/public/tiny_box.js")%>" type="text/javascript"></script>
    <%}%>
</asp:Content>

<asp:Content ID="subMenuContent" ContentPlaceHolderID="subMenu" runat="server">

    <div id="submenuitems" class="span-19 last submenuitems" style="-width: 750px">
        <div id="submenuitems_grad" class="span-19 last submenuitems" style="-display: inline; -margin: 0px; -padding: 0px">
            <%for (int i = 1; i < 4; i++)
              { %>
            <div class="span-3 push-1" style="-display: inline;">
                <%=Html.ActionLink(" ","Index", "Event", new { controller = "Event", action = "Index"}, new { @class = "category"+i.ToString() }) %>
            </div>
            <%} %>
            <div class="span-3 push-2" style="-display: inline; -margin: 0 -10px 1.5em 10px;">
                <%=Html.ActionLink(" ","Index", "Event", new { controller = "Event", action = "Index"}, new { @class = "category4" }) %>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="rightCategoriesContent" ContentPlaceHolderID="rightCategories" runat="server">

    <div id="right_sidebar_grad" class="span-5 last" style="-display: inline;">
        <%for (int i = 5; i < 10; i++)
          {
              Response.Write(Html.ActionLink(" ", "Index", "Event", new { controller = "Event", action = "Index" }, new { @class = "category" + i.ToString() }));
              Response.Write("<br />");
          }
        %>
    </div>

</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
        <% decimal version; if (Decimal.TryParse(Request.Browser.Version, out version) && version < 7 && Request.Browser.Browser == "IE")
           { %>
        <% Html.RenderPartial("IE6Message"); %>
        <%} %>
        <% EventDetail currentEvent = ViewData["CurrentEvent"] as EventDetail;%>

        <% Html.RenderPartial("EditContent", new EditContentParams { Height = 500, Width = 530, File = "_Index" }); %>

        <%  if (currentEvent != null)
            { %>
        <% if (currentEvent.IsClickable && currentEvent.IsViewable)
           { %>
        <div class="row">
            <div class="internet-auction full-width overflow-hidden margin-vertical-lg padding-vertical">
                <div class="col-sm-9">
                    <p class="text-uppercase text-primary text-lg margin-bottom-none text-weight-700"><%=currentEvent.Title%></p>
                    <p class="text-uppercase margin-bottom-none text-md"><%=currentEvent.StartTime%> TO <%=currentEvent.EndTime%></p>
                </div>
                <div class="col-sm-3">
                    <a href="<%=this.ResolveUrl(String.Format("/Auction/EventDetailed/{0}/{1}", currentEvent.ID, currentEvent.UrlTitle)) %>" class="text-uppercase btn btn-danger btn-lg text-weight-700" onclick="document.forms[0].submit();">enter auction</a>
                </div>
            </div>
            <div class="text-center">
                <% if (currentEvent.DateEnd < DateTime.Now)
                   { %>
                <p class="text-uppercase margin-bottom-none text-md"><%=currentEvent.Title.ToUpper() %> HAS ENDED -- THANK YOU</p>
                <%} %>
            </div>
        </div>
        <br />
        <%}
           else
           { %>
        <div class="row">
            <div class="internet-auction full-width overflow-hidden bg-color-fourth margin-vertical-lg padding-vertical">
                <div class="col-sm-9">
                    <p class="text-uppercase text-primary text-lg margin-bottom-none text-weight-700"><%=currentEvent.Title%></p>
                    <p class="text-uppercase margin-bottom-none text-md"><%=currentEvent.StartTime%> TO <%=currentEvent.EndTime%></p>
                </div>
            </div>
        </div>
        <br />
        <%} %>
        <p class="text-primary">
            <%=currentEvent.Description%>
        </p>
        <% }%>
        <div class="col-sm-12">
            <h2 class="text-uppercase text-center text-primary title-featured" ><span style="margin-top: 13px;margin-bottom: 13px;">featured products</span></h2>
        </div>
        <%Html.RenderPartial("~/Views/Auction/_AuctionGrid.ascx", ViewData["Lots"]); %>
        <%--<% SessionUser cuser = AppHelper.CurrentUser; %>
    <% Html.RenderAction((!Request.IsAuthenticated || cuser == null) ? "pIndexUn" : "pIndex", "Home", new { event_id = currentEvent.ID, user_id = (cuser == null ? -1 : cuser.ID), param = ViewData["AuctionFilterParams"], iscurrent = currentEvent.IsCurrent });%>--%>
    </div>
</asp:Content>
