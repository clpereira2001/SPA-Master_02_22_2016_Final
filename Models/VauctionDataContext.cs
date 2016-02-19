using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace Vauction.Models
{
  partial class VauctionDataContext
  {
    //[Function(Name = "dbo.spBid_WinningBid")]    
    //public ISingleResult<IBid> sp_Bid_WinningBid([Parameter(DbType = "BigInt")] System.Nullable<long> auction_id)
    //{
    //  IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), auction_id);
    //  return ((ISingleResult<IBid>)(result.ReturnValue));
    //}

    //[Function(Name = "dbo.spBid_UserTopBid")]    
    //public ISingleResult<IBid> sp_Bid_GetUsersBidForAuction([Parameter(DbType = "BigInt")] System.Nullable<long> user_id, [Parameter(DbType = "BigInt")] System.Nullable<long> auction_id)
    //{
    //  IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), user_id, auction_id);
    //  return ((ISingleResult<IBid>)(result.ReturnValue));
    //}   







    // NOT DONE




    //[Function(Name = "dbo.sp_Event_Close")]
    //public int Event_Close([Parameter(Name = "EventID", DbType = "BigInt")] System.Nullable<long> eventID)
    //{
    //  IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), eventID);
    //  return ((int)(result.ReturnValue));
    //}

    [Function(Name = "dbo.sp_GetWatchBidForUser")]
    public ISingleResult<sp_GetWatchBidForUserResult> GetWatchBidForUser([Parameter(Name = "User_ID", DbType = "BigInt")] System.Nullable<long> user_ID)
    {
      IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), user_ID);
      return ((ISingleResult<sp_GetWatchBidForUserResult>)(result.ReturnValue));
    }

    //[Function(Name = "dbo.sp_GetAuctionWinnerTable")]    
    //public ISingleResult<BidCurrent> GetDutchAuctionWinnerTable([Parameter(Name = "AuctionID", DbType = "BigInt")] System.Nullable<long> auctionID, [Parameter(Name = "IsWinner", DbType = "Int")] System.Nullable<int> IsWinner)
    //{
    //  IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), new object[] { auctionID, IsWinner });
    //  return ((ISingleResult<BidCurrent>)(result.ReturnValue));
    //}

    [Function(Name = "dbo.sp_GetHighBidderButLosers")]
    public ISingleResult<sp_GetHighBidderButLosers> GetHighBidderButLosers([Parameter(Name = "EventID", DbType = "BigInt")] System.Nullable<long> event_ID)
    {
      IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), event_ID);
      return ((ISingleResult<sp_GetHighBidderButLosers>)(result.ReturnValue));
    }

    [Function(Name = "dbo.sp_UsersWinningBids")]
    public ISingleResult<Bid> GetUsersWinningBids([Parameter(Name = "IdUser", DbType = "BigInt")] System.Nullable<long> userId, [Parameter(Name = "IdEvent", DbType = "BigInt")] System.Nullable<long> eventId)
    {
      IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), new object[] { userId, eventId });
      return ((ISingleResult<Bid>)(result.ReturnValue));
    }

    [Function(Name = "dbo.sp_UsersLosers")]
    public ISingleResult<User> GetUsersLosers([Parameter(Name = "IdEvent", DbType = "BigInt")] System.Nullable<long> idEvent)
    {
      IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), idEvent);
      return ((ISingleResult<User>)(result.ReturnValue));
    }

   
  }

  public class sp_GetHighBidderButLosers
  {
    public long Auction_ID;
    public long User_ID;
    public int Quantity;
  }
}
