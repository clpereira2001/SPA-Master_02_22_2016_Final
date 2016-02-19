using System;

namespace Vauction.Models
{
  public interface IAuctionResult
  {
    Int64 ID { get; set; }
    long Auction_ID { get; set; }
    long? User_ID_1 { get; set; }
    long? User_ID_2 { get; set; }
    decimal? CurrentBid_1 { get; set; }
    decimal? CurrentBid_2 { get; set; }
    decimal? MaxBid_1 { get; set; }
    decimal? MaxBid_2 { get; set; }
  }
}
