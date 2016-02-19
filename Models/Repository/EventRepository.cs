using System;
using System.Collections.Generic;
using System.Linq;
using Vauction.Utils.Perfomance;
using Vauction.Utils.Lib;
using System.Data.Linq;

namespace Vauction.Models
{
  public class EventRepository : IEventRepository
  {
    #region init
    private VauctionDataContext dataContext;
    private ICacheDataProvider CacheRepository;

    public EventRepository(VauctionDataContext dataContext, ICacheDataProvider cacheDataProvider)
    {
      this.dataContext = dataContext;
      CacheRepository = cacheDataProvider;
    }

    //SubmitChanges
    private void SubmitChanges()
    {
      try
      {
        dataContext.SubmitChanges();
      }
      catch (ChangeConflictException e)
      {
        Logger.LogWarning(e.Message);
        foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        {
          occ.Resolve(RefreshMode.KeepCurrentValues);
        }
      }
    }

    #endregion

    #region get event / current event
    //GetCurrent
    public Event GetCurrent()
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "GETCURRENT", new object[] { }, CachingExpirationTime.Hours_01);
      Event result = CacheRepository.Get(dco) as Event;
      if (result != null) return result;
      result = dataContext.spEvent_GetCurrent().FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetEventByID
    public Event GetEventByID(long event_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "GETEVENTBYID", new object[] { event_id }, CachingExpirationTime.Hours_01);
      Event result = CacheRepository.Get(dco) as Event;
      if (result != null) return result;
      result = dataContext.spSelect_Event(event_id).SingleOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetEventDetals
    public EventDetail GetEventDetail(long? event_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "GETEVENTDETAIL", new object[] { event_id }, CachingExpirationTime.Hours_01);
      EventDetail result = CacheRepository.Get(dco) as EventDetail;
      if (result != null) return result;
      result = (from p in dataContext.spEvent_Detail(event_id)
                select new EventDetail
                {
                  BuyerFee = p.BuyerFee,
                  DateEnd = p.DateEnd,
                  DateStart = p.DateStart,
                  Description = p.Description,
                  ID = p.Event_ID,
                  IsClickable = p.IsClickable,
                  IsCurrent = p.IsCurrent,
                  IsPrivate = p.IsPrivate.GetValueOrDefault(false),
                  IsViewable = p.IsViewable,
                  RegisterRequired = p.RegisterRequired,
                  Ordinary = p.Ordinary,
                  Title = p.Title
                }).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    #endregion

    //GetUpcomingList
    public List<Event> GetUpcomingList(bool? IsAdmin)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "GETUPCOMINGLIST", new object[] { IsAdmin }, CachingExpirationTime.Hours_01);
      List<Event> result = CacheRepository.Get(dco) as List<Event>;
      if (result != null && result.Count() > 0) return result;
      result = (IsAdmin.HasValue && IsAdmin.Value ?
        (from E in dataContext.Events
         where E.ID != 0
         orderby E.Ordinary ascending, E.DateEnd descending
         select E) :
        (from E in dataContext.Events
         where E.IsViewable && E.DateEnd > DateTime.Now.AddDays(-45) && E.ID != 0
         orderby E.Ordinary ascending, E.DateEnd descending
         select E)).ToList();
      if (result.Count() > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetEventRegistration
    private EventRegistration GetEventRegistration(long user_id, long event_id, bool iscache)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.EVENTREGISTRATIONS, "GETEVENTREGISTRATION", new object[] { user_id, event_id }, CachingExpirationTime.Days_01);
      EventRegistration result = CacheRepository.Get(dco) as EventRegistration;
      if (result != null && iscache) return new EventRegistration(result);
      result = dataContext.spSelect_EventRegistration(event_id, user_id).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result != null ? new EventRegistration(result) : null;
    }

    // RegisterUserForEvent
    public bool RegisterUserForEvent(long event_id, long user_id)
    {
      try
      {
        EventRegistration registration = GetEventRegistration(user_id, event_id, true);
        if (registration != null) return true;
        registration = new EventRegistration { Event_ID = event_id, User_ID = user_id };
        dataContext.EventRegistrations.InsertOnSubmit(registration);
        SubmitChanges();
        CacheRepository.Put(new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.EVENTREGISTRATIONS, "GETEVENTREGISTRATION", new object[] { user_id, event_id }, CachingExpirationTime.Days_01, registration));
      }
      catch (Exception ex)
      {
        Logger.LogException("[user_id=" + user_id.ToString() + ";event_id=" + event_id.ToString() + "]", ex);
        return false;
      }
      return true;
    }

    //GetFutureEvents
    public List<Event> GetFutureEvents()
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "GETFUTUREEVENTS", new object[] { }, CachingExpirationTime.Minutes_01);
      List<Event> result = CacheRepository.Get(dco) as List<Event>;
      if (result != null && result.Count() > 0) return result;
      result = dataContext.spEvent_FutureList().ToList();
      if (result.Count() > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //IsUserRegisterForEvent
    public bool IsUserRegisterForEvent(long user_id, long event_id)
    {
      //Event evnt = GetEventByID(event_id);
      return /*!evnt.RegisterRequired ||*/ GetEventRegistration(user_id, event_id, true) != null;
    }

    //RemoveEventCash
    public void RemoveEventCache(long event_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.EVENTS, "GETCURRENT");
      CacheRepository.Remove(dco);
      dco.Method = "GETEVENTBYID";
      dco.Params = new object[] { event_id };
      CacheRepository.Remove(dco);
      dco.Method = "GETEVENTDETAIL";
      CacheRepository.Remove(dco);
      dco.Method = "GETUPCOMINGLIST";
      dco.Params = new object[] { true };
      CacheRepository.Remove(dco);
      dco.Method = "GETUPCOMINGLIST";
      dco.Params = new object[] { false };
      CacheRepository.Remove(dco);
      dco.Method = "GETFUTUREEVENTS";
      dco.Params = new object[] { };
      CacheRepository.Remove(dco);
      dco.Method = "GETEVENTCATEGORYBYID";
      dco.Params = new object[] { event_id };
      CacheRepository.Remove(dco);
      dco.Method = "GETALLOWEDCATEGORIESFORTHEEVENT";
      CacheRepository.Remove(dco);
      dco.Region = DataCacheRegions.CATEGORIES;
      dco.Method = "GETCATEGORIESMAPTREEPREVIEW";
      CacheRepository.Remove(dco);
      dco.Method = "GETCATEGORIESMAPTREE";
      dco.Params = new object[] { event_id, false };
      CacheRepository.Remove(dco);
      dco.Params = new object[] { event_id, true };
      CacheRepository.Remove(dco);
    }

    //RemoveEventCacheForListing
    public void RemoveEventCacheForListing(long event_id)
    {
      var dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.CATEGORIES, "GETCATEGORIESMAPTREEPREVIEW", new object[] { event_id });
      CacheRepository.Remove(dco);
      dco.Method = "GETCATEGORIESMAPTREE";
      dco.Params = new object[] { event_id, true };
      CacheRepository.Remove(dco);
      dco.Params = new object[] { event_id, false };
      CacheRepository.Remove(dco);
    }
  }
}
