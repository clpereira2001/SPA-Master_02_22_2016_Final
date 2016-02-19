using System;
using System.Collections.Generic;
using System.Linq;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  public class BiddingResult
  {
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public long AuctionType { get; set; }
    public List<BidCurrent> WinningBids { get; set; }
    public List<BidCurrent> LoserBids { get; set; }
    public List<BidCurrent> UserTopBids { get; set; }

    public BiddingResult(decimal price, int quantity, long auctiontype)
    {
      Price = price;
      Quantity = quantity;
      AuctionType = auctiontype;
      WinningBids = new List<BidCurrent>();
      LoserBids = new List<BidCurrent>();
      UserTopBids = new List<BidCurrent>();
    }

    public BidCurrent MinWinBid
    {
      get
      {
        return AuctionType == (byte)Consts.AuctionType.Normal ? (WinningBids == null ? null : WinningBids.LastOrDefault()) : (WinningBids != null && WinningBids.Count() < Quantity ? null : WinningBids[Quantity - 1]);
      }
    }
    public BidCurrent UserWinBid
    {
      get
      {
        return UserTopBids == null ? null : UserTopBids.FirstOrDefault();
      }
    }
    public decimal MinBid
    {
      get
      {
        BidCurrent bc = MinWinBid;
        BidCurrent ub = UserWinBid;
        decimal mb = 0;
        if (AuctionType == (byte)Consts.AuctionType.Normal)
        {
          mb = bc == null ? Price - Consts.GetIncrement(Price) : bc.Amount;
          if (ub != null && mb < ub.MaxBid && !ub.IsProxy)
            mb = ub.MaxBid;
        }
        else
        {
          mb = bc == null ? Price : bc.Amount + Consts.GetIncrement(bc.Amount);
          if (ub != null && ub.Amount > mb && ub.IsActive)
            mb = ub.Amount + (UserTopBids.Count() <= Quantity ? 0 : Consts.GetIncrement(ub.Amount));
        }
        return mb;
      }
    }
    public List<BidCurrent> WinTable
    {
      get
      {
        List<BidCurrent> res = new List<BidCurrent>();
        var r = from d in WinningBids
                group d by d.User_ID;
        BidCurrent bid;
        foreach (var elem in r)
        {
          bid = elem.ToArray()[0];
          bid.Comments = String.Format("{0} of {1}", elem.Count(), elem.ToArray()[0].Quantity);
          res.Add(bid);
        }
        return res;
      }
    }
    public List<BidCurrent> LoserTable
    {
      get
      {
        List<BidCurrent> res = new List<BidCurrent>();
        var r2 = from d in LoserBids
                 group d by new { d.User_ID, d.IsActive } into newd
                 where newd.Key.IsActive
                 orderby newd.Key.IsActive descending
                 select newd;
        BidCurrent bid;
        foreach (var elem in r2)
        {
          bid = elem.ToArray()[0];
          bid.Comments = (elem.ToArray()[0].Quantity == 0) ? "---" : String.Format("{0} of {1}", elem.Count(), elem.ToArray()[0].Quantity);
          res.Add(bid);
        }
        return res.Count() > 3 ? res.GetRange(0, 3) : res;
      }
    }
  }
}