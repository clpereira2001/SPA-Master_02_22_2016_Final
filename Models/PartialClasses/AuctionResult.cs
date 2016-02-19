

namespace Vauction.Models
{
  partial class AuctionResult : IAuctionResult
  {
    public override string ToString()
    {
      return string.Format("[auction_id:{0};u1:{1};u2:{2};cb1:{3};cb2:{4};mb1:{5};mb2:{6}]", Auction_ID, User_ID_1, User_ID_2, CurrentBid_1, CurrentBid_2, MaxBid_1, MaxBid_2);
    }
  }
}