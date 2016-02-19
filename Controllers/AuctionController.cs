using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Relatives.Models.CustomBinders;
using Vauction.Models;
using Vauction.Utils.Helpers;
using Vauction.Utils;
using Vauction.Models.CustomModels;
using Vauction.Utils.Perfomance;
using Vauction.Utils.Autorization;

namespace Vauction.Controllers
{
    [HandleError, CrossSessionCheck]
    public class AuctionController : BaseController
    {
        #region init
        IAuctionRepository AuctionRepository;
        ICategoryRepository CategoryRepository;
        IBidRepository BidRepository;
        IInvoiceRepository InvoiceRepository;
        IImageRepository ImageRepository;
        IVariableRepository VariableRepository;
        IEventRepository EventRepository;
        public AuctionController()
        {
            AuctionRepository = dataProvider.AuctionRepository;
            CategoryRepository = dataProvider.CategoryRepository;
            BidRepository = dataProvider.BidRepository;
            InvoiceRepository = dataProvider.InvoiceRepository;
            ImageRepository = dataProvider.ImageRepository;
            VariableRepository = dataProvider.VariableRepository;
            EventRepository = dataProvider.EventRepository;
        }
        #endregion

        #region index
        [HttpGet, Compress, NoCache]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Event");
        }
        #endregion

        #region EventCategory
        // EventCategory
        [HttpGet, RequireSslFilter(IsRequired = "False"), Compress, NoCache]
        public ActionResult EventCategory([ModelBinder(typeof(CustomItemBinder))]CategoryFilterParams param)
        {
            EventCategoryDetail ec = CategoryRepository.GetEventCategoryDetail(param.Id);
            if (ec == null || !ec.IsActive) return RedirectToAction("Index", "Event");
            InitCurrentEvent();
            EventDetail currentEvent = ViewData["CurrentEvent"] as EventDetail ?? new EventDetail();
            EventDetail ed = (currentEvent.ID == ec.Event_ID) ? currentEvent : dataProvider.EventRepository.GetEventDetail(ec.Event_ID);
            SessionUser cuser = AppHelper.CurrentUser;
            if (ed == null || (!ed.IsViewable || ed.DateEnd.CompareTo(DateTime.Now.AddDays(-45)) < 0) && !(cuser != null && cuser.IsAccessable)) return RedirectToAction("Index", "Event");
            ViewData["CurrentEvent"] = ed;
            ViewData["CurrentCategory"] = ec;
            ViewData["LelandsBanner"] = ec.Category_ID == 138 || ec.Category_ID == 135;
            SetFilterParams(param);
            ViewData["PageActionPath"] = "EventCategory";
            ViewData["IsRegisteredForEvent"] = (currentEvent.ID == ec.Event_ID) ? ViewData["UserRegisterForEvent"] : (cuser != null && dataProvider.EventRepository.IsUserRegisterForEvent(cuser.ID, ec.Event_ID));
            ViewData["Lots"] = AuctionRepository.GetListForEventCategory(param, cuser == null ? -1 : cuser.ID, Convert.ToBoolean(ViewData["UserRegisterForEvent"]) || (cuser != null && cuser.IsAccessable));
            if (cuser != null && cuser.ID > 0)
            {
                ViewData["IsRefreshAvailable"] = ed.IsCurrent && Convert.ToBoolean(ViewData["UserRegisterForEvent"]);
                ViewData["PreviewMethod"] = 0;
                ViewData["PageParams"] = String.Join(",", (new object[] { param.Id, cuser.ID, (param.page > 0) ? param.page - 1 : 0, param.PageSize }));
            }
            return View(ed);
        }
        /*
        // pEventCategory - private
        private object pEventCategory(CategoryFilterParams param, long event_id)
        {
          ViewData["PageActionPath"] = "EventCategory";
          SetFilterParams(param);
          return AuctionRepository.GetListForEventCategory(param, AppHelper.CurrentUser == null ? -1 : AppHelper.CurrentUser.ID);
        }
        // pEventCategoryUn
        [ChildActionOnly] //, ActionOutputCache(Consts.CachingTime_60min)
        public ActionResult pEventCategoryUn(long eventcategory_id, long event_id, CategoryFilterParams param, int page, int viewmode, bool iscurrent, bool isregistered)
        {
          ViewData["IsRegisteredForEvent"] = isregistered;
          return View("PartialAuctionGrid", pEventCategory(param, event_id));
        }
        // pEventCategory
        [ChildActionOnly]
        public ActionResult pEventCategory(long eventcategory_id, long event_id, CategoryFilterParams param, int page, int viewmode, bool iscurrent, bool isregistered)
        {      
          ViewData["IsRefreshAvailable"] = iscurrent && isregistered;
          ViewData["PreviewMethod"] = 0;
          ViewData["PageParams"] = String.Join(",", (new object[] { param.Id, AppHelper.CurrentUser == null ? -1 : AppHelper.CurrentUser.ID, (param.page > 0) ? param.page - 1 : 0, param.PageSize }));
          ViewData["IsRegisteredForEvent"] = isregistered;
          return View("PartialAuctionGrid", pEventCategory(param,  AppHelper.CurrentUser == null ? -1 : AppHelper.CurrentUser.ID));
        }
         */
        #endregion

