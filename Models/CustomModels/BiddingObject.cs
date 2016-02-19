using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  [Serializable]
  public class BiddingObject
  {
    public BidCurrent Bid { get; set; }
    public BidLogCurrent BidLog { get; set; }
  }
}
