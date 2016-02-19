<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<link rel="stylesheet" href="/public/css/bootstrap.css" />
<link rel="stylesheet" href="/public/css/hover.css" />
<link rel="stylesheet" href="/public/css/base-class.css" />
<link rel="stylesheet" href="/public/css/styles.css" />
<%--<div class="container margin-top-xs">
	<div class="row">
		<div class="col-sm-6">
			<div class="row">
				<ol class="breadcrumb">
					<li><a href="index.html">Home</a></li>
					<li class="active">Login </li>
				</ol>
			</div>
		</div>
		<div class="col-sm-6 hidden-xs">
			<ul class="list-unstyled list-inline pull-right select-top">
				<li>
					<a href="#">
						<span class="fa-stack fa-lg">
							<i class="fa fa-circle fa-stack-2x"></i>
							<i class="fa fa-bookmark-o fa-stack-1x fa-inverse"></i>
						</span>
					</a>
				</li>
				<li>
					<a href="#">
						<span class="fa-stack fa-lg">
							<i class="fa fa-circle fa-stack-2x"></i>
							<i class="fa fa-envelope fa-stack-1x fa-inverse"></i>
						</span>
					</a>
				</li>
				<li>
					<a href="#">
						<span class="fa-stack fa-lg">
							<i class="fa fa-circle fa-stack-2x"></i>
							<i class="fa fa-print fa-stack-1x fa-inverse"></i>
						</span>
					</a>
				</li>
			</ul>
		</div>
	</div>
</div>--%>

<%--<div class="row">
        <div class="col-sm-12">
            <p class="text-primary text-weight-900 text-lg border-bottom border-weight-xs border-primary">
                Login
				<a href="#" class="btn btn-link pull-right margin-top-xs text-muted margin-767-top-none">Back to Item <i class="fa fa-caret-square-o-right"></i></a>
            </p>
        </div>
    </div>--%>

<div class="row">
    <%--<div class="col-sm-12">--%>
    <div class="row-sm-height">
        <div class="col-sm-6 col-sm-height col-sm-top padding-top-lg">
            <div class="bg-white full-height border border-weight-xs border-primary col-sm-12">
                <p class="text-uppercase margin-top text-primary text-weight-700 text-md">existing customers</p>
                <p class="text-primary">I am a Returning Customer</p>
                <%
                    bool isLocked = ViewData["IsLocked"] != null;
                    bool notConfirmed = ViewData["notConfirmed"] != null;
                    bool wrongPass = ViewData["wrongPass"] != null;
    %>

                <form name="frmLogOn" method="post" action="<%=Consts.ProtocolSitePort %>/Account/LogOn">
                    <%=Html.AntiForgeryToken() %>
                    <%=Html.Hidden("returnUrl", ViewData["AuctionDetailReturnUrl"] == null ? Request["returnUrl"] ?? String.Empty : ViewData["AuctionDetailReturnUrl"] as string)%>

                    <div class="form-group">
                        <label class="text-uppercase" for="username">User ID:</label><em>*</em>
                        <input type="text" class="form-control" id="login" name="login">
                    </div>
                    <div class="form-group">
                        <label class="text-uppercase" for="password">Password</label><em>*</em>
                        <input type="password" class="form-control" id="password" name="password">
                    </div>
                    <p class="red">
                        <em>
                            <% if (isLocked)
                               {%>
        Your account with
       
                <%= ConfigurationManager.AppSettings["siteName"]%>
        has been locked do to a violation in our site policy. If you feel this was in error
        please contact
       
                <a href="mailto:<%= ConfigurationManager.AppSettings["siteEmail"]%>"><%= ConfigurationManager.AppSettings["siteEmail"]%></a>
                            <%}
                               else
                                   if (notConfirmed)
                                   {%>
        Your account is pending. Please check your email for your confirmation link.
       
                <%}
                                       else if (wrongPass)
                                       {%>
        Wrong User ID or password
       
                <%}
        %>
      </em>
                    </p>
                    <a href="ForgotPassword/Account" class="text-dark">Forgot Password? Click Here
                    </a>

                    <div class="text-center padding-top margin-bottom">
                        <button id="btnSignIn" name="SignIn" type="submit" class="text-uppercase btn btn-danger btn-lg padding-horizontal-xl" onclick="submit();">sign in</button>


                    </div>
                </form>
            </div>
        </div>
        <div class="col-sm-6 col-sm-height col-sm-top padding-top-lg">
            <div class="bg-white full-height border border-weight-xs border-primary col-sm-12">
                <p class="text-uppercase margin-top text-primary text-weight-700 text-md">NEW CUSTOMERS</p>
                <p class="text-primary">I am a New Customer</p>
                <p>By creating an account at SPA you will be able to shop faster, be up to date on an orders status, and keep track of the orders you have previously placed.</p>
                <div class="text-center padding-top-xl margin-bottom">
                    <form name="frmRegister" method="post" action="<%=Consts.ProtocolSitePort %>/Account/Register">
<%--                        <a href="<%=Consts.ProtocolSitePort %>/Account/Register" class="text-uppercase btn btn-danger btn-lg padding-horizontal-xl" onclick="$(this).closest('form').submit()">COntinue</a>--%>
                        <button id="btnRegister" name="Register" type="submit" class="text-uppercase btn btn-danger btn-lg padding-horizontal-xl" onclick="submit();">continue</button>
                    </form>
                </div>
            </div>
        </div>


    </div>
    <%--</div>--%>
</div>





