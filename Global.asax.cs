using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Vauction.Utils.Lib;
using Vauction.Controllers;
using System.Web.Security;
using Vauction.Utils.Autorization;
using Vauction.Utils;
using Vauction.Utils.Perfomance;
using Vauction.Utils.Routes;

namespace Vauction
{
  public class MvcApplication : HttpApplication
  {
    private static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.IgnoreRoute("crossdomain.xml");
      routes.Add(new LegacyURLRoutes());
      routes.MapRoute("lot", "{controller}/{action}/{id}/{evnt}/{cat}/{lot}/", "Vauction", new { });
      routes.MapRoute("category", "{controller}/{action}/{id}/{evnt}/{cat}", "Vauction", new { });
      routes.MapRoute("event", "{controller}/{action}/{id}/{evnt}", "Vauction", new { });
      routes.MapRoute("url_id", "{controller}/{action}/{id}/", "Vauction", new { });
      routes.MapRoute("action_only", "{controller}/{action}/", "Vauction", new { });
      routes.MapRoute("Default.aspx", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = "" });
    }

   //Application_Start
    protected void Application_Start()
    {
      System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
      System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
      RegisterRoutes(RouteTable.Routes);

      if (Consts.DataCachingTechnology == DataCacheTechnology.MEMORYOBJECT)
      {
        AppHelper.CacheDataProvider = new CacheDataProvider();
        Logger.LogInfo("Init: Memory Cache");
      }
      else
      {
        try
        {
          AppHelper.CacheDataProvider = new AppFabricCacheProviderSystemRegions(Consts.ProductName);
          //AppHelper.CacheDataProvider = new AFCDataProvider(Consts.ProductName);
          //Dictionary<string, List<string>> regions = new Dictionary<string, List<string>>();
          //List<string> reg = new List<string>() { "COUNTRIES", "STATES", "CATEGORIES", "EVENTS" };
          //regions.Add(DataCacheType.REFERENCE.ToString(), reg);
          //reg = new List<string>() { "AUCTIONS", "IMAGES", "AUCTIONLISTS" };
          //regions.Add(DataCacheType.RESOURCE.ToString(), reg);
          //reg = new List<string>() { "USERS", "WATCHLISTS", "EVENTREGISTRATIONS", "BIDS", "INVOICES" };
          //regions.Add(DataCacheType.ACTIVITY.ToString(), reg);
          //(AppHelper.CacheDataProvider as AFCDataProvider).InitDataCacheRegions(regions);
          Logger.LogInfo("Init: AppFabric Cache");
        }
        catch
        {
          AppHelper.CacheDataProvider = new CacheDataProvider();
          Logger.LogInfo("Init: Memory Cache");
        }
      }

      Logger.LogInfo("Application starting");
    }

    // Application_AuthenticateRequest
    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {
      //decimal version; bool isIE_6 = Decimal.TryParse(Request.Browser.Version, out version) && version<7 && Request.Browser.Browser == "IE";
      //if (!isIE_6 && (Context.Request.AcceptTypes != null && Context.Request.AcceptTypes.Any() && Context.Request.AcceptTypes[0] != "text/html" && Context.Request.AcceptTypes[0] != "application/json" && Context.Request.AcceptTypes[0] != "application/xml" && Context.Request.AcceptTypes[0] != "text/*" && Context.Request.AcceptTypes[0] != "*/*" && Context.Request.AcceptTypes[0] != "application/x-ms-application")) return;

      if (Request.Browser.Browser != "IE" && (Context.Request.AcceptTypes != null && Context.Request.AcceptTypes.Any() && Context.Request.AcceptTypes[0] != "text/html" && Context.Request.AcceptTypes[0] != "application/json" && Context.Request.AcceptTypes[0] != "application/xml" && Context.Request.AcceptTypes[0] != "text/*" && Context.Request.AcceptTypes[0] != "*/*" && Context.Request.AcceptTypes[0] != "application/x-ms-application")) return;

      if (HttpContext.Current.User != null && Request.IsAuthenticated)
      {
        HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
        if (authCookie == null || String.IsNullOrEmpty(authCookie.Value)) return;
        FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
        if (ticket != null)
          Context.User = new VauctionPrincipal(new VauctionIdentity(ticket.Name, ticket.UserData));
      }
    }

    //GetVaryByCustomString
    //public override string GetVaryByCustomString(HttpContext context, string custom)
    //{
    //  return (custom != "CurrentUser") ? base.GetVaryByCustomString(context, custom) : ((context.User != null && context.User.Identity.IsAuthenticated) ? context.User.Identity.Name : "none");
    //}

    // Application_Error
    protected void Application_Error(object sender, EventArgs e)
    {
      SessionUser cuser = AppHelper.CurrentUser;
      Exception exception = Server.GetLastError();
      string adds = "[" + Request.Path + " | " + Consts.UsersIPAddress + " | " + (cuser != null ? cuser.Login : String.Empty) + " | " + (Request.Browser != null ? Request.Browser.Browser + "-" + Request.Browser.Version : String.Empty) + " | " + (Request.UrlReferrer != null ? Request.UrlReferrer.AbsoluteUri : String.Empty) + "]" + ((exception is HttpUnhandledException) ? "\n >> Inner: " + (exception as HttpUnhandledException).InnerException.Message : String.Empty);
      Logger.LogException(adds, exception);

      Response.Clear();

      RouteData routeData = new RouteData();
      if (exception is HttpAntiForgeryException)
      {
        Server.ClearError();
        Response.Redirect("/");
        return;
      }

      routeData.Values.Add("controller", "Error");
      HttpException httpException = exception as HttpException;
      if (httpException == null)
        routeData.Values.Add("action", "General");
      else //It's an Http Exception, Let's handle it.
      {
        switch (httpException.GetHttpCode())
        {
          case 404:
            // Page not found.
            routeData.Values.Add("action", "HttpError404");
            break;
          case 500:
            // Server error.
            routeData.Values.Add("action", "HttpError500");
            break;

          // Here you can handle Views to other error codes.
          // I choose a General error template  
          default:
            routeData.Values.Add("action", "General");
            break;
        }
      }

      // Pass exception details to the target error View.
      routeData.Values.Add("error", exception);

      // Clear the error on server.
      Server.ClearError();
      try
      {
        // Call target Controller and pass the routeData.
        IController errorController = new ErrorController();
        errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }
    }

    //Session_Start
    protected void Session_Start(object sender, EventArgs e)
    {
      HttpContext.Current.Session.Add("__Application", "App");
    }

    //Session_Start
    protected void Session_End(object sender, EventArgs e)
    {
    }

    // Application_BeginRequest
    //protected void Application_BeginRequest(object sender, EventArgs e)
    //{ 
    //  //Stopwatch stopwatch = new Stopwatch();
    //  //try
    //  //{
    //  //  stopwatch.Start();
    //  //  Context.Items.Add(Context.Request.Cookies["ASP.NET_SessionId"].Value, stopwatch);
    //  //}
    //  //catch
    //  //{
    //  //  stopwatch.Stop();
    //  //}
    //}

    //Application_EndRequest
    //protected void Application_EndRequest(object sender, EventArgs e)
    //{
    //  try
    //  {
    //    string username = Context.User.Identity.IsAuthenticated ? Context.User.Identity.Name : "---";
    //    if (Context.Request.HttpMethod != "POST") return;
    //    if (username != "u216363") return;
    //    StringBuilder sb = new StringBuilder();
    //    sb.AppendLine("-------------------------------");
    //    sb.AppendFormat("\tREQUEST: user - {0} | url - {1}\n", username, Context.Request.Url);
    //    sb.AppendLine("\t-------------------------------");

    //    Stopwatch stopwatch = Context.Items[Context.Request.Cookies["ASP.NET_SessionId"].Value] as Stopwatch;
    //    if (stopwatch != null)
    //    {
    //      stopwatch.Stop();
    //      sb.AppendFormat("\tRESULT: {0} ticks; {1} ms; {2} s\n", stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds, (stopwatch.ElapsedMilliseconds / 1000m).GetPrice());
    //      sb.AppendLine("\t-------------------------------");
    //      if (stopwatch.ElapsedMilliseconds > 10000) sb.AppendLine("\tMORETHAN_10000");
    //    }
    //    Logger.LogInfo(sb.ToString());
    //  }
    //  catch
    //  {
    //  }
    //}
  }
}