using System;

namespace Vauction.Models
{
  public interface IBid
  {
    Int64 ID { get; set; }
    Int64 Auction_ID { get; set; }
    Int64 User_ID { get; set; }
    Decimal Amount { get; set; }
    Decimal MaxBid { get; set; }
    Int32 Quantity { get; set; }
    DateTime DateMade { get; set; }
    string IP { get; set; }        
    string Comments { get; set; }
    bool IsActive { get; set; }
    bool IsProxy { get; set; }
    Int32 ItemNumber { get; set; }
  }
}
