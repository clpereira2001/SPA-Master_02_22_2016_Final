using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public interface IEvent
  {
    Int64 ID { get; set; }
    string Title { get; set; }
    DateTime DateStart { get; set; }
    DateTime DateEnd { get; set; }
    decimal? BuyerFee { get; set; }
    decimal? EnteringFee { get; set; }
    string Description { get; set; }
    bool IsViewable { get; set; }
    bool IsClickable { get; set; }
    Int32? Ordinary { get; set; }
    bool IsCurrent { get; set; }
    bool? IsPrivate { get; set; }
    bool IsAccessable { get; }
    bool RegisterRequired { get; set; }
  }
}
