using System;

namespace Vauction.Models
{
  public interface IInvoice
  {
    Int64 ID { get; set; }
    Int64 Auction_ID { get; set; }
    Int64 User_ID { get; set; }
    DateTime DateCreated { get; set; }
    bool IsPaid { get; set; }
    decimal Amount { get; set; }
    string Comments { get; set; }
    decimal? Shipping { get; set; }
    decimal? Insurance { get; set; }
    decimal? Tax { get; set; }
    decimal? Discount { get; set; }
    Int64? DiscountReason_ID { get; set; }
    Int32 Quantity { get; set; }
    Int64? Payment_ID { get; set; }
    bool IsSent { get; set; }
    Int64? UserInvoices_ID { get; set; }
    Int64? Consignments_ID { get; set; }
    bool IsSpoiled { get; set; }
    Int64? PaymentDeposit_Id { get; set; }
    decimal? BuyerPremium { get; set; }
    decimal BuyerFee { get; }
    bool IsPickUp { get; set; }
    int Status { get; set; }
    long? Discount_ID { get; set; }
    decimal? Real_BP { get; set; }
    decimal? Real_Sh { get; set; }
    decimal? Real_Discount { get; set; }
  }
}
