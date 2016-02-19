using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Vauction.Utils.Autorization;
using Vauction.Utils.Helpers;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  partial class Event : IEvent
  {
    public bool IsUserRegistered(long user_id)
    {
      EventRegistration er = EventRegistrations.SingleOrDefault(ER => ER.User_ID == user_id);
      return (er != null) || IsAccessable;
    }

    public string StartEndTime
    {
      get { return String.Format("({0} ET to {1} ET)", DateStart, DateEnd); }
    }
    public string StartTime
    {
      get { return String.Format("{0} ET", DateStart); }
    }
    public string EndTime
    {
      get { return String.Format("{0} ET", DateEnd); }
    }
    public string EventDetailsInfo
    {
      get { return (IsViewable && IsClickable) || IsAccessable ? String.Format("<a href='{0}'>{1}</a>", AppHelper.GetSiteUrl("/Auction/EventDetailed/" + ID.ToString() + "/" + UrlTitle), Title) : Title; }
    }
    public string EventDetailsInfoDemo
    {
      get { return String.Format("<a href='{0}'>{1}</a>", AppHelper.GetSiteUrl("/Preview/EventDetailed/" + ID.ToString()+"/"+UrlTitle), Title); }
    }
    public string UrlTitle
    {
      get { return Vauction.Utils.UrlParser.TitleToUrl(Title); }
    }

    public bool IsAccessable
    {
      get
      {
        SessionUser cuser = AppHelper.CurrentUser;
        return cuser != null && cuser.IsAccessable;
      }
    }

    public bool isPrivate
    {
      get { return IsPrivate.HasValue && IsPrivate.Value; }
    }
  }
}
