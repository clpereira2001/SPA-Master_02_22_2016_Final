<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<% 
   bool isDemo = (ViewData["DEMO_MODE"] != null) ? Convert.ToBoolean(ViewData["DEMO_MODE"]) : false;
   if (isDemo) 
       Response.Write("&nbsp;");
   else
   {%> 
        

             <%--<div class="search-small">
                   <span><i class="glyphicon glyphicon-search"></i></span>
               </div>--%>


       <%using (Html.BeginForm("Search", "Home", FormMethod.Get, new { @id = "formSearch", @class = "form-inline text-right search-block" })) {%>        
           
             
           <label for="search" class="text-uppercase text-white hidden-xs">want to Sell? consign NOW</label>
            <br>
            <div class="form-group">
                <div class="input-group">
                    <input type="search" class="form-control" id="Description" name="Description" placeholder="Search">
                    <div class="input-group-addon btn btn-default" onclick="javascript:$('#formSearch')[0].submit();"><span class="glyphicon glyphicon-search"></span></div>
                </div>
            </div>
            <%= Html.ActionLink("Advanced Search", "AdvancedSearch", "Home", new { @class="btn btn-primary search-button" }).ToNonSslLink()%>
       <%} %>
    <%}%>
