using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Vauction.Models;
using Vauction.Models.CustomClasses;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Helpers;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [HandleError, CrossSessionCheck]
  public class ConsignorController : BaseController
  {
    #region init
    IAuctionRepository AuctionRepository;
    ICategoryRepository CategoryRepository;
    IImageRepository ImageRepository;
    IEventRepository EventRepository;
    private IUserRepository UserRepository;
    public ConsignorController()
    {
      AuctionRepository = dataProvider.AuctionRepository;
      CategoryRepository = dataProvider.CategoryRepository;
      ImageRepository = dataProvider.ImageRepository;
      EventRepository = dataProvider.EventRepository;
      UserRepository = dataProvider.UserRepository;
    }
    #endregion

    //Index
    [VauctionAuthorize(Roles = "Seller,SellerBuyer,Reviewer,Root,Admin"), RequireSslFilter(IsRequired = "False"), HttpGet, Compress]
    public ActionResult Index()
    {
      InitCurrentEvent();
      return View(AppHelper.CurrentUser.IsSellerType ? "Index" : "AuctionPreview", EventRepository.GetFutureEvents());
    }

    //ConsignNow
    [HttpGet, Compress]
    public ActionResult ConsignNow()
    {
      InitCurrentEvent();
      DateTime now = DateTime.Now;
      return View(new ConsignNowForm { ID = string.Format("{0}-{1}", now.ToShortDateString().Replace("/", "-"), now.Ticks) });
    }

    //ConsignNow
    [HttpPost, Compress]
    public ActionResult ConsignNow(ConsignNowForm form)
    {
      try
      {
        form.FileLinks = Directory.GetFiles(ImageRepository.GetTempImagePath(form.ID), "sf_*.*").Select(file => new FileInfo(file)).Select(fileInfo => string.Format("{0}{1}", Consts.ResourceHostName, AppHelper.TempImage(form.ID, fileInfo.Name))).ToList();
        form.Attachments = Directory.GetFiles(ImageRepository.GetTempImagePath(form.ID), "thmb_*.*").ToList();
        var isValidCaptchaValue = CaptchaController.IsValidCaptchaValue(form.CaptchaValue);
        if (!isValidCaptchaValue)
        {
          throw new Exception("Invalid captcha!");
        }
        string sendEmails = ConfigurationManager.AppSettings["ConsignorMessagesEmail"];
        List<string> emails = new List<string>(sendEmails.Split(','));
        foreach (string email in emails.Where(t => !string.IsNullOrEmpty(t)).Distinct())
        {
          Mail.SendMessageFromConsignor(email, form);
        }
        InitCurrentEvent();
      }
      catch (Exception ex)
      {
        ViewData["Error"] = ex.Message;
        form.CaptchaValue = string.Empty;
        return View(form);
      }
      return View("ConsignNowSuccess");
    }

    //GetAuctionLisingImages
    [HttpPost, Compress]
    public JsonResult GetConsignNowImages(string tmpID)
    {
      string path = ImageRepository.GetTempImagePath(tmpID);
      return JSON(Directory.GetFiles(path, "thmb_*.*").Select(file => new FileInfo(file)).Select(fileInfo => new { ID = 0, Title = fileInfo.Name.Remove(0, 5), Description = AppHelper.TempImage(tmpID, fileInfo.Name) }));
    }

    //ConsignNowUploadImage
    [HttpPost, Compress]
    public ActionResult ConsignNowUploadImage(HttpPostedFileBase fileData, string tmpID)
    {
      if (fileData == null) return Content("error");
      string path = ImageRepository.GetTempImagePath(tmpID);
      System.Drawing.Image img = System.Drawing.Image.FromStream(fileData.InputStream);
      string prefix = string.Format("{0}{1}{2}", Path.GetFileNameWithoutExtension(fileData.FileName), DateTime.Now.Ticks, Path.GetExtension(fileData.FileName));
      img.Save(Path.Combine(path, "sf_" + prefix), System.Drawing.Imaging.ImageFormat.Jpeg);
      System.Drawing.Image imgThumb = ImageRepository.ChangeImageSize(img, Consts.AuctionImageThumbnailSize, true);
      img.Dispose();
      imgThumb.Save(Path.Combine(path, "thmb_" + prefix), System.Drawing.Imaging.ImageFormat.Jpeg);
      imgThumb.Dispose();
      return Content("ok");
    }

    //ConsignNowDeleteImage
    [HttpPost, Compress]
    public JsonResult ConsignNowDeleteImage(string tmpID, string fileName)
    {
      try
      {
        FileInfo fileInfo = new FileInfo(Server.MapPath(AppHelper.TempImage(tmpID, "sf_" + fileName)));
        if (fileInfo.Exists) fileInfo.Delete();
        fileInfo = new FileInfo(Server.MapPath(AppHelper.TempImage(tmpID, "thmb_" + fileName)));
        if (fileInfo.Exists) fileInfo.Delete();
        return JSON(new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS));
      }
      catch (Exception ex)
      {
        return JSON(new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message));
      }
    }

    //AddAuction
    [VauctionAuthorize(Roles = "Seller,SellerBuyer"), HttpGet, Compress]
    public ActionResult AddAuction(long id)
    {
      SessionUser usr = AppHelper.CurrentUser;
      List<EventCategoryDetail> cats = CategoryRepository.GetAllowedCategoriesForTheEvent(id);
      ViewData["AllowedCategories"] = cats;
      string path = Server.MapPath(string.Format(AppHelper.UserImageFolder(usr.ID)));
      if (!Directory.Exists(path)) Directory.CreateDirectory(path);
      Event evnt = EventRepository.GetEventByID(id);
      AuctionListing al = new AuctionListing();
      al.Event_ID = id;
      al.Location = usr.SellerLocation;
      al.IsConsignorShip = usr.IsConsignorShip;
      al.Quantity = 1;
      al.Price = al.Reserve = al.Cost = 1;
      al.Owner_ID = usr.ID;
      al.DateIN = DateTime.Now;
      al.Auction_ID = null;
      al.InternalID = al.Descr = String.Empty;
      al.IsBold = al.IsFeatured = al.IsTaxable = false;
      al.Title = " ";
      al.Category_ID = cats != null ? cats.First().EventCategory_ID : 0;
      al = AuctionRepository.UpdateLotToAuctionListing(al);
      al.Title = String.Empty;
      al.EventCategory = null;
      al.Event = evnt;
      return View(al);
    }

    //CancelListing
    [VauctionAuthorize(Roles = "Seller,SellerBuyer,Reviewer,Root,Admin"), RequireSslFilter(IsRequired = "False"), HttpGet, Compress]
    public ActionResult CancelListing(long id, long? Auction_ID, byte? edittype)
    {
      if (id == 0 && !Auction_ID.HasValue)
        return RedirectToAction("Index", edittype.GetValueOrDefault(0) == 0 ? "Consignor" : "Home");
      AuctionRepository.DeleteAuctionListing(id, AppHelper.CurrentUser.ID);
      return edittype.GetValueOrDefault(0) == 0 ? RedirectToAction(!Auction_ID.HasValue ? "Index" : "EditAuction", "Consignor") : RedirectToAction("AuctionDetail", "Auction", new { id = Auction_ID.GetValueOrDefault(0) });
    }

    //GetAuctionLisingImages
    [VauctionAuthorize(Roles = "Seller,SellerBuyer,Reviewer,Root,Admin"), HttpGet, Compress]
    public JsonResult GetAuctionLisingImages(string sidx, string sord, int page, int rows, long? auctionlisting_id, long? auction_id)
    {
      AuctionListing al = AuctionRepository.GetAuctionListing(auctionlisting_id.GetValueOrDefault(0));
      return (!auctionlisting_id.HasValue) ? JSON(new { total = 0, page = 0, records = 0 }) : JSON(AuctionRepository.GetAuctionListingImages(auctionlisting_id.Value, auction_id, al != null ? al.Owner_ID : 0));
    }

    //ResortImages
    [HttpPost, Compress]
    public JsonResult ResortImages(long? auctionlisting_id)
    {
      return (!auctionlisting_id.HasValue) ? JSON(false) : JSON(ImageRepository.ResortImages(auctionlisting_id.Value));
    }

    //UploadPicture
    [HttpPost, Compress]
    public ActionResult UploadPicture(HttpPostedFileBase fileData, long? user_id, long? auctionlisting_id, long? auction_id)
    {
      if (fileData == null || !auctionlisting_id.HasValue || !user_id.HasValue) return Content("error");
      ImageRepository.UploadImage(fileData, user_id.Value, auctionlisting_id.Value, auction_id);
      return Content("ok");
    }

    //DeleteImage
    [VauctionAuthorize(Roles = "Seller,SellerBuyer,Reviewer,Root,Admin"), HttpPost, Compress]
    public JsonResult DeleteImage(long? image_id, long? auctionlisting_id)
    {
      return (!image_id.HasValue || !auctionlisting_id.HasValue) ? JSON(false) : JSON(ImageRepository.DeleteImage(auctionlisting_id.Value, image_id.Value));
    }

    //MoveImage
    [VauctionAuthorize(Roles = "Seller,SellerBuyer,Reviewer,Root,Admin"), HttpPost, Compress]
    public JsonResult MoveImage(long? image_id, long? auctionlisting_id, bool isup)
    {
      return (!image_id.HasValue || !auctionlisting_id.HasValue) ? JSON(false) : JSON(ImageRepository.MoveImage(auctionlisting_id.Value, image_id.Value, isup));
    }

    //SetDefaultImage
    [VauctionAuthorize(Roles = "Seller,SellerBuyer,Reviewer,Root,Admin"), HttpPost, Compress]
    public JsonResult SetDefaultImage(long? image_id, long? auctionlisting_id)
    {
      return (!image_id.HasValue || !auctionlisting_id.HasValue) ? JSON(false) : JSON(ImageRepository.SetImageAsDefault(auctionlisting_id.Value, image_id.Value));
    }

    //PreviewItem
    [VauctionAuthorize(Roles = "Seller,SellerBuyer,Reviewer,Root,Admin"), HttpPost, ValidateInput(false), Compress] //, ValidateAntiForgeryToken
    public ActionResult PreviewItem(AuctionListing al, byte? edittype)
    {
      ViewData["edittype"] = edittype.GetValueOrDefault(0);
      if (!ModelState.IsValid)
      {
        ViewData["AllowedCategories"] = CategoryRepository.GetAllowedCategoriesForTheEvent(al.Event_ID);
        return View("AddAuction", al);
      }
      al = AuctionRepository.UpdateLotToAuctionListing(al);
      if (al == null) return RedirectToAction("General", "Error");
      ViewData["ALImages"] = ImageRepository.GetAuctionListingImages(al.ID).ToList();
      return View(al);
    }

    //UpdateAuction
    [VauctionAuthorize(Roles = "Seller,SellerBuyer,Reviewer,Root,Admin"), HttpGet, Compress]
    public ActionResult UpdateAuction(long id, byte? edittype)
    {
      AuctionListing al = AuctionRepository.GetAuctionListing(id);
      if (al == null) return RedirectToAction("General", "Error");
      ViewData["AllowedCategories"] = CategoryRepository.GetAllowedCategoriesForTheEvent(al.Event_ID);
      ViewData["edittype"] = edittype.GetValueOrDefault(0);
      return View("AddAuction", al);
    }

    //SaveAuction
    [VauctionAuthorize(Roles = "Seller,SellerBuyer,Reviewer,Root,Admin"), HttpGet, Compress]
    public ActionResult SaveAuction(long id, long? Auction_ID, byte? edittype)
    {
      InitCurrentEvent();
      AuctionListing al = AuctionRepository.GetAuctionListing(id);
      if (al == null)
        return RedirectToAction("ModifyAuction", new { id = Auction_ID.GetValueOrDefault(-1), edittype = edittype });
      User user = UserRepository.GetUser(al.Owner_ID, true);
      Auction auction = AuctionRepository.SaveAuction(id, user != null ? user.CommRate_ID : AppHelper.CurrentUser.CommRate_ID);
      if (auction != null)
        ViewData["IsNew"] = !Auction_ID.HasValue;
      ViewData["edittype"] = edittype.GetValueOrDefault(0);
      return View("SaveItemConfirmation", auction);
    }

    //EditAuction
    [VauctionAuthorize(Roles = "Seller,SellerBuyer"), HttpGet, Compress]
    public ActionResult EditAuction()
    {
      ViewData["Events"] = AuctionRepository.GetConsignorEvents(AppHelper.CurrentUser.ID);
      return View();
    }

    //GetConsignedItems
    [VauctionAuthorize(Roles = "Seller,SellerBuyer"), HttpGet, Compress]
    public JsonResult GetConsignedItems(string sidx, string sord, int page, int rows, long? Lot, long? Event, string Auction, string Category, decimal? Price, decimal? Cost, decimal? Shipping, string InternalID)
    {
      SessionUser cuser = AppHelper.CurrentUser;
      return JSON(AuctionRepository.GetUsersEditableAuctionList(sidx, sord, page, rows, cuser != null ? cuser.ID : -1, Lot, Event, Auction, Category, Price, Cost, Shipping, InternalID));
    }

    //ModifyAuction
    [VauctionAuthorize(Roles = "Seller,SellerBuyer,Reviewer,Root,Admin"), HttpGet, Compress]
    public ActionResult ModifyAuction(long id, byte? edittype)
    {
      Auction auction = AuctionRepository.GetAuction(id);
      AuctionListing al = new AuctionListing();
      al.Auction_ID = id;
      al.Category_ID = auction.Category_ID;
      al.Cost = auction.Cost.Value;
      al.DateIN = auction.NotifiedOn.Value;
      al.Descr = auction.Description;
      al.Event_ID = auction.Event_ID;
      al.ID = 0;
      al.InternalID = auction.InternalID;
      al.IsBold = auction.IsBold;
      al.IsConsignorShip = auction.IsConsignorShip;
      al.IsFeatured = auction.IsFeatured;
      al.IsTaxable = auction.IsTaxable;
      al.Location = auction.Location;
      al.Owner_ID = auction.Owner_ID;
      al.Price = auction.Price;
      al.Quantity = auction.Quantity;
      al.Reserve = auction.Reserve.Value;
      al.Shipping = auction.Shipping.Value;
      al.Title = auction.Title;
      al = AuctionRepository.UpdateLotToAuctionListing(al);
      ImageRepository.GetAuctionListingImagesFromImages(al.ID, al.Auction_ID.Value, al);
      al.EventCategory = CategoryRepository.GetEventCategoryById(auction.Category_ID);
      al.Event = EventRepository.GetEventByID(al.Event_ID);
      al.User = UserRepository.GetUser(auction.Owner_ID, true);
      ViewData["AllowedCategories"] = CategoryRepository.GetAllowedCategoriesForTheEvent(al.Event_ID);
      ViewData["edittype"] = edittype.GetValueOrDefault(0);
      return View("AddAuction", al);
    }

    //ExportConsignorItemsToExcel
    [VauctionAuthorize(Roles = "Seller,SellerBuyer"), HttpPost]
    public ActionResult ExportConsignorItemsToExcel(long? lot, long? Event, string auction, string category, decimal? price, decimal? cost, decimal? shipping, string internalID)
    {
      try
      {
        DateTime dt = DateTime.Now;
        string fileName = string.Format("ExportConsignorItems_{0}_{1}{2}{3}.xlsx", dt.ToShortDateString().Replace("/", ""), dt.TimeOfDay.Hours, dt.TimeOfDay.Minutes, dt.TimeOfDay.Seconds);
        string path = Server.MapPath(Path.Combine(AppHelper.FilesRoot, fileName));
        Utils.Reports.ExcelReport.ExportAuctionConsignorItems(path, AuctionRepository.GetAuctionConsignorItems(AppHelper.CurrentUser.ID, lot, Event, auction, category, price, cost, shipping, internalID), "Export Consignor Items", "SPA", "", "SPA");
        return File(path, "application/vnd.ms-excel", fileName);
      }
      catch
      {
        return null;
      }
    }

    //DeletePendingAuction
    [VauctionAuthorize(Roles = "Seller,SellerBuyer"), HttpPost, Compress]
    public JsonResult DeletePendingAuction(long id)
    {
      return JSON(AuctionRepository.DeletePendingAuction(id));
    }

    [VauctionAuthorize(Roles = "Seller,SellerBuyer"), HttpGet, Compress]
    public ActionResult BatchImageUpload()
    {
      return View();
    }

    //UploadBatchPicture
    [HttpPost, Compress]
    public ActionResult UploadBatchPicture(HttpPostedFileBase fileData, long user_id)
    {
      if (fileData == null) return Content("error");
      ImageRepository.UploadBatchImage(fileData, user_id);
      return Content("ok");
    }

    //GetBatchImages
    [VauctionAuthorize(Roles = "Seller,SellerBuyer"), HttpGet, Compress]
    public JsonResult GetBatchImages(int page, int rows)
    {
      return JSON(AuctionRepository.GetBatchImages(page, rows, AppHelper.CurrentUser.ID));
    }

    //AsignImages
    [VauctionAuthorize(Roles = "Seller,SellerBuyer"), HttpPost, Compress]
    public JsonResult AsignImages(string images)
    {
      JavaScriptSerializer serializer = new JavaScriptSerializer();
      var imgs = serializer.Deserialize<string[]>(images);
      return JSON(AuctionRepository.AsignImages(imgs, AppHelper.CurrentUser.ID));
    }

    //DeleteBatchImages
    [VauctionAuthorize(Roles = "Seller,SellerBuyer"), HttpPost, Compress]
    public JsonResult DeleteBatchImages(string images)
    {
      JavaScriptSerializer serializer = new JavaScriptSerializer();
      var imgs = serializer.Deserialize<string[]>(images);
      return JSON(AuctionRepository.DeleteBatchImages(imgs, AppHelper.CurrentUser.ID));
    }

    //EditImageName
    [VauctionAuthorize(Roles = "Seller,SellerBuyer"), HttpPost, Compress]
    public JsonResult EditImageName(string title, string id)
    {
      return JSON(AuctionRepository.EditBatchImageName(id, title, AppHelper.CurrentUser.ID));
    }

  }
}