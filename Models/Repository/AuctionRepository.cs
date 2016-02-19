using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using Vauction.Utils;
using Vauction.Utils.Lib;
using Vauction.Utils.Paging;
using Vauction.Utils.Perfomance;
using Vauction.Models.CustomClasses;
using Vauction.Models.CustomModels;

namespace Vauction.Models
{
  public class AuctionRepository : IAuctionRepository
  {
    #region init
    private VauctionDataContext dataContext;
    private ICacheDataProvider CacheRepository;

    public AuctionRepository(VauctionDataContext dataContext, ICacheDataProvider cacheDataProvider)
    {
      this.dataContext = dataContext;
      CacheRepository = cacheDataProvider;
    }

    //SubmitChanges
    private void SubmitChanges()
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

    #region get auction
    //GetAuction
    public Auction GetAuction(Int64 id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTION", new object[] { id }, CachingExpirationTime.Minutes_10);
      Auction result = CacheRepository.Get(dco) as Auction;
      if (result != null) return result;
      result = dataContext.spSelect_Auction(id).SingleOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //RemoveAuctionCache
    public void RemoveAuctionCache(long auction_id)
    {
      RemoveAuctionCacheWithoutImages(auction_id);
      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.IMAGES, "GETAUCTIONIMAGES", new object[] { auction_id });
      CacheRepository.Remove(dco);
    }

    //RemoveAuctionCacheWithoutImages
    public void RemoveAuctionCacheWithoutImages(long auction_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONDETAIL", new object[] { auction_id });
      CacheRepository.Remove(dco);
      dco.Method = "GETAUCTION";
      CacheRepository.Remove(dco);
      dco.Method = "GETAUCTIONRESULTCURRENT";
      CacheRepository.Remove(dco);
      dco.Method = "GETAUCTIONRESULT";
      CacheRepository.Remove(dco);
    }

