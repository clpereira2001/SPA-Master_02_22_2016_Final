using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  public interface IAuctionListingImage
  {
    Int64 ID { get; set; }
    Int64 Auction_ID { get; set; }
    string PicturePath { get; set; }
    string ThumbNailPath { get; set; }
    bool Default { get; set; }
    int Order { get; set; }
  }
}