        #region Category
        //Category
        [HttpGet, RequireSslFilter(IsRequired = "False"), Compress, NoCache]
        public ActionResult Category([ModelBinder(typeof(CustomItemBinder))]CategoryFilterParams param, long? e)
        {
            if (!e.HasValue) return RedirectToAction("Index", "Event");
            InitCurrentEvent();
            EventDetail currentEvent = ViewData["CurrentEvent"] as EventDetail ?? new EventDetail();
            EventDetail ed = (currentEvent.ID == e.GetValueOrDefault(currentEvent.ID)) ? currentEvent : dataProvider.EventRepository.GetEventDetail(e.GetValueOrDefault(currentEvent.ID));
            SessionUser cuser = AppHelper.CurrentUser;
            if (ed == null || (!ed.IsViewable || ed.DateEnd.CompareTo(DateTime.Now.AddDays(-45)) < 0) && !(cuser != null && cuser.IsAccessable)) return RedirectToAction("Index", "Event");
            ViewData["CurrentCategory"] = CategoryRepository.GetCategoryMapDetail(param.Id);
            ViewData["CurrentEvent"] = ed;

            SetFilterParams(param);
            ViewData["PageActionPath"] = "Category";

            ViewData["IsRegisteredForEvent"] = (ed.ID == e.GetValueOrDefault(ed.ID)) ? ViewData["UserRegisterForEvent"] : (cuser != null && dataProvider.EventRepository.IsUserRegisterForEvent(cuser.ID, e.GetValueOrDefault(ed.ID)));
            ViewData["Lots"] = AuctionRepository.GetListForCategoryMap(param, ed.ID, cuser == null ? -1 : cuser.ID, Convert.ToBoolean(ViewData["UserRegisterForEvent"]) || (cuser != null && cuser.IsAccessable));

            if (cuser != null && cuser.ID > 0)
            {
                ViewData["IsRefreshAvailable"] = ed.IsCurrent && Convert.ToBoolean(ViewData["UserRegisterForEvent"]);
                ViewData["PreviewMethod"] = 1;
                ViewData["PageParams"] = String.Join(",", (new object[] { param.Id, ed.ID, cuser.ID, (param.page > 0) ? param.page - 1 : 0, param.PageSize }));
            }
            return View(ed);
        }
        /*
        // pCategory - private
        private object pCategory(CategoryFilterParams param, long event_id)
        {
          ViewData["PageActionPath"] = "Category";
          SetFilterParams(param);
          return AuctionRepository.GetListForCategoryMap(param, event_id, AppHelper.CurrentUser == null ? -1 : AppHelper.CurrentUser.ID);
        }
        // pCategoryUn
        [ChildActionOnly] //, ActionOutputCache(Consts.CachingTime_60min)
        public ActionResult pCategoryUn(long category_id, long event_id, CategoryFilterParams param, int page, int viewmode, bool iscurrent, bool isregistered)
        {
          ViewData["IsRegisteredForEvent"] = isregistered;
          return View("PartialAuctionGrid", pCategory(param, event_id));
        }
        // pCategory
        [ChildActionOnly]
        public ActionResult pCategory(long category_id, long event_id, CategoryFilterParams param, int page, int viewmode, bool iscurrent, bool isregistered)
        {
          ViewData["IsRefreshAvailable"] = iscurrent && isregistered;
          ViewData["PreviewMethod"] = 1;
          ViewData["PageParams"] = String.Join(",", (new object[] { param.Id, event_id, AppHelper.CurrentUser == null ? -1 : AppHelper.CurrentUser.ID, (param.page > 0) ? param.page - 1 : 0, param.PageSize }));
          ViewData["IsRegisteredForEvent"] = isregistered;
          return View("PartialAuctionGrid", pCategory(param, event_id));
        }*/
        #endregion

        #region EventDetailed
        //EventDetailed
        [RequireSslFilter(IsRequired = "False"), Compress, NoCache]
        public ActionResult EventDetailed([ModelBinder(typeof(CustomItemBinder))]AuctionFilterParams param)
        {
            InitCurrentEvent();
            EventDetail ed = (ViewData["CurrentEvent"] as EventDetail);
            if (ed == null && !param.ID.HasValue) return RedirectToAction("Index", "Event");
            ed = EventRepository.GetEventDetail(param.ID.HasValue ? param.ID.Value : ed.ID);
            SessionUser cuser = AppHelper.CurrentUser;
            if (ed == null || (!ed.IsViewable || ed.DateEnd.CompareTo(DateTime.Now.AddDays(-45)) < 0) && !(cuser != null && cuser.IsAccessable)) return RedirectToAction("Index", "Event");
            ViewData["CurrentEvent"] = ed;
            ViewData["IsRegisteredForEvent"] = ViewData["UserRegisterForEvent"] = cuser != null && dataProvider.EventRepository.IsUserRegisterForEvent(cuser.ID, ed.ID);
            SetFilterParams(param);

            ViewData["PageActionPath"] = "EventDetailed";
            ViewData["Lots"] = AuctionRepository.GetFeaturedList(ed.ID, cuser == null ? -1 : cuser.ID, Convert.ToBoolean(ViewData["UserRegisterForEvent"]) || (cuser != null && cuser.IsAccessable), param.page, param.PageSize);

            if (cuser != null && cuser.ID > 0)
            {
                ViewData["IsRefreshAvailable"] = ed.IsCurrent && Convert.ToBoolean(ViewData["UserRegisterForEvent"]);
                ViewData["PreviewMethod"] = 2;
                ViewData["PageParams"] = String.Join(",", (new object[] { ed.ID, cuser.ID, (param.page > 0) ? param.page - 1 : 0, param.PageSize }));
            }

            return View(ed);
        }
        /*
        // EventDetailed - private
        private object pEventDetailed(AuctionFilterParams param, long event_id)
        {
          ViewData["PageActionPath"] = "EventDetailed";
          SetFilterParams(param);
          return AuctionRepository.GetFeaturedList(event_id, AppHelper.CurrentUser == null ? -1 : AppHelper.CurrentUser.ID, param.page, param.PageSize);
        }
        // pEventDetailedUn
        [ChildActionOnly] //ActionOutputCache(Consts.CachingTime_60min)
        public ActionResult pEventDetailedUn(long event_id, AuctionFilterParams param, int page, int viewmode, bool iscurrent, bool isregistered)
        {
          ViewData["IsRegisteredForEvent"] = isregistered;
          return View("PartialAuctionGrid", pEventDetailed(param, event_id));
        }
        // pEventDetailed
        [ChildActionOnly]
        public ActionResult pEventDetailed(long event_id, AuctionFilterParams param, int page, int viewmode, bool iscurrent, bool isregistered)
        {
          ViewData["IsRefreshAvailable"] = iscurrent && isregistered;
          ViewData["PreviewMethod"] = 2;
          ViewData["PageParams"] = String.Join(",", (new object[] { event_id, AppHelper.CurrentUser == null ? -1 : AppHelper.CurrentUser.ID, (param.page > 0) ? param.page - 1 : 0, param.PageSize }));
          ViewData["IsRegisteredForEvent"] = isregistered;
          return View("PartialAuctionGrid", pEventDetailed(param, event_id));
        }*/
        #endregion

