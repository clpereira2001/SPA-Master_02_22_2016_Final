using System;
using System.Collections.Generic;
using System.Linq;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  public enum PaymentConfirmationType { ITEMS = 0, ILLTAKEIT = 1, DOW = 2 };

  [Serializable]
  public class PaymentConfirmation
  {
    public string TransactionID { get; set; }
    public string TransactionType { get; set; }
    public bool IsShipping { get; set; }
    public Address Address_Billing { get; set; }
    public Address Address_Shipping { get; set; }
    public PaymentConfirmationType PaymentConfirmationType { get; set; }

    public Consts.PaymentType PaymentType { get; set; }

    public decimal Deposit { get; set; }
    public decimal DepositLimit { get; set; }
    public bool IsDepositRefunded { get; set; }

    public long? DiscountCoupon_ID { get; set; }
    public string CouponDiscountValue { get; set; }
    public Consts.DiscountAssignedType DiscountAssignedType { get; set; }
    public List<InvoiceDetail> Invoices { get; set; }

    public decimal DiscountAmount
    {
      get
      {
        switch (DiscountAssignedType)
        {
          case Consts.DiscountAssignedType.Shipping:
            return HasDiscount ? Math.Max(Invoices.Sum(i => i.RealSh.GetValueOrDefault(i.Shipping) - i.Shipping), 0) : 0;
          case Consts.DiscountAssignedType.BuyerPremium:
            return HasDiscount ? Math.Max(Invoices.Sum(i => i.RealBP.GetValueOrDefault(i.BuyerPremium) - i.BuyerPremium), 0) : 0;
          default:
            return HasDiscount ? Invoices.Sum(i => i.TotalOrderDiscount.GetValueOrDefault(0)) : 0;
        }
      }
    }

    public bool HasDiscount
    {
      get { return DiscountCoupon_ID.HasValue; }
    }

    public int Quantity
    {
      get { return (Invoices != null || Invoices.Count() == 0) ? 0 : (Invoices.Count() < 2 ? Invoices.First().Quantity : Invoices.Count()); }
    }

    public decimal TotalCost
    {
      get { return (Invoices == null || Invoices.Count() == 0) ? 0 : Invoices.Sum(i => i.TotalCost).GetPrice(); }
    }

    public decimal TotalCostWithoutDiscount
    {
      get { return (Invoices == null || Invoices.Count() == 0) ? 0 : Invoices.Sum(i => i.TotalCostWithoutCouponDiscount).GetPrice(); }
    }

    public decimal TotalOrDeposit
    {
      get { return (TotalCost >= Deposit) ? Deposit : TotalCost; }
    }

    public PaymentConfirmation()
    {
      Invoices = new List<InvoiceDetail>();
      IsDepositRefunded = false;
    }
  }
}