using System;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  public class EventDetail
  {
    public long ID { get; set; }
    public string Title { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public decimal? BuyerFee { get; set; }
    public string Description { get; set; }
    public bool IsViewable { get; set; }
    public bool IsClickable { get; set; }
    public Int32? Ordinary { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsPrivate { get; set; }
    public bool RegisterRequired { get; set; }

    public static Event CreateEventObject(EventDetail ed)
    {
      Event evnt = new Event();
      evnt.ID = ed.ID;
      evnt.Title = ed.Title;
      evnt.DateStart = ed.DateStart;
      evnt.DateEnd = ed.DateEnd;
      evnt.BuyerFee = ed.BuyerFee;
      evnt.Description = ed.Description;
      evnt.IsViewable = ed.IsViewable;
      evnt.IsClickable = ed.IsClickable;
      evnt.Ordinary = ed.Ordinary;
      evnt.IsCurrent = ed.IsCurrent;
      evnt.IsPrivate = ed.IsPrivate;
      evnt.RegisterRequired = ed.RegisterRequired;
      return evnt;
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
    public string UrlTitle
    {
      get { return Vauction.Utils.UrlParser.TitleToUrl(Title); }
    }
    public string EventDetailsInfo
    {
      get { return (IsViewable && IsClickable) || (AppHelper.CurrentUser != null && AppHelper.CurrentUser.IsAccessable) ? String.Format("<a href='{0}'>{1}</a>", AppHelper.GetSiteUrl("/Auction/EventDetailed/" + ID.ToString() + "/" + UrlTitle), Title) : Title; }
    }
    public string EventDetailsInfoDemo
    {
      get { return String.Format("<a href='{0}'>{1}</a>", AppHelper.GetSiteUrl("/Preview/EventDetailed/" + ID.ToString() + "/" + UrlTitle), Title); }
    }
  }
}