<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>

<% bool isDemo = false; // (ViewData["DEMO_MODE"] != null) && Convert.ToBoolean(ViewData["DEMO_MODE"]);

   if (isDemo) Response.Write("&nbsp;");
   else
   {
       SessionUser cuser = AppHelper.CurrentUser;
       if (cuser != null)
       {
           string UserTypeDescribed = "Seller & Buyer";
           switch (cuser.UserType)
           {
               case ((byte)Consts.UserTypes.Seller):
                   UserTypeDescribed = "Seller";
                   break;
               case ((byte)Consts.UserTypes.Admin):
                   UserTypeDescribed = "Admin";
                   break;
               case ((byte)Consts.UserTypes.Buyer):
                   UserTypeDescribed = "Buyer";
                   break;
               case ((byte)Consts.UserTypes.HouseBidder):
                   UserTypeDescribed = "HouseBidder";
                   break;
               case ((byte)Consts.UserTypes.Reviewer):
                   UserTypeDescribed = "Reviewer";
                   break;
           }
%>


<div class="container margin-top-xs">
	<div class="row">
		<div class="col-sm-6">
			<div class="row">
				<ol class="breadcrumb">
					<li>Welcome <b><%: cuser.Login %></b> !  You are logged in as <%= UserTypeDescribed %></li>
					 
				</ol>
			</div>
		</div>
		<div class="col-sm-6 hidden-xs">
		 <ul class="list-unstyled list-inline pull-right select-top">
		    <li>
			<a href="#">
			    <span class="fa-stack fa-lg">
				<%--<i class="fa fa-circle fa-stack-2x"></i>--%>
				<%= Html.ActionLink("Log Off", "LogOff", "Account").ToNonSslLink() %>
				<%-- <i class="fa fa-bookmark-o fa-stack-1x fa-inverse"></i>--%>
			    </span>
			</a>
		    </li>
		    <li>
			<a href="#">
			    <span class="fa-stack fa-lg">
				<%= Html.ActionLink("Watch list", "WatchBid", "Account").ToNonSslLink() %>
				<%-- <i class="fa fa-envelope fa-stack-1x fa-inverse"></i>--%>
			    </span>
			</a>
		    </li>

		    <li>
			<a href="#">
			    <span class="fa-stack fa-lg">
				<% if (cuser.IsSellerType)
				   { %>
				<%= Html.ActionLink("Seller Tools", "Index", "Consignor").ToNonSslLink() %>
				<% }
				   else
				   { %>
				<%= Html.ActionLink("My Account", "MyAccount", "Account").ToSslLink() %>
				<i class="fa fa-print fa-stack-1x fa-inverse"></i>
				<% } %>
			    </span>

			</a>
		    </li>
		</ul>
		</div>
	</div>
</div>
<% if (ViewData["IsHomePage"] != null && Convert.ToBoolean(ViewData["IsHomePage"]))
       {%>
<div class="precontent bg-color-second padding-bottom-xl margin-bottom-xl">
<div class="container">
    <div class="bg-color-third padding-vertical overflow-hidden box-precontaent">
        <div class="col-sm-4">
            <p class="text-uppercase think">thinking of selling?</p>
            <p class="text-uppercase consing">Consign now!</p>
        </div>
        <div class="col-sm-6">
            <p class="text-uppercase over">over $50 Million in fine art, coins, jewelry and Collectables sold online!</p>
        </div>
        <div class="col-sm-2">
            <a href="<%= Url.Action("ConsignNow", "Consignor") %>" class="btn btn-primary btn-lg text-uppercase margin-767-top">Consign Now</a>
        </div>
    </div>
</div>
</div>
<%}%>

<%--<div style="margin-left:2px;float:left;width:100%;background:url(/Zip/Image?path=/public/images/right_top_bg.jpg) repeat-y" class="topBlock" >
    <a class="linkTop" href="<%= Url.Action("ConsignNow", "Consignor") %>">
      <img style="margin:0" src='<%= AppHelper.CompressImage("consignnow.jpg") %>' alt='' />
     </a>
    
     <div class="top_menu_user">
        <p>Welcome <b><%: cuser.Login %></b>!</p>
    </div>  
    
     <div class="top_menu_user_menu">
  <ul class="reg"> 
         <li class="firstitem">
            <%= Html.ActionLink("Log Off", "LogOff", "Account").ToNonSslLink() %>            
         </li>
         <li> 
            <%= Html.ActionLink("Watch list", "WatchBid", "Account").ToNonSslLink() %>
        </li>
        <% if (cuser.IsSellerType)
           { %>   
    <li>  
     <%= Html.ActionLink("Seller Tools", "Index", "Consignor").ToNonSslLink() %>    
     </li>
     <% }
           else
           { %>
     <li>          
        <%= Html.ActionLink("My Account", "MyAccount", "Account").ToSslLink() %> 
    </li>
     <% } %>
    </ul>
    </div>
</div>--%>

<%--<div class="top_menu_full" style="-display:inline">
    <div class="top_menu_user">
        <p>Welcome <b><%: cuser.Login %></b>!</p>
    </div>        
    <div class="top_menu_user"><p>You are logged in as <%= UserTypeDescribed %></p></div>
    <div class="top_menu_user_menu">
  <ul class="reg"> 
         <li class="firstitem">
            <%= Html.ActionLink("Log Off", "LogOff", "Account").ToNonSslLink() %>            
         </li>
         <li> 
            <%= Html.ActionLink("Watch list", "WatchBid", "Account").ToNonSslLink() %>
        </li>
        <% if (cuser.IsSellerType)
           { %>   
    <li>  
     <%= Html.ActionLink("Seller Tools", "Index", "Consignor").ToNonSslLink() %>    
     </li>
     <% }
           else
           { %>
     <li>          
        <%= Html.ActionLink("My Account", "MyAccount", "Account").ToSslLink() %> 
    </li>
     <% } %>
    </ul>
    </div>
</div>    --%>
<%
       }
       else
       {
%> 
<% if (ViewData["IsHomePage"] != null && Convert.ToBoolean(ViewData["IsHomePage"]))
       {%>
<div class="precontent bg-color-second padding-bottom-xl margin-bottom-xl">
<div class="container">
    <div class="bg-color-third padding-vertical overflow-hidden box-precontaent">
        <div class="col-sm-4">
            <p class="text-uppercase think">thinking of selling?</p>
            <p class="text-uppercase consing">Consign now!</p>
        </div>
        <div class="col-sm-6">
            <p class="text-uppercase over">over $50 Million in fine art, coins, jewery and Collectables sold online!</p>
        </div>
        <div class="col-sm-2">
            <a href="<%= Url.Action("ConsignNow", "Consignor") %>" class="btn btn-primary btn-lg text-uppercase margin-767-top">Consign Now</a>
        </div>
    </div>
</div>
</div>
<%} %>
    <div style="margin-left:2px;float:left;width:100%;background:url(/Zip/Image?path=/public/images/right_top_bg.jpg) repeat-y" class="topBlock" >
         <%-- <a class="linkTop" href="">
            <img style="margin:0" src='<%= AppHelper.CompressImage("consignnow.jpg") %>' alt='' />
          </a>--%>
     
          <a href="<%= Url.Action("FreeEmailAlertsRegister", "Home") %>">
            <%--<img style="margin:0" src='<%= AppHelper.CompressImage("join.jpg") %>' alt='' />--%>
         </a>
             <div id="form_login" title="Log In" style="display:none" >              
              <% Html.RenderPartial("_LogOn"); %>
              <br />
           <span id="spCloseLogInForm" style="float:right;text-decoration:underline;font-size:12px;cursor:pointer;color:#518AAA">close</span>             
     </div>
   </div>
<% }
   } %>