        #region FeaturedItems
        //FeaturedItems
        [RequireSslFilter(IsRequired = "False"), Compress, NoCache]
        public ActionResult FeaturedItems([ModelBinder(typeof(CustomItemBinder))]AuctionFilterParams param)
        {
            InitCurrentEvent();
            EventDetail ed = (ViewData["CurrentEvent"] as EventDetail);
            if (ed == null && !param.ID.HasValue) return RedirectToAction("Index", "Event");
            ed = EventRepository.GetEventDetail(param.ID.HasValue ? param.ID.Value : ed.ID);
            SessionUser cuser = AppHelper.CurrentUser;
            if (ed == null || (!ed.IsViewable || ed.DateEnd.CompareTo(DateTime.Now.AddDays(-45)) < 0) && !(cuser != null && cuser.IsAccessable)) return RedirectToAction("Index", "Event");
            ViewData["CurrentEvent"] = ed;
            ViewData["IsRegisteredForEvent"] = ViewData["UserRegisterForEvent"] = cuser != null && dataProvider.EventRepository.IsUserRegisterForEvent(cuser.ID, ed.ID);
            SetFilterParams(param);
            ViewData["PageActionPath"] = "FeaturedItems";
            ViewData["Lots"] = AuctionRepository.GetFeaturedList(ed.ID, cuser == null ? -1 : cuser.ID, Convert.ToBoolean(ViewData["UserRegisterForEvent"]) || (cuser != null && cuser.IsAccessable), param.page, param.PageSize);

            if (cuser != null && cuser.ID > 0)
            {
                ViewData["IsRefreshAvailable"] = ed.IsCurrent && Convert.ToBoolean(ViewData["UserRegisterForEvent"]);
                ViewData["PreviewMethod"] = 2;
                ViewData["PageParams"] = String.Join(",", (new object[] { ed.ID, cuser.ID, (param.page > 0) ? param.page - 1 : 0, param.PageSize }));
            }
            return View(ed);
        }/*
    // FeaturedItems - private
    private object pFeaturedItems(AuctionFilterParams param, long event_id)
    {
      ViewData["PageActionPath"] = "FeaturedItems";
      SetFilterParams(param);
      return AuctionRepository.GetFeaturedList(event_id, AppHelper.CurrentUser == null ? -1 : AppHelper.CurrentUser.ID, param.page, param.PageSize);
    }
    // pFeaturedItemsUn
    [ChildActionOnly] //ActionOutputCache(Consts.CachingTime_60min)
    public ActionResult pFeaturedItemsUn(long event_id, AuctionFilterParams param, int page, int viewmode, bool iscurrent, bool isregistered)
    {
      ViewData["IsRegisteredForEvent"] = isregistered;
      return View("PartialAuctionGrid", pFeaturedItems(param, event_id));
    }
    // pFeaturedItems
    [ChildActionOnly]
    public ActionResult pFeaturedItems(long event_id, AuctionFilterParams param, int page, int viewmode, bool iscurrent, bool isregistered)
    {
      ViewData["IsRefreshAvailable"] = iscurrent && isregistered;
      ViewData["PreviewMethod"] = 2;
      ViewData["PageParams"] = String.Join(",", (new object[] { event_id, AppHelper.CurrentUser == null ? -1 : AppHelper.CurrentUser.ID, (param.page > 0) ? param.page - 1 : 0, param.PageSize }));
      ViewData["IsRegisteredForEvent"] = isregistered;
      return View("PartialAuctionGrid", pFeaturedItems(param, event_id));
    }*/
        #endregion

        #region UpdatePageResult
        //UpdatePageResult
        [HttpPost, Compress, NoCache]
        public JsonResult UpdatePageResult(int method, string prms)
        {
            return JSON(AuctionRepository.UpdatePageResult(method, prms));
        }
        #endregion

