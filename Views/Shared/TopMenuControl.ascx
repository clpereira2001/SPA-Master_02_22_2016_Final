<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<%@ Import Namespace="Vauction.Models.Enums" %>
<%
    Auction currentAuction = ViewData["CurrentAuction"] as Auction;
    string returnUrl = string.Empty;
    if (currentAuction != null)
    {
        returnUrl = Url.Action("AuctionDetail", "Auction", new { id = currentAuction.ID });
    }
    bool isDemo = false; // (ViewData["DEMO_MODE"] != null) ? Convert.ToBoolean(ViewData["DEMO_MODE"]) : false;
    SessionUser cuser = AppHelper.CurrentUser;
    List<CategoryParentChild> lstCategoryParentChild = null;
    if (Session["lnkCategoryParentChild"] != null)
    {
        // List<CategoryParentChild> lstCategoryParentChild = new List<CategoryParentChild>();
        // lstCategoryParentChild = List<CategoryParentChild>(Session["lstCategoryParentChild"]);
        lstCategoryParentChild = (System.Collections.Generic.List<Vauction.Models.Enums.CategoryParentChild>)(Session["lnkCategoryParentChild"]);
        // Session["lstCategoryParentChild"] = null;
    }
    
%>


<div class="navbar menu-left-click">
    <ul>
        <li>
            <a><i class="fa fa-bars"></i></a>
        </li>
    </ul>

