using System;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  public class AuctionResultDetail
  {
    public long Auction_ID { get; set; }
    public decimal? CurrentBid_1 { get; set; }
    public decimal? CurrentBid_2 { get; set; }
    public string HighBidder_1 { get; set; }
    public string HighBidder_2 { get; set; }
    public long? User_ID_1 { get; set; }
    public long? User_ID_2 { get; set; }

    //AuctionResultDetail
    public AuctionResultDetail()
    {
      
    }

    //AuctionResultDetail (AuctionResultDetail)
    public AuctionResultDetail(AuctionResultDetail ard)
    {
      Auction_ID = ard.Auction_ID;
      CurrentBid_1 = ard.CurrentBid_1;
      CurrentBid_2 = ard.CurrentBid_2;
      HighBidder_1 = ard.HighBidder_1;
      HighBidder_2 = ard.HighBidder_2;
      User_ID_1 = ard.User_ID_1;
      User_ID_2 = ard.User_ID_2;
    }

    public string CurrentBid
    {
      get { return HasBid ? String.Format("{0}{1}{2}", CurrentBid_1.GetValueOrDefault(0).GetCurrency(), (CurrentBid_2.HasValue && CurrentBid_1 != CurrentBid_2.Value) ? " - " : String.Empty, (CurrentBid_2.HasValue && CurrentBid_1 != CurrentBid_2.Value) ? CurrentBid_2.GetValueOrDefault(0).GetCurrency() : String.Empty) : String.Empty; }
    }
    public string HighBidder
    {
      get { return HasBid ? String.Format("{0}{1}{2}", HighBidder_1, (!String.IsNullOrEmpty(HighBidder_2) && HighBidder_1 != HighBidder_2) ? " - " : String.Empty, (!String.IsNullOrEmpty(HighBidder_2) && HighBidder_1 != HighBidder_2) ? HighBidder_2 : String.Empty) : String.Empty; }
    }

    public bool HasBid
    {
      get { return CurrentBid_1.HasValue && CurrentBid_1.Value > 0; }
    }
  }
}