        #region AuctionDetail
        [RequireSslFilter(IsRequired = "False"), HttpGet, Compress, NoCache]
        public ActionResult AuctionDetail(long? id)
        {
            Session["AUCTIONDETAIL"] = "AUCTIONDETAIL";


            ViewData["DEMO_MODE"] = false;
            ViewData["LeftUserControlVisibility"] = false;
            if (!id.HasValue) return RedirectToAction("Index", "Home");
            AuctionDetail auction = AuctionRepository.GetAuctionDetail(id.Value, true);
            if (auction == null || ((auction.IsPrivate && !auction.IsViewable) && !auction.IsAccessable)) return RedirectToAction("EventDetailed", "Auction");

            ViewData["ShowAds"] = (DateTime.Now < new DateTime(auction.EventDateEnd.Year, auction.EventDateEnd.Month, auction.EventDateEnd.Day, 0, 0, 1) || DateTime.Now >= auction.EventDateEnd.AddMinutes(10));

            AuctionResultDetail aresult = ((auction.IsCurrent || auction.EndDate.AddMinutes(20) > DateTime.Now) ? AuctionRepository.GetAuctionResultCurrent(id.Value, true) : AuctionRepository.GetAuctionResult(id.Value, true)) ?? new AuctionResultDetail();
            ViewData["FullCategoryLink"] = CategoryRepository.GetFullEventCategoryLink(auction.LinkParams.EventCategory_ID);
            AuctionUserInfo aui = new AuctionUserInfo { IsInWatchList = false, IsRegisterForEvent = false };
            SessionUser cuser = AppHelper.CurrentUser;
            if (cuser != null)
            {
                aui.IsRegisterForEvent = EventRepository.IsUserRegisterForEvent(cuser.ID, auction.LinkParams.Event_ID) || cuser.IsAdminType;
                aui.IsInWatchList = AuctionRepository.IsUserWatchItem(cuser.ID, auction.LinkParams.ID);
            }
            ViewData["AuctionUserInfo"] = aui;
            ViewData["LotResult"] = aresult;
            ViewData["Lots"] = AuctionRepository.GetFeaturedList(auction.LinkParams.Event_ID, cuser == null ? -1 : cuser.ID, Convert.ToBoolean(ViewData["UserRegisterForEvent"]), 0, 21);
            BiddingResult bresult = new BiddingResult(auction.Price, auction.Quantity, auction.AuctionType);
            if (cuser == null || (aresult != null && !aresult.HasBid))
            {
                ViewData["AuctionDetailReturnUrl"] = String.Format("/Auction/AuctionDetail/{0}/{1}/{2}/{3}", auction.LinkParams.ID, auction.LinkParams.EventUrl, auction.LinkParams.CategoryUrl, auction.LinkParams.LotTitleUrl);
                ViewData["BiddingResult"] = bresult;
                ViewData["CurrentAuctionImages"] = ImageRepository.GetAuctionImages(auction.LinkParams.ID, true);
                ViewData["auction_id"] = auction.LinkParams.ID;
                return View(auction);
            }
            if (auction.Status == (byte)Consts.AuctionStatus.Open)
            {
                if (auction.AuctionType == (long)Consts.AuctionType.Normal)
                {
                    BidCurrent bc = BidRepository.GetTopBidForItem(auction.LinkParams.ID, auction.Quantity, true);
                    if (bc != null) bresult.WinningBids.Add(bc);
                    if (aui.IsInWatchList)
                    {
                        bc = BidRepository.GetUserTopBidForItem(auction.LinkParams.ID, cuser.ID, true);
                        if (bc != null) bresult.UserTopBids.Add(bc);
                    }
                }
                else
                {
                    List<BidCurrent> bc = BidRepository.GetBidsForMultipleItem(auction.LinkParams.ID, 1, auction.Quantity, true);
                    if (bc.Any()) bresult.WinningBids.AddRange(bc);
                    bc = BidRepository.GetBidsForMultipleItem(auction.LinkParams.ID, 0, auction.Quantity, true);
                    if (bc.Any()) bresult.LoserBids.AddRange(bc);
                    bresult.UserTopBids.AddRange(bresult.WinningBids.Where(wb => wb.User_ID == cuser.ID).ToList());
                }
                ViewData["BiddingResult"] = bresult;
            }
            return View(auction);
        }
        //pBiddingHistory
        [ChildActionOnly]
        public ActionResult pBiddingHistory(long auction_id)
        {
            return View("pBiddingHistory", BidRepository.GetBidHistory(auction_id));
        }
        //pAuctionImages
        [ChildActionOnly]
        public ActionResult pAuctionImages(long auction_id)
        {
            ViewData["auction_id"] = auction_id;

            return View("pAuctionImages", ImageRepository.GetAuctionImages(auction_id, true));
        }
        //pAuctionImages
        [ChildActionOnly]
        public ActionResult pAuctionImagesSlider(long auction_id)
        {
            ViewData["auction_id"] = auction_id;
            return View("pAuctionImagesSlider", ImageRepository.GetAuctionImages(auction_id, true));
        }
        //PrintAuction
        [RequireSslFilter(IsRequired = "False"), Compress, NoCache]
        public ActionResult PrintAuction(long? id)
        {
            AuctionDetail auction = AuctionRepository.GetAuctionDetail(id.GetValueOrDefault(-1), true);
            if (auction == null)
            {
                ViewData["Title"] = "The lot is unavailable.";
                ViewData["CurrentAuction"] = null;
                return View();
            }
            AuctionResultDetail aresult = (auction.IsCurrent) ? AuctionRepository.GetAuctionResultCurrent(id.GetValueOrDefault(-1), true) : new AuctionResultDetail();
            ViewData["CurrentAuction"] = auction;
            ViewData["LotResult"] = aresult;
            ViewData["FullCategoryLink"] = CategoryRepository.GetEventCategoryById(auction.LinkParams.EventCategory_ID).FullCategory;
            AuctionUserInfo aui = new AuctionUserInfo();
            SessionUser cuser = AppHelper.CurrentUser;
            if (cuser != null)
                aui.IsRegisterForEvent = EventRepository.IsUserRegisterForEvent(cuser.ID, auction.LinkParams.Event_ID);
            else aui = new AuctionUserInfo { IsInWatchList = false, IsRegisterForEvent = false };
            ViewData["IsRegisteredForTheEvent"] = aui.IsRegisterForEvent;
            ViewData["IsAuthenticated"] = cuser != null;
            ViewData["CurrentAuctionImages"] = ImageRepository.GetAuctionImages(auction.LinkParams.ID, true);
            ViewData["Title"] = String.Format("{0} - {1} (LOT#{2} ENDS {3})", auction.LinkParams.Title, Consts.CompanyTitleName, auction.LinkParams.Lot, auction.EndTime);
            return View();
        }
        //AddBidWatch
        [VauctionAuthorize, HttpGet, Compress, NoCache]
        public ActionResult AddBidWatch(long? id)
        {
            if (!id.HasValue) return RedirectToAction("Index", "Home");
            AuctionRepository.AddAuctionToWatchList(AppHelper.CurrentUser.ID, id.GetValueOrDefault(-1));
            return RedirectToAction("AuctionDetail", new { id = id.GetValueOrDefault(-1) });
        }
        //UpdateAuctionResult
        [HttpPost, Compress, NoCache]
        public JsonResult UpdateAuctionResult(long? id)
        {
            if (!id.HasValue) return JSON(new { currentbid = "", highbidder = "", minbid = "", quantity = "" });
            AuctionDetail auction = AuctionRepository.GetAuctionDetail(id.Value, true);
            AuctionResultDetail rauction = AuctionRepository.GetAuctionResultCurrent(id.Value, true);
            if (auction == null || rauction == null) return JSON(new { currentbid = "", highbidder = "", minbid = "", quantity = "" });
            string minBid = String.Empty;
            if (auction.AuctionType == (long)Consts.AuctionType.Normal)
            {
                BidCurrent bc = BidRepository.GetTopBidForItem(id.Value, auction.Quantity, true);
                minBid = ((bc == null ? auction.Price : bc.Amount) + (bc == null ? 0 : Consts.GetIncrement(bc.Amount))).GetCurrency();
            }
            else
            {
                List<BidCurrent> bcW = BidRepository.GetBidsForMultipleItem(auction.LinkParams.ID, 1, auction.Quantity, true);
                minBid = (bcW.Count() < auction.Quantity ? auction.Price : bcW.Last().Amount + Consts.GetIncrement(bcW.Last().Amount)).GetCurrency();
            }
            return
                 JSON(
                   new
                   {
                       currentbid = rauction.HasBid ? rauction.CurrentBid : "none",
                       highbidder = rauction.HasBid ? rauction.HighBidder : String.Empty,
                       minbid = minBid,
                       quantity = auction.Quantity == 0 || auction.Status == (byte)Consts.AuctionStatus.Closed
                         ? auction.IQuantity.ToString(CultureInfo.InvariantCulture)
                         : auction.Quantity.ToString(CultureInfo.InvariantCulture)
                   });

        }
        #endregion

