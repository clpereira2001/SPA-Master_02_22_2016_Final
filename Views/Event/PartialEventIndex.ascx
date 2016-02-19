<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Event>>" %>
<% 
    long? event_id = (long?)ViewData["EventID"];
    if (Model != null && Model.Count() > 0)
    { %>
<% foreach (Event evnt in Model)
   { %>
<%  if (evnt.IsCurrent) continue; %>

<div class="row">
    <div class="col-sm-12">
        <div class="well well-lg bg-white">
            <p class="text-primary text-uppercase text-weight-700"><%= (evnt.IsClickable || evnt.IsAccessable)?Html.ActionLink(evnt.Title, "EventDetailed", new { controller = "Auction", action = "EventDetailed", id = evnt.ID, evnt=evnt.UrlTitle }).ToHtmlString() : evnt.Title%><span class="pull-right text-weight-700"><%=evnt.DateStart.ToString("yyyy, MMMM ,dd") %></span></p>
            <p><%= evnt.Description%></p>
            <p>
                  <%= Html.ActionLink("enter auction", "EventDetailed", new { controller = "Auction", action = "EventDetailed", id = evnt.ID, evnt=evnt.UrlTitle }, new { @class = "btn btn-danger text-uppercase btn-lg" })%>
               <%-- <a href="#" class="btn btn-danger text-uppercase btn-lg">Enter Auction</a>--%>
            </p>
        </div>
    </div>
</div>



<br />
<%}
    }
    else if (!event_id.HasValue)
    { %>
              No Upcoming Events
     
<%  } %>
