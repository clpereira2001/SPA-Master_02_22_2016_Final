using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Vauction.Utils.Perfomance;
using Vauction.Utils;
using Vauction.Utils.Lib;

namespace Vauction.Models
{
  [Serializable]
  partial class CategoriesMap : ICategoriesMap
  {
    private ICacheDataProvider CacheRepository = AppHelper.CacheDataProvider;

    private void GetParentCategory(StringBuilder sb, CategoriesMap cm)
    {
      if (cm.Category==null) return;
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
        DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "FULLCATEGORY_CM", new object[] { ID }, CachingExpirationTime.Days_01);
        string result = CacheRepository.Get(dco) as string;
        if (!String.IsNullOrEmpty(result)) return result;
        StringBuilder sb = new StringBuilder();
        try
        {
          GetParentCategory(sb, this);
          result = sb.ToString();
          if (!string.IsNullOrEmpty(result))
          {
            dco.Data = result;
            CacheRepository.Add(dco);
          }
        }
        catch (Exception ex)
        {
          Logger.LogException("[CategpriesMap_ID:" + ID.ToString() + "]", ex);
        }
        return sb.ToString();
      }
    }

  }
}