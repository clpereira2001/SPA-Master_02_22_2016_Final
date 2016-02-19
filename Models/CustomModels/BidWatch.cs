using System;
using Vauction.Utils;

namespace Vauction.Models
{
  public class UserBidWatch
  {
    public decimal Amount { get; set; }
    public decimal MaxBid { get; set; }
    public string HighBidder_1 { get; set; }
    public string HighBidder_2 { get; set; }
    public decimal? CurrentBid_1 { get; set; }
    public decimal? CurrentBid_2 { get; set; }
    public int? Quantity { get; set; }
    public byte Option { get; set; }

    public LinkParams LinkParams { get; set; }

    public string CurrentBid
    {
      get { return HasBid?String.Format("{0}{1}{2}", CurrentBid_1.GetCurrency(), (CurrentBid_2.HasValue && CurrentBid_1 != CurrentBid_2.Value) ? " - " : String.Empty, (CurrentBid_2.HasValue && CurrentBid_1 != CurrentBid_2.Value) ? CurrentBid_2.GetValueOrDefault(0).GetCurrency() : String.Empty):String.Empty; }
    }

    public bool HasBid
    {
      get { return CurrentBid_1.HasValue && CurrentBid_1.Value>0; }
    }

    public string HighBidder
    {
      get { return HasBid?String.Format("{0}{1}{2}", HighBidder_1, (!String.IsNullOrEmpty(HighBidder_2) && HighBidder_1 != HighBidder_2) ? " - " : String.Empty, (!String.IsNullOrEmpty(HighBidder_2) && HighBidder_1 != HighBidder_2) ? HighBidder_2 : String.Empty):String.Empty; }
    }

  }
}