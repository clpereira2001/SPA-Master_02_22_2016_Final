using System;
using System.Web;
using System.Web.Routing;
using System.Text;

namespace Vauction.Utils.Routes
{
  public class LegacyURLRoutes : RouteBase
  {
    public override RouteData GetRouteData(HttpContextBase httpContext)
    {
      const string status = "301 Moved Permanently";
      HttpRequestBase request = httpContext.Request;
      HttpResponseBase response = httpContext.Response;
      string legacyUrl = request.Url.ToString();
      string legacylower = legacyUrl.ToLower();
      string newUrl = String.Empty;

      if (legacylower.Contains("utm_source") && legacylower.Contains("utm_content") || legacylower.Contains("?gclid="))
      {
        newUrl = legacyUrl.Substring(0, legacyUrl.IndexOf("?"));
      } else 
      if (legacylower.Contains("category.neo") || legacylower.Contains("/aupm/notify.do"))
      {
        newUrl = "/";
      } else
      if (legacylower.Contains(AppHelper.GetSiteUrl("/?").ToLower()))
      {
        newUrl = "/Error/HttpError";
      } else
      if (legacylower.Contains("events.htm") || legacylower.Contains("top10.neo"))
      {
        newUrl = "/Event";
      } else
      if (legacylower.Contains("products.htm"))
      {
        newUrl = "/Home/Product";
      } else
      if (legacylower.Contains("faq.htm"))
      {
        newUrl = "/Home/FAQs";
      } else
      if (legacylower.Contains("contact.htm"))
      {
        newUrl = "/Home/ContactUs";
      } else
        if (legacylower.Contains("/viewitem.neo") || legacylower.Contains("/featured.neo") || legacylower.Contains("/viewevent.neo"))
      {
        //Regex regex = new Regex(@"[0-9]{1,3}$", RegexOptions.Singleline);
        //Match match = regex.Match(legacyUrl);
        //EventRepository ev = new EventRepository(new VauctionDataContext(), AppHelper.CacheDataProvider);
        //if (match.Success && ev!=null)
        //{ 
        //  Event e = ev.GetEventByID(Convert.ToInt64(match.Value)) as Event;
        //  newUrl = (e != null && match.Success) ? "/Auction/EventDetailed/" + match.Value + "/" + e.UrlTitle : "/Event";
        //}
        //else
          newUrl = "/Event";
      } else
      //if (legacylower.Contains("/featured.neo"))
      //{
      //  Regex regex = new Regex(@"[0-9]{1,3}$", RegexOptions.Singleline);
      //  Match match = regex.Match(legacyUrl);
      //  EventRepository ev = new EventRepository(new VauctionDataContext(), AppHelper.CacheDataProvider);
      //  if (match.Success && ev!=null)
      //  {
      //    Event e = ev.GetEventByID(Convert.ToInt64(match.Value)) as Event;          
      //    newUrl = (e!=null && match.Success) ? "/Auction/FeaturedItems/" + match.Value + "/" + e.UrlTitle : "/Event";
      //  }
      //  else 
      //    newUrl = "/Event";
      //} else
      if (legacylower.Contains("/neo/neo.exe/payhere"))
      {
        newUrl = "/Account/PayForItems";
      } else
      if (legacylower.Contains("/neo/neo.exe/searchauctions"))
      {
        newUrl = "/Home/AdvancedSearch";
      } else
        if (legacylower.Contains("/neo/neo.exe/services") || legacylower.Contains("/neo/neo.exe/viewaccount") || legacylower.Contains("/neo/neo.exe/prepareitem"))
      {
        newUrl = "/Account/MyAccount";
      } else      
      if (legacylower.Contains("/neo/neo.exe/buyerhistory"))
      {
        newUrl = "/Account/PastAuction";
      } else
      if (legacylower.Contains("/neo/neo.exe/sendshoppers"))
      {
        newUrl = "/Account/ReceivePersonalShopperUpdate";
      }
      else
      if (legacylower.Contains("/neo/neo.exe/editshopper"))
      {
        newUrl = "/Account/EditPersonalShopper";
      } else      
      if (legacylower.Contains("/neo/neo.exe/register") || legacylower.Contains("/register.htm"))
      {
        newUrl = "/Account/Register";
      } else
      if (legacylower.Contains("/terms.htm"))
      {
        newUrl = "/Home/Terms";
      } else
      if (legacylower.Contains("/privacy.htm"))
      {
        newUrl = "/Home/Privacy";
      } else      
      if (legacylower.Contains("bidwatch.neo"))
      {
        newUrl = "/Account/WatchBid";
      } else
      if (legacylower.Contains("/neo/neo.exe/editlist"))
      {
        newUrl = "/Consignor/EditAuction";
      } else
        if (legacylower.Contains("/neo/neo.dll/resendconfirmation"))
      {
        newUrl = "/Account/ResendConfirmationCode";
      } else
      if (legacylower.Contains("/neo/neo.exe/listauctions") || legacylower.Contains("/neo/neo.exe/closeauctions") || legacylower.Contains("/neo/neo.exe/relist") || legacylower.Contains("/neo/neo.exe/quickrelist") || legacylower.Contains("/neo/neo.exe/stats") || legacylower.Contains("/neo/neo.exe/results") || legacylower.Contains("/neo/neo.exe/senditemupdates"))
      {
        newUrl = "/Consignor";
      } else      
      if (legacylower.Contains("/neo/neo.exe/edit"))
      {
        newUrl = "/Account/Profile";
      } else 
      if (legacylower.Contains("/neo/neo.exe") || legacylower.Contains("/neo/neo.dll"))
      {
        newUrl = "/Account/MyAccount";
      } else
      if (legacylower.Contains("/emailtemplate") && !legacylower.Contains("public/"))
      {
        newUrl = legacylower.Replace("/emailtemplate", "/public/emailtemplate");
      } else
      if (legacylower.Contains("/product/index"))
      {
        newUrl = "/Home/Product";
      } else
       if (legacylower.Contains("/home/emailalertsunsubscribesuccess?id=%3C%=[ID]%%3E&t=%3C%=[T]%%3E") || legacylower.Contains("/home/emailalertsunsubscribesuccess?id=<%=[ID]%>&t=<%=[T]%>") || legacylower.Contains("/home/emailalertsunsubscribesuccess?id=<%25=[ID]%25>&t=<%25=[T]%25>"))
      {
        newUrl = "/Home/FreeEmailAlertsRegister";
      } else
      if (legacylower.Contains("/images/users/"))
      {
        try
        {
          StringBuilder sb = new StringBuilder(legacylower);
          sb.Remove(0, legacyUrl.LastIndexOf("/") + 1);
          string filename = sb.ToString();
          int index = sb.ToString().IndexOf("-");
          if (index > 0)
          {
            sb.Remove(index, sb.Length - index);
            long id;
            newUrl = (Int64.TryParse(sb.ToString(), out id)) ? "/public/AuctionImages/" + Consts.GetImagePath(id, true) + filename : "/Error/HttpError";
          }
          else newUrl = "/Home/Index";
        }
        catch
        {
          newUrl = "/Error/HttpError";
        }
      } else
      if (legacylower.Contains("/images/") && !legacylower.Contains("/zip/image?"))
      {
        newUrl = "/Error/HttpError";
      }

      if (!String.IsNullOrEmpty(newUrl))
      {
        response.Status = status;
        response.RedirectLocation = newUrl;
        response.End();
      }

      return null;
    }

    public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
    {
      return null;
    }
  }
}