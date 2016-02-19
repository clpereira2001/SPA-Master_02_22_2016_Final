using System.Collections.Generic;
using System.Web;
using Vauction.Models.CustomClasses;

namespace Vauction.Models
{
  public interface IImageRepository
  {
    string GetTempImagePath(string tmpId);
    List<Image> GetAuctionImages(long id, bool iscahce);
    void RemoveImages(long auction_id);
    System.Drawing.Image ChangeImageSize(System.Drawing.Image imgPhoto, int Width, bool IsEqual);
    void UploadImage(HttpPostedFileBase file, long user_id, long auctionlisting_id, long? auction_id);
    JsonExecuteResult UploadImage(Auction auc, string file);
    void GetAuctionListingImagesFromImages(long auctionlisting_id, long auction_id, AuctionListing al);    
    JsonExecuteResult DeleteImage(long auctionlisting_id, long image_id);
    JsonExecuteResult MoveImage(long auctionlisting_id, long image_id, bool isup);
    JsonExecuteResult SetImageAsDefault(long auctionlisting_id, long image_id);
    List<AuctionListingImage> GetAuctionListingImages(long auctionlisting_id);
    JsonExecuteResult ResortImages(long auctionlisting_id);
    void UploadBatchImage(HttpPostedFileBase file, long user_id);

    // NOT DONE
    //
    //IEnumerable<IMainPageLinkItem> GetMainPictures();
    //IMainPageLinkItem GetMainPicture(Int64 position);
    //string ChangePicture(int position, string category, string image);
  }
}
