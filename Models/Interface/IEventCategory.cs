using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public interface IEventCategory
  {
    Int64 ID { get; set; }    
    Int64 Event_ID { get; set; }
    Int64 CategoryMap_ID { get; set; }
    Int32 Order { get; set; }
    Boolean IsActive { get; set; }
    Int64 Owner_ID { get; set; }
    DateTime LastUpdate { get; set; }
  }
}
