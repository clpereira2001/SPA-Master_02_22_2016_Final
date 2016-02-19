using System;

namespace Vauction.Models
{
  public interface IEmail
  {
    long ID { get; set; }
    string Title { get; set; }
    long? Event_ID { get; set; }
    bool IsEnable { get; set; }
    DateTime CreatedDate { get; set; }
  }
}