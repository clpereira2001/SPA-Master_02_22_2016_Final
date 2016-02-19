using System;
using System.Collections.Generic;
using Vauction.Models.CustomModels;
using Vauction.Models.CustomClasses;
using Vauction.Utils.Paging;

namespace Vauction.Models
{
  public interface IAuctionRepository
  {
    Auction GetDealOfTheWeek(bool fromcache);
    IPagedList<AuctionShort> GetFeaturedList(long event_id, long? user_id, bool isregistered, int pageindex, int pagesize);
    IList<AuctionShort> GetBySimpleCriterias(AuctionFilterParams filter, bool FullRights, long user_id);
    IList<AuctionShort> GetByAuctionID(AuctionFilterParams filter, bool FullRights, long user_id);
    IList<AuctionShort> GetByCriterias(AuctionFilterParams filter, bool FullRights, long user_id);
    IPagedList<AuctionShort> GetListForEventCategory(CategoryFilterParams filter, long user_id, bool isregistered);
    IPagedList<AuctionShort> GetListForCategoryMap(CategoryFilterParams filter, long event_id, long user_id, bool isregistered);
    Auction GetAuction(Int64 id);
    AuctionResultDetail GetAuctionResultCurrent(long auction_id, bool fromcache);
    AuctionResultDetail GetAuctionResult(long auction_id, bool fromcache);
    void RemoveAuctionCache(long auction_id);
    void RemoveAuctionCacheWithoutImages(long auction_id);
    void RemoveAuctionResultsCache(long auction_id);
    AuctionDetail GetAuctionDetail(long auction_id, bool iscaching);
    AuctionDetail GetDowDetail(long auction_id, long? user_id);
    IPagedList<AuctionShort> GetFutureEventForPersShopper(long user_id, int pageindex, int pagesize);
    void RemoveAuctionFromWatchList(long user_id, long auction_ID);
    AuctionListing UpdateLotToAuctionListing(AuctionListing al);    
    AuctionListing GetAuctionListing(long id);
    Auction GetAuctionNotInListing(long auction_id);
    bool DeleteAuctionListing(long id, long user_id);
    bool CheckAndAddConsignorStatement(long user_id, long event_id);
    string GetConsignorEvents(long user_id);
    object GetUsersEditableAuctionList(string sidx, string sord, int page, int rows, long user_id, long? lot, long? event_id, string title, string category, decimal? price, decimal? cost, decimal? shipping, string internalid);
    string ExportLotGridToExcel(long user_id, long? lot, long? event_id, string auction, string сategory, decimal? price, decimal? cost, decimal? shipping, string internalID);
    List<spAuction_View_ConsignorItemsResult> GetAuctionConsignorItems(long userID, long? lot, long? eventID,
      string title, string category, decimal? price, decimal? cost, decimal? shipping, string internalID);
    JsonExecuteResult DeletePendingAuction(long auction_id);
    object GetBatchImages(int page, int rows, long user_id);
    JsonExecuteResult AsignImages(string[] filenames, long user_id);
    JsonExecuteResult DeleteBatchImages(string[] filenames, long user_id);
    JsonExecuteResult EditBatchImageName(string oldname, string newname, long user_id);
    bool AddAuctionToWatchList(long user_id, long auction_id);
    void UpdateAuctionBiddingResult(long auction_id, long user_id_1, long? user_id_2, decimal currentbid_1, decimal? currentbid_2, decimal maxbid_1, decimal? maxbid_2);
    bool RemoveReservedAuctionQuantityForAuction(long user_id, long auction_id);
    bool ReserveAuctionQuantity(long user_id, long auction_id, int quantity);
    int GetAuctionQuantityReserveForUser(long user_id, long auction_id);
    object GetAuctionListingImages(long auctionlisting_id, long? auction_id, long user_id);
    Auction SaveAuction(long auctionlisting_id, int commrate);
    bool IsUserWatchItem(long user_id, long auction_id);
    object UpdatePageResult(int method, string prms);
  }
}