    //RemoveAuctionResultsCache
    public void RemoveAuctionResultsCache(long auction_id)
    {
      CacheRepository.Remove(new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONRESULTCURRENT", new object[] { auction_id }));
    }
    #endregion

    #region search
    // GetBySimpleCriterias > search by simple criteria - lot or title/description
    public IList<AuctionShort> GetBySimpleCriterias(AuctionFilterParams filter, bool FullRights, long user_id)
    {
      int pageindex = (filter.page > 0) ? filter.page - 1 : 0;
      long? lot; long l;
      lot = (Int64.TryParse(filter.Description, out l)) ? l : (long?)null;

      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONLISTS, "GETBYSIMPLECRITERIAS", new object[] { FullRights ? 180 : 45, !lot.HasValue ? filter.Description : null, !lot.HasValue ? filter.Title : null, lot, FullRights, user_id, pageindex, filter.PageSize }, CachingExpirationTime.Minutes_01);
      TableViewResult<AuctionShort> result = CacheRepository.Get(dco) as TableViewResult<AuctionShort>;
      if (result != null && result.TotalRecords > 0)
        return new PagedList<AuctionShort>(result.Records, pageindex, filter.PageSize, result.TotalRecords);

      result = new TableViewResult<AuctionShort>();
      int? totalrecords = 0;
      dataContext.CommandTimeout = 600000;
      result.Records =
        (from p in dataContext.spAuction_View_GetBySimpleCriterias(FullRights ? 180 : 45, !lot.HasValue ? filter.Description : null, !lot.HasValue ? filter.Title : null, lot, FullRights, user_id, pageindex, filter.PageSize, ref totalrecords)
         select new AuctionShort
         {
           CurrentBid_1 = p.CurrentBid_1,
           CurrentBid_2 = p.CurrentBid_2,
           IsBold = p.IsBold.GetValueOrDefault(false),
           IsFeatured = p.IsFeatured.GetValueOrDefault(false),
           LinkParams = new LinkParams { ID = p.Auction_ID.GetValueOrDefault(0), Lot = p.Lot.GetValueOrDefault(0), Title = p.Title, EventTitle = p.EventTitle, CategoryTitle = p.CategoryTitle },
           Price = p.Price.GetValueOrDefault(0),
           Status = p.Status.GetValueOrDefault(0),
           ThumbnailPath = p.ThumbnailPath,
           StartDate = p.StartDate.GetValueOrDefault(DateTime.Now),
           EndDate = p.EndDate.GetValueOrDefault(DateTime.Now),
           IsAccessable = p.IsAccessable.GetValueOrDefault(false),
           IsClickable = p.IsClickable.GetValueOrDefault(false),
           IsUserRegisteredForEvent = p.IsUserRegisteredForEvent.GetValueOrDefault(false)
         }).ToList();
      result.TotalRecords = totalrecords.GetValueOrDefault(0);
      if (result.TotalRecords > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return totalrecords.GetValueOrDefault(0) > 0 ? new PagedList<AuctionShort>(result.Records, pageindex, filter.PageSize, result.TotalRecords) : new PagedList<AuctionShort>(new List<AuctionShort>(), pageindex, filter.PageSize, 0);
    }

    //GetByAuctionID
    public IList<AuctionShort> GetByAuctionID(AuctionFilterParams filter, bool FullRights, long user_id)
    {
      int? totalrecords = 0;
      int pageindex = (filter.page > 0) ? filter.page - 1 : 0;
      long? lot; long l;
      lot = (Int64.TryParse(filter.Description, out l)) ? l : (long?)null;

      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONLISTS, "GETBYAUCTIONID", new object[] { null, null, null, filter.LotNo.GetValueOrDefault(-1), FullRights, user_id, pageindex, filter.PageSize }, CachingExpirationTime.Minutes_01);
      TableViewResult<AuctionShort> result = CacheRepository.Get(dco) as TableViewResult<AuctionShort>;
      if (result != null && result.TotalRecords > 0)
        return new PagedList<AuctionShort>(result.Records, pageindex, filter.PageSize, result.TotalRecords);

      result = new TableViewResult<AuctionShort>();
      dataContext.CommandTimeout = 600000;
      result.Records =
        (from p in dataContext.spAuction_View_GetBySimpleCriterias(null, null, null, filter.LotNo.GetValueOrDefault(-1), FullRights, user_id, pageindex, filter.PageSize, ref totalrecords)
         select new AuctionShort
         {
           CurrentBid_1 = p.CurrentBid_1,
           CurrentBid_2 = p.CurrentBid_2,
           IsBold = p.IsBold.GetValueOrDefault(false),
           IsFeatured = p.IsFeatured.GetValueOrDefault(false),
           LinkParams = new LinkParams { ID = p.Auction_ID.GetValueOrDefault(0), Lot = p.Lot.GetValueOrDefault(0), Title = p.Title, EventTitle = p.EventTitle, CategoryTitle = p.CategoryTitle },
           Price = p.Price.GetValueOrDefault(0),
           Status = p.Status.GetValueOrDefault(0),
           ThumbnailPath = p.ThumbnailPath,
           StartDate = p.StartDate.GetValueOrDefault(DateTime.Now),
           EndDate = p.EndDate.GetValueOrDefault(DateTime.Now),
           IsAccessable = p.IsAccessable.GetValueOrDefault(false),
           IsClickable = p.IsClickable.GetValueOrDefault(false),
           IsUserRegisteredForEvent = p.IsUserRegisteredForEvent.GetValueOrDefault(false)
         }).ToList();
      result.TotalRecords = totalrecords.GetValueOrDefault(0);
      if (result.TotalRecords > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return totalrecords.GetValueOrDefault(0) > 0 ? new PagedList<AuctionShort>(result.Records, pageindex, filter.PageSize, totalrecords.Value) : new PagedList<AuctionShort>(new List<AuctionShort>(), pageindex, filter.PageSize, 0);
    }

    //GetByCriterias
    public IList<AuctionShort> GetByCriterias(AuctionFilterParams filter, bool FullRights, long user_id)
    {
      int? totalrecords = 0;
      int pageindex = (filter.page > 0) ? filter.page - 1 : 0;

      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONLISTS, "GETBYCRITERIAS", new object[] { (filter.ClosedAuctions.HasValue && filter.ClosedAuctions.Value) ? null : (int?)30, filter.Description, filter.Title, filter.FromPrice, filter.ToPrice, FullRights, user_id, pageindex, filter.PageSize }, CachingExpirationTime.Minutes_01);
      TableViewResult<AuctionShort> result = CacheRepository.Get(dco) as TableViewResult<AuctionShort>;
      if (result != null && result.TotalRecords > 0)
        return new PagedList<AuctionShort>(result.Records, pageindex, filter.PageSize, result.TotalRecords);

      result = new TableViewResult<AuctionShort>();
      dataContext.CommandTimeout = 600000;
      result.Records =
        (from p in dataContext.spAuction_View_GetByCriterias((filter.ClosedAuctions.HasValue && filter.ClosedAuctions.Value) ? (int?)null : (int?)30, filter.Description, filter.Title, filter.FromPrice, filter.ToPrice, FullRights, user_id, pageindex, filter.PageSize, ref totalrecords)
         select new AuctionShort
         {
           CurrentBid_1 = p.CurrentBid_1,
           CurrentBid_2 = p.CurrentBid_2,
           IsBold = p.IsBold.GetValueOrDefault(false),
           IsFeatured = p.IsFeatured.GetValueOrDefault(false),
           LinkParams = new LinkParams { ID = p.Auction_ID.GetValueOrDefault(0), Lot = p.Lot.GetValueOrDefault(0), Title = p.Title, EventTitle = p.EventTitle, CategoryTitle = p.CategoryTitle },
           Price = p.Price.GetValueOrDefault(0),
           Status = p.Status.GetValueOrDefault(0),
           ThumbnailPath = p.ThumbnailPath,
           StartDate = p.StartDate.GetValueOrDefault(DateTime.Now),
           EndDate = p.EndDate.GetValueOrDefault(DateTime.Now),
           IsAccessable = p.IsAccessable.GetValueOrDefault(false),
           IsClickable = p.IsClickable.GetValueOrDefault(false),
           IsUserRegisteredForEvent = p.IsUserRegisteredForEvent.GetValueOrDefault(false)
         }).ToList();
      result.TotalRecords = totalrecords.GetValueOrDefault(0);
      if (result.TotalRecords > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return totalrecords.GetValueOrDefault(0) > 0 ? new PagedList<AuctionShort>(result.Records, pageindex, filter.PageSize, totalrecords.Value) : new PagedList<AuctionShort>(new List<AuctionShort>(), pageindex, filter.PageSize, 0);
    }
    #endregion

    #region eventcategory / category / featured
    //GetListForEventCategory
    public IPagedList<AuctionShort> GetListForEventCategory(CategoryFilterParams filter, long user_id, bool isregistered)
    {
      int pageindex = (filter.page > 0) ? filter.page - 1 : 0;
      return GetListForEventCategory(filter.Id, user_id, isregistered, pageindex, filter.PageSize);
    }
    //GetListForEventCategory
    private IPagedList<AuctionShort> GetListForEventCategory(long eventcategory_id, long user_id, bool isregistered, int pageindex, int pagesize)
    {
      int? totalrecords = 0;
      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONLISTS, "GETLISTFOREVENTCATEGORY", new object[] { eventcategory_id, isregistered, pageindex, pagesize }, isregistered ? CachingExpirationTime.Minutes_01 : CachingExpirationTime.Hours_01);
      TableViewResult<AuctionShort> result = CacheRepository.Get(dco) as TableViewResult<AuctionShort>;
      if (result != null && result.TotalRecords > 0)
        return new PagedList<AuctionShort>(result.Records, pageindex, pagesize, result.TotalRecords);

      result = new TableViewResult<AuctionShort>();
      dataContext.CommandTimeout = 600000;
      result.Records =
        (from p in dataContext.spAuction_View_EventCategory(eventcategory_id, user_id, pageindex, pagesize, ref totalrecords)
         select new AuctionShort
         {
           CurrentBid_1 = p.CurrentBid_1,
           CurrentBid_2 = p.CurrentBid_2,
           IsBold = p.IsBold.GetValueOrDefault(false),
           IsFeatured = p.IsFeatured.GetValueOrDefault(false),
           LinkParams = new LinkParams { ID = p.Auction_ID.GetValueOrDefault(0), Lot = p.Lot.GetValueOrDefault(0), Title = p.Title, EventTitle = p.EventTitle, CategoryTitle = p.CategoryTitle },
           Price = p.Price.GetValueOrDefault(0),
           Status = p.Status.GetValueOrDefault(0),
           ThumbnailPath = p.ThumbnailPath,
           StartDate = p.StartDate.GetValueOrDefault(DateTime.Now),
           EndDate = p.EndDate.GetValueOrDefault(DateTime.Now),
           IsAccessable = p.IsAccessable.GetValueOrDefault(false),
           IsClickable = p.IsClickable.GetValueOrDefault(false),
           IsUserRegisteredForEvent = p.IsUserRegisteredForEvent.GetValueOrDefault(false)
         }).ToList();
      result.TotalRecords = totalrecords.GetValueOrDefault(0);
      if (result.TotalRecords > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return totalrecords.GetValueOrDefault(0) > 0 ? new PagedList<AuctionShort>(result.Records, pageindex, pagesize, totalrecords.GetValueOrDefault(0)) : new PagedList<AuctionShort>(new List<AuctionShort>(), pageindex, pagesize, 0);
    }

    //GetListForCategoryMap
    public IPagedList<AuctionShort> GetListForCategoryMap(CategoryFilterParams filter, long event_id, long user_id, bool isregistered)
    {
      int pageindex = (filter.page > 0) ? filter.page - 1 : 0;
      return GetListForCategoryMap(filter.Id, event_id, user_id, isregistered, pageindex, filter.PageSize);
    }
    //GetListForCategoryMap
    private IPagedList<AuctionShort> GetListForCategoryMap(long categorymap_id, long event_id, long user_id, bool isregistered, int pageindex, int pagesize)
    {
      int? totalrecords = 0;

      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONLISTS, "GETLISTFORCATEGORYMAP", new object[] { event_id, categorymap_id, isregistered, pageindex, pagesize }, isregistered ? CachingExpirationTime.Minutes_01 : CachingExpirationTime.Hours_01);
      TableViewResult<AuctionShort> result = CacheRepository.Get(dco) as TableViewResult<AuctionShort>;
      if (result != null && result.TotalRecords > 0)
        return new PagedList<AuctionShort>(result.Records, pageindex, pagesize, result.TotalRecords);

      result = new TableViewResult<AuctionShort>();
      dataContext.CommandTimeout = 600000;
      result.Records =
        (from p in dataContext.spAuction_View_CategoryMap(event_id, categorymap_id, user_id, pageindex, pagesize, ref totalrecords)
         select new AuctionShort
         {
           CurrentBid_1 = p.CurrentBid_1,
           CurrentBid_2 = p.CurrentBid_2,
           IsBold = p.IsBold.GetValueOrDefault(false),
           IsFeatured = p.IsFeatured.GetValueOrDefault(false),
           LinkParams = new LinkParams { ID = p.Auction_ID.GetValueOrDefault(0), Lot = p.Lot.GetValueOrDefault(0), Title = p.Title, EventTitle = p.EventTitle, CategoryTitle = p.CategoryTitle },
           Price = p.Price.GetValueOrDefault(0),
           Status = p.Status.GetValueOrDefault(0),
           ThumbnailPath = p.ThumbnailPath,
           StartDate = p.StartDate.GetValueOrDefault(DateTime.Now),
           EndDate = p.EndDate.GetValueOrDefault(DateTime.Now),
           IsAccessable = p.IsAccessable.GetValueOrDefault(false),
           IsClickable = p.IsClickable.GetValueOrDefault(false),
           IsUserRegisteredForEvent = p.IsUserRegisteredForEvent.GetValueOrDefault(false)
         }).ToList();
      result.TotalRecords = totalrecords.GetValueOrDefault(0);
      if (result.TotalRecords > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return totalrecords.GetValueOrDefault(0) > 0 ? new PagedList<AuctionShort>(result.Records, pageindex, pagesize, totalrecords.Value) : new PagedList<AuctionShort>(new List<AuctionShort>(), pageindex, pagesize, 0);
    }

    // GetFeaturedList
    public IPagedList<AuctionShort> GetFeaturedList(long event_id, long? user_id, bool isregistered, int pageindex, int pagesize)
    {
      pageindex = (pageindex > 0) ? pageindex - 1 : 0;
      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONLISTS, "GETFEATUREDLIST", new object[] { event_id, isregistered, pageindex, pagesize }, isregistered ? CachingExpirationTime.Minutes_01 : CachingExpirationTime.Hours_01);
      TableViewResult<AuctionShort> result = CacheRepository.Get(dco) as TableViewResult<AuctionShort>;
      if (result != null && result.TotalRecords > 0)
        return new PagedList<AuctionShort>(result.Records, pageindex, pagesize, result.TotalRecords);

      result = new TableViewResult<AuctionShort>();
      int? totalrecords = 0;
      dataContext.CommandTimeout = 600000;
      result.Records = (from p in dataContext.spAuction_View_FeaturedItems(event_id, user_id, pageindex, pagesize, ref totalrecords)
                        select new AuctionShort
                        {
                          CurrentBid_1 = p.CurrentBid_1,
                          CurrentBid_2 = p.CurrentBid_2,
                          IsBold = p.IsBold.GetValueOrDefault(false),
                          IsFeatured = p.IsFeatured.GetValueOrDefault(false),
                          LinkParams = new LinkParams { ID = p.Auction_ID.GetValueOrDefault(0), Lot = p.Lot.GetValueOrDefault(0), Title = p.Title, EventTitle = p.EventTitle, CategoryTitle = p.CategoryTitle },
                          Price = p.Price.GetValueOrDefault(0),
                          Status = p.Status.GetValueOrDefault(0),
                          ThumbnailPath = p.ThumbnailPath,
                          StartDate = p.StartDate.GetValueOrDefault(DateTime.Now),
                          EndDate = p.EndDate.GetValueOrDefault(DateTime.Now),
                          IsAccessable = p.IsAccessable.GetValueOrDefault(false),
                          IsClickable = p.IsClickable.GetValueOrDefault(false),
                          IsUserRegisteredForEvent = p.IsUserRegisteredForEvent.GetValueOrDefault(false)
                        }).ToList();
      result.TotalRecords = totalrecords.GetValueOrDefault(0);
      if (result.TotalRecords > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return totalrecords.GetValueOrDefault(0) > 0 ? new PagedList<AuctionShort>(result.Records, pageindex, pagesize, totalrecords.GetValueOrDefault(0)) : new PagedList<AuctionShort>(new List<AuctionShort>(), pageindex, pagesize, 0);
    }


    #endregion

    #region update page result
    //UpdatePageResult
    public object UpdatePageResult(int method, string prms)
    {
      return null;
      //if (String.IsNullOrEmpty(prms)) return "null";
      //List<object> res = new List<object>();
      //try
      //{
      //  string[] prm = prms.Split(',');
      //  StringBuilder sb = new StringBuilder();
      //  IList<AuctionShort> result = new List<AuctionShort>();
      //  switch (method)
      //  {
      //    case 0:
      //      result = GetListForEventCategory(Convert.ToInt64(prm[0]), Convert.ToInt64(prm[1]), Convert.ToInt32(prm[2]), Convert.ToInt32(prm[3]));
      //      break;
      //    case 1:
      //      result = GetListForCategoryMap(Convert.ToInt64(prm[0]), Convert.ToInt64(prm[1]), Convert.ToInt64(prm[2]), Convert.ToInt32(prm[3]), Convert.ToInt32(prm[4]));
      //      break;
      //    default:
      //      result = GetFeaturedList(Convert.ToInt64(prm[0]), Convert.ToInt64(prm[1]), Convert.ToInt32(prm[2]), Convert.ToInt32(prm[3]));
      //      break;
      //  }
      //  string price;
      //  foreach (var item in result)
      //  {
      //    price = (Consts.IsShownOpenBidOne) ? (!item.IsUserRegisteredForEvent ? "$1.00" : item.Price.GetCurrency()) : item.Price.GetCurrency();
      //    res.Add(new { id = item.LinkParams.ID, col1 = item.IsUserRegisteredForEvent ? ((!item.HasBid) ? item.Price.GetCurrency() : item.CurrentBid) : price, col2 = ((item.IsUserRegisteredForEvent && item.HasBid) ? "Current" : "Opening") + " Bid" + ((item.HasBid && item.CurrentBid_2.HasValue && item.IsUserRegisteredForEvent) ? " Range:<br/>" : ": ") + (item.IsUserRegisteredForEvent ? (!item.HasBid ? item.Price.GetCurrency() : item.CurrentBid) : price) });
      //  }
      //}
      //catch (Exception ex)
      //{
      //  Logger.LogException("[method=" + method.ToString() + ", prms=" + prms + "]", ex);
      //}
      //return res;
    }
    #endregion

    #region personal shopper
    // GetFutureEventForPersShopper
    public IPagedList<AuctionShort> GetFutureEventForPersShopper(long user_id, int pageindex, int pagesize)
    {
      int? totalrecords = 0;
      pageindex = (pageindex > 0) ? pageindex - 1 : 0;

      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONLISTS, "GETFUTUREEVENTFORPERSSHOPPER", new object[] { user_id, pageindex, pagesize }, CachingExpirationTime.Minutes_01);
      TableViewResult<AuctionShort> result = CacheRepository.Get(dco) as TableViewResult<AuctionShort>;
      if (result != null && result.TotalRecords > 0)
        return new PagedList<AuctionShort>(result.Records, pageindex, pagesize, result.TotalRecords);

      result = new TableViewResult<AuctionShort>();
      dataContext.CommandTimeout = 600000;
      result.Records =
        (from p in dataContext.spAuction_View_PersonalShopperAuctions(user_id, pageindex, pagesize, ref totalrecords)
         select new AuctionShort
         {
           CurrentBid_1 = p.CurrentBid_1,
           CurrentBid_2 = p.CurrentBid_2,
           IsBold = p.IsBold.GetValueOrDefault(false),
           IsFeatured = p.IsFeatured.GetValueOrDefault(false),
           LinkParams = new LinkParams { ID = p.Auction_ID.GetValueOrDefault(0), Lot = p.Lot.GetValueOrDefault(0), Title = p.Title, EventTitle = p.EventTitle, CategoryTitle = p.CategoryTitle },
           Price = p.Price.GetValueOrDefault(0),
           Status = p.Status.GetValueOrDefault(0),
           ThumbnailPath = p.ThumbnailPath,
           StartDate = p.StartDate.GetValueOrDefault(DateTime.Now),
           EndDate = p.EndDate.GetValueOrDefault(DateTime.Now),
           IsAccessable = p.IsAccessable.GetValueOrDefault(false),
           IsClickable = p.IsClickable.GetValueOrDefault(false),
           IsUserRegisteredForEvent = p.IsUserRegisteredForEvent.GetValueOrDefault(false)
         }).ToList();
      result.TotalRecords = totalrecords.GetValueOrDefault(0);
      if (result.TotalRecords > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return totalrecords.GetValueOrDefault(0) > 0 ? new PagedList<AuctionShort>(result.Records, pageindex, pagesize, totalrecords.GetValueOrDefault(0)) : new PagedList<AuctionShort>(new List<AuctionShort>(), pageindex, pagesize, 0);
    }
    #endregion

    #region auction listing
    //GetAuctionNotInListing
    public Auction GetAuctionNotInListing(long auction_id)
    {
      AuctionListing al = dataContext.AuctionListings.Where(A => A.Auction_ID == auction_id).FirstOrDefault();
      return al == null ? GetAuction(auction_id) : null;
    }
    //GetAuctionListing
    public AuctionListing GetAuctionListing(long id)
    {
      return dataContext.AuctionListings.FirstOrDefault(AL => AL.ID == id);
    }
    //UpdateLotToAuctionListing
    public AuctionListing UpdateLotToAuctionListing(AuctionListing al)
    {
      AuctionListing alist;
      try
      {
        if (al.ID == 0)
        {
          dataContext.AuctionListings.InsertOnSubmit(al);
          SubmitChanges();
          return al;
        }
        else
        {
          alist = dataContext.AuctionListings.FirstOrDefault(AL => AL.ID == al.ID);
          if (alist == null)
          {
            dataContext.AuctionListings.InsertOnSubmit(al);
            SubmitChanges();
            return al;
          }
          else
          {
            alist.Auction_ID = al.Auction_ID;
            alist.Category_ID = al.Category_ID;
            alist.Cost = al.Cost;
            alist.DateIN = al.DateIN;
            alist.Descr = al.Descr;
            alist.Event_ID = al.Event_ID;
            alist.InternalID = al.InternalID;
            alist.IsBold = al.IsBold;
            alist.IsConsignorShip = al.IsConsignorShip;
            alist.IsFeatured = al.IsFeatured;
            alist.IsTaxable = al.IsTaxable;
            alist.Location = al.Location;
            alist.Owner_ID = al.Owner_ID;
            alist.Price = al.Price;
            alist.Quantity = al.Quantity;
            alist.Reserve = al.Reserve;
            alist.Shipping = al.Shipping;
            alist.Title = al.Title;
            alist.IsMultipleShipping = al.IsMultipleShipping;
            SubmitChanges();
          }
        }
      }
      catch (Exception ex)
      {
        Logger.LogException("[al.ID=" + (al != null ? al.ID : -1).ToString() + "]", ex);
        return null;
      }
      return alist;
    }
    //DeleteAuctionListing
    public bool DeleteAuctionListing(long id, long user_id)
    {
      bool result = false;
      try
      {
        AuctionListing al = GetAuctionListing(id);
        if (al != null)
        {
          dataContext.AuctionListings.DeleteOnSubmit(al);
          SubmitChanges();
        }
        string path = ImageRepository.GetAuctionImageDirForUser(user_id, id);
        if (Directory.Exists(path))
          Directory.Delete(path, true);
        result = true;
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }
      return result;
    }
    //GetAuctionListingImages
    public object GetAuctionListingImages(long auctionlisting_id, long? auction_id, long user_id)
    {
      List<AuctionListingImage> aimages = dataContext.spAuction_ListingImages(auctionlisting_id).ToList();
      var jsonData1 = new
      {
        total = 1,
        page = 0,
        records = aimages.Count(),
        rows = (
            from query in aimages
            select new
            {
              i = query.ID,
              cell = new string[] {
                query.ID.ToString(),
                String.Format("<img src='{0}' />", ImageRepository.GetAuctionImageWebForUser(user_id, query.Auction_ID, query.ThumbNailPath)),
                String.Format("<b>Image:</b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0}<br/><b>Thumbnail:</b>&nbsp;&nbsp;{1}", query.PicturePath, query.ThumbNailPath),
                query.Order.ToString()
              }
            }).ToArray()
      };
      return jsonData1;
    }
    //SaveAuction
    public Auction SaveAuction(long auctionlisting_id, int commrate)
    {
      Auction auction;
      AuctionListing al = GetAuctionListing(auctionlisting_id);
      List<Image> oldimgs = new List<Image>();
      try
      {
        if (al == null) throw new Exception("AuctionListing doesn't exist (ID=" + auctionlisting_id.ToString() + ")");
        if (!al.Auction_ID.HasValue)
        {
          auction = new Auction();
          auction.Category_ID = al.Category_ID;
          auction.Cost = al.Cost;
          auction.NotifiedOn = al.DateIN;
          auction.Description = al.Descr;
          auction.Event_ID = al.Event_ID;
          auction.InternalID = al.InternalID;
          auction.IsBold = al.IsBold;
          auction.IsConsignorShip = al.IsConsignorShip;
          auction.IsFeatured = al.IsFeatured;
          auction.IsTaxable = al.IsTaxable;
          auction.Location = al.Location;
          auction.Owner_ID = al.Owner_ID;
          auction.Price = al.Price;
          auction.IQuantity = auction.Quantity = al.Quantity;
          auction.Reserve = al.Reserve;
          auction.Shipping = al.Shipping;
          auction.Title = al.Title;
          auction.AuctionType_ID = (al.Quantity == 1) ? (long)Consts.AuctionType.Normal : (long)Consts.AuctionType.Dutch;
          auction.Priority = 999999;
          auction.CommissionRate_ID = commrate;
          auction.StartDate = al.Event.DateStart;
          auction.EndDate = al.Event.DateEnd;
          auction.Status = (byte)Consts.AuctionStatus.Pending;
          auction.IsMultipleShipping = al.Quantity > 1 && al.IsMultipleShipping;
          dataContext.Auctions.InsertOnSubmit(auction);
          Variable v = dataContext.Variables.SingleOrDefault(V => V.Name == "CurrentLotNumber");
          if (v != null)
          {
            v.Value += 1;
            auction.Lot = Convert.ToInt64(v.Value);
          }
          else auction.Lot = auction.ID;
          oldimgs = new List<Image>();
          SubmitChanges();
          if (dataContext.Auctions.Any(q => q.Category_ID == auction.Category_ID && (q.Status == (int)Consts.AuctionStatus.Open || q.Status == (int)Consts.AuctionStatus.Pending)))
          {
            IEventRepository eventRepository = new EventRepository(dataContext, CacheRepository);
            eventRepository.RemoveEventCacheForListing(auction.Event_ID);
          }
        }
        else
        {
          auction = dataContext.Auctions.SingleOrDefault(A => A.ID == al.Auction_ID.Value);
          bool isnewcategory = auction.Category_ID != al.Category_ID;
          auction.Category_ID = al.Category_ID;
          auction.Cost = al.Cost;
          auction.Description = al.Descr;
          auction.Event_ID = al.Event_ID;
          auction.InternalID = al.InternalID;
          auction.IsBold = al.IsBold;
          auction.IsConsignorShip = al.IsConsignorShip;
          auction.IsFeatured = al.IsFeatured;
          auction.IsTaxable = al.IsTaxable;
          auction.Location = al.Location;
          auction.Owner_ID = al.Owner_ID;
          auction.Price = al.Price;
          auction.IQuantity = auction.Quantity = al.Quantity;
          auction.Reserve = al.Reserve;
          auction.Shipping = al.Shipping;
          auction.Title = al.Title;
          auction.AuctionType_ID = (al.Quantity == 1) ? (long)Consts.AuctionType.Normal : (long)Consts.AuctionType.Dutch;
          auction.CommissionRate_ID = commrate;
          auction.StartDate = al.Event.DateStart;
          auction.EndDate = al.Event.DateEnd;
          auction.IsMultipleShipping = al.Quantity > 1 && al.IsMultipleShipping;
          oldimgs = dataContext.Images.Where(A => A.Auction_ID == auction.ID).ToList();
          SubmitChanges();
          if (isnewcategory && dataContext.Auctions.Any(q => q.Category_ID == auction.Category_ID && (q.Status == (int)Consts.AuctionStatus.Open || q.Status == (int)Consts.AuctionStatus.Pending)))
          {
            IEventRepository eventRepository = new EventRepository(dataContext, CacheRepository);
            eventRepository.RemoveEventCacheForListing(auction.Event_ID);
          }
        }
        int order = 0;
        string pathD = ImageRepository.GetAuctionImagePath(auction.ID);
        string pathS = ImageRepository.GetAuctionImageDirForUser(auction.Owner_ID, auctionlisting_id);
        bool res1, res2;
        List<AuctionListingImage> alimages = dataContext.spAuction_ListingImages(auctionlisting_id).ToList();
        List<Image> newimages = new List<Image>();
        foreach (AuctionListingImage ali in alimages)
        {
          Image img = new Image();
          img.Auction_ID = auction.ID;
          img.Default = ali.Default;
          img.Order = ++order;
          img.PicturePath = String.Format("{0}-{1}_{2}{3}", auction.ID, order, DateTime.Now.Ticks, ali.PicturePath.Substring(ali.PicturePath.IndexOf(".")));
          img.ThumbNailPath = "thmb_" + img.PicturePath;
          res1 = ImageRepository.CopyAuctionListingImage(Path.Combine(pathS, ali.PicturePath), Path.Combine(pathD, img.PicturePath));
          res2 = ImageRepository.CopyAuctionListingImage(Path.Combine(pathS, ali.ThumbNailPath), Path.Combine(pathD, img.ThumbNailPath));
          if (res1 && res2)
          {
            dataContext.Images.InsertOnSubmit(img);
            newimages.Add(img);
          }
        }
        SubmitChanges();
        RemoveAuctionCache(auction.ID);
        if (oldimgs.Count() > 0)
        {
          List<FileInfo> files = (new DirectoryInfo(ImageRepository.GetAuctionImagePath(auction.ID))).GetFiles().ToList();
          if (files.Count() > 0)
          {
            foreach (FileInfo fi in files)
              foreach (Image img in oldimgs)
              {
                if (String.Compare(fi.Name, img.PicturePath, true) != 0 && String.Compare(fi.Name, img.ThumbNailPath, true) != 0) continue;
                try
                {
                  fi.Delete();
                  break;
                }
                catch (IOException ex)
                {
                  Logger.LogException("Deleting old image:" + fi.FullName, ex);
                }
              }
          }
          dataContext.Images.DeleteAllOnSubmit(oldimgs);
        }
        CheckAndAddConsignorStatement(auction.Owner_ID, auction.Event_ID);
      }
      catch (Exception ex)
      {
        Logger.LogException("[auctionlisting_id=" + auctionlisting_id.ToString() + "]", ex);
        auction = null;
      }

      try
      {
        if (auction != null)
          DeleteAuctionListing(al.ID, auction.Owner_ID);
      }
      catch (Exception ex)
      {
        Logger.LogException("[deleteimages_auction.ID=" + (auction != null ? auction.ID.ToString() : "-1") + "]", ex);
      }
      return auction;
    }
    //GetConsignmentForEvent
    private Consignment GetConsignmentForEvent(long user_id, long event_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.INVOICES, "GETCONSIGNMENTFOREVENT", new object[] { user_id, event_id }, CachingExpirationTime.Days_01);
      Consignment result = CacheRepository.Get(dco) as Consignment;
      if (result != null) return result;
      result = dataContext.Consignments.Where(C => C.User_ID == user_id && C.Event_ID == event_id).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }
    //CheckAndAddConsignorStatement
    public bool CheckAndAddConsignorStatement(long user_id, long event_id)
    {
      Consignment cons = GetConsignmentForEvent(user_id, event_id);
      if (cons != null) return true;
      try
      {
        cons = new Consignment();
        dataContext.Consignments.InsertOnSubmit(cons);
        cons.User_ID = user_id;
        cons.Event_ID = event_id;
        cons.ConsDate = DateTime.Now;
        SubmitChanges();
        CacheRepository.Put(new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.INVOICES, "GETCONSIGNMENTFOREVENT", new object[] { user_id, event_id }, CachingExpirationTime.Days_01, cons));
      }
      catch (Exception ex)
      {
        Logger.LogException(String.Format("[user_id={0}; event_id={1}]", user_id, event_id), ex);
        return false;
      }
      return true;
    }
    //GetConsignorEvents
    public string GetConsignorEvents(long user_id)
    {
      var res = (from A in dataContext.Auctions
                 where A.Owner_ID == user_id && A.Status == (byte)Consts.AuctionStatus.Pending
                 orderby A.Event_ID descending
                 select A.Event).Distinct();
      StringBuilder sb = new StringBuilder();
      sb.Append(":;");
      foreach (Event evnt in res.Count() > 0 ? res.ToList() : new List<Event>())
        sb.AppendFormat("{0}:{1};", evnt.ID, evnt.Title);
      sb.Remove(sb.Length - 1, 1);
      return sb.ToString();
    }
    //GetUsersEditableAuctionList
    public object GetUsersEditableAuctionList(string sidx, string sord, int page, int rows, long user_id, long? lot, long? event_id, string title, string category, decimal? price, decimal? cost, decimal? shipping, string internalid)
    {
      int pageindex = (page > 0) ? page - 1 : 0;
      int? totalRecords = 0;
      title = String.IsNullOrEmpty(title) ? String.Empty : title.Replace(" ", "%");
      category = String.IsNullOrEmpty(category) ? String.Empty : category.Replace(" ", "%");
      internalid = String.IsNullOrEmpty(internalid) ? String.Empty : internalid.Replace(" ", "%");
      dataContext.CommandTimeout = 600000;
      var result = dataContext.spAuction_View_ConsignorItems(user_id, lot, event_id, title, category, price, cost, shipping, internalid, pageindex, rows, ref totalRecords);
      if (totalRecords.GetValueOrDefault(0) == 0) return new { total = 0, page = page, records = 0 };
      return new
      {
        total = (int)Math.Ceiling((float)totalRecords.Value / (float)rows),
        page = page,
        records = totalRecords,
        rows = (
            from query in result
            select new
            {
              i = query.Auction_ID,
              cell = new string[] {
                query.Auction_ID.ToString(),
                query.Status == (byte)Consts.AuctionStatus.Closed?query.Lot.ToString():String.Format("<a href='{0}/Consignor/ModifyAuction/{1}'>{2}</a>", AppHelper.GetSiteUrl(), query.Auction_ID, query.Lot),
                query.EventTitle,
                query.Title,
                query.FullCategoy,
                query.Price.GetCurrency(false),
                query.Cost.GetCurrency(false),
                query.Shipping.GetCurrency(false),
                query.InternalID,
                query.IsConsignorShip.ToString(),
                query.StartDate.ToString(),
                query.EndDate.ToString(),                
                query.AuctionStatus
              }
            }).ToArray()
      };
    }
    //ExportLotGridToExcel
    public string ExportLotGridToExcel(long user_id, long? lot, long? event_id, string auction, string category, decimal? price, decimal? cost, decimal? shipping, string internalID)
    {
      StringWriter sw = new StringWriter();
      try
      {
        int? totalRecords = 0;
        category = String.IsNullOrEmpty(category) ? String.Empty : category.Replace(" ", "%");
        dataContext.CommandTimeout = 600000;
        var result = dataContext.spAuction_View_ConsignorItems(user_id, lot, null, null, category, price, cost, shipping, null, 0, 1000000, ref totalRecords);
        if (totalRecords.GetValueOrDefault(0) == 0) return String.Empty;
        var res = from A in result
                  select new
                  {
                    AuctionID = A.Auction_ID,
                    Lot = A.Lot,
                    Event = A.EventTitle,
                    Auction = A.Title,
                    Category = A.FullCategoy,
                    Price = A.Price.GetCurrency(false),
                    Cost = A.Cost.GetCurrency(false),
                    Shipping = A.Shipping.GetCurrency(false),
                    InternalID = A.InternalID,
                    ConsSh = A.IsConsignorShip.GetValueOrDefault(false) ? "yes" : "no",
                    DateStart = A.StartDate.ToString(),
                    DateEnd = A.EndDate.ToString()
                  };
        var grid = new System.Web.UI.WebControls.GridView();
        grid.DataSource = res.ToList();
        grid.DataBind();
        System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
        grid.RenderControl(htw);
      }
      catch
      {
        return String.Empty;
      }
      return sw.ToString();
    }