</div>
<div id="sidebar">
    <nav id="nav">
        <%--  <a href="javascript:void(0);" class="toggle-menu open-menu fa fa-bars">&nbsp;</a>
    <a href="javascript:void(0);" class="title-menu-toggle text-uppercase open-menu">Menu</a>--%>
        <div class="container">
            <ul class="nav navbar-nav main-menu">
                <li>
                    <%=(isDemo) ? "<a href='#' class='dropdown-toggle' data-toggle='dropdown'>Home</a>" : Html.ActionLink("Home", "Index", "Home").ToNonSslLink()%> 
                </li>
                <li class="dropdown grid-menu">
                    <%=(isDemo) ? "<a href='#' class='dropdown-toggle' data-toggle='dropdown'>Auction Events</a>" : Html.ActionLink("Auction Events", "Index", "Event").ToNonSslLink()%>


                    <% if (lstCategoryParentChild != null && lstCategoryParentChild.Count > 0)
                       {%>
                    <div class="dropdown-menu col-sm-12 padding-vertical-lg">

                        <% for (int i = 0; i < lstCategoryParentChild.Count; i++)
                           {%>
                        <div class="col-sm-5">
                            <div class="row">


                                <div class="col-sm-6">
                                    <ul class="list-unstyled">
                                        <li class="title-category"><a href="<%= lstCategoryParentChild[i].parentLinkUrl %>"><%= lstCategoryParentChild[i].parentLinkName %> </a></li>

                                        <% if (lstCategoryParentChild[i].Childs != null && lstCategoryParentChild[i].Childs.Count > 0)
                                           {%>
                                        <%for (int j = 0; j < lstCategoryParentChild[i].Childs.Count; j++)
                                          {%>

                                        <li><a href="<%=lstCategoryParentChild[i].Childs[j].ChildLinkUrl%>"><%= lstCategoryParentChild[i].Childs[j].ChildLinkName  %> </a></li>
                                        <%} %>
                                        <%} %>
                                    </ul>
                                </div>
                                <div class="col-sm-6">
                                    <ul class="list-unstyled">
                                        <li></li>
                                        <% if (lstCategoryParentChild[i].Childs != null && lstCategoryParentChild[i].Childs.Count > 0)
                                           {%>
                                        <%for (int j = 13; j < lstCategoryParentChild[i].Childs.Count; j++)
                                          {%>

                                        <li><a href="<%=lstCategoryParentChild[i].Childs[j].ChildLinkUrl%>"><%= lstCategoryParentChild[i].Childs[j].ChildLinkName  %> </a></li>
                                        <%} %>
                                        <%} %>
                                    </ul>
                                </div>
                            </div>
                        </div>
                        <%} %>

                        <div class="col-sm-7">
                            <div class="row">
                                <div class="col-sm-4">
                                    <ul class="list-unstyled">
                                        <% if (lstCategoryParentChild != null)
                                           {%>
                                        <% for (int i = 2; i < lstCategoryParentChild.Count; i++)
                                           {%>
                                        <%-- <li class="title-category"><a href="#">ARTIFACTS</a></li>--%>
                                        <li class="title-category"><a href="<%= lstCategoryParentChild[i].parentLinkUrl %>"><%= lstCategoryParentChild[i].parentLinkName %> </a></li>
                                        <% if (lstCategoryParentChild[i].Childs != null && lstCategoryParentChild[i].Childs.Count > 0)
                                           {%>
                                        <%for (int j = 0; j < lstCategoryParentChild[i].Childs.Count; j++)
                                          {%>

                                        <li><a href="<%=lstCategoryParentChild[i].Childs[j].ChildLinkUrl%>"><%= lstCategoryParentChild[i].Childs[j].ChildLinkName  %> </a></li>
                                        <%} %>
                                        <%} %>
                                        <%} %>
                                        <%} %>
                                    </ul>
                                </div>


                                <div class="col-sm-4">
                                    <ul class="list-unstyled">
                                        <% if (lstCategoryParentChild != null)
                                           {%>
                                        <% for (int i = 6; i < lstCategoryParentChild.Count; i++)
                                           {%>
                                        <%-- <li class="title-category"><a href="#">ARTIFACTS</a></li>--%>
                                        <li class="title-category"><a href="<%= lstCategoryParentChild[i].parentLinkUrl %>"><%= lstCategoryParentChild[i].parentLinkName %> </a></li>
                                        <% if (lstCategoryParentChild[i].Childs != null && lstCategoryParentChild[i].Childs.Count > 0)
                                           {%>
                                        <%for (int j = 0; j < lstCategoryParentChild[i].Childs.Count; j++)
                                          {%>
                                        <%if (i == 8)
                                          {%>
                                        <%if (j < lstCategoryParentChild[i].Childs.Count)
                                          {%>
                                        <li><a href="<%=lstCategoryParentChild[i].Childs[j].ChildLinkUrl%>"><%= lstCategoryParentChild[i].Childs[j].ChildLinkName  %></a></li>
                                        <%} %>
                                        <%} %>
                                        <%else
                                          {%>
                                        <li><a href="<%=lstCategoryParentChild[i].Childs[j].ChildLinkUrl%>"><%= lstCategoryParentChild[i].Childs[j].ChildLinkName  %> </a></li>
                                        <%} %>
                                        <%} %>
                                        <%} %>
                                        <%} %>
                                        <%} %>
                                        <%-- <li class="title-category"><a href="#">CRYSTAL &amp; GLASS FIGURINES</a></li>
                                    <li class="title-category"><a href="#">CRYSTAL / GLASSWARE</a></li>
                                    <li class="title-category"><a href="#">ELECTRONICS / TOOLS</a></li>
                                    <li class="title-category"><a href="#">HUMMELS</a></li>
                                    <li class="title-category"><a href="#">JEWELRY</a></li>
                                    <li><a href="#">Contents of Safe Deposit Boxes</a></li>
                                    <li><a href="#">Dealer Closeouts</a></li>
                                    <li><a href="#">Diamonds</a></li>
                                    <li><a href="#">Earrings</a></li>
                                    <li><a href="#">Emeralds</a></li>
                                    <li><a href="#">Gemstones</a></li>
                                    <li><a href="#">Gold</a></li>
                                    <li><a href="#">Mens</a></li>
                                    <li><a href="#">Rubies</a></li>
                                    <li><a href="#">Sapphires</a></li>--%>
                                    </ul>
                                </div>
                                <div class="col-sm-4">
                                    <ul class="list-unstyled">
                                        <% if (lstCategoryParentChild != null)
                                           {%>
                                        <% for (int i = 8; i < lstCategoryParentChild.Count; i++)
                                           {%>

                                        <% if (i != 8)
                                           {%>
                                        <li class="title-category"><a href="<%= lstCategoryParentChild[i].parentLinkUrl %>"><%= lstCategoryParentChild[i].parentLinkName %> </a></li>
                                        <%} %>
                                        <% if (lstCategoryParentChild[i].Childs != null && lstCategoryParentChild[i].Childs.Count > 0)
                                           {%>
                                        <%for (int j = 0; j < lstCategoryParentChild[i].Childs.Count; j++)
                                          {%>

                                        <% if (i == 8)
                                           {%>
                                        <%if (j > 10)
                                          {%>
                                        <li><a href="<%=lstCategoryParentChild[i].Childs[j].ChildLinkUrl%>"><%= lstCategoryParentChild[i].Childs[j].ChildLinkName  %> </a></li>
                                        <%} %>
                                        <%} %>

                                        <%--<li><a href="<%=lstCategoryParentChild[i].Childs[j].ChildLinkUrl%>"><%= lstCategoryParentChild[i].Childs[j].ChildLinkName  %> </a></li>--%>
                                        <%} %>
                                        <%} %>
                                        <%} %>
                                        <%} %>
                                        <%--<li><a href="#">Silver</a></li>
                                    <li><a href="#">Vintage</a></li>
                                    <li><a href="#">Watches</a></li>
                                    <li class="title-category"><a href="#">JUDAICA</a></li>
                                    <li class="title-category"><a href="#">MUSICAL INSTRUMENTS</a></li>
                                    <li class="title-category"><a href="#">OTHER</a></li>
                                    <li class="title-category"><a href="#">PORCELAIN</a></li>
                                    <li class="title-category"><a href="#">RUGS</a></li>
                                    <li class="title-category"><a href="#">SIGNED COLLECTIBLES</a></li>
                                    <li class="title-category"><a href="#">SPORTS MEMORABILIA</a></li>
                                    <li class="title-category"><a href="#">VINTAGE ITEMS</a></li>
                                    <li class="title-category"><a href="#">WHOLESALE CLOSEOUTS</a></li>--%>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>

                    <%} %>
                </li>
                <li>
                    <%=(isDemo) ? "<a href='#'>Consigner Services</a>" : Html.ActionLink("Consigner Services", "Product", "Home").ToNonSslLink()%> 
                </li>
                <li>
                    <%=(isDemo) ? "<a href='#'>FAQs</a>" : Html.ActionLink("FAQs", "FAQs", "Home").ToNonSslLink()%> 
                </li>
                <li>
                    <%=(isDemo) ? "<a href='#'>Resource Center</a>" : Html.ActionLink("Resource Center", "ResourceCenter", "Home").ToNonSslLink() %> 
                </li>
                <li>
                    <%=(isDemo) ? "<a href='#'>Contact Us</a>" : Html.ActionLink("Contact Us", "ContactUs", "Home").ToNonSslLink()%> 
                </li>
                <% if (Request.IsAuthenticated && cuser != null)
                   { %>
                <% if (cuser.UserType == (byte)Consts.UserTypes.Seller)
                   {%>
                <li>
                    <%=(isDemo) ? "<a href='#'>Seller Tools</a>" : Html.ActionLink("Seller Tools", "Index", "Consignor").ToSslLink()%>
                </li>
                <% }
                   else
                   {%>
                <li>
                    <%=(isDemo) ? "<a href='#'>My Account</a>" : Html.ActionLink("My Account", "MyAccount", "Account").ToSslLink()%> 
                </li>
                <% }%>
                <li>
                    <%=(isDemo) ? "<a href='#'>Profile</a>" : Html.ActionLink("Profile", "Profile", "Account").ToSslLink()%> 
                </li>
                <% }
                   else
                   { %>
                <%--            <li class="firstitem" style="margin-left: 160px">--%>
                <li>
                    <%=(isDemo) ? "<a href='#'>Register</a>" : Html.ActionLink("Register", "Register", "Account").ToSslLink()%>
                </li>
                <li>
                    <%=(isDemo) ? "<a href='#'>Log in</a>" :  Html.ActionLink("Log in", "LogOn", "Account", new { @ReturnUrl = returnUrl }, null).ToSslLink()%> 
                </li>
                <% } %>
            </ul>
        </div>
    </nav>

</div>



<script src="http://code.jquery.com/jquery-latest.min.js"
    type="text/javascript"></script>

<%--<script src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.12.0.min.js"></script>--%>
<script>

    $(function () {
        $('.menu-left-click').on('click', function () {
            $('#sidebar').toggle('slide', { direction: 'left' }, 1000);
            //$('#main-content').animate({
            //    'margin-left': $('#main-content').css('margin-left') == '0px' ? '210px' : '0px'
            //}, 1000);
        });



    });

    $(function () {
        $('.categories-click-menu').on('click', function () {
            $('.categories-menu-open').toggle('slide', { direction: 'left' }, 1000);

        });
    });

    $(function () {
        $('.search-small').on('click', function () {
            $('#formSearch').toggle('slide', { direction: 'left' }, 1000);

        });
    });


    $(document).ready(function () {

        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {       
            $("#pagecontentall").click(function (e) {
            e.preventDefault();
            if ($("#sidebar").is(":visible")) {
                $("#sidebar").toggle('slide', { direction: 'left' }, 1000);
            }
        });      
        }
    });



</script>
