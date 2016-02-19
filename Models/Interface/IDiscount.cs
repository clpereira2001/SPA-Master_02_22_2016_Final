using System;

namespace Vauction.Models
{
  public interface IDiscount
  {
    long ID { get; set; }
    string Name { get; set; }
    long Type_ID { get; set; }
    long Requirement_ID { get; set; }
    long Limitation_ID { get; set; }
    int? LimitationValue { get; set; }
    decimal DiscountValue { get; set; }
    bool IsPercent { get; set; }
    bool UnlimitedTime { get; set; }
    DateTime? StartDate { get; set; }
    DateTime? EndDate { get; set; }
    bool RequiresCoupon { get; set; }
    string CouponCode { get; set; }
    bool IsActive { get; set; }
    long UserAmountLimitation { get; set; }
    string DiscountValueText { get; set; }
  }
}