using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public interface IAuction
  {
    Int64 ID { get; set; }
    Int64 Category_ID { get; set; }
    byte Status { get; set; }
    bool IsFeatured { get; set; }
    DateTime? DateSold { get; set; }
    DateTime? NotifiedOn { get; set; }
    Int64 Owner_ID { get; set; }
    string Title { get; set; }
    string Description { get; set; }
    decimal Price { get; set; }
    Int32 Quantity { get; set; }
    decimal? Reserve { get; set; }
    DateTime StartDate { get; set; }
    DateTime EndDate { get; set; }
    string Location { get; set; }
    decimal? Shipping { get; set; }
    Int64 Event_ID { get; set; }
    Int64 AuctionType_ID { get; set; }
    bool IsBold { get; set; }
    Int32? Priority { get; set; }
    Int32 CommissionRate_ID { get; set; }
    string InternalID { get; set; }
    decimal? Cost { get; set; }
    Int32 IQuantity { get; set; }
    bool IsTaxable { get; set; }
    bool IsConsignorShip { get; set; }
    Int64 Lot { get; set; }
    decimal? BuyPrice { get; set; }
    long? OldAuction_ID { get; set; }
    bool IsMultipleShipping { get; set; }
    bool? IsSpecialInstruction { get; set; }
  }
}
