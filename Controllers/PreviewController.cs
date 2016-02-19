using System.Web.Mvc;
using System.Web.UI;
using Relatives.Models.CustomBinders;
using Vauction.Models;
using Vauction.Models.CustomModels;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
  [CrossSessionCheck, VauctionAuthorize(Roles = "Seller,SellerBuyer,Reviewer,Root,Admin")]
  public class PreviewController : BaseController
  {
    #region init
    IImageRepository ImageRepository;
    IAuctionRepository AuctionRepository;
    ICategoryRepository CategoryRepository;
    IEventRepository EventRepository;
    public PreviewController()
    {
      AuctionRepository = dataProvider.AuctionRepository;
      ImageRepository = dataProvider.ImageRepository;
      CategoryRepository = dataProvider.CategoryRepository;
      EventRepository = dataProvider.EventRepository;
    }
    #endregion

    //Index
    [HttpGet, Compress]
    public ActionResult Index([ModelBinder(typeof(CustomItemBinder))]AuctionFilterParams param, long event_id)
    {
      EventDetail ev = EventRepository.GetEventDetail(event_id);
      if (ev == null) return RedirectToAction("General", "Error");
      ViewData["CurrentEvent"] = ev;
      ViewData["PageActionPath"] = "Index";
      ViewData["HidePager"] = true;
      ViewData["HidePreview"] = true;      
      ViewData["DEMO_MODE"] = true;
      SetFilterParams(param);
      param.ViewMode = (int)Consts.AuctonViewMode.Grid;
      return View(AuctionRepository.GetFeaturedList(ev.ID, AppHelper.CurrentUser.ID, false, 0, 21));      
    }

    //EventDetailed
    [HttpGet, Compress]
    public ActionResult EventDetailed([ModelBinder(typeof(CustomItemBinder))]AuctionFilterParams param)
    {
      ViewData["DEMO_MODE"] = true;
      if (!param.ID.HasValue) return RedirectToAction("Index", "Preview");
      EventDetail ev = EventRepository.GetEventDetail(param.ID.Value);
      if (ev == null) return RedirectToAction("Index", "Preview");            
      ViewData["CurrentEvent"] = ev;
      ViewData["PageActionPath"] = "EventDetailed";
      SetFilterParams(param);      
      return View(AuctionRepository.GetFeaturedList(ev.ID, AppHelper.CurrentUser.ID, false, param.page, param.PageSize));
    }

    //EventCategory
    [HttpGet, Compress]
    public ActionResult EventCategory([ModelBinder(typeof(CustomItemBinder))]CategoryFilterParams param)
    {
      ViewData["DEMO_MODE"] = true;
      //EventCategory ec = CategoryRepository.GetEventCategoryById(param.Id);
      EventCategoryDetail ec = CategoryRepository.GetEventCategoryDetail(param.Id);
      if (ec == null) return RedirectToAction("Index", "Preview");
      ViewData["PageActionPath"] = "EventCategory";
      ViewData["CurrentCategory"] = ec;
      ViewData["CurrentEvent"] = EventRepository.GetEventDetail(ec.Event_ID);
      SetFilterParams(param);
      SessionUser cuser = AppHelper.CurrentUser;
      return View(AuctionRepository.GetListForEventCategory(param, cuser == null ? -1 : cuser.ID, true));
    }

    //Category
    [HttpGet, Compress]
    public ActionResult Category([ModelBinder(typeof(CustomItemBinder))]CategoryFilterParams param, long? e)
    {
      ViewData["DEMO_MODE"] = true;
      if (!e.HasValue) return RedirectToAction("Index", "Preview");
      EventDetail ev = EventRepository.GetEventDetail(e.Value);
      if (ev == null) return RedirectToAction("Index", "Preview");
      ViewData["PageActionPath"] = "Category";
      ViewData["CurrentCategory"] = CategoryRepository.GetCategoryMapDetail(param.Id);
      ViewData["CurrentEvent"] = ev;
      SetFilterParams(param);
      return View(AuctionRepository.GetListForCategoryMap(param, e.Value, AppHelper.CurrentUser.ID, true));
    }
    
    //AuctionDetail   
    [HttpGet, Compress]
    public ActionResult AuctionDetail(long id)
    {
      ViewData["DEMO_MODE"] = true;
      Auction currentAuction = AuctionRepository.GetAuction(id);
      if (currentAuction == null || currentAuction.AuctionType_ID == (byte)Consts.AuctionType.DealOfTheWeek || currentAuction.Status == (byte)Consts.AuctionStatus.Locked) return RedirectToAction("Index", "Preview");
      ViewData["CurrentAuction"] = currentAuction;
      ViewData["CurrentAuctionImages"] = ImageRepository.GetAuctionImages(id, false);
      ViewData["CurrentCategory"] = currentAuction.EventCategory;
      ViewData["CurrentEvent"] = EventRepository.GetEventDetail(currentAuction.Event_ID);
      return View();
    }
  }
}
