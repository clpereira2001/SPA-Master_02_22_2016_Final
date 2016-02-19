<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div title="Please update your browser" style="background-color:#EEE;padding:5px">
    <div class="alert alert-danger">
    You are using Internet Explorer &lt; 9. Please update your browser to increase safety and your browsing experience. Choose one of the following links to download a modern browser:</div>
    <table>
      <tr>
        <td colspan="2"><a href="http://www.mozilla.com/firefox/" title="Firefox" class="a_title">Firefox</a></td>
        <td style="width:20px" rowspan="4">&nbsp;</td>
        <td colspan="2"><a href="http://www.opera.com/" title="Opera" class="a_title">Opera</a></td>
      </tr>
      <tr>
        <td>
          <img src="<%=AppHelper.CompressImage("firefox_64x64.png") %>" />
        </td>
        <td style="text-align:justify">
            "The Web is all about innovation, and Firefox sets the pace with dozens of new features, including the smart location bar, one-click bookmarking and blindingly fast performance." - <a href="http://www.mozilla.com/firefox/features/" class="a_feature" title="Firefox's features">Firefox's features</a>
        </td>
        <td>
          <img src="<%=AppHelper.CompressImage("opera_64x64.png") %>" />
        </td>
        <td  style="text-align:justify">
           "Opera is the only browser that comes with everything you need to be productive, safe and speedy online. Learn everything Opera can do for you..." - <a href="http://www.opera.com/discover/browser/" class="a_feature" title="Discover Opera!">Discover Opera!</a>
        </td>
      </tr>
      
      <tr>
        <td colspan="2"><a href="http://www.apple.com/safari/" title="Safari" class="a_title">Safari</a></td>
        <td colspan="2"><a href="http://www.google.com/chrome" title="Chrome" class="a_title">Chrome</a></td>
      </tr>
      <tr>
        <td>
          <img src="<%=AppHelper.CompressImage("safari_64x64.png") %>" />          
        </td>
        <td  style="text-align:justify">
            "The fastest, easiest-to-use web browser in the world. With its simple, elegant interface, Safari gets out of your way and lets you enjoy the web faster than any browser." - <a href="http://www.apple.com/safari/" class="a_feature" title="Safari's features">Safari's features</a>
        </td>
        <td>
          <img src="<%=AppHelper.CompressImage("chrome_64x64.png") %>" />
        </td>
        <td  style="text-align:justify">
          "Google Chrome is a browser that combines a minimal design with sophisticated technology to make the web faster, safer, and easier." - <a href="http://www.google.com/chrome/intl/en/features.html" class="a_feature" title="Google Chrome's features">Google Chrome's features</a>
        </td>
      </tr>
    </table>
</div>