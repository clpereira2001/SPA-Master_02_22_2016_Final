<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<AuctionShort>>" %>
<%int i = 0;
  int count = Model.Count();

  int icount = 0;
  for (int j = 0; j < count; j++)
  {
      if (j != 0 && j % 4 == 0)
      {
          icount = j;
      }
  }
  %>

<div class="col-md-12 col-sm-12">
    <div class="grid-col-block">



        <div class="row" style="margin:3px; ">
            <%  
                foreach (AuctionShort item in Model)
                {
            %>
            <% if (i != 0 && i % 4 == 0)
               { %>
        </div>
        <div class="row" style="margin:3px; ">
            <% } %>
            <div >
                <% 
                    if (i < icount)
                        Html.RenderPartial("~/Views/Auction/_ItemView.ascx", item); 
                    %>
            </div>
            


            <% i++;%>
            <% }

            %>
        </div>
    </div>
</div>
