using System;
using System.Web;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;

namespace Vauction.Utils
{
  public static class AppHelper
  {
    #region properties

    //CurrentUser
    public static SessionUser CurrentUser
    {
      get { return AppSession.CurrentUser; }
      set { AppSession.CurrentUser = value; }
    }

    //CacheDataProvider
    public static ICacheDataProvider CacheDataProvider
    {
      get { return AppApplication.CacheProvider; }
      set { AppApplication.CacheProvider = value; }
    }

    //ContentRoot
    private static string ContentRoot
    {
      get { return VirtualPathUtility.ToAbsolute(String.Format("{0}/public", "~")); }
    }

    //ImageRoot
    private static string ImageRoot
    {
      get { return String.Format("{0}/{1}", ContentRoot, "images"); }
    }

    //ImageMenuRoot
    private static string ImageMenuRoot
    {
      get { return String.Format("{0}/{1}", ImageRoot, "menu"); }
    }

    //AuctionImagesRoot
    private static string AuctionImagesRoot
    {
      get { return String.Format("{0}/{1}", ContentRoot, "auctionimages"); }
    }

    //TempImagesRoot
    private static string TempImagesRoot
    {
      get { return String.Format("{0}/{1}", ContentRoot, "tempImages"); }
    }

    //FilesRoot
    public static string FilesRoot
    {
      get { return String.Format("{0}/{1}", ContentRoot, "Files"); }
    }

    //UserImageRoot
    private static string UserImageRoot
    {
      get { return String.Format("{0}/{1}", AuctionImagesRoot, "UserImages"); }
    }

    //CssRoot
    private static string CssRoot
    {
      get { return String.Format("{0}/{1}", ContentRoot, "css"); }
    }

    //ScriptRoot
    private static string ScriptRoot
    {
      get { return String.Format("{0}/{1}", ContentRoot, "scripts"); }
    }

    //EmailTemplatesRoot
    private static string EmailTemplatesRoot
    {
      get { return HttpContext.Current.Server.MapPath("~/Templates/Email"); }
    }

    //ImageCompressMethod
    private static string ImageCompressMethod
    {
      get { return String.Format("{0}/Zip/Image", !HttpContext.Current.Request.IsSecureConnection ? Consts.ResourceHostName : Consts.ResourceSSLHostName); }
    }

    //ScriptCompressMethod
    private static string ScriptCompressMethod
    {
      get { return String.Format("{0}/Zip/Script", !HttpContext.Current.Request.IsSecureConnection ? Consts.ResourceHostName : Consts.ResourceSSLHostName); }
    }

    //ScriptCompressMethod
    private static string StyleCompressMethod
    {
      get { return String.Format("{0}/Zip/Style", !HttpContext.Current.Request.IsSecureConnection ? Consts.ResourceHostName : Consts.ResourceSSLHostName); }
    }


    #endregion

    #region methods

    //ImageUrl
    public static string ImageUrl(string file)
    {
      return String.Format("{0}/{1}", ImageRoot, file.ToLower());
    }

    //ImageMenuUrl
    public static string ImageMenuUrl(string file)
    {
      return String.Format("{0}/{1}", ImageMenuRoot, file.ToLower());
    }

    //AuctionImageFolder
    public static string AuctionImageFolder(long auction_id)
    {
      return String.Format("{0}/{1}", AuctionImagesRoot, String.Format("{0}/{1}/{2}", auction_id / 1000000, auction_id / 1000, auction_id));
    }

    //AuctionImage
    public static string AuctionImage(long auction_id, string file)
    {
      return String.Format("{0}/{1}/{2}", AuctionImagesRoot, String.Format("{0}/{1}/{2}", auction_id / 1000000, auction_id / 1000, auction_id), String.IsNullOrEmpty(file) ? String.Empty : file.ToLower());
    }

    //TempImageFolder
    public static string TempImageFolder(string tmpID)
    {
      return String.Format("{0}/{1}", TempImagesRoot, tmpID);
    }

    //TempImage
    public static string TempImage(string tmpID, string file)
    {
      return String.Format("{0}/{1}/{2}", TempImagesRoot, tmpID, string.IsNullOrEmpty(file) ? string.Empty : file.ToLower());
    }

    //UserImageFolder
    public static string UserImageFolder(long user_id)
    {
      return String.Format("{0}/{1}", UserImageRoot, user_id.ToString());
    }

    //UserImageFolder
    public static string UserImageFolder(long user_id, long listingauction_id)
    {
      return String.Format("{0}/{1}", UserImageFolder(user_id), listingauction_id.ToString());
    }

    //UserImage
    public static string UserImage(long user_id, long auction_id, string file)
    {
      return String.Format("{0}/{1}", UserImageFolder(user_id, auction_id), String.IsNullOrEmpty(file) ? String.Empty : file.ToLower());
    }

    //CssUrl
    public static string CssUrl(string file)
    {
      return String.Format("{0}/{1}", CssRoot, file.ToLower());
    }

    //ScriptUrl
    public static string ScriptUrl(string file)
    {
      return String.Format("{0}/{1}", ScriptRoot, file.ToLower());
    }

    public static string GetHTMLEmailTemplateFile(long email_id)
    {
      return String.Format("{0}/{1}/email.htm", EmailTemplatesRoot, email_id);
    }

    //CompressImage
    public static string CompressImage(string file)
    {
      return String.Format("{0}?path={1}", ImageCompressMethod, ImageUrl(file));
    }

    //CompressImagePath
    public static string CompressImagePath(string path)
    {
      return String.Format("{0}?path={1}", ImageCompressMethod, path.ToLower());
    }

    //CompressAuctionImage
    public static string CompressAuctionImage(long auction_id, string file)
    {
      return String.Format("{0}?path={1}", ImageCompressMethod, AuctionImage(auction_id, file));
    }

    //CompressStyle
    public static string CompressStyle(string file)
    {
      return String.Format("{0}?path={1}", StyleCompressMethod, CssUrl(file));
    }

    //CompressStyleForFiles
    public static string CompressStyleForFiles(string path)
    {
      return String.Format("{0}?path={1}", StyleCompressMethod, path);
    }

    //CompressScript
    public static string CompressScript(string file)
    {
      return String.Format("{0}?path={1}", ScriptCompressMethod, AppHelper.ScriptUrl(file));
    }

    //CompressScriptForFiles
    public static string CompressScriptForFiles(string path)
    {
      return String.Format("{0}?path={1}", ScriptCompressMethod, path);
    }

    //GetSiteUrl
    public static string GetSiteUrl()
    {
      string Port = String.Empty, Protocol = String.Empty, BaseUrl = String.Empty;
      Port = HttpContext.Current.Request.ServerVariables["SERVER_PORT"].ToString();
      Port = (Port == null || Port == "80" || Port == "443") ? "" : ":" + Port;
      Protocol = HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"].ToString();
      Protocol = Protocol == null || Protocol == "0" ? "http://" : "https://";
      BaseUrl = Protocol + HttpContext.Current.Request.ServerVariables["Server_Name"].ToString() + Port + (HttpContext.Current.Request.ApplicationPath == @"/" ? "" : HttpContext.Current.Request.ApplicationPath);
      return BaseUrl;
    }

    //GetSiteUrl
    public static string GetSiteUrl(string url)
    {
      return String.Format("{0}{1}", GetSiteUrl(), url);
    }

    //IsUrlLocalToHost
    public static bool IsUrlLocalToHost(this HttpRequestBase request, string url)
    {
      if (String.IsNullOrEmpty(url)) return false;
      Uri absoluteUri;
      return (Uri.TryCreate(url, UriKind.Absolute, out absoluteUri)) ? String.Equals(request.Url.Host, absoluteUri.Host, StringComparison.OrdinalIgnoreCase) : !url.StartsWith("http:", StringComparison.OrdinalIgnoreCase) && !url.StartsWith("https:", StringComparison.OrdinalIgnoreCase) && Uri.IsWellFormedUriString(url, UriKind.Relative);
    }
    #endregion
  }
}