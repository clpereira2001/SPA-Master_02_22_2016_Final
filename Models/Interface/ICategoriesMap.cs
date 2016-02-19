using System;

namespace Vauction.Models
{
  public interface ICategoriesMap
  {
    Int64 ID { get; set; }    
    Int64 Category_ID { get; set; }
    Int64? Parent_ID { get; set; }
    Int32? Level { get; set; }
    Int64 Owner_ID { get; set; }
    DateTime LastUpdate { get; set; }
    bool IsTaxable { get; set; }
  }
}