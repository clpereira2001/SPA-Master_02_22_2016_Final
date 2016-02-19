using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Vauction.Utils;
using Vauction.Utils.Perfomance;
using Vauction.Utils.Lib;

namespace Vauction.Models
{
  [Serializable]
  partial class EventCategory : IEventCategory
  {
    private ICacheDataProvider CacheRepository = AppHelper.CacheDataProvider;

    private void GetParentCategory(StringBuilder sb, CategoriesMap cm)
    {
      if (cm == null || cm.Category == null) return;
      sb.Insert(0, cm.Category.Title);
      if (cm.CategoriesMap_Parent == null || cm.CategoriesMap_Parent.ID == cm.ID) return;
      sb.Insert(0, " > ");
      GetParentCategory(sb, cm.CategoriesMap_Parent);
    }
   
    public string FullCategory
    {
      get
      {
        if (CacheRepository == null) CacheRepository = new CacheDataProvider();
        DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "FULLCATEGORY_EC", new object[] { ID }, CachingExpirationTime.Days_01);
        string result = CacheRepository.Get(dco) as string;
        if (!String.IsNullOrEmpty(result)) return result;
        StringBuilder sb = new StringBuilder();
        try
        {
          GetParentCategory(sb, CategoriesMap);
          result = sb.ToString();
          if (!string.IsNullOrEmpty(result))
          {
            dco.Data = result;
            CacheRepository.Add(dco);
          }
        }
        catch (Exception ex)
        {
          Logger.LogException("[EventCategory_ID:"+ID.ToString()+"]", ex);
        }
        return sb.ToString();
      }
    }

    private void GetParentCategoryLink(StringBuilder sb, CategoriesMap cm, bool demo)
    {
      if (cm==null || cm.CategoriesMap_Parent==null || cm.CategoriesMap_Parent.ID == cm.ID) return;
      cm = cm.CategoriesMap_Parent;
      sb.Insert(0, "<span>&nbsp;&gt;&nbsp;</span>");
      sb.Insert(0, String.Format("<a href='{0}/?e={2}'>{1}</a>", AppHelper.GetSiteUrl("/"+(demo?"Preview":"Auction")+"/Category/" + cm.ID +"/"+ UrlParser.TitleToUrl(cm.Category.Title)), cm.Category.Title, Event_ID));
      GetParentCategoryLink(sb, cm, demo);
    }

    public string FullCategoryLink
    {
      get
      {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("<a href='{0}'>{1}</a>", AppHelper.GetSiteUrl("/Auction/EventCategory/" + ID +"/"+UrlParser.TitleToUrl(FullCategory)), CategoriesMap!=null && CategoriesMap.Category!=null? CategoriesMap.Category.Title : "---");
        GetParentCategoryLink(sb, CategoriesMap, false);
        return sb.ToString();
      }
    }

    public string FullCategoryLinkDemo
    {
      get
      {
        if (CacheRepository == null) CacheRepository = new CacheDataProvider();
        DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "FULLCATEGORYLINKDEMO_EC", new object[] { ID }, CachingExpirationTime.Days_01);
        string result = CacheRepository.Get(dco) as string;
        if (!String.IsNullOrEmpty(result)) return result;
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("<a href='{0}'>{1}</a>", AppHelper.GetSiteUrl("/Preview/EventCategory/" + ID + "/" + UrlParser.TitleToUrl(FullCategory)), CategoriesMap != null && CategoriesMap.Category != null ? CategoriesMap.Category.Title : "---");
        GetParentCategoryLink(sb, CategoriesMap, true);
        result = sb.ToString();
        if (!string.IsNullOrEmpty(result))
        {
          dco.Data = result;
          CacheRepository.Add(dco);
        }
        return result;
      }
    }
  }
}
