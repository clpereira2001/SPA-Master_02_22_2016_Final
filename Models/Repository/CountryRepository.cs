using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils.Perfomance;
using Vauction.Utils;

namespace Vauction.Models
{
  public class CountryRepository : ICountryRepository
  {
    #region init
    private VauctionDataContext dataContext;
    private ICacheDataProvider CacheRepository;

    public CountryRepository(VauctionDataContext dataContext, ICacheDataProvider cacheDataProvider)
    {
      this.dataContext = dataContext;
      CacheRepository = cacheDataProvider;
    }
    #endregion

    //GetListPage
    public List<Country> GetCountryList()
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.COUNTRIES, "GETCOUNTRYLIST", new object[] { }, CachingExpirationTime.Days_01);
      List<Country> result = CacheRepository.Get(dco) as List<Country>;
      if (result != null && result.Count() > 0) return result;
      result = dataContext.spCountry_List().ToList();
      if (result.Count() > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetCountryByID
    public Country GetCountryByID(long ID)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.COUNTRIES, "GETCOUNTRYBYID", new object[] { ID }, CachingExpirationTime.Days_01);
      Country result = CacheRepository.Get(dco) as Country;
      if (result != null) return result;
      result = GetCountryList().Where(c => c.ID == ID).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetStateList
    public List<State> GetStateList(long? country_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.STATES, "GETSTATELIST", new object[] { country_id }, CachingExpirationTime.Days_01);
      List<State> result = CacheRepository.Get(dco) as List<State>;
      if (result != null && result.Count() > 0) return result;
      result = dataContext.spState_List(country_id).ToList();
      if (result.Count() > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetStateByCode
    public State GetStateByCode(string code)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.STATES, "GETSTATEBYCODE", new object[] { code }, CachingExpirationTime.Days_01);
      State result = CacheRepository.Get(dco) as State;
      try
      {
        if (result != null) return result;
        result = GetStateList(null).FirstOrDefault(S => S.Code.ToLower() == code.ToLower());
        if (result == null)
          result = dataContext.States.Where(S => S.ID == 0).FirstOrDefault();
        else
        {
          dco.Data = result;
          CacheRepository.Add(dco);
        }
      }
      catch (Exception ex)
      {
        Vauction.Utils.Lib.Logger.LogException("[code=" + code + "]", ex);
      }
      return result;
    }


  }
}
