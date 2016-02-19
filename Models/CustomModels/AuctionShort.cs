using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  public class AuctionShort
  {    
    public bool IsBold { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsUserRegisteredForEvent { get; set; }
    public decimal Price { get; set; }
    public decimal? CurrentBid_1 { get; set; }
    public decimal? CurrentBid_2 { get; set; }
    public bool IsClickable { get; set; }
    public bool IsAccessable { get; set; }
    public string ThumbnailPath { get; set; }
    public byte Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public LinkParams LinkParams { get; set; }

    public string CurrentBid
    {
      get { return HasBid?String.Format("{0}{1}{2}", CurrentBid_1.GetCurrency(), (CurrentBid_2.HasValue && CurrentBid_1 != CurrentBid_2.Value) ? " - " : String.Empty, (CurrentBid_2.HasValue && CurrentBid_1 != CurrentBid_2.Value) ? CurrentBid_2.GetValueOrDefault(0).GetCurrency() : String.Empty) : String.Empty; }
    }

    public bool HasBid
    {
      get { return CurrentBid_1.HasValue && CurrentBid_1.Value>0; }
    }
  }
}