    public List<spAuction_View_ConsignorItemsResult> GetAuctionConsignorItems(long userID, long? lot, long? eventID, string title, string category, decimal? price, decimal? cost, decimal? shipping, string internalID)
    {
      int? totalRecords = 0;
      return dataContext.spAuction_View_ConsignorItems(userID, lot, eventID, title, category, price, cost, shipping, internalID, 0, 1000000, ref totalRecords).ToList();
    }

    //DeletePendingAuction    
    public JsonExecuteResult DeletePendingAuction(long auction_id)
    {
      try
      {
        Auction auc = GetAuction(auction_id);
        if (auc == null)
          throw new Exception("The auction doesn't exist.");
        if (auc.Status != (byte)Consts.AuctionStatus.Pending)
          throw new Exception("You can't delete this auction, because it's status is not Pending.");
        dataContext.CommandTimeout = 600000;
        dataContext.spAuction_Delete(auc.ID);
        CacheRepository.Remove(new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTION", new object[] { auction_id }));
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }
    //GetBatchImages
    public object GetBatchImages(int page, int rows, long user_id)
    {
      string[] files = Directory.GetFiles(ImageRepository.GetAuctionImageDirForUser(user_id, 0), "*.*");
      List<FileInfo> fi = files.Select(s => new FileInfo(s)).OrderBy(f => f.Name).ToList();

      int pageIndex = Convert.ToInt32(page) - 1;
      int pageSize = rows;
      int totalRecords = fi.Count();
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)pageSize);

      fi = fi.Skip(pageIndex * pageSize).Take(pageSize).ToList();

      return new
      {
        total = totalPages,
        page = page,
        records = totalRecords,
        rows = (
            from query in fi
            select new
            {
              i = query.Name,
              cell = new string[] {
                query.Name,
                String.Format("<img src='{0}' style='max-width:{1}px' />", ImageRepository.GetAuctionImageWebForUser(user_id, 0, query.Name), Consts.AuctionImageThumbnailSize),
                Path.GetFileNameWithoutExtension(query.FullName),
                query.CreationTime.ToString(CultureInfo.InvariantCulture)
              }
            }).ToArray()
      };
    }
    //AsignImages
    public JsonExecuteResult AsignImages(string[] filenames, long user_id)
    {
      IImageRepository imageRepository = new ImageRepository(dataContext, CacheRepository);

      StringBuilder sbE = new StringBuilder();
      StringBuilder sbO = new StringBuilder();
      StringBuilder sbNM = new StringBuilder();
      StringBuilder res = new StringBuilder();
      List<long> auctionids = new List<long>();
      JsonExecuteResult jer;
      try
      {
        string path = ImageRepository.GetAuctionImageDirForUser(user_id, 0);
        List<string> files = (filenames.Any(q => !String.IsNullOrEmpty(q)) ? filenames.Where(q => !String.IsNullOrEmpty(q)) : Directory.GetFiles(path, "*.*")).ToList();
        if (!files.Any()) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The image list is empty.");
        files.Sort();
        FileInfo fi;
        foreach (string file in files)
        {
          fi = new FileInfo(Path.Combine(path, file));
          if (!fi.Exists)
          {
            sbE.AppendLine(fi.Name + ", ");
            continue;
          }
          Regex regex = new Regex(@"^\d{1,14}([a-zA-Z])?$", RegexOptions.Singleline);
          Match match = regex.Match(Path.GetFileNameWithoutExtension(fi.FullName) ?? String.Empty);
          if (!match.Success)
          {
            sbNM.Append(fi.Name + ", ");
            continue;
          }
          regex = new Regex(@"^\d{1,14}", RegexOptions.Singleline);
          match = regex.Match(Path.GetFileNameWithoutExtension(fi.FullName) ?? String.Empty);
          long auction_id;
          Auction auc;
          if (!Int64.TryParse(match.Value, out auction_id) || (auc = dataContext.Auctions.FirstOrDefault(q => q.ID == auction_id && q.Status == (byte)Consts.AuctionStatus.Pending && q.Owner_ID == user_id)) == null)
          {
            sbNM.Append(fi.Name + ", ");
            continue;
          }
          if (dataContext.Images.Any(i => i.Auction_ID == auc.ID && fi.Name.CompareTo(i.UploadedFileName) == 0))
          {
            sbO.Append(fi.Name + ", ");
            fi.Delete();
            continue;
          }
          jer = imageRepository.UploadImage(auc, fi.FullName);
          if (jer.Type == JsonExecuteResultTypes.SUCCESS)
          {
            if (!auctionids.Contains(auc.ID)) auctionids.Add(auc.ID);
            try
            {
              fi.Delete();
            }
            catch (Exception ex)
            {
              Logger.LogException(ex);
            }
          }
          else
            Logger.LogInfo(jer.Message);
        }
        auctionids.ForEach(q => CacheRepository.Remove(new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.IMAGES, "GETAUCTIONIMAGES", new object[] { q })));
        if (sbE.Length > 0)
          res.AppendLine("<li><b>Files doesn't exist:</b> " + sbE.Remove(sbE.Length - 2, 2) + "</li>");
        if (sbNM.Length > 0)
          res.AppendLine("<li><b>Files doesn't match or lots are not in pending state:</b> " + sbNM.Remove(sbNM.Length - 2, 2) + "</li>");
        if (sbO.Length > 0)
          res.AppendLine("<li><b>Files are already assigned:</b> " + sbO.Remove(sbO.Length - 2, 2) + "</li>");
        if (res.Length > 0) throw new Exception("This is the error/warning list:<ul>" + res + "</ul>");
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }
    //DeleteBatchImages
    public JsonExecuteResult DeleteBatchImages(string[] filenames, long user_id)
    {
      try
      {
        string path = ImageRepository.GetAuctionImageDirForUser(user_id, 0);
        List<string> allfiles = Directory.GetFiles(path, "*.*").ToList();
        List<string> files = filenames.Where(q => !String.IsNullOrEmpty(q)).ToList();
        if (!files.Any())
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "Please select images at first.");
        StringBuilder sb = new StringBuilder();
        foreach (var file in files)
        {
          FileInfo fi = new FileInfo(allfiles.FirstOrDefault(q => q.EndsWith(file)));
          try
          {
            if (fi.Exists)
              fi.Delete();
          }
          catch (Exception ex1)
          {
            sb.AppendLine(file + ": " + ex1.Message);
          }
        }
        if (sb.Length > 0)
          Logger.LogInfo("ERROR (DeleteBatchImages): \n" + sb);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }
    //EditBatchImageName
    public JsonExecuteResult EditBatchImageName(string oldname, string newname, long user_id)
    {
      try
      {
        if (String.IsNullOrEmpty(oldname))
          throw new NullReferenceException("The source file name can't be null");
        if (String.IsNullOrEmpty(newname))
          throw new NullReferenceException("The destination file name can't be null");
        string path = ImageRepository.GetAuctionImageDirForUser(user_id, 0);
        FileInfo fi = new FileInfo(Path.Combine(path, oldname));
        FileInfo newfi = new FileInfo(Path.Combine(path, newname + fi.Extension));
        if (newfi.Exists)
          throw new IOException("The file with the same name already exists");
        if (!fi.Exists) throw new Exception("The file doesn't exist.");
        fi.CopyTo(newfi.FullName);
        try
        {
          fi.Delete();
        }
        catch (IOException ioex)
        {
          Logger.LogException(ioex);
        }
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    #endregion

    #region auction detail / bidding
    //GetAuctionDetail
    public AuctionDetail GetAuctionDetail(long auction_id, bool iscaching)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONDETAIL", new object[] { auction_id }, CachingExpirationTime.Days_01);
      AuctionDetail result = CacheRepository.Get(dco) as AuctionDetail;
      if (result != null && iscaching) return result;
      result = (from a in dataContext.spAuction_Detail(auction_id)
                select new AuctionDetail
                {
                  AuctionType = a.AuctionType_ID,
                  BuyerFee = a.BuyerFee.GetValueOrDefault(0),
                  Description = a.Description,
                  EndDate = a.EndDate,
                  EventDateEnd = a.EventDateEnd,
                  EventDateStart = a.EventDateStart,
                  IsClickable = a.IsClickable,
                  IsViewable = a.IsViewable,
                  IsCurrent = a.IsCurrent,
                  IsPrivate = a.IsPrivate.GetValueOrDefault(false),
                  LinkParams = new LinkParams { ID = a.Auction_ID, Event_ID = a.Event_ID, EventCategory_ID = a.EventCategory_ID, EventTitle = a.EventTitle, Lot = a.Lot, Title = a.AuctionTitle, CategoryTitle = a.Category },
                  Owner_ID = a.Owner_ID,
                  Price = a.Price,
                  Quantity = a.Quantity,
                  IQuantity = a.IQuantity,
                  Shipping = a.Shipping.GetValueOrDefault(0),
                  StartDate = a.StartDate,
                  Status = a.Status,
                  BuyPrice = a.BuyPrice,
                  IsTaxable = a.IsTaxable,
                  IsConsignorShip = a.IsConsignorShip,
                  IsSpecialInstruction = a.IsSpecialInstruction
                }).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetAuctionResultCurrent
    public AuctionResultDetail GetAuctionResultCurrent(long auction_id, bool fromcache)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONRESULTCURRENT", new object[] { auction_id }, CachingExpirationTime.Hours_01);
      AuctionResultDetail result = CacheRepository.Get(dco) as AuctionResultDetail;
      if (result != null && fromcache) return new AuctionResultDetail(result);
      result = (from a in dataContext.spAuction_ResultDetail_Current(auction_id)
                select new AuctionResultDetail
                {
                  Auction_ID = a.Auction_ID,
                  CurrentBid_1 = a.CurrentBid_1,
                  CurrentBid_2 = a.CurrentBid_2,
                  HighBidder_1 = a.HighBidder_1,
                  HighBidder_2 = a.HighBidder_2,
                  User_ID_1 = a.User_ID_1,
                  User_ID_2 = a.User_ID_2
                }).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result != null ? new AuctionResultDetail(result) : null;
    }

    //GetAuctionResult
    public AuctionResultDetail GetAuctionResult(long auction_id, bool fromcache)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETAUCTIONRESULT", new object[] { auction_id }, CachingExpirationTime.Hours_01);
      AuctionResultDetail result = CacheRepository.Get(dco) as AuctionResultDetail;
      if (result != null && fromcache) return new AuctionResultDetail(result);
      result = (from a in dataContext.spAuction_ResultDetail(auction_id)
                select new AuctionResultDetail
                {
                  Auction_ID = a.Auction_ID,
                  CurrentBid_1 = a.CurrentBid_1,
                  CurrentBid_2 = a.CurrentBid_2,
                  HighBidder_1 = a.HighBidder_1,
                  HighBidder_2 = a.HighBidder_2,
                  User_ID_1 = a.User_ID_1,
                  User_ID_2 = a.User_ID_2
                }).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result != null ? new AuctionResultDetail(result) : null;
    }

    //UpdateAuctionBiddingResult
    public void UpdateAuctionBiddingResult(long auction_id, long user_id_1, long? user_id_2, decimal currentbid_1, decimal? currentbid_2, decimal maxbid_1, decimal? maxbid_2)
    {
      try
      {
        //AuctionResult ar = dataContext.AuctionResults.FirstOrDefault(q => q.Auction_ID == auction_id);
        //if (ar == null)
        //{
        //  ar = new AuctionResult();
        //  dataContext.AuctionResults.InsertOnSubmit(ar);
        //}
        //ar.Auction_ID = auction_id;
        //ar.User_ID_1 = user_id_1;
        //ar.User_ID_2 = user_id_2;
        //ar.CurrentBid_1 = currentbid_1;
        //ar.CurrentBid_2 = currentbid_2;
        //ar.MaxBid_1 = maxbid_1;
        //ar.MaxBid_2 = maxbid_2;

        //SubmitChanges();
        dataContext.spAuctionResultsCurrent_Update(auction_id, user_id_1, currentbid_1, maxbid_1, user_id_2, currentbid_2, maxbid_2);
        RemoveAuctionResultsCache(auction_id);
      }
      catch (Exception ex)
      {
        Logger.LogException(String.Format("[auction_id={0}; user_1={1}; user_2={2}; cb_1={3}; cb_2={4}", auction_id, user_id_1, user_id_2, currentbid_1, currentbid_2), ex);
      }
    }

