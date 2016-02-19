using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
    public interface IDiscountReason
    {
        Int64 ID { get; set; }
        string Title { get; set; }
    }
}
