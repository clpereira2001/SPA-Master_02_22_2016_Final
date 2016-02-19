using System;

namespace Vauction.Models
{
  [Serializable]
  public class EventCategoryDetail
  {
    public long EventCategory_ID { get; set; }
    public long Category_ID { get; set; }
    public long CategoryMap_ID { get; set; }
    public string CategoryTitle { get; set; }
    public string CategoryDescription { get; set; }
    public bool IsActive { get; set; }
    public long Event_ID { get; set; }
    public bool IsTaxable { get; set; }
  }
}