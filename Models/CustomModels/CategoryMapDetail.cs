using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  [Serializable]
  public class CategoryMapDetail
  {
    public long Category_ID { get; set; }
    public long CategoryMap_ID { get; set; }
    public string CategoryTitle { get; set; }
    public string CategoryDescription { get; set; }    
  }
}