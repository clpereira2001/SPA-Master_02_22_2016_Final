using System;
using System.Globalization;
using Vauction.Utils;
using Vauction.Utils.Autorization;

namespace Vauction.Models
{
  [Serializable]
  public class AuctionDetail
  {
    public DateTime EventDateStart { get; set; }
    public DateTime EventDateEnd { get; set; }
    public bool IsClickable { get; set; }
    public bool IsViewable { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsPrivate { get; set; }
    public decimal BuyerFee { get; set; }
    public long Owner_ID { get; set; }    
    public byte Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }    
    public decimal Price { get; set; }
    public decimal Shipping { get; set; }
    public long AuctionType { get; set; }
    public int Quantity { get; set; }
    public int IQuantity { get; set; }
    public string Description { get; set; }
    public decimal? BuyPrice { get; set; }
    public bool IsTaxable { get; set; }
    public bool IsConsignorShip { get; set; }
    public bool? IsSpecialInstruction { get; set; }

    public LinkParams LinkParams { get; set; }

    public string StartEndTime
    {
      get { return String.Format("({0} ET to {1} ET)", EventDateStart, EventDateEnd); }
    }
    public string StartTime
    {
      get { return String.Format("{0} ET", EventDateStart); }
    }
    public string EndTime
    {
      get { return String.Format("{0} ET", EventDateEnd); }
    }
    public string EventDetailsInfo
    {
      get { return ((IsViewable && IsClickable) || IsPrivate) || IsAccessable ? String.Format("<a href='{0}'>{1}</a>", AppHelper.GetSiteUrl("/Auction/EventDetailed/" + LinkParams.Event_ID.ToString(CultureInfo.InvariantCulture) + "/" + LinkParams.EventUrl), LinkParams.EventTitle) : LinkParams.EventTitle; }
    }
    public string EventDetailsInfoDemo
    {
      get { return String.Format("<a href='{0}'>{1}</a>", AppHelper.GetSiteUrl("/Preview/EventDetailed/" + LinkParams.Event_ID.ToString(CultureInfo.InvariantCulture) + "/" + LinkParams.EventUrl), LinkParams.EventTitle); }
    }
    public bool IsAccessable
    {
      get
      {
        SessionUser cuser = AppHelper.CurrentUser;
        return cuser != null && cuser.IsAccessable;
      }
    }    
  }
}