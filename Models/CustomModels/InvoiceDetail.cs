using System;

namespace Vauction.Models
{
  [Serializable]
  public class InvoiceDetail
  {
    public long Invoice_ID { get; set; }
    public DateTime DateCreated { get; set; }
    public decimal Amount { get; set; }
    public decimal BuyerPremium { get; set; }
    public decimal Shipping { get; set; }
    public decimal Insurance { get; set; }
    public decimal Tax { get; set; }
    public decimal Discount { get; set; }
    public long? DiscountReason_ID { get; set; }
    public decimal Total { get; set; }
    

    public int Quantity { get; set; }
    public long AuctionType { get; set; }

    public LinkParams LinkParams { get; set; }

    public bool IsConsignorShip { get; set; }
    public long? UserInvoice_ID { get; set; }

    public long? Discount_ID { get; set; }
    public decimal? RealBP { get; set; }
    public decimal? RealSh { get; set; }
    public decimal? RealDiscount { get; set; }
    public decimal? TotalOrderDiscount { get; set; }

    public decimal TotalCost
    {
      get { return Amount + BuyerPremium + Shipping + Insurance + Tax - Discount; }
    }

    public decimal TotalCostWithoutCouponDiscount
    {
      get { return Amount + RealBP.GetValueOrDefault(BuyerPremium) + RealSh.GetValueOrDefault(Shipping) + Insurance + Tax - Discount + TotalOrderDiscount.GetValueOrDefault(0); }
    }
  }
}