        #region Bidding
        //PreviewBid
        [VauctionAuthorize, HttpPost, Compress, NoCache]
        public ActionResult PreviewBid(bool? ProxyBidding, decimal? BidAmount, long? id, int? Quantity, decimal? OutBidAmount)
        {
            ViewData["LeftUserControlVisibility"] = false;
            if (id == null)
                id = 0;
            if (!id.HasValue) return RedirectToAction("Index", "Home");
            if (!ProxyBidding.HasValue || !BidAmount.HasValue) return RedirectToAction("AuctionDetail", new { id });
            AuctionDetail auction = AuctionRepository.GetAuctionDetail(id.GetValueOrDefault(-1), true);

            ViewData["CurrentAuctionImages"] = ImageRepository.GetAuctionImages(Convert.ToInt64(id), true);
            if (auction == null) return RedirectToAction("Index", "Home");
            if ((!auction.IsCurrent && !auction.IsPrivate) || auction.EndDate.CompareTo(DateTime.Now) < 0) return RedirectToAction("AuctionDetail", new { @id = auction.LinkParams.ID });
            SessionUser cuser = AppHelper.CurrentUser;
            PreviewBid previewBid = new PreviewBid
                                      {
                                          IsProxy = ProxyBidding.Value,
                                          Amount = BidAmount.Value,
                                          LinkParams = auction.LinkParams,
                                          Quantity = (Quantity.HasValue && Quantity.Value <= auction.Quantity) ? Quantity.Value : auction.Quantity
                                      };
            if (auction.AuctionType == (long)Consts.AuctionType.Normal)
            {
                #region single bidding
                BidCurrent prevMaxBid = BidRepository.GetTopBidForItem(auction.LinkParams.ID, auction.Quantity, true);
                if (prevMaxBid != null)
                {
                    previewBid.IsOutBid = previewBid.Amount <= prevMaxBid.Amount && previewBid.Amount <= prevMaxBid.MaxBid;
                    if (prevMaxBid.User_ID == cuser.ID)
                    {
                        previewBid.IsUpdate = true;
                        previewBid.PreviousAmount = prevMaxBid.Amount;
                        previewBid.PreviousMaxBid = prevMaxBid.MaxBid;
                        previewBid.RealAmount = (ProxyBidding.Value) ? prevMaxBid.Amount : BidAmount.Value;
                        previewBid.Amount = (BidAmount < prevMaxBid.MaxBid) ? prevMaxBid.MaxBid : BidAmount.Value;
                    }
                    else
                        previewBid.RealAmount = (!ProxyBidding.Value) ? BidAmount.Value : (((!OutBidAmount.HasValue) ? prevMaxBid.Amount : OutBidAmount.Value) + Consts.GetIncrement(prevMaxBid.Amount));
                }
                else
                    previewBid.RealAmount = (ProxyBidding.Value) ? auction.Price : BidAmount.Value;
                #endregion
            }
            else
            {
                #region multiple bidding
                List<BidCurrent> winT, loserT;
                BidRepository.GetBidsForMultipleItem(auction, out winT, out loserT);
                List<BidCurrent> currentUsersBid = winT.Where(b => b.User_ID == cuser.ID).ToList();
                List<BidCurrent> loserUsersBid = loserT.Where(b => b.User_ID == cuser.ID).ToList();
                if (currentUsersBid.Count() != previewBid.Quantity && currentUsersBid.Count > 0)
                {
                    if (currentUsersBid.Count > previewBid.Quantity)
                        previewBid.IsQuantityBig = true;
                    else if (currentUsersBid.Count + loserUsersBid.Count < previewBid.Quantity)
                    {
                        previewBid.QuantityType = 1;
                        previewBid.PreviousQuantity = currentUsersBid.Count + loserUsersBid.Count;
                    }
                    else if (currentUsersBid.Count + loserUsersBid.Count > previewBid.Quantity)
                    {
                        previewBid.QuantityType = 0;
                        previewBid.PreviousQuantity = currentUsersBid.Count + loserUsersBid.Count;
                    }
                }
                else
                {
                    previewBid.QuantityType = 255;
                    previewBid.PreviousQuantity = currentUsersBid.Count + loserUsersBid.Count;
                }

                previewBid.WinnerBidsCount = currentUsersBid.Count;
                previewBid.LoserBidsCount = loserUsersBid.Count;
                previewBid.Amount = BidAmount.Value * previewBid.Quantity;

                if (loserUsersBid.Count == 0 && currentUsersBid.Count != 0)
                {
                    previewBid.IsUpdate = true;
                    previewBid.PreviousAmount = currentUsersBid[0].Amount;
                    previewBid.PreviousMaxBid = currentUsersBid[0].MaxBid;
                    previewBid.RealAmount = BidAmount.Value;
                }
                else if (winT.Count - previewBid.Quantity == 0)
                {
                    previewBid.PreviousAmount = winT[winT.Count - 1].Amount;
                    previewBid.PreviousMaxBid = BidAmount.Value;
                    previewBid.RealAmount = BidAmount.Value;
                }
                else
                {
                    previewBid.PreviousAmount = (winT.Count > 0) ? winT[winT.Count - 1].Amount : auction.Price;
                    previewBid.PreviousMaxBid = BidAmount.Value;
                    previewBid.RealAmount = BidAmount.Value;
                }
                previewBid.IsOutBid = (BidAmount.Value < previewBid.RealAmount);
                #endregion
            }
            InitCurrentEvent();
            return View(previewBid);
        }
        //PlaceBid (post)
        [VauctionAuthorize, HttpPost, Compress, NoCache] //, ValidateAntiForgeryToken
        public ActionResult PlaceBid(bool? ProxyBidding, decimal? BidAmount, long id, int quantity, decimal RealBidAmount)
        {
            if (!ProxyBidding.HasValue || !BidAmount.HasValue) return RedirectToAction("AuctionDetail", new { id });
            SessionUser cuser = AppHelper.CurrentUser;

            //Stopwatch stopwatchMain = new Stopwatch();
            //stopwatchMain.Start();

            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine("-------------------------------");
            //sb.AppendFormat("\tPLACE BID: user-{0} | item-{1} | amount-{2}\n", cuser.Login, id, BidAmount.GetCurrency());
            //sb.AppendLine("\t-------------------------------");

            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();

            AuctionDetail auction = AuctionRepository.GetAuctionDetail(id, true);

            //stopwatch.Stop();
            //sb.AppendFormat("\t- get auction: {0} ticks; {1} ms\n", stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds);

            if (auction == null) return RedirectToAction("Index", "Home");
            if ((!auction.IsCurrent && !auction.IsPrivate) || auction.EndDate.CompareTo(DateTime.Now) < 0) return RedirectToAction("AuctionDetail", new { @id = auction.LinkParams.ID });
            InitCurrentEvent();

            //stopwatch = new Stopwatch();
            //stopwatch.Start();

            if (!AuctionRepository.IsUserWatchItem(cuser.ID, id))
                AuctionRepository.AddAuctionToWatchList(cuser.ID, id);

            //stopwatch.Stop();
            //sb.AppendFormat("\t- check/add item into the watch list: {0} ticks; {1} ms\n", stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds);

            PreviewBid previewBid = new PreviewBid
                                      {
                                          LinkParams = auction.LinkParams,
                                          IsProxy = ProxyBidding.Value,
                                          Amount = BidAmount.Value,
                                          Quantity = quantity,
                                          RealAmount = RealBidAmount
                                      };

            //stopwatch = new Stopwatch();
            //stopwatch.Start();

            User currentUser = dataProvider.UserRepository.GetUser(cuser.ID, true);
            if (!cuser.IsHouseBidder && !currentUser.NotDepositNeed && (previewBid.DepositNeed = BidRepository.DepositNeeded(BidAmount.Value * quantity, cuser.ID, currentUser.IsMaxDepositOnlyNeed, auction.LinkParams.Event_ID, auction.LinkParams.ID)) > 0)
            {
                Session["PreviewBid"] = previewBid;
                return RedirectToAction("DepositNeeded", new { auction_id = id });
                //InvoiceRepository.AddDeposit(previewBid.DepositNeed, auction.LinkParams.ID, auction.LinkParams.Event_ID, cuser.ID, (long) Consts.PaymentType.CreditCard, "12345", cuser.Address_Billing, "*11111");
            }

            //stopwatch.Stop();
            //sb.AppendFormat("\t- check deposit: {0} ticks; {1} ms\n", stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds);

            if (auction.AuctionType == (long)Consts.AuctionType.Normal)
            {
                #region single bidding

                byte result;
                BidCurrent currentBid = new BidCurrent
                {
                    Amount = BidAmount.GetValueOrDefault(0),
                    Auction_ID = id,
                    DateMade = DateTime.Now,
                    IP = Consts.UsersIPAddress,
                    IsActive = true,
                    IsProxy = ProxyBidding.GetValueOrDefault(false),
                    MaxBid = BidAmount.GetValueOrDefault(0),
                    Quantity = quantity,
                    User_ID = cuser.ID
                };
                BidCurrent previousBid, loserBid, winnerBid;

                //lock (auction)
                {
                    //stopwatch = new Stopwatch();
                    //stopwatch.Start();

                    result = BidRepository.BiddingForSingleAuction(auction, currentBid, out previousBid, out loserBid, out winnerBid);

                    //stopwatch.Stop();
                    //sb.AppendFormat("\t- place bid: {0} ticks; {1} ms\n", stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds);

                    if (result == 3) return RedirectToAction("AuctionDetail", new { id });

                    //stopwatch = new Stopwatch();
                    //stopwatch.Start();

                    AuctionRepository.RemoveAuctionResultsCache(id);

                    //stopwatch.Stop();
                    //sb.AppendFormat("\t- remove lot result cache: {0} ticks; {1} ms\n", stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds);

                    //stopwatch = new Stopwatch();
                    //stopwatch.Start();

                    BidRepository.RemoveTopBidForItemCache(id);

                    //stopwatch.Stop();
                    //sb.AppendFormat("\t- remove top bid for item cache: {0} ticks; {1} ms\n", stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds);

                    //stopwatch = new Stopwatch();
                    //stopwatch.Start();

                    BidRepository.UpdateUsersTopBidCache(id, cuser.ID, currentBid);

                    //stopwatch.Stop();
                    //sb.AppendFormat("\t- updates users top bid cache: {0} ticks; {1} ms\n", stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds);
                }

                if (result == 1)
                {
                    //stopwatch = new Stopwatch();
                    //stopwatch.Start();

                    previewBid.Amount = winnerBid.Amount;
                    BidRepository.UpdateUserWinLotsCache(cuser.ID, auction.LinkParams.Event_ID, auction.LinkParams.ID, 0);

                    //stopwatch.Stop();
                    //sb.AppendFormat("\t- update user win lot cache: {0} ticks; {1} ms\n", stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds);

                    //stopwatch = new Stopwatch();
                    //stopwatch.Start();

                    if (!cuser.NotRecievingOutBidNotice && !String.IsNullOrEmpty(cuser.Email) && !cuser.IsHouseBidder)
                        Mail.SendOutBidLetter(cuser.Email, auction.LinkParams.Lot, cuser.Login,
                                              auction.LinkParams.Title, winnerBid.Amount.GetCurrency(),
                                              String.Format("{0} at {1} {2}", auction.EndDate.ToString("d"), auction.EndDate.ToString("t"), Consts.DateTimeEnd),
                                              auction.LinkParams.AuctionDetailUrl);

                    //stopwatch.Stop();
                    //sb.AppendFormat("\t- send out bid email: {0} ticks; {1} ms\n", stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds);

                    //stopwatchMain.Stop();
                    //sb.AppendLine("\t-------------------------------");
                    //sb.AppendFormat("\tRESULT: {0} ticks; {1} ms\n", stopwatchMain.ElapsedTicks, stopwatchMain.ElapsedMilliseconds);
                    //sb.AppendLine("\t-------------------------------");
                    //if (stopwatchMain.ElapsedMilliseconds > 1000) sb.AppendLine("MORETHAN_1000");
                    //Logger.LogInfo(sb.ToString());

                    return View("OutBid", previewBid);
                }

                //stopwatch = new Stopwatch();
                //stopwatch.Start();

                if (!cuser.NotRecievingBidConfirmation && !String.IsNullOrEmpty(cuser.Email) && !cuser.IsHouseBidder)
                {
                    BidRepository.UpdateUserWinLotsCache(cuser.ID, auction.LinkParams.Event_ID, auction.LinkParams.ID, currentBid.MaxBid);
                    if (result == 2)
                        Mail.SendSuccessfulBidUpdateLetter(cuser.Email, auction.LinkParams.Lot,
                                                           cuser.Login, auction.LinkParams.Title, currentBid.Amount.GetCurrency(),
                                                           currentBid.MaxBid.GetCurrency(),
                                                           String.Format("{0} at {1} {2}", auction.EndDate.ToString("d"), auction.EndDate.ToString("t"), Consts.DateTimeEnd),
                                                           auction.LinkParams.AuctionDetailUrl,
                                                           currentBid.MaxBid > previousBid.MaxBid);
                    else
                        Mail.SendSuccessfulBidLetter(cuser.Email, auction.LinkParams.Lot,
                                                     cuser.Login, auction.LinkParams.Title, currentBid.Amount.GetCurrency(),
                                                     BidAmount.GetCurrency(),
                                                     String.Format("{0} at {1} {2}", auction.EndDate.ToString("d"), auction.EndDate.ToString("t"), Consts.DateTimeEnd),
                                                     auction.LinkParams.AuctionDetailUrl);
                }

                if (loserBid != null && loserBid.User_ID != cuser.ID)
                {
                    BidRepository.UpdateUserWinLotsCache(loserBid.User_ID, auction.LinkParams.Event_ID, auction.LinkParams.ID, 0);
                    User usr = dataProvider.UserRepository.GetUser(loserBid.User_ID, true);
                    if (!usr.NotRecievingOutBidNotice && !String.IsNullOrEmpty(usr.Email) && !usr.IsHouseBidder)
                    {
                        //AddressCard ac = dataProvider.UserRepository.GetAddressCard(usr.Billing_AddressCard_ID.GetValueOrDefault(-1), true);
                        Mail.SendOutBidLetter(usr.Email, auction.LinkParams.Lot, usr.Login,
                                              auction.LinkParams.Title, winnerBid.Amount.GetCurrency(),
                                              String.Format("{0} at {1} {2}", auction.EndDate.ToString("d"), auction.EndDate.ToString("t"), Consts.DateTimeEnd),
                                              auction.LinkParams.AuctionDetailUrl);
                    }
                }

                //stopwatch.Stop();
                //sb.AppendFormat("\t- sending emails to winner and outbidder: {0} ticks; {1} ms\n", stopwatch.ElapsedTicks, stopwatch.ElapsedMilliseconds);
                //stopwatchMain.Stop();
                //sb.AppendLine("\t-------------------------------");
                //sb.AppendFormat("\tRESULT: {0} ticks; {1} ms\n", stopwatchMain.ElapsedTicks, stopwatchMain.ElapsedMilliseconds);
                //sb.AppendLine("\t-------------------------------");
                //if (stopwatchMain.ElapsedMilliseconds > 1000) sb.AppendLine("MORETHAN_1000");
                //Logger.LogInfo(sb.ToString());

                #endregion
            }
            else
            {
                #region multiple bidding

                List<BidCurrent> tblCurWin, tblCurLos, tblPrevLos;
                BidRepository.GetBidsForMultipleItem(auction, out tblCurWin, out tblPrevLos);

                BidRepository.PlaceMultipleBids(id, ProxyBidding.Value, BidAmount.Value, cuser.ID, quantity);

                BidRepository.RemoveTopBidForMultipleItemCache(id);

                decimal curminbid = (tblCurWin.Any() && tblCurWin.Count() == auction.Quantity) ? tblCurWin[tblCurWin.Count - 1].Amount : auction.Price;
                if (curminbid > BidAmount)
                {
                    previewBid.Amount = curminbid;
                    return View("OutBid", previewBid);
                }

                BidRepository.GetBidsForMultipleItem(auction, out tblCurWin, out tblCurLos);

                if (tblCurWin[0].Amount == tblCurWin[tblCurWin.Count - 1].Amount && tblCurWin[0].User_ID == tblCurWin[tblCurWin.Count - 1].User_ID)
                    AuctionRepository.UpdateAuctionBiddingResult(id, tblCurWin[0].User_ID, null, tblCurWin[0].Amount, null, tblCurWin[0].MaxBid, null);
                else
                    AuctionRepository.UpdateAuctionBiddingResult(id, tblCurWin[tblCurWin.Count - 1].User_ID, tblCurWin[0].User_ID, tblCurWin[tblCurWin.Count - 1].Amount, tblCurWin[0].Amount, tblCurWin[tblCurWin.Count - 1].MaxBid, tblCurWin[0].MaxBid);
                AuctionRepository.RemoveAuctionResultsCache(id);

                List<BidCurrent> currentUsersBid = tblCurWin.Where(p => p.User_ID == cuser.ID).ToList();

                if (currentUsersBid.Count > 0 && !cuser.NotRecievingBidConfirmation && !String.IsNullOrEmpty(cuser.Email) && !cuser.IsHouseBidder)
                    Mail.SendSuccessfulMultipleBidLetter(cuser.Email, auction.LinkParams.Lot, cuser.Login, auction.LinkParams.Title, String.Format("{0} at {1} {2}", auction.EndDate.ToString("d"), auction.EndDate.ToString("t"), Consts.DateTimeEnd), quantity, (BidAmount * quantity).GetCurrency(), currentUsersBid.Count.ToString(CultureInfo.InvariantCulture), auction.LinkParams.AuctionDetailUrl);

                var diffLos = from b in tblCurLos.Except(tblPrevLos)
                              group b by b.User_ID;

                List<BidCurrent> loserUsersBid = new List<BidCurrent>();
                User current;

                foreach (IGrouping<long, BidCurrent> bidder in diffLos)
                {
                    if (bidder == null || bidder.Key == cuser.ID) continue;
                    current = dataProvider.UserRepository.GetUser(bidder.Key, true);
                    if (current.NotRecievingOutBidNotice && String.IsNullOrEmpty(current.Email)) continue;
                    currentUsersBid = tblCurWin.Where(p => p.User_ID == bidder.Key).ToList();
                    loserUsersBid = tblCurLos.Where(p => p.User_ID == bidder.Key && p.IsActive).ToList();
                    if (!current.NotRecievingBidConfirmation && !String.IsNullOrEmpty(current.Email) && !current.IsHouseBidder)
                    {
                        Mail.SendOutBidMultipleLetter(current.Email, auction.LinkParams.Lot, current.Login, auction.LinkParams.Title, String.Format("{0} at {1} {2}", auction.EndDate.ToString("d"), auction.EndDate.ToString("t"), Consts.DateTimeEnd), currentUsersBid.Count.ToString(CultureInfo.InvariantCulture), (currentUsersBid.Count + loserUsersBid.Count).ToString(CultureInfo.InvariantCulture), auction.LinkParams.AuctionDetailUrl);
                    }
                }
                #endregion
            }

            return View("SuccessfulBid", auction);
        }
        //DepositNeeded
        [VauctionAuthorize, HttpGet, Compress, NoCache]
        public ActionResult DepositNeeded(long? auction_id)
        {
            ViewData["LeftUserControlVisibility"] = false;

            if (!auction_id.HasValue) return RedirectToAction("Index", "Home");
            AuctionDetail auction = AuctionRepository.GetAuctionDetail(auction_id.Value, true);
            if (auction == null) return RedirectToAction("Index", "Home");
            if ((!auction.IsCurrent && !auction.IsPrivate) || auction.EndDate.CompareTo(DateTime.Now) < 0) return RedirectToAction("AuctionDetail", "Auction", new { @id = auction.LinkParams.ID });
            PreviewBid pb = Session["PreviewBid"] as PreviewBid;
            if (pb == null) return RedirectToAction("AuctionDetail", "Auction", new { @id = auction.LinkParams.ID });
            pb.PreviousDeposit = InvoiceRepository.GetDepositAmount(AppHelper.CurrentUser.ID, pb.LinkParams.Event_ID);
            Session["PreviewBid"] = pb;
            InitCurrentEvent();
            return View(pb);
        }
        //PlaceBid (get)
        [VauctionAuthorize, HttpGet, Compress, RequireSslFilter(IsRequired = "False"), NoCache]
        public ActionResult PlaceBid()
        {
            PreviewBid pb = Session["PreviewBid"] as PreviewBid;
            if (pb == null) return RedirectToAction("Index", "Home");
            if (pb.DepositNeed == 0) return RedirectToAction("AuctionDetail", "Auction", new { id = pb.LinkParams.ID });
            Session.Remove("PreviewBid");
            return PlaceBid(pb.IsProxy, pb.Amount, pb.LinkParams.ID, pb.Quantity, pb.RealAmount);
        }
        #endregion

