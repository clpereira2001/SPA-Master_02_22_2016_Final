using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils.Paging;

namespace Vauction.Models
{
  public interface IBidRepository
  {
    List<BiddingHistory> GetBidHistory(long auction_id);
    List<Event> GetPastEventBiddingHistory(long user_id);
    PagedList<MyBid> GetPastUsersWatchList(long event_id, long user_id, int pageindex, int pagesize);
    List<UserBidWatch> GetBidWatchForUser(long userID, long eventID);
    BidCurrent GetTopBidForItem(long auction_id);
    BidCurrent GetTopBidForItem(long auction_id, int quantity, bool fromcache);
    BidCurrent GetUserTopBidForItem(long auction_id, long user_id, bool iscaching);
    decimal DepositNeeded(decimal amount, long user_id, bool isneedonlymaxdeposit, long event_id, long auction_id);
    BiddingObject PlaceSingleBid(long auction_id, bool isproxy, decimal amount, long user_id, int quantity, bool isproxyraise, decimal aprice, BidCurrent prevUsersBid, BidCurrent lastTop);
    List<BidCurrent> PlaceMultipleBids(long auction_id, bool isproxy, decimal amount, long user_id, int quantity);
    void ResolveProxyBiddingSituation(long auction_id, long user_id, bool isproxy, BiddingObject placedBid, BidCurrent lastTop, decimal aprice, List<BidLogCurrent> newbidlogs);
    bool Update(BidCurrent updBid);
    BidLogCurrent AddBidLogCurrent(long auction_id, int quantity, long user_id, bool isproxy, decimal amount, decimal maxamount, bool isproxyraise, string IP);
    void UpdateUsersTopBidCache(long auction_id, long user_id, BidCurrent bid);
    void UpdateTopBidForItemCache(long auction_id, BidCurrent bid);
    void RemoveTopBidForItemCache(long auction_id);
    void RemoveUsersTopBidCache(long auction_id, long user_id);
    object UpdateWatchListPage(long user_id, long event_id);

    byte BiddingForSingleAuction(AuctionDetail auction, BidCurrent currentBid, out BidCurrent previousBid, out BidCurrent loserBid, out BidCurrent winnerBid);
    void UpdateUserWinLotsCache(long user_id, long event_id, long auction_id, decimal maxbid);
    void GetBidsForMultipleItem(AuctionDetail auction, out List<BidCurrent> winners, out List<BidCurrent> losers);
    void RemoveTopBidForMultipleItemCache(long auction_id);
    List<BidCurrent> GetBidsForMultipleItem(long auction_id, byte iswinner, int quantity, bool fromcache);
  }
}

