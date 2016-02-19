using System;

namespace Vauction.Models
{
  [Serializable]
  partial class EventRegistration : IEventRegistration
  {

    public EventRegistration(EventRegistration er): this()
    {
      Event_ID = er.Event_ID;
      ID = er.ID;
      User_ID = er.User_ID;
    }
  }
}
