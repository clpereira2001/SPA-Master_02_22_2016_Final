using System;
using System.Configuration;
using System.Text;
using System.Web.Mvc;
using Vauction.Configuration;
using Vauction.Models;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Lib;
using Vauction.Utils.Perfomance;
using System.Collections.Generic;

namespace Vauction.Controllers
{
  [ValidateInput(false)]
  public class BaseController : Controller
  {
    public IVauctionDataProvider dataProvider;

    public IVauctionConfiguration Config { get; protected set; }
    public BaseController()
    {
      Config = (IVauctionConfiguration)ConfigurationManager.GetSection("Vauction");
      dataProvider = Config.DataProvider.GetInstance();
    }

    protected virtual void SetFilterParams(GeneralFilterParams param)
    {
      param.ViewMode = BaseHelpers.GetViewMode();
      param.PageSize = Consts.PageSize;
      ViewData["filterParam"] = param;
    }

    [NonAction]
    public void InitCurrentEvent()
    {
      EventDetail ed = dataProvider.EventRepository.GetEventDetail(null);
      ViewData["CurrentEvent"] = ed;
      SessionUser cuser = AppHelper.CurrentUser;
      ViewData["UserRegisterForEvent"] = cuser != null && dataProvider.EventRepository.IsUserRegisterForEvent(cuser.ID, ed.ID);
      ViewData["ShowAds"] = false;//DateTime.Now < new DateTime(ed.DateEnd.Year, ed.DateEnd.Month, ed.DateEnd.Day, 0, 0, 1) || DateTime.Now > ed.DateEnd.AddMinutes(10);
    }

    public JsonResult JSON(object obj)
    {
      return Json(obj, JsonRequestBehavior.AllowGet);
    }

    private Dictionary<string, List<string>> GetVauctionRegions()
    {
      Dictionary<string, List<string>> regions = new Dictionary<string, List<string>>();
      List<string> reg = new List<string>() { "COUNTRIES", "STATES", "CATEGORIES", "EVENTS" };
      regions.Add(DataCacheType.REFERENCE.ToString(), reg);
      reg = new List<string>() { "AUCTIONS", "IMAGES", "AUCTIONLISTS", "BIDS" };
      regions.Add(DataCacheType.RESOURCE.ToString(), reg);
      reg = new List<string>() { "USERS", "WATCHLISTS", "EVENTREGISTRATIONS", "BIDS", "INVOICES" };
      regions.Add(DataCacheType.ACTIVITY.ToString(), reg);
      return regions;
    }

    [HttpGet, NoCache]
    public void ClearAllCache()
    {
      ICacheDataProvider CacheRepository;
      List<string> keys = new List<string>();
      if (Consts.DataCachingTechnology == DataCacheTechnology.MEMORYOBJECT)
      {
        CacheRepository = new CacheDataProvider();
        keys = CacheRepository.Clear();
      }
      else
      {
        CacheRepository = new AppFabricCacheProviderSystemRegions(Consts.ProductName);
        Dictionary<string, List<string>> cache = GetVauctionRegions();
        string tp = DataCacheType.REFERENCE.ToString();
        keys.AddRange((CacheRepository as AppFabricCacheProviderSystemRegions).Clear(tp, cache[tp]));
        tp = DataCacheType.RESOURCE.ToString();
        keys.AddRange((CacheRepository as AppFabricCacheProviderSystemRegions).Clear(tp, cache[tp]));
        tp = DataCacheType.ACTIVITY.ToString();
        keys.AddRange((CacheRepository as AppFabricCacheProviderSystemRegions).Clear(tp, cache[tp]));
      }
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("[ALL CACHE CLEARED: " + System.DateTime.Now.ToString() + "]");
      keys.ForEach(k => sb.AppendLine(k));
      Logger.LogInfo(sb.ToString());
    }

    [HttpGet, NoCache]
    public void ClearADP(int id)
    {
      dataProvider.AuctionRepository.RemoveAuctionCache(id);
      Logger.LogInfo("[(ClearADP) Cache removed for auction #" + id.ToString() + "]");
    }

    [HttpGet, NoCache]
    public void ClearEvent(int id)
    {
      dataProvider.EventRepository.RemoveEventCache(id);
      Logger.LogInfo("[(ClearEvent) Cache removed for event #" + id.ToString() + "]");
    }

    [HttpGet, NoCache]
    public void ClearEventDataForListing(int id)
    {
      dataProvider.EventRepository.RemoveEventCache(id);
      Logger.LogInfo("[(ClearEventDataForListing) Cache removed for event #" + id.ToString() + "]");
    }

    [HttpGet, NoCache]
    public void ClearAuctionListRegion()
    {
      ICacheDataProvider CacheRepository;
      if (Consts.DataCachingTechnology == DataCacheTechnology.MEMORYOBJECT)
        CacheRepository = new CacheDataProvider();
      else
        CacheRepository = new AppFabricCacheProviderSystemRegions(Consts.ProductName);
      List<string> keys = CacheRepository.Clear(DataCacheType.RESOURCE.ToString(), "AUCTIONLISTS");
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("[(ClearAuctionListRegion) CACHE CLEARED: " + System.DateTime.Now.ToString() + "]");
      keys.ForEach(k => sb.AppendLine(k));
      Logger.LogInfo(sb.ToString());
    }

    [HttpGet, NoCache]
    public void ClearUser(long id)
    {
      dataProvider.UserRepository.ClearUserCache(id);
      Logger.LogInfo("[USER CACHE CLEARED: " + id + "]");
    }
  }
}
