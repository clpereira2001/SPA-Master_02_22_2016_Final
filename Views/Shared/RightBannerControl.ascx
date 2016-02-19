<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<br />
<% decimal version; 
   bool isIE = Decimal.TryParse(Request.Browser.Version, out version) && Request.Browser.Browser == "IE";
   bool showAds = ViewData["ShowAds"] != null && Convert.ToBoolean(ViewData["ShowAds"]);
 %>
<% if (showAds && Request.Url.Scheme == "http" && !(isIE && version < 7))
   {%>
  <%--<script type="text/javascript">
    google_ad_client = "ca-pub-7260697468154574";
    google_ad_slot = "5622952757";
    google_ad_width = 160;
    google_ad_height = 600;
  </script>
  <script type="text/javascript" src="http://pagead2.googlesyndication.com/pagead/show_ads.js"></script>
  <script type="text/javascript">
    google_ad_client = "ca-pub-7260697468154574";
    google_ad_slot = "5380019078";
    google_ad_width = 160;
    google_ad_height = 600;
  </script>
  <script type="text/javascript" src="http://pagead2.googlesyndication.com/pagead/show_ads.js"></script> --%>
  
  <script async src="//pagead2.googlesyndication.com/pagead/js/adsbygoogle.js"></script>
  <ins class="adsbygoogle"
     style="display:inline-block;width:160px;height:600px"
     data-ad-client="ca-pub-7260697468154574"
     data-ad-slot="5622952757"></ins>
  <script>
    (adsbygoogle = window.adsbygoogle || []).push({});
  </script>
  <script type="text/javascript">
    google_ad_client = "ca-pub-7260697468154574";
    google_ad_slot = "5380019078";
    google_ad_width = 160;
    google_ad_height = 600;
  </script>
  <script type="text/javascript" src="//pagead2.googlesyndication.com/pagead/show_ads.js"></script>
<%} else { %>
<%} %>
  <% if(ViewData["LelandsBanner"]!=null && Convert.ToBoolean(ViewData["LelandsBanner"])){
       SessionUser cuser = AppHelper.CurrentUser; %>
  <a href="http://www.lelands.com/?trng=spa&u=<%=cuser==null?-1:cuser.ID %>" title="Lelands.com ~ Sports, Americana, Rock'n'Roll & Vintage Photography Auction Going On Now!"><img src="<%=AppHelper.CompressImage("Lelands_spa.jpg") %>" alt="" /></a>
  <%} %>