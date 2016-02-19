using System;

namespace Vauction.Models
{
  public interface IAuctionListing
  {
    Int64 ID { get; set; }
    Int64 Owner_ID { get; set; }
    DateTime DateIN { get; set; }
    Int64 Event_ID { get; set; }
    string Title { get; set; }
    string Descr { get; set; }    
    Int64 Category_ID { get; set; }
    Int32 Quantity { get; set; }
    decimal Price { get; set; }
    decimal Reserve { get; set; }
    decimal Cost { get; set; }
    bool IsTaxable { get; set; }
    string Location { get; set; }
    bool IsFeatured { get; set; }
    bool IsBold { get; set; }
    string InternalID { get; set; }
    decimal Shipping { get; set; }
    bool IsConsignorShip { get; set; }
    long? Auction_ID { get; set; }
    bool IsMultipleShipping { get; set; }
  }
}