        #region DOW
        [RequireSslFilter(IsRequired = "False"), Compress, NoCache]
        public ActionResult DealOfTheWeek(long? id)
        {
            //  id = 143049;
            if (!id.HasValue)
            {
                Auction adow = AuctionRepository.GetDealOfTheWeek(true);
                if (adow == null) return RedirectToAction("Index", "Home");
                id = adow.ID;
            }
            SessionUser cuser = AppHelper.CurrentUser;
            if (cuser != null && !cuser.IsBuyerType) return RedirectToAction("Index", "Home");
            AuctionDetail dow = AuctionRepository.GetDowDetail(id.GetValueOrDefault(0), cuser != null ? cuser.ID : -1);
            if (dow == null || dow.Status != 2) return RedirectToAction("Index", "Home");
            if (cuser != null)
                AuctionRepository.RemoveReservedAuctionQuantityForAuction(cuser.ID, id.GetValueOrDefault(-1));
            ViewData["CurrentAuctionImages"] = ImageRepository.GetAuctionImages(dow.LinkParams.ID, true);
            ViewData["Insurance"] = VariableRepository.GetInsurance();
            ViewData["LeftUserControlVisibility"] = false;
            return View(dow);
        }
        #endregion


        #region for testing

        //AuctionDetailTest
        [NonAction]
        [RequireSslFilter(IsRequired = "False"), HttpGet, Compress, NoCache]
        public ActionResult AuctionDetailTest(long? id)
        {
            if (!id.HasValue) return RedirectToAction("Index", "Home");
            AuctionDetail auction = AuctionRepository.GetAuctionDetail(id.Value, true);
            if (auction == null || ((auction.IsPrivate && !auction.IsViewable) && !auction.IsAccessable)) return RedirectToAction("EventDetailed", "Auction");
            AuctionResultDetail aresult = ((auction.IsCurrent || auction.EndDate.AddMinutes(20) > DateTime.Now) ? AuctionRepository.GetAuctionResultCurrent(id.Value, true) : AuctionRepository.GetAuctionResult(id.Value, true)) ?? new AuctionResultDetail();
            ViewData["FullCategoryLink"] = CategoryRepository.GetFullEventCategoryLink(auction.LinkParams.EventCategory_ID);
            AuctionUserInfo aui = new AuctionUserInfo { IsInWatchList = false, IsRegisterForEvent = false };
            SessionUser cuser = AppHelper.CurrentUser;
            if (cuser != null)
            {
                aui.IsRegisterForEvent = EventRepository.IsUserRegisterForEvent(cuser.ID, auction.LinkParams.Event_ID) || cuser.IsAdminType;
                aui.IsInWatchList = AuctionRepository.IsUserWatchItem(cuser.ID, auction.LinkParams.ID);
            }
            ViewData["AuctionUserInfo"] = aui;
            ViewData["LotResult"] = aresult;
            BiddingResult bresult = new BiddingResult(auction.Price, auction.Quantity, auction.AuctionType);
            if (cuser == null || (aresult != null && !aresult.HasBid))
            {
                ViewData["AuctionDetailReturnUrl"] = String.Format("/Auction/AuctionDetail/{0}/{1}/{2}/{3}", auction.LinkParams.ID, auction.LinkParams.EventUrl, auction.LinkParams.CategoryUrl, auction.LinkParams.LotTitleUrl);
                ViewData["BiddingResult"] = bresult;
                return View(auction);
            }
            if (auction.Status == (byte)Consts.AuctionStatus.Open)
            {
                if (auction.AuctionType == (long)Consts.AuctionType.Normal)
                {
                    BidCurrent bc = BidRepository.GetTopBidForItem(auction.LinkParams.ID, auction.Quantity, true);
                    if (bc != null) bresult.WinningBids.Add(bc);
                    if (aui.IsInWatchList)
                    {
                        bc = BidRepository.GetUserTopBidForItem(auction.LinkParams.ID, cuser.ID, true);
                        if (bc != null) bresult.UserTopBids.Add(bc);
                    }
                }
                else
                {
                    List<BidCurrent> bc = BidRepository.GetBidsForMultipleItem(auction.LinkParams.ID, 1, auction.Quantity, true);
                    if (bc.Any()) bresult.WinningBids.AddRange(bc);
                    bc = BidRepository.GetBidsForMultipleItem(auction.LinkParams.ID, 0, auction.Quantity, true);
                    if (bc.Any()) bresult.LoserBids.AddRange(bc);
                    bresult.UserTopBids.AddRange(bresult.WinningBids.Where(wb => wb.User_ID == cuser.ID).ToList());
                }
                ViewData["BiddingResult"] = bresult;
            }
            return View(auction);
        }

