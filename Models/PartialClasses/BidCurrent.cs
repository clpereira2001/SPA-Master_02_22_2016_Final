using System;

namespace Vauction.Models
{
  [Serializable]
  public partial class BidCurrent : IBid
  {
    //public string Bidder { get; set; }

    public BidCurrent(BidCurrent bc)
    {
      Amount = bc.Amount;
      Auction_ID = bc.Auction_ID;
      Bidder = bc.Bidder;
      Comments = bc.Comments;
      DateMade = bc.DateMade;
      ID = bc.ID;
      IP = bc.IP;
      IsActive = bc.IsActive;
      IsProxy = bc.IsProxy;
      ItemNumber = bc.ItemNumber;
      MaxBid = bc.MaxBid;
      Quantity = bc.Quantity;
      User_ID = bc.User_ID;
    }
  }
}
