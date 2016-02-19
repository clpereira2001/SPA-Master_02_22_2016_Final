using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public interface IPayment
  {
    Int64 ID { get; set; }
    Int64 User_ID { get; set; }
    string Description { get; set; }
    Int64? Auction_ID { get; set; }
    Decimal Amount { get; set; }
    DateTime PostDate { get; set; }
    Int64 PaymentType_ID { get; set; }
    DateTime? PaidDate { get; set; }
    string CCNum { get; set; }
    string Notes { get; set; }
    string Address { get; set; }
    string AuthCode { get; set; }
    string City { get; set; }
    string State { get; set; }
    string Zip { get; set; }
    Decimal? Discount { get; set; }
    Int64? DiscountReason_ID { get; set; }
    Int32? Status { get; set; }
    Int64? UserInvoices_ID { get; set; }
    Int64? Consignments_ID { get; set; }
    string ShippingAddress { get; set; }
  }
}
