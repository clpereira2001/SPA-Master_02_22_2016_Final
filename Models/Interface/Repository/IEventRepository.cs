using System.Collections.Generic;

namespace Vauction.Models
{
  public interface IEventRepository
  {    
    Event GetCurrent();
    List<Event> GetUpcomingList(bool? IsAdmin);
    bool RegisterUserForEvent(long id, long user_id);
    Event GetEventByID(long event_id);
    List<Event> GetFutureEvents();
    EventDetail GetEventDetail(long? event_id);
    bool IsUserRegisterForEvent(long user_id, long event_id);
    void RemoveEventCache(long event_id);
    void RemoveEventCacheForListing(long event_id);
  }
}