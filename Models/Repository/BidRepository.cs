using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using Vauction.Utils;
using Vauction.Utils.Paging;
using Vauction.Utils.Lib;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
  public class BidRepository : IBidRepository
  {
    #region init
    private VauctionDataContext dataContext;
    private ICacheDataProvider CacheRepository;

    public BidRepository(VauctionDataContext dataContext, ICacheDataProvider cacheDataProvider)
    {
      this.dataContext = dataContext;
      CacheRepository = cacheDataProvider;
    }

    //SubmitChages
    private void SubmitChages()
    {
      try
      {
        dataContext.SubmitChanges();
      }
      catch (ChangeConflictException e)
      {
        Logger.LogWarning(e.Message);
        foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        {
          occ.Resolve(RefreshMode.KeepCurrentValues);
        }
      }
    }
    #endregion

    //GetBidHistory
    public List<BiddingHistory> GetBidHistory(long auction_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.BIDS, "GETBIDHISTORY", new object[] { auction_id }, CachingExpirationTime.Days_01);
      List<BiddingHistory> result = CacheRepository.Get(dco) as List<BiddingHistory>;
      if (result != null && result.Count() > 0) return result;
      dataContext.CommandTimeout = 600000;
      result = (from p in dataContext.spBid_BidsHistory(auction_id)
                select new BiddingHistory
                {
                  Login = p.Bidder,
                  Amount = p.Bid.GetValueOrDefault(0),
                  DateMade = p.DateMade.GetValueOrDefault(DateTime.MinValue),
                  IsWinner = p.IsWinner.GetValueOrDefault(false)
                }).ToList();
      if (result.Count() > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetPastEventBiddingHistory
    public List<Event> GetPastEventBiddingHistory(long user_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.BIDS, "GETPASTEVENTBIDDINGHISTORY", new object[] { user_id }, CachingExpirationTime.Days_01);
      List<Event> result = CacheRepository.Get(dco) as List<Event>;
      if (result != null && result.Count() > 0) return result;
      dataContext.CommandTimeout = 600000;
      result = dataContext.spBid_Event_PastAuctionsBiddingHistory(user_id).ToList();
      if (result.Count() > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetPastUsersWatchList
    public PagedList<MyBid> GetPastUsersWatchList(long event_id, long user_id, int pageindex, int pagesize)
    {
      int? totalrecords = 0;
      pageindex = (pageindex > 0) ? pageindex - 1 : 0;
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.BIDS, "GETPASTUSERSWATCHLIST", new object[] { event_id, user_id, pageindex, pagesize }, CachingExpirationTime.Days_01);

      TableViewResult<MyBid> result = CacheRepository.Get(dco) as TableViewResult<MyBid>;
      if (result != null && result.TotalRecords > 0)
        return new PagedList<MyBid>(result.Records, pageindex, pagesize, result.TotalRecords);
      result = new TableViewResult<MyBid>();

      dataContext.CommandTimeout = 600000;
      result.Records = (from SP in dataContext.spBid_View_GetPastBidWatch(user_id, event_id, pageindex, pagesize, ref totalrecords)
                        select new MyBid
                        {
                          LinkParams = new LinkParams { EventTitle = SP.EventTitle, CategoryTitle = SP.CategoryTitle, Lot = SP.Lot.GetValueOrDefault(0), ID = SP.Auction_ID.GetValueOrDefault(0), Title = SP.Title },
                          Amount = SP.Amount.GetValueOrDefault(0),
                          MaxBid = SP.MaxBid.GetValueOrDefault(0),
                          DateMade = SP.DateMade.GetValueOrDefault(DateTime.Now),
                          IsWinner = SP.IsWinner.GetValueOrDefault(false),
                          CurrentBid_1 = SP.CurrentBid_1,
                          CurrentBid_2 = SP.CurrentBid_2,
                          ThubnailImage = SP.ThumbnailPath
                        }).ToList();
      result.TotalRecords = totalrecords.GetValueOrDefault(0);
      if (result.TotalRecords > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return totalrecords.GetValueOrDefault(0) > 0 ? new PagedList<MyBid>(result.Records, pageindex, pagesize, totalrecords.GetValueOrDefault(0)) : new PagedList<MyBid>(new List<MyBid>(), pageindex, pagesize, 0);
    }

    //GetBidWatchForUser
    public List<UserBidWatch> GetBidWatchForUser(long userID, long eventID)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.BIDS, "GETBIDWATCHFORUSER", new object[] { userID, eventID }, CachingExpirationTime.Seconds_15);
      List<UserBidWatch> result = CacheRepository.Get(dco) as List<UserBidWatch>;
      if (result != null && result.Any()) return result;
      dataContext.CommandTimeout = 600000;
      result = (from p in dataContext.spBid_BidWatch(userID, eventID)
                select new UserBidWatch
                {
                  Amount = p.Amount.GetValueOrDefault(0),
                  CurrentBid_1 = p.CurrentBid_1,
                  CurrentBid_2 = p.CurrentBid_2,
                  HighBidder_1 = p.HighBidder_1,
                  HighBidder_2 = p.HighBidder_2,
                  Quantity = p.WinQuantity.GetValueOrDefault(0) > 0 ? p.WinQuantity.GetValueOrDefault(0) : p.BidQuantity,
                  MaxBid = p.MaxBid.GetValueOrDefault(0),
                  Option = (byte)(p.Amount.GetValueOrDefault(-1) == -1 ? 2 : (p.WinQuantity.GetValueOrDefault(0) == 0 ? 0 : 1)),
                  LinkParams = new LinkParams { ID = p.Auction_ID.GetValueOrDefault(0), Lot = p.Lot.GetValueOrDefault(0), Title = p.Title },
                  Cost = p.Cost.GetValueOrDefault(1)
                }).ToList();
      if (result.Any())
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //UpdateWatchListPage
    public object UpdateWatchListPage(long user_id, long event_id)
    {
      List<UserBidWatch> result = GetBidWatchForUser(user_id, event_id);
      List<object> res = new List<object>();
      string[] col = new string[6];
      foreach (UserBidWatch item in result)
      {
        switch (item.Option)
        {
          case 0:
            col[0] = "<span style=\"color:red\">" + item.HighBidder + "</span>";
            col[1] = "<span style=\"color:red\">" + item.CurrentBid + "</span>";
            col[2] = "<span style=\"color:red\">" + item.Amount.GetCurrency() + "</span>";
            col[3] = "<span style=\"color:red\">" + item.MaxBid.GetCurrency() + "</span>";
            break;
          case 1:
            col[0] = "<span style=\"color:green\">" + item.HighBidder + "</span>";
            col[1] = "<span style=\"color:green\">" + item.CurrentBid + "</span>";
            col[2] = "<span style=\"color:green\">" + item.Amount.GetCurrency() + "</span>";
            col[3] = "<span style=\"color:green\">" + item.MaxBid.GetCurrency() + "</span>";
            break;
          default:
            col[0] = item.HighBidder;
            col[1] = item.CurrentBid;
            col[2] = item.Amount.GetCurrency();
            col[3] = item.MaxBid.GetCurrency();
            break;
        }
        col[4] = item.Quantity.HasValue ? item.Quantity.Value.ToString() : String.Empty;
        col[5] = (item.Option != 2) ? "&nbsp;" : "<a href='/Account/RemoveBid/" + item.LinkParams.ID + "' onclick = 'return confirm(\'Do you really want to remove this item from your Watch list?\')' >Remove</a>";
        res.Add(new { id = item.LinkParams.ID, col1 = col[0], col2 = col[1], col3 = col[2], col4 = col[3], col5 = col[4], col6 = col[5] });
      }
      return res;
    }

    //GetTopBidForItem
    public BidCurrent GetTopBidForItem(long auction_id)
    {
      dataContext.CommandTimeout = 600000;
      return dataContext.spBid_WinningBid(auction_id).FirstOrDefault();
    }

    //GetTopBidForItem
    public BidCurrent GetTopBidForItem(long auction_id, int quantity, bool fromcache)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.BIDS, "GETTOPBIDFORITEM", new object[] { auction_id }, CachingExpirationTime.Hours_01);
      BidCurrent result = CacheRepository.Get(dco) as BidCurrent;
      if (result != null && fromcache) return new BidCurrent(result);
      dataContext.CommandTimeout = 600000;
      result = dataContext.spBid_WinningBid_Current(auction_id, quantity).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result != null ? new BidCurrent(result) : null;
    }

    //GetUsersTopBidForItem
    public BidCurrent GetUserTopBidForItem(long auction_id, long user_id, bool iscaching)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.BIDS, "GETUSERTOPBIDFORITEM", new object[] { auction_id, user_id }, CachingExpirationTime.Hours_01);
      BidCurrent result = CacheRepository.Get(dco) as BidCurrent;
      if (result != null && iscaching) return result;
      dataContext.CommandTimeout = 600000;
      result = dataContext.spBid_UserTopBid(user_id, auction_id).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result != null ? new BidCurrent(result) : null;
    }

    //DepositNeeded
    public decimal DepositNeeded(decimal amount, long user_id, bool isneedonlymaxdeposit, long event_id, long auction_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "NOTNEEDDEPOSIT", new object[] { event_id, user_id }, CachingExpirationTime.Days_01);
      bool? result = (bool?)CacheRepository.Get(dco);
      if (result.HasValue && result.Value) return 0;

      DataCacheObject dcol = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "USERWINLOTS", new object[] { event_id, user_id }, CachingExpirationTime.Hours_01);
      Dictionary<long, decimal> winlots = CacheRepository.Get(dcol) as Dictionary<long, decimal>;

      if (winlots == null || !winlots.Any())
      {
        winlots = new Dictionary<long, decimal>();
        dataContext.spBid_UserWinningBids(event_id, user_id).ToList().ForEach(q => winlots.Add(q.Auction_ID, q.MaxBid.GetValueOrDefault(0)));
        dcol.Data = winlots;
        CacheRepository.Add(dcol);
      }
      decimal maxbid = winlots.Where(q => q.Key != auction_id).Select(q => q.Value).Sum() + amount;
      if ((maxbid < Consts.TopDepositLimit && isneedonlymaxdeposit) || maxbid < Consts.BottomDepositLimit) return 0;

      decimal deposit = dataContext.fUser_GetDeposit(user_id, event_id).GetValueOrDefault(0);

      if (deposit >= Consts.TopDepositLimitAmount * 0.1M)
      {
        dco.Data = true;
        CacheRepository.Add(dco);
        return 0;
      }
      return (maxbid >= Consts.BottomDepositLimit && maxbid < Consts.TopDepositLimit ? Consts.BottomDepositLimitAmount : Consts.TopDepositLimitAmount) * 0.1M - deposit;
    }

    //UpdateUserWinLotsCache
    public void UpdateUserWinLotsCache(long user_id, long event_id, long auction_id, decimal maxbid)
    {
      DataCacheObject dcol = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "USERWINLOTS", new object[] { event_id, user_id }, CachingExpirationTime.Hours_01);
      Dictionary<long, decimal> winlots = CacheRepository.Get(dcol) as Dictionary<long, decimal>;
      if (winlots == null || !winlots.Any()) winlots = new Dictionary<long, decimal>();
      if (winlots.ContainsKey(auction_id))
        winlots[auction_id] = maxbid;
      else
        winlots.Add(auction_id, maxbid);
      dcol.Data = winlots;
      CacheRepository.Put(dcol);
    }

    //PlaceSingleBid
    public BiddingObject PlaceSingleBid(long auction_id, bool isproxy, decimal amount, long user_id, int quantity, bool isproxyraise, decimal aprice, BidCurrent prevUsersBid, BidCurrent lastTop)
    {
      BidCurrent newBid;
      bool isHighBidder = lastTop != null && lastTop.User_ID == user_id;
      if (prevUsersBid != null)
        newBid = prevUsersBid;
      else
      {
        newBid = new BidCurrent();
        dataContext.BidCurrents.InsertOnSubmit(newBid);
      }
      if (!isproxy)
      {
        newBid.Amount = amount;
        if (newBid.MaxBid > amount)
          newBid.IsProxy = true;
        else
        {
          newBid.MaxBid = amount;
          newBid.IsProxy = false;
        }
      }
      else
      {
        decimal price = (lastTop == null) ? aprice : lastTop.Amount;
        newBid.MaxBid = (newBid.MaxBid < amount) ? amount : newBid.MaxBid;
        newBid.Amount = (price + Consts.GetIncrement(price) <= newBid.MaxBid) ? price : newBid.MaxBid;
        newBid.Amount += (lastTop != null && !isHighBidder && newBid.Amount + Consts.GetIncrement(price) <= newBid.MaxBid) ? Consts.GetIncrement(price) : 0;
        newBid.IsProxy = true;
      }
      newBid.Auction_ID = auction_id;
      newBid.User_ID = user_id;
      newBid.Quantity = quantity;
      newBid.DateMade = DateTime.Now;
      newBid.IP = Consts.UsersIPAddress;
      newBid.IsActive = true;

      BidLogCurrent log = new BidLogCurrent();
      dataContext.BidLogCurrents.InsertOnSubmit(log);
      log.Quantity = quantity;
      log.User_ID = newBid.User_ID;
      log.IsProxy = isproxy;
      log.MaxBid = newBid.MaxBid;
      log.Amount = newBid.Amount;
      log.IP = newBid.IP;
      log.Auction_ID = newBid.Auction_ID;
      log.DateMade = DateTime.Now;
      log.IsProxyRaise = isproxyraise;

      SubmitChages();

      return new BiddingObject { Bid = newBid, BidLog = log };
    }

    //PlaceMultipleBids
    public List<BidCurrent> PlaceMultipleBids(long auction_id, bool isproxy, decimal amount, long user_id, int quantity)
    {
      dataContext.CommandTimeout = 600000;
      List<BidCurrent> newBids = dataContext.spBid_UserTopBid(user_id, auction_id).ToList();

      List<int> newItems = new List<int>(1);

      int count = newBids.Count;

      if (newBids.Count < quantity)
      {
        for (int i = count; i < quantity; i++)
        {
          newBids.Add(new BidCurrent());
          newItems.Add(i);
        }
      }
      else if (newBids.Count > quantity)
      {
        for (int i = count - 1; i >= quantity; i--)
        {
          newBids[i].Comments = String.Format("(-/-)");
          newBids[i].Quantity = 0;
          newBids[i].MaxBid = 0;
          newBids[i].IsActive = false;
        }
        newBids.RemoveRange(quantity, newBids.Count - quantity);
      }

      count = 0;
      BidLogCurrent log;
      foreach (BidCurrent bid in newBids)
      {
        bid.IsProxy = isproxy;
        bid.Amount = (bid.Amount <= amount) ? amount : bid.Amount;
        bid.MaxBid = amount * quantity;
        bid.Auction_ID = auction_id;
        bid.User_ID = user_id;
        bid.Quantity = quantity;
        bid.DateMade = DateTime.Now;
        bid.IsActive = true;
        bid.IP = Consts.UsersIPAddress;
        bid.ItemNumber = ++count;
        bid.Comments = String.Format("({0}/{1})", bid.ItemNumber, quantity);

        log = new BidLogCurrent();
        dataContext.BidLogCurrents.InsertOnSubmit(log);
        log.Quantity = bid.Quantity;
        log.User_ID = bid.User_ID;
        log.IsProxy = bid.IsProxy;
        log.MaxBid = bid.MaxBid;
        log.Amount = bid.Amount;
        log.IP = bid.IP;
        log.Auction_ID = bid.Auction_ID;
        log.DateMade = DateTime.Now;
        log.ItemNumber = bid.ItemNumber;
        log.Comments = bid.Comments;
      }
      foreach (int t in newItems)
        dataContext.BidCurrents.InsertOnSubmit(newBids[t]);

      SubmitChages();
      return newBids;
    }

    //ResolveProxyBiddingSituation
    public void ResolveProxyBiddingSituation(long auction_id, long user_id, bool isproxy, BiddingObject placedBid, BidCurrent lastTop, decimal aprice, List<BidLogCurrent> newbidlogs)
    {
      if (placedBid.Bid == null || placedBid.BidLog == null || lastTop == null) return;
      //List<BidCurrent> bids = dataContext.BidCurrents.Where(B => B.Auction_ID == auction_id && B.ID != placedBid.Bid.ID && B.User_ID != user_id).OrderBy(B3 => B3.DateMade).OrderByDescending(B2 => B2.MaxBid).OrderByDescending(B1 => B1.Amount).ToList();
      List<BidCurrent> bids = dataContext.spBid_BidsExceptCurrent(auction_id, placedBid.Bid.ID, user_id).ToList();
      if (bids.Count() == 0) return;

      decimal price = (lastTop == null) ? aprice : lastTop.Amount;
      decimal Increment = Consts.GetIncrement(price);

      BidCurrent queryBid = bids.FirstOrDefault();

      if (placedBid.Bid.IsProxy && placedBid.Bid.Amount <= queryBid.MaxBid)
      {
        if (queryBid.MaxBid + Increment >= placedBid.Bid.MaxBid)
        {
          placedBid.BidLog.Amount = placedBid.Bid.Amount = placedBid.Bid.MaxBid;
        }
        else
        {
          Increment = Consts.GetIncrement(queryBid.MaxBid);
          if (Increment + queryBid.MaxBid > placedBid.Bid.MaxBid)
            placedBid.BidLog.Amount = placedBid.Bid.Amount = placedBid.Bid.MaxBid;
          else if (placedBid.Bid.User_ID != lastTop.User_ID)
            placedBid.BidLog.Amount = placedBid.Bid.Amount = queryBid.MaxBid + Increment;
        }
      }
      foreach (BidCurrent runner in bids)
      {
        if (!runner.IsProxy) continue;
        if (runner.MaxBid == placedBid.Bid.MaxBid)
          runner.Amount = placedBid.Bid.MaxBid;
        else
        {
          Increment = Consts.GetIncrement(placedBid.Bid.MaxBid);
          if (runner.MaxBid >= placedBid.Bid.MaxBid + Increment)
            runner.Amount = placedBid.Bid.MaxBid + Increment;
          else
          {
            if (runner.Amount != runner.MaxBid)
            {
              BidLogCurrent log = new BidLogCurrent();
              dataContext.BidLogCurrents.InsertOnSubmit(log);
              log.Quantity = runner.Quantity;
              log.User_ID = runner.User_ID;
              log.IsProxy = runner.IsProxy;
              log.MaxBid = runner.MaxBid;
              log.Amount = runner.MaxBid;
              log.IP = runner.IP;
              log.Auction_ID = runner.Auction_ID;
              log.DateMade = DateTime.Now;
              log.IsProxyRaise = false;
              newbidlogs.Add(log);
            }
            runner.Amount = runner.MaxBid;
          }
        }
      }
      SubmitChages();
    }

    //UpdateUsersTopBidCache
    public void UpdateUsersTopBidCache(long auction_id, long user_id, BidCurrent bid)
    {
      CacheRepository.Put(new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.BIDS, "GETUSERTOPBIDFORITEM", new object[] { auction_id, user_id }, CachingExpirationTime.Hours_01, bid));
    }

    //RemoveUsersTopBidCache
    public void RemoveUsersTopBidCache(long auction_id, long user_id)
    {
      CacheRepository.Remove(new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.BIDS, "GETUSERTOPBIDFORITEM", new object[] { auction_id, user_id }));
    }

    //UpdateTopBidForItemCache
    public void UpdateTopBidForItemCache(long auction_id, BidCurrent bid)
    {
      CacheRepository.Put(new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.BIDS, "GETTOPBIDFORITEM", new object[] { auction_id }, CachingExpirationTime.Hours_01, bid));
    }

    //RemoveTopBidForItemCache
    public void RemoveTopBidForItemCache(long auction_id)
    {
      CacheRepository.Remove(new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.BIDS, "GETTOPBIDFORITEM", new object[] { auction_id }));
    }

    //BiddingForSingleAuction
    /// <summary>
    /// Add bid, resolve proxy bidding situation
    /// </summary>
    /// <returns> 0 - winner, 1 - outbidder, 2 - update bid, 3 - wrong bid</returns>
    public byte BiddingForSingleAuction(AuctionDetail auction, BidCurrent currentBid, out BidCurrent previousBid, out BidCurrent loserBid, out BidCurrent winnerBid)
    {
      long? bid_id = -1;
      previousBid = loserBid = winnerBid = null;

      List<BidCurrent> allbids = dataContext.spBid_LotBids(auction.LinkParams.ID).ToList();

      // no bids
      if (!allbids.Any())
      {
        currentBid.Amount = currentBid.IsProxy ? auction.Price : currentBid.MaxBid;
        dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID, currentBid.Amount,
                                                    currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                                                    currentBid.IsProxy, false, currentBid.IsActive, null, null, null,
                                                    null, null, null, null, null, null, null, null, currentBid.User_ID,
                                                    currentBid.Amount, currentBid.MaxBid);
        currentBid.ID = bid_id.GetValueOrDefault(-1);
        winnerBid = new BidCurrent(currentBid);
        return 0;
      }

      previousBid = allbids.FirstOrDefault(q => q.User_ID == currentBid.User_ID);

      // the duplicate
      if (previousBid != null)
      {
        currentBid.ID = previousBid.ID;
        if (previousBid.IsProxy == currentBid.IsProxy && previousBid.MaxBid >= currentBid.Amount) return 3;
      }

      BidCurrent topBid = allbids.First();

      // current is highbidder
      if (topBid.User_ID == currentBid.User_ID)
      {
        currentBid.Amount = (!currentBid.IsProxy && currentBid.Amount > topBid.Amount)
                              ? currentBid.Amount
                              : topBid.Amount;
        currentBid.MaxBid = Math.Max(currentBid.MaxBid, topBid.MaxBid);
        currentBid.IsProxy = currentBid.Amount <= currentBid.MaxBid;
        bid_id = topBid.ID;
        dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID, currentBid.Amount,
                                                    currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                                                    currentBid.IsProxy, true, currentBid.IsActive, null, null, null,
                                                    null, null, null, null, null, null, null, null, currentBid.User_ID,
                                                    currentBid.Amount, currentBid.MaxBid);
        currentBid.ID = bid_id.GetValueOrDefault(-1);
        previousBid = new BidCurrent(topBid);
        winnerBid = new BidCurrent(currentBid);
        return 2;
      }

      decimal amount;
      bool isautobid;
      // current max bid is bigger than the top bid
      if (topBid.MaxBid < currentBid.MaxBid)
      {
        amount = currentBid.IsProxy ? Math.Min(topBid.Amount + Consts.GetIncrement(topBid.Amount), currentBid.MaxBid) : currentBid.MaxBid;
        isautobid = topBid.Amount != topBid.MaxBid;
        topBid.Amount = topBid.MaxBid;
        currentBid.Amount = !isautobid ? amount : (currentBid.IsProxy ? Math.Min(topBid.Amount + Consts.GetIncrement(topBid.Amount), currentBid.MaxBid) : currentBid.MaxBid);
        bid_id = previousBid == null ? -1 : currentBid.ID;
        if (isautobid)
          dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID, amount, currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                                                      currentBid.IsProxy, false, currentBid.IsActive, currentBid.Amount, currentBid.MaxBid, DateTime.Now,
                                                      topBid.ID, topBid.User_ID, topBid.Amount, topBid.MaxBid, topBid.DateMade, DateTime.Now, topBid.IP, topBid.IsProxy,
                                                      currentBid.User_ID, currentBid.Amount, currentBid.MaxBid);
        else
          dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID, currentBid.Amount,
                                                   currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                                                   currentBid.IsProxy, false, currentBid.IsActive, null, null, null, null, null, null, null, null, null, null, null,
                                                   currentBid.User_ID, currentBid.Amount, currentBid.MaxBid);
        currentBid.ID = bid_id.GetValueOrDefault(-1);
        loserBid = new BidCurrent(topBid);
        winnerBid = new BidCurrent(currentBid);
        return 0;
      }

      amount = currentBid.IsProxy ? Math.Min(topBid.Amount + Consts.GetIncrement(topBid.Amount), currentBid.MaxBid) : currentBid.MaxBid;
      isautobid = amount < currentBid.MaxBid;
      currentBid.Amount = currentBid.MaxBid;
      bid_id = previousBid == null ? -1 : currentBid.ID;

      if (topBid.Amount == topBid.MaxBid || (topBid.Amount > currentBid.Amount && topBid.MaxBid > currentBid.MaxBid))
      {
        dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID, amount,
                                                    currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                                                    currentBid.IsProxy, false, currentBid.IsActive,
                                                    isautobid ? (decimal?)currentBid.Amount : null,
                                                    isautobid ? (decimal?)currentBid.MaxBid : null,
                                                    isautobid ? (DateTime?)DateTime.Now : null,
                                                    null, null, null, null, null, null, null, null, null, null, null);
      }
      else
      {
        topBid.Amount = Math.Min(currentBid.MaxBid + Consts.GetIncrement(currentBid.MaxBid), topBid.MaxBid);
        dataContext.spBid_PlaceBidsForSingleBidding(auction.LinkParams.ID, ref bid_id, currentBid.User_ID, amount,
                                                    currentBid.MaxBid, currentBid.DateMade, currentBid.IP,
                                                    currentBid.IsProxy, false, currentBid.IsActive,
                                                    isautobid ? (decimal?)currentBid.Amount : null,
                                                    isautobid ? (decimal?)currentBid.MaxBid : null,
                                                    isautobid ? (DateTime?)DateTime.Now : null,
                                                    topBid.ID, topBid.User_ID, topBid.Amount, topBid.MaxBid,
                                                    topBid.DateMade, DateTime.Now,
                                                    topBid.IP, topBid.IsProxy, topBid.User_ID, topBid.Amount, topBid.MaxBid);
      }
      currentBid.ID = bid_id.GetValueOrDefault(-1);
      loserBid = new BidCurrent(currentBid);
      winnerBid = new BidCurrent(topBid);
      return 1;
    }

    //Update
    public bool Update(BidCurrent updBid)
    {
      try
      {
        //BidCurrent b = dataContext.BidCurrents.Where(B => B.ID == updBid.ID).SingleOrDefault();
        //if (b == null) return false;
        //b.Amount = updBid.Amount;
        //b.User_ID = updBid.User_ID;
        //b.DateMade = updBid.DateMade;
        //b.IP = updBid.IP;
        //b.IsProxy = updBid.IsProxy;        
        //b.MaxBid = updBid.MaxBid;
        //b.Quantity = updBid.Quantity;
        SubmitChages();
      }
      catch (Exception ex)
      {
        Logger.LogException("[bidlogcurrent_id=" + updBid.ID + "]", ex);
        return false;
      }
      return true;
    }

    //AddBidLogCurrent
    public BidLogCurrent AddBidLogCurrent(long auction_id, int quantity, long user_id, bool isproxy, decimal amount, decimal maxamount, bool isproxyraise, string IP)
    {
      BidLogCurrent log = new BidLogCurrent();
      try
      {
        dataContext.BidLogCurrents.InsertOnSubmit(log);
        log.Quantity = quantity;
        log.User_ID = user_id;
        log.IsProxy = isproxy;
        log.MaxBid = maxamount;
        log.Amount = amount;
        log.IP = String.IsNullOrEmpty(IP) ? Consts.UsersIPAddress : IP;
        log.Auction_ID = auction_id;
        log.DateMade = DateTime.Now;
        log.IsProxyRaise = isproxyraise;
        SubmitChages();
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        return null;
      }
      return log;
    }

    //GetTopBidsForMultipleItem
    public List<BidCurrent> GetBidsForMultipleItem(long auction_id, byte iswinner, int quantity, bool fromcache)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.BIDS, "GETBIDSFORMULTIPLEITEM", new object[] { auction_id, iswinner }, CachingExpirationTime.Hours_01);
      List<BidCurrent> result = CacheRepository.Get(dco) as List<BidCurrent>;
      if (result != null && fromcache) return result;
      result = (from p in dataContext.spBid_BidsForMultipleItem(auction_id, iswinner, quantity)
                select new BidCurrent
                {
                  Amount = p.Amount,
                  Auction_ID = p.Auction_ID,
                  Bidder = p.Bidder,
                  Comments = p.Comments,
                  DateMade = p.DateMade,
                  ID = p.ID,
                  IP = p.IP,
                  IsActive = p.IsActive,
                  IsProxy = p.IsProxy,
                  ItemNumber = p.ItemNumber,
                  MaxBid = p.MaxBid,
                  Quantity = p.Quantity,
                  User_ID = p.User_ID
                }).ToList();
      if (result.Any())
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //RemoveTopBidForMultipleItemCache
    public void RemoveTopBidForMultipleItemCache(long auction_id)
    {
      CacheRepository.Remove(new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.BIDS, "GETBIDSFORMULTIPLEITEM", new object[] { auction_id, 0 }));
      CacheRepository.Remove(new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.BIDS, "GETBIDSFORMULTIPLEITEM", new object[] { auction_id, 1 }));
    }

    //GetBidsForMultipleItem
    public void GetBidsForMultipleItem(AuctionDetail auction, out List<BidCurrent> winners, out List<BidCurrent> losers)
    {
      List<BidCurrent> allbids = dataContext.spBid_LotBids(auction.LinkParams.ID).ToList();
      winners = allbids.Take(auction.Quantity).ToList();
      losers = allbids.Skip(auction.Quantity).ToList();
    }

  }
}