        //PlaceBidTest
        [NonAction]
        [HttpPost, Compress, RequireSslFilter(IsRequired = "False"), NoCache]
        public ActionResult PlaceBidTest(long? id)
        {
            if (!id.HasValue) return RedirectToAction("Index", "Home");
            AuctionDetail auction = AuctionRepository.GetAuctionDetail(id.Value, true);
            if (auction == null || ((!auction.IsPrivate && !auction.IsViewable) && !auction.IsAccessable)) return RedirectToAction("EventDetailed", "Auction");
            AuctionUserInfo aui = new AuctionUserInfo { IsInWatchList = false, IsRegisterForEvent = false };

            SessionUser cuser = AppHelper.CurrentUser;
            if (cuser != null)
            {
                aui.IsRegisterForEvent = EventRepository.IsUserRegisterForEvent(cuser.ID, auction.LinkParams.Event_ID) || cuser.IsAdminType;
                aui.IsInWatchList = AuctionRepository.IsUserWatchItem(cuser.ID, auction.LinkParams.ID);
            }
            BiddingResult bresult = new BiddingResult(auction.Price, auction.Quantity, auction.AuctionType);
            if (auction.AuctionType == (long)Consts.AuctionType.Normal)
            {
                BidCurrent bc = BidRepository.GetTopBidForItem(auction.LinkParams.ID, auction.Quantity, true);
                if (bc != null) bresult.WinningBids.Add(bc);
                if (aui.IsInWatchList)
                {
                    bc = BidRepository.GetUserTopBidForItem(auction.LinkParams.ID, cuser.ID, true);
                    if (bc != null) bresult.UserTopBids.Add(bc);
                }
            }
            //else
            //{
            //  List<BidCurrent> bc = BidRepository.GetTopBidsForMultipleItem(auction.LinkParams.ID);
            //  if (bc.Any()) bresult.WinningBids.AddRange(bc);
            //  bc = BidRepository.GetLowerBidsForMultipleItem(auction.LinkParams.ID);
            //  if (bc.Any()) bresult.LoserBids.AddRange(bc);
            //  bc = bresult.WinningBids.Where(WB => WB.User_ID == cuser.ID).ToList();
            //  if (bc.Any()) bresult.UserTopBids.AddRange(bc);
            //}
            //ViewData["BiddingResult"] = bresult;
            decimal bid = bresult.MinBid == 0 ? auction.Price : (bresult.UserWinBid == null || bresult.UserWinBid.MaxBid < bresult.MinBid) ? bresult.MinBid : bresult.UserWinBid.MaxBid;
            bid = bresult.MinBid == 0 ? bid : bid + Consts.GetIncrement(bid);
            bid = bid + Consts.GetIncrement(bid);
            return PlaceBid(true, bid, auction.LinkParams.ID, 1, bid);
        }
        #endregion
    }
}