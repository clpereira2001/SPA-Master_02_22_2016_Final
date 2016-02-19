function AddBanner_http(zoneid) { <!--//<![CDATA[    
  if (!document.MAX_used) document.MAX_used = ',';
  document.write("<script type='text/javascript' src='http://d1.openx.org/ajs.php");
  document.write("?zoneid=" + zoneid);
  document.write('&amp;cb=' + Math.floor(Math.random() * 99999999999));
  if (document.MAX_used != ',') document.write("&amp;exclude=" + document.MAX_used);
  document.write(document.charset ? '&amp;charset=' + document.charset : (document.characterSet ? '&amp;charset=' + document.characterSet : ''));
  document.write("&amp;loc=" + escape(window.location));
  if (document.referrer) document.write("&amp;referer=" + escape(document.referrer));
  if (document.context) document.write("&context=" + escape(document.context));
  if (document.mmm_fo) document.write("&amp;mmm_fo=1");
  document.write("'><\/script>");//]]>-->  
}


function InitBanner_http(ad_slot, width, height){
  google_ad_client = "ca-pub-7260697468154574";
  google_ad_slot = ad_slot;
  google_ad_width = width;
  google_ad_height = height;  
}