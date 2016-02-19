using System;

namespace Vauction.Models
{
  public interface IDiscountUserLimitation
  {
    long ID { get; set; }
    long Discount_ID { get; set; }
    long User_ID { get; set; }
  }
}
