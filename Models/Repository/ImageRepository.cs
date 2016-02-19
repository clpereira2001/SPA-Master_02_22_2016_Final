using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.IO;
using System.Web;
using System.Data.Linq;
using Vauction.Utils;
using Vauction.Utils.Lib;
using Vauction.Models.CustomClasses;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
  public class ImageRepository : IImageRepository
  {
    #region init
    private VauctionDataContext dataContext;
    private ICacheDataProvider CacheRepository;

    public ImageRepository(VauctionDataContext dataContext, ICacheDataProvider cacheDataProvider)
    {
      this.dataContext = dataContext;
      CacheRepository = cacheDataProvider;
    }

    private void SubmitChanges()
    {
      try
      {
        dataContext.SubmitChanges();
      }
      catch (ChangeConflictException e)
      {
        Logger.LogWarning(e.Message);
        foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        {
          occ.Resolve(RefreshMode.KeepCurrentValues);
        }
      }
    }
    #endregion

    //GetAuctionImages
    public List<Image> GetAuctionImages(long id, bool iscahce)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.RESOURCE, DataCacheRegions.IMAGES, "GETAUCTIONIMAGES", new object[] { id }, CachingExpirationTime.Hours_01);
      List<Image> result = CacheRepository.Get(dco) as List<Image>;
      if (result != null && result.Count() > 0 && iscahce) return result;
      dataContext.CommandTimeout = 600000;
      result = dataContext.spAuction_Images(id).ToList();
      if (result.Count() > 0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetAuctionImageDirForUser
    public static string GetAuctionImageDirForUser(long user_id, long auctionlisting_id)
    {
      return CheckAndCreateDirectiory(HttpContext.Current.Server.MapPath(AppHelper.UserImageFolder(user_id, auctionlisting_id)));
    }

    //GetAuctionImagePath
    public static string GetAuctionImagePath(long auction_id)
    {
      return CheckAndCreateDirectiory(HttpContext.Current.Server.MapPath(AppHelper.AuctionImageFolder(auction_id)));
    }

    //GetTempImagePath
    public string GetTempImagePath(string tmpId)
    {
      return CheckAndCreateDirectiory(HttpContext.Current.Server.MapPath(AppHelper.TempImageFolder(tmpId)));
    }

    //CheckAndCreateDirectiory (path)
    private static string CheckAndCreateDirectiory(string path)
    {
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      return path;
    }

    //GetAuctionImageWebForUser
    public static string GetAuctionImageWebForUser(long user_id, long auctionlisting_id, string filename)
    {
      return AppHelper.UserImage(user_id, auctionlisting_id, filename);
    }

    //GetAuctionImagePath
    public static string GetAuctionImageWebPath(Int64 auction_id, string filename)
    {
      return Consts.GetImagePath(auction_id, true) + "/" + filename;
    }

    //ChangeImageSize
    public System.Drawing.Image ChangeImageSize(System.Drawing.Image imgPhoto, int Width, bool IsEqual)
    {
      int sourceWidth = imgPhoto.Width;
      int sourceHeight = imgPhoto.Height;
      int sourceX = 0;
      int sourceY = 0;
      int destX = 0;
      int destY = 0;

      float nPercent = 0;
      float nPercentW = 0;
      float nPercentH = 0;

      nPercentW = ((float)Width / (float)sourceWidth);
      int Height = (IsEqual) ? Width : (int)((float)nPercentW * (float)sourceHeight);
      nPercentH = ((float)Height / (float)sourceHeight);

      if (nPercentH < nPercentW)
      {
        nPercent = nPercentH;
        destX = System.Convert.ToInt16((Width - (sourceWidth * nPercent)) * 0.5);
      }
      else
      {
        nPercent = nPercentW;
        destY = System.Convert.ToInt16((Height - (sourceHeight * nPercent)) * 0.5);
      }

      int destWidth = (int)(sourceWidth * nPercent);
      int destHeight = (int)(sourceHeight * nPercent);

      System.Drawing.Bitmap bmPhoto = new System.Drawing.Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
      bmPhoto.SetResolution(imgPhoto.VerticalResolution, imgPhoto.HorizontalResolution);

      System.Drawing.Graphics grPhoto = System.Drawing.Graphics.FromImage(bmPhoto);
      grPhoto.Clear(System.Drawing.Color.White);
      grPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
      grPhoto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      grPhoto.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
      grPhoto.DrawImage(imgPhoto, new System.Drawing.Rectangle(destX, destY, destWidth, destHeight), new System.Drawing.Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), System.Drawing.GraphicsUnit.Pixel);

      grPhoto.Dispose();
      return bmPhoto;
    }

    //UploadImage
    public void UploadImage(HttpPostedFileBase file, long user_id, long auctionlisting_id, long? auction_id)
    {
      System.Drawing.Image img, imgNew, imgThumb;
      try
      {
        string path = GetAuctionImageDirForUser(user_id, auctionlisting_id);
        Auction auc = (!auction_id.HasValue) ? null : dataContext.Auctions.SingleOrDefault(a => a.ID == auction_id.Value);
        img = System.Drawing.Image.FromStream(file.InputStream);
        string prefix = (auc == null) ? DateTime.Now.Ticks.ToString() : auc.ID.ToString() + "-" + (auc.Images == null ? 0 : auc.Images.Count + 1).ToString() + "_" + DateTime.Now.Ticks.ToString();
        prefix += Path.GetExtension(file.FileName);
        int imgwidth = Math.Min(Consts.AuctionImageSize, img.Width);
        imgNew = ChangeImageSize(img, imgwidth, false);
        imgThumb = ChangeImageSize(img, Consts.AuctionImageThumbnailSize, false);
        imgNew.Save(Path.Combine(path, prefix), System.Drawing.Imaging.ImageFormat.Jpeg);
        imgThumb.Save(Path.Combine(path, "thmb_" + prefix), System.Drawing.Imaging.ImageFormat.Jpeg);
        img.Dispose();
        imgNew.Dispose();
        imgThumb.Dispose();
        AddAuctionListingImage(auctionlisting_id, prefix);
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }
    }

    //UploadImage
    public JsonExecuteResult UploadImage(Auction auc, string file)
    {
      try
      {
        if (auc == null) throw new NullReferenceException("Lot can't be null");
        string path = HttpContext.Current.Server.MapPath(CheckAndCreateDirectiory(AppHelper.AuctionImageFolder(auc.ID)));
        System.Drawing.Image img, imgNew, imgThumb;

        img = System.Drawing.Image.FromFile(file);

        string prefix = auc.ID.ToString(CultureInfo.InvariantCulture) + "-" + (auc.Images == null ? 0 : auc.Images.Count + 1).ToString(CultureInfo.InvariantCulture) + "_" + DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture);

        prefix += Path.GetExtension(file);

        imgNew = ChangeImageSize(img, Consts.AuctionImageSize, false);
        imgThumb = ChangeImageSize(img, Consts.AuctionImageThumbnailSize, false);
        imgNew.Save(Path.Combine(path, prefix), System.Drawing.Imaging.ImageFormat.Jpeg);
        imgThumb.Save(Path.Combine(path, "thmb_" + prefix), System.Drawing.Imaging.ImageFormat.Jpeg);

        img.Dispose();
        imgNew.Dispose();
        imgThumb.Dispose();

        Image imgnew = AddAuctionImage(auc.ID, prefix, Path.GetFileName(file));
        if (imgnew != null)
        {
          if (auc.Images == null) auc.Images = new EntitySet<Image>();
          auc.Images.Add(imgnew);
          SubmitChanges();
        }
      }
      catch (System.Runtime.InteropServices.ExternalException ex)
      {
        throw new System.Runtime.InteropServices.ExternalException(ex.Message);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //AddAuctionImage
    public Image AddAuctionImage(long auction_id, string image, string uploadedfilename)
    {
      Image img = new Image();
      try
      {
        List<Image> images = dataContext.spAuction_Images(auction_id).ToList();
        img.Auction_ID = auction_id;
        img.Default = !images.Any();
        img.Order = (!images.Any()) ? 1 : images[images.Count() - 1].Order + 1;
        img.PicturePath = image;
        img.ThumbNailPath = "thmb_" + image;
        img.UploadedFileName = uploadedfilename;
        dataContext.Images.InsertOnSubmit(img);
        SubmitChanges();
      }
      catch
      {
        img = null;
      }
      return img;
    }

    //AddAuctionListingImage
    public long AddAuctionListingImage(long auctionlising_id, string image)
    {
      long image_id;
      try
      {
        AuctionListingImage img = new AuctionListingImage();
        List<AuctionListingImage> images = GetAuctionListingImages(auctionlising_id);
        img.Auction_ID = auctionlising_id;
        img.Default = images.Count() == 0;
        img.Order = (images.Count() == 0) ? 1 : images[images.Count() - 1].Order + 1;
        img.PicturePath = image;
        img.ThumbNailPath = "thmb_" + image;
        dataContext.AuctionListingImages.InsertOnSubmit(img);
        SubmitChanges();
        image_id = img.ID;
      }
      catch
      {
        image_id = -1;
      }
      return image_id;
    }

    //GetAuctionListingImages
    public List<AuctionListingImage> GetAuctionListingImages(long auctionlisting_id)
    {
      return dataContext.spAuction_ListingImages(auctionlisting_id).ToList();
    }

    //GetAuctionListingImagesFromImages
    public void GetAuctionListingImagesFromImages(long auctionlisting_id, long auction_id, AuctionListing al)
    {
      List<Image> images = dataContext.spAuction_Images(auction_id).ToList();
      al.AuctionListingImages.Clear();
      bool res1, res2;
      string pathS = GetAuctionImagePath(auction_id);
      string pathD = GetAuctionImageDirForUser(al.Owner_ID, auctionlisting_id);
      foreach (Image img in images)
      {
        AuctionListingImage ali = new AuctionListingImage();
        ali.Auction_ID = auctionlisting_id;
        ali.Default = img.Default;
        ali.Order = img.Order;
        ali.PicturePath = img.PicturePath;
        ali.ThumbNailPath = img.ThumbNailPath;
        res1 = CopyAuctionListingImage(Path.Combine(pathS, img.PicturePath), Path.Combine(pathD, ali.PicturePath));
        res2 = CopyAuctionListingImage(Path.Combine(pathS, img.ThumbNailPath), Path.Combine(pathD, ali.ThumbNailPath));
        if (res1 && res2)
          al.AuctionListingImages.Add(ali);
      }
      SubmitChanges();
    }

    //MoveAuctionListingImage
    public static bool MoveAuctionListingImage(string source, string destination)
    {
      try
      {
        FileInfo fi = new FileInfo(source);
        if (!fi.Exists) return false;
        if (!Directory.Exists(Path.GetDirectoryName(destination)))
          Directory.CreateDirectory(Path.GetDirectoryName(destination));
        fi.CopyTo(destination);
        fi.Delete();
      }
      catch (Exception ex)
      {
        Logger.LogException("[source=" + source + ",dest=" + destination + "]", ex);
        return false;
      }
      return true;
    }

    //CopyAuctionListingImage
    public static bool CopyAuctionListingImage(string source, string destination)
    {
      try
      {
        FileInfo fi = new FileInfo(source);
        if (!fi.Exists) return false;
        if (!Directory.Exists(Path.GetDirectoryName(destination)))
          Directory.CreateDirectory(Path.GetDirectoryName(destination));
        fi.CopyTo(destination, true);
      }
      catch (Exception ex)
      {
        Logger.LogException("[source=" + source + ",dest=" + destination + "]", ex);
        return false;
      }
      return true;
    }

    //ResortImages
    public JsonExecuteResult ResortImages(long auctionlisting_id)
    {
      try
      {
        List<AuctionListingImage> images = GetAuctionListingImages(auctionlisting_id);
        if (images.Count() == 0) return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        int order = 0;
        images.ForEach(i => i.Order = ++order);
        images.First().Default = true;
        images.Where(i => i.Order > 1).ToList().ForEach(i2 => i2.Default = false);
        SubmitChanges();
      }
      catch (IOException ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //DeleteImage
    public JsonExecuteResult DeleteImage(long auctionlisting_id, long image_id)
    {
      try
      {
        AuctionListing auc = dataContext.AuctionListings.SingleOrDefault(A => A.ID == auctionlisting_id);
        AuctionListingImage img = dataContext.AuctionListingImages.SingleOrDefault(I => I.ID == image_id);
        List<AuctionListingImage> images = GetAuctionListingImages(auctionlisting_id);

        string path = GetAuctionImageDirForUser(auc.Owner_ID, auctionlisting_id);
        FileInfo fi = new FileInfo(Path.Combine(path, img.PicturePath));
        if (fi.Exists) fi.Delete();
        fi = new FileInfo(Path.Combine(path, img.ThumbNailPath));
        if (fi.Exists) fi.Delete();

        if (images.Count() > 1)
        {
          int order = img.Order;
          images.Where(I => I.Order > img.Order).ToList().ForEach(i => i.Order = order++);
          img.Order = int.MaxValue;
        }
        if (img.Default && images.Count > 1)
          images.OrderBy(I => I.Order).FirstOrDefault().Default = true;
        dataContext.AuctionListingImages.DeleteOnSubmit(img);
        SubmitChanges();
      }
      catch (IOException ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //MoveImage
    public JsonExecuteResult MoveImage(long auctionlisting_id, long image_id, bool isup)
    {
      try
      {
        AuctionListing auc = dataContext.AuctionListings.SingleOrDefault(A => A.ID == auctionlisting_id);
        AuctionListingImage img = dataContext.AuctionListingImages.SingleOrDefault(I => I.ID == image_id);
        List<AuctionListingImage> images = GetAuctionListingImages(auctionlisting_id);
        if (img == null || images.Count() == 1 || (img.Order == 1 && isup) || (img.Order == images.Count() && !isup))
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, String.Format("You can't move {0} this image.", ((isup) ? "up" : "down")));
        AuctionListingImage img2 = images.Where(I => ((I.Order <= img.Order && isup) || (I.Order >= img.Order && !isup)) && I.ID != img.ID).AsQueryable().OrderBy(((isup) ? "Order desc" : "Order asc")).FirstOrDefault();
        int order = img.Order;
        img.Order = img2.Order;
        img2.Order = order;

        if (isup && img2.Default && img.Order == 1)
        {
          img2.Default = false;
          img.Default = true;
        }
        else if (!isup && img.Default && img2.Order == 1)
        {
          img2.Default = true;
          img.Default = false;
        }
        SubmitChanges();
      }
      catch (IOException ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //SetImageAsDefault
    public JsonExecuteResult SetImageAsDefault(long auctionlisting_id, long image_id)
    {
      try
      {
        AuctionListing auc = dataContext.AuctionListings.SingleOrDefault(A => A.ID == auctionlisting_id);
        AuctionListingImage img = dataContext.AuctionListingImages.SingleOrDefault(I => I.ID == image_id);
        List<AuctionListingImage> images = GetAuctionListingImages(auctionlisting_id);

        if (img == null || images.Count() == 1 || img.Order == 1)
          return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
        img.Default = true;
        img.Order = 1;
        int order = 1;
        foreach (AuctionListingImage i in images)
        {
          if (i.ID == img.ID) continue;
          i.Order = ++order;
          i.Default = false;
        }
        SubmitChanges();
      }
      catch (IOException ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS);
    }

    //RemoveImages
    public void RemoveImages(long auction_id)
    {
      var query = from A in dataContext.Images
                  where A.Auction_ID == auction_id
                  select A;
      if (!query.Any()) return;
      dataContext.Images.DeleteAllOnSubmit(query);
      dataContext.SubmitChanges();
    }

    //UploadBatchImage
    public void UploadBatchImage(HttpPostedFileBase file, long user_id)
    {
      try
      {
        string path = GetAuctionImageDirForUser(user_id, 0);
        System.Drawing.Image img = System.Drawing.Image.FromStream(file.InputStream);
        FileInfo fi = new FileInfo(Path.Combine(path, file.FileName));
        if (fi.Exists) fi.Delete();
        img.Save(fi.FullName);
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }
    }

    // NOT DONE



    //public IEnumerable<IMainPageLinkItem> GetMainPictures()
    //{
    //  var query = from p in dataContext.MainPageLinkItems
    //              where p.PositionNuber > 0 && p.PositionNuber < 10
    //              select p;

    //  return query.Cast<IMainPageLinkItem>();
    //}
    //public IMainPageLinkItem GetMainPicture(Int64 position)
    //{
    //  var query = from p in dataContext.MainPageLinkItems
    //              where p.PositionNuber == position
    //              select p;
    //  if (query.Count() == 1)
    //    return query.First();
    //  return null;
    //}
    //public string ChangePicture(int position, string category, string image)
    //{
    //  var catQuery = from c in dataContext.Categories where c.Title == category select c;
    //  if (catQuery.Count() != 1)
    //    return null;

    //  var query = from p in dataContext.MainPageLinkItems where p.PositionNuber == position select p;
    //  if (query.Count() == 1)
    //  {
    //    string oldName = query.First().Picture_URL;
    //    query.First().Category_ID = catQuery.First().ID;
    //    if (image != string.Empty)
    //      query.First().Picture_URL = image;
    //    dataContext.SubmitChanges();
    //    if (image != string.Empty)
    //      return oldName;
    //    else
    //      return string.Empty;
    //  }
    //  return null;
    //}




  }
}