    //IsUserWatchItem
    private BidWatchCurrent GetBidWatch(long user_id, long auction_id, bool iscache)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.WATCHLISTS, "GETBIDWATCH", new object[] { user_id, auction_id }, CachingExpirationTime.Days_01);
      BidWatchCurrent result = CacheRepository.Get(dco) as BidWatchCurrent;
      if (result != null && iscache) return new BidWatchCurrent(result);
      result = dataContext.spSelect_BidWatchCurrent(auction_id, user_id).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result != null ? new BidWatchCurrent(result) : null;
    }

    //IsUserWatchItem
    public bool IsUserWatchItem(long user_id, long auction_id)
    {
      return GetBidWatch(user_id, auction_id, true) != null;
    }

    //AddAuctionToWatchList
    public bool AddAuctionToWatchList(long user_id, long auction_id)
    {
      bool result = true;
      try
      {
        //BidWatchCurrent bw = GetBidWatch(user_id, auction_id, true);
        //if (bw != null) return true;
        BidWatchCurrent bw = new BidWatchCurrent();
        dataContext.BidWatchCurrents.InsertOnSubmit(bw);
        bw.User_ID = user_id;
        bw.Auction_ID = auction_id;
        SubmitChanges();
        CacheRepository.Put(new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.WATCHLISTS, "GETBIDWATCH", new object[] { user_id, auction_id }, CachingExpirationTime.Days_01, bw));
      }
      catch (Exception ex)
      {
        Logger.LogException(String.Format("[user_id={0}, auction_id={1}]", user_id, auction_id), ex);
        result = false;
      }
      return result;
    }

    //RemoveAuctionFromWatchList
    public void RemoveAuctionFromWatchList(long user_id, long auction_id)
    {
      try
      {
        BidWatchCurrent bw = GetBidWatch(user_id, auction_id, true);
        if (bw == null) return;
        dataContext.spDelete_BidWatchCurrent(bw.ID);
        CacheRepository.Remove(new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.WATCHLISTS, "GETBIDWATCH", new object[] { user_id, auction_id }));
      }
      catch (Exception ex)
      {
        Logger.LogException("[user_id=" + user_id.ToString() + ",auction_id=" + auction_id.ToString() + "]", ex);
      }
    }

    #endregion

    #region dow / i'll take it
    //GetDealOfTheWeek
    public Auction GetDealOfTheWeek(bool fromcache)
    {
      dataContext.CommandTimeout = 600000;
      return dataContext.spAuction_DOW_Current().FirstOrDefault();
      //DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.AUCTIONS, "GETDEALOFTHEWEEK", new object[] { }, CachingExpirationTime.Days_01);
      //Auction result = CacheRepository.Get(dco) as Auction;
      //if (fromcache) return result;
      //dataContext.CommandTimeout = 600000;
      //result = dataContext.spAuction_DOW_Current().FirstOrDefault();
      //if (result != null)
      //{
      //  dco.Data = result;
      //  CacheRepository.Add(dco);
      //}
      //return result;
    }

    //GetDowDetail
    public AuctionDetail GetDowDetail(long auction_id, long? user_id)
    {
      return (from a in dataContext.spAuction_Detail_DOW(auction_id, user_id)
              select new AuctionDetail
              {
                AuctionType = a.AuctionType_ID,
                BuyerFee = a.BuyerFee.GetValueOrDefault(0),
                //CurrentBid_1 = a.CurrentBid_1,
                //CurrentBid_2 = a.CurrentBid_2,
                Description = a.Description,
                EndDate = a.EndDate,
                EventDateEnd = a.EventDateEnd,
                EventDateStart = a.EventDateStart,
                //HighBidder_1 = a.HighBidder_1,
                //HighBidder_2 = a.HighBidder_2,
                IsClickable = a.IsClickable,
                IsViewable = a.IsViewable,
                IsCurrent = a.IsCurrent,
                IsPrivate = a.IsPrivate.GetValueOrDefault(false),
                LinkParams = new LinkParams { ID = a.Auction_ID, Event_ID = a.Event_ID, EventCategory_ID = a.EventCategory_ID, EventTitle = a.EventTitle, Lot = a.Lot, Title = a.AuctionTitle, CategoryTitle = a.Category },
                Owner_ID = a.Owner_ID,
                Price = a.Price,
                Quantity = a.Quantity,
                IQuantity = a.IQuantity,
                Shipping = a.Shipping.GetValueOrDefault(0),
                StartDate = a.StartDate,
                Status = a.Status,
                //User_ID_1 = a.User_ID_1,
                //User_ID_2 = a.User_ID_2,
                BuyPrice = a.BuyPrice,
                IsTaxable = a.IsTaxable,
                IsConsignorShip = a.IsConsignorShip
              }).FirstOrDefault();
    }
    // RemoveReservedAuctionQuantityForAuction
    public bool RemoveReservedAuctionQuantityForAuction(long user_id, long auction_id)
    {
      try
      {
        dataContext.CommandTimeout = 600000;
        dataContext.sp_Service_RemoveReservedAuctionQuantityForAuction(user_id, auction_id);
      }
      catch (Exception ex)
      {
        Logger.LogException(String.Format("[user_id={0}, auction_id={1}]", user_id, auction_id), ex);
        return false;
      }
      return true;
    }
    // ReserveAuctionQuantity
    public bool ReserveAuctionQuantity(long user_id, long auction_id, int quantity)
    {
      try
      {
        dataContext.CommandTimeout = 6000000;
        dataContext.sp_Service_ReserveAuctionQuantity(user_id, auction_id, quantity);
      }
      catch (Exception ex)
      {
        Logger.LogException(String.Format("[user_id={0}, auction_id={1}]", user_id, auction_id), ex);
        return false;
      }
      return true;
    }
    //GetAuctionQuantityReserveForUser
    public int GetAuctionQuantityReserveForUser(long user_id, long auction_id)
    {
      TableForReserve tfr = dataContext.TableForReserves.Where(r => r.User_ID == user_id && r.Auction_ID == auction_id).FirstOrDefault();
      return tfr == null ? 0 : tfr.Quantity;
    }
    #endregion

  }
}