using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Collections.Specialized;
using System.Web.Caching;

namespace Vauction
{
  public class IPBlackList : IHttpModule
  {
    private EventHandler onBeginRequest;

    public IPBlackList()
    {
      onBeginRequest = new EventHandler(this.HandleBeginRequest);
    }

    void IHttpModule.Dispose()
    {
    }

    void IHttpModule.Init(HttpApplication context)
    {
      context.BeginRequest += onBeginRequest;
    }

    const string BLOCKEDIPSKEY = "blockedips";
    const string BLOCKEDIPSFILE = "~/blockedips.txt";

    public static StringDictionary GetBlockedIPs(HttpContext context)
    {
      StringDictionary ips = (StringDictionary)context.Cache[BLOCKEDIPSKEY];
      if (ips == null)
      {
        ips = GetBlockedIPs(GetBlockedIPsFilePathFromCurrentContext(context));
        context.Cache.Insert(BLOCKEDIPSKEY, ips, new CacheDependency(GetBlockedIPsFilePathFromCurrentContext(context)));
      }
      return ips;
    }
    private static string BlockedIPFileName = null;
    private static object blockedIPFileNameObject = new object();
    public static string GetBlockedIPsFilePathFromCurrentContext(HttpContext context)
    {
      if (BlockedIPFileName != null)
        return BlockedIPFileName;
      lock (blockedIPFileNameObject)
      {
        if (BlockedIPFileName == null)
        {
          BlockedIPFileName = context.Server.MapPath(BLOCKEDIPSFILE);
        }
      }
      return BlockedIPFileName;
    }

    public static StringDictionary GetBlockedIPs(string configPath)
    {
      StringDictionary retval = new StringDictionary();
      //using (StreamReader sr = new StreamReader(configPath))
      //{
      //  String line;
      //  while ((line = sr.ReadLine()) != null)
      //  {
      //    line = line.Trim();
      //    if (!String.IsNullOrEmpty(line) && !retval.ContainsKey(line)) retval.Add(line, null);
      //  }
      //}

      List<string> ips = File.ReadAllText(configPath).Split(new string[]{",",";"}, StringSplitOptions.RemoveEmptyEntries).ToList();
      ips = ips.Distinct().Where(i => !String.IsNullOrEmpty(i.Trim())).ToList();
      ips.ForEach(i=>retval.Add(i.Trim(), null));
      return retval;
    }

    private void HandleBeginRequest(object sender, EventArgs evargs)
    { 
      HttpApplication app = sender as HttpApplication;      
      if (app != null)
      {
        string IPAddr = app.Context.Request.ServerVariables["REMOTE_ADDR"];
        if (IPAddr == null || IPAddr.Length == 0)
        {
          return;
        }

        StringDictionary badIPs = GetBlockedIPs(app.Context);
        if (badIPs != null && badIPs.ContainsKey(IPAddr))
        {
          app.Context.Response.StatusCode = 404;
          app.Context.Response.SuppressContent = true;          
          app.Context.Response.End();
        }
      }
    }
  }
}
