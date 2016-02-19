using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  public class LinkParams
  {
    public long ID { get; set; }
    public long Lot { get; set; }
    public string Title { get; set; }    
    public string EventTitle { get; set; }
    public string CategoryTitle { get; set; }
    public long Event_ID { get; set; }
    public long EventCategory_ID { get; set; }
    public long Category_ID { get; set; }

    public string EventUrl
    {
      get { return String.IsNullOrEmpty(EventTitle) ? String.Empty : UrlParser.TitleToUrl(EventTitle); }
    }

    public string CategoryUrl
    {
      get { return String.IsNullOrEmpty(CategoryTitle) ? String.Empty : UrlParser.TitleToUrl(CategoryTitle); }
    }

    public string GetLotTitleUrl(long lot, string title)
    {
      return UrlParser.TitleToUrl(String.Format("Lot{0}~{1}", lot, title.Replace("\"","'")));
    }

    public string LotTitleUrl
    {
      get { return GetLotTitleUrl(Lot, Title); }
    }

    public string GetLotTitle(long lot, string title)
    {
      return String.Format("{0}. {1}", lot, title.Replace("\"", "'"));
    }

    public string LotTitle
    {
      get { return GetLotTitle(Lot, Title); }
    }

    public string AuctionDetailUrl
    {
      get { return AppHelper.GetSiteUrl(String.Format("/Auction/AuctionDetail/{0}/{1}/{2}/{3}", ID, EventUrl, CategoryUrl, LotTitleUrl)); }
    }
  }
}
