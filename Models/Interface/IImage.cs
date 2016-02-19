using System;

namespace Vauction.Models
{
  public interface IImage
  {
    Int64 ID { get; set; }
    Int64 Auction_ID { get; set; }
    string PicturePath { get; set; }
    string ThumbNailPath { get; set; }
    bool Default { get; set; }
    int Order { get; set; }
    string UploadedFileName { get; set; }

    string DefaultString { get; }

    string FullThumbNailPath(Int64 Id, string folder);
    string FullPicturePath(Int64 Id, string folder);
  }
}
