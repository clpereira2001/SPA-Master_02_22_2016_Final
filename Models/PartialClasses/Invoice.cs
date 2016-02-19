using System;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  partial class Invoice : IInvoice
  {
    public decimal Total
    {
      get
      {
        decimal value = Amount +
            ((Auction.AuctionType_ID != (byte)Consts.AuctionType.DealOfTheWeek) ? (BuyerPremium.HasValue ? BuyerPremium.Value : 0) : 0) +
            (Tax.HasValue ? Tax.Value : 0) +
            (Shipping.HasValue ? Shipping.Value : 0) +
            (Insurance.HasValue ? Insurance.Value : 0) -
            (Discount.HasValue ? Discount.Value : 0);
        return value.GetPrice();
      }
    }

    public decimal TotalCostWithoutCouponDiscount
    {
      get { return Amount + ((Auction.AuctionType_ID != (byte)Consts.AuctionType.DealOfTheWeek) ? Real_BP.GetValueOrDefault(BuyerPremium.GetValueOrDefault(0)) : 0) + Real_Sh.GetValueOrDefault(Shipping.GetValueOrDefault(0)) + Insurance.GetValueOrDefault(0) + Tax.GetValueOrDefault(0) - Real_Discount.GetValueOrDefault(Discount.GetValueOrDefault(0)); }
    }

    public decimal BuyerFee
    {
      get
      {
        return (Auction.Event.BuyerFee.HasValue) ? Auction.Event.BuyerFee.Value / 100 : 0;
      }
    }
  }
}
