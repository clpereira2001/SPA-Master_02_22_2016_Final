using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using Relatives.Models.CustomBinders;
using Vauction.Models;
using Vauction.Models.CustomClasses;
using Vauction.Models.CustomModels;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Helpers;
using Vauction.Utils.Perfomance;
using System.Text;
using System.Collections.Generic;
using Vauction.Models.Enums;

namespace Vauction.Controllers
{
    [HandleError, CrossSessionCheck]
    public class HomeController : BaseController
    {
        #region init
        IAuctionRepository AuctionRepository;
        ICategoryRepository CategoryRepository;
        IUserRepository UserRepository;
        IEventRepository EventRepository;
        public HomeController()
        {
            AuctionRepository = dataProvider.AuctionRepository;
            CategoryRepository = dataProvider.CategoryRepository;
            UserRepository = dataProvider.UserRepository;
            EventRepository = dataProvider.EventRepository;
        }
        #endregion

        #region Index
        // Index
        [HttpGet, RequireSslFilter(IsRequired = "False"), Compress, NoCache/*,OutputCache(Location = OutputCacheLocation.None, NoStore = true, Duration = 0, VaryByParam = "*")*/]
        public ActionResult Index([ModelBinder(typeof(CustomItemBinder))]AuctionFilterParams param)
        {
            InitCurrentEvent();
            ViewData["AuctionFilterParams"] = param;
            ViewData["IsHomePage"] = true;
            ViewData["DOW"] = AuctionRepository.GetDealOfTheWeek(true);

            bool? demo = (ViewData["DEMO_MODE"] != null) ? Convert.ToBoolean(ViewData["DEMO_MODE"]) : false;

            ViewData["PageActionPath"] = "Index";
            ViewData["HidePager"] = true;
            ViewData["HidePreview"] = true;
            SetFilterParams(param);
            param.ViewMode = (int)Consts.AuctonViewMode.Grid;

            EventDetail ed = ViewData["CurrentEvent"] as EventDetail;
            SessionUser cuser = AppHelper.CurrentUser;
            ViewData["Lots"] = AuctionRepository.GetFeaturedList(ed.ID, cuser == null ? -1 : cuser.ID, Convert.ToBoolean(ViewData["UserRegisterForEvent"]), 0, 21);
            ViewData["lnkCategoryParentChild"] = CategoryRepository.GetCategoriesMapTreeJSON(ed.ID, (demo.HasValue) ? demo.Value && (cuser != null && cuser.IsAccessable) : false, "");
          
            Session["lnkCategoryParentChild"] = ViewData["lnkCategoryParentChild"];

            Session["AUCTIONDETAIL"] = "";

            return View();
        }
        /*
        // pIndex
        private object pIndex(AuctionFilterParams param, long event_id, long user_id)
        {
          ViewData["PageActionPath"] = "Index";
          ViewData["HidePager"] = true;
          ViewData["HidePreview"] = true;
          SetFilterParams(param);
          param.ViewMode = (int)Consts.AuctonViewMode.Grid;
          return AuctionRepository.GetFeaturedList(event_id, user_id, 0, 21);
        }
        // pIndexUn
        [ChildActionOnly] //, ActionOutputCache(Consts.CachingTime_60min)
        public ActionResult pIndexUn(long event_id, long user_id, AuctionFilterParams param, bool iscurrent)
        {
          return View("PartialAuctionGrid", pIndex(param, event_id, user_id));
        }
        // pIndex
        [ChildActionOnly]
        public ActionResult pIndex(long event_id, long user_id, AuctionFilterParams param, bool iscurrent)
        {
          return View("PartialAuctionGrid", pIndex(param, event_id, user_id));
        }*/
        #endregion

        #region Free Alerts Register
        //FreeRegisterLinkedData
        public void FreeRegisterLinkedData()
        {
            ViewData["Countries"] = new SelectList(dataProvider.CountryRepository.GetCountryList(), "Title", "Title");
            ViewData["States"] = new SelectList(dataProvider.CountryRepository.GetStateList(null), "Code", "Code");
        }

        //FreeEmailAlertsRegister
        [RequireSslFilter, HttpGet, Compress, NoCache]
        public ActionResult FreeEmailAlertsRegister()
        {
            InitCurrentEvent();
            FreeRegisterLinkedData();
            return View(new OuterSubscription());
        }

        //FreeEmailAlertsRegistrationSuccess
        [RequireSslFilter, HttpPost, NoCache] //, ValidateAntiForgeryToken
        public ActionResult FreeEmailAlertsRegistrationSuccess()
        {
            InitCurrentEvent();
            OuterSubscription os = new OuterSubscription();
            if (!TryUpdateModel(os, new[] { "Country", "Email", "EmailConfirm", "FirstName", "LastName", "State", "IsRecievingWeeklySpecials", "IsRecievingUpdates" }))
            {
                FreeRegisterLinkedData();
                return View("FreeEmailAlertsRegister", os);
            }
            os.Validate(ModelState);
            if (ModelState.IsValid)
            {
                os.IPAddress = Consts.UsersIPAddress;
                os.IsActive = false;
                if (!UserRepository.AddOuterSubscription(os))
                {
                    FreeRegisterLinkedData();
                    return View("FreeEmailAlertsRegister", os);
                }
                Mail.SendFreeEmailRegisterConfirmation(os.Email, os.FirstName, os.LastName, AppHelper.GetSiteUrl(Url.Action("FreeEmailAlertsRegisterConfirm", "Home", new { id = os.ID })));
                return View();
            }
            FreeRegisterLinkedData();
            return View("FreeEmailAlertsRegister", os);
        }

        //FreeEmailAlertsRegisterConfirm
        [RequireSslFilter, HttpGet, Compress, NoCache]
        public ActionResult FreeEmailAlertsRegisterConfirm(long? id)
        {
            InitCurrentEvent();
            if (!id.HasValue || !UserRepository.ActivateOuterSubscription(id))
            {
                FreeRegisterLinkedData();
                return View("FreeEmailAlertsRegister", new OuterSubscription());
            }
            return View();
        }

        //FreeEmailAlertsUnsubscribeSuccess
        [RequireSslFilter, HttpPost, NoCache] //, ValidateAntiForgeryToken
        public ActionResult FreeEmailAlertsUnsubscribeSuccess(string Email)
        {
            InitCurrentEvent();
            bool res1, res2;
            res1 = res2 = false;
            if (string.IsNullOrEmpty(Email)) return RedirectToAction("FreeEmailAlertsRegister");
            res1 = UserRepository.UnsubscribeFromEmail(Email);
            res2 = UserRepository.UnsubscribeRegisterUser(Email);
            if (!res1 && !res2) return RedirectToAction("FreeEmailAlertsRegister");
            ViewData["Email"] = Email;
            return View("EmailAlertsUnsubscribeSuccess");
        }

        //EmailAlertsUnsubscribeSuccess
        [HttpGet, RequireSslFilter, NoCache]
        public ActionResult EmailAlertsUnsubscribeSuccess(long? id, char? t)
        {
            if (!id.HasValue)
            {
                return RedirectToAction("FreeEmailAlertsRegister");
            }
            if (t.HasValue)
            {
                if (t.Value == 'U')
                {
                    User usr = UserRepository.GetUser(id.Value, false);
                    if (usr != null)
                    {
                        UserRepository.UnsubscribeFromEmail(usr.Email);
                        UserRepository.UnsubscribeRegisterUser(id.Value);
                        ViewData["Email"] = usr.Email;
                    }
                }
                else if (t.Value == 'M')
                {
                    OuterSubscription os = UserRepository.GetOuterSubscription(id.Value);
                    if (os != null)
                    {
                        UserRepository.UnsubscribeRegisterUser(os.Email);
                        UserRepository.UnsubscribeFromOuterSubscribtionByID(id.Value);
                        ViewData["Email"] = os.Email;
                    }
                }
            }
            else
            {
                User usr = UserRepository.GetUser(id.Value, false);
                if (usr != null)
                {
                    UserRepository.UnsubscribeFromEmail(usr.Email);
                    UserRepository.UnsubscribeRegisterUser(id.Value);
                    ViewData["Email"] = usr.Email;
                }
            }
            if (ViewData["Email"] == null) return RedirectToAction("FreeEmailAlertsRegister");
            InitCurrentEvent();
            return View();
        }

        #endregion

        #region html pages
        //ResourceCenter
        [Compress, NoCache]
        public ActionResult ResourceCenter()
        {
            InitCurrentEvent();
            return View();
        }

        //FAQs
        [Compress, NoCache]
        public ActionResult FAQs()
        {
            InitCurrentEvent();
            return View();
        }

        //ConstactUs
        [Compress, NoCache]
        public ActionResult ContactUs()
        {
            InitCurrentEvent();
            return View();
        }

        //Terms
        [Compress, NoCache]
        public ActionResult Terms()
        {
            InitCurrentEvent();
            return View();
        }

        //Privacy
        [Compress, NoCache]
        public ActionResult Privacy()
        {
            InitCurrentEvent();
            return View();
        }

        //SiteMap
        [Compress, NoCache]
        public ActionResult SiteMap()
        {
            InitCurrentEvent();
            SessionUser cuser = AppHelper.CurrentUser;
            ViewData["UpcomingEvents"] = EventRepository.GetUpcomingList(Request.IsAuthenticated && cuser != null && cuser.IsAccessable);
            return View();
        }

        //Help
        [Compress, NoCache]
        public ActionResult Help()
        {
            InitCurrentEvent();
            return View();
        }

        //Product
        [Compress, NoCache]
        public ActionResult Product()
        {
            InitCurrentEvent();
            return View();
        }
        #endregion

        //GetCategoriesTreeChild
        [ChildActionOnly]
        public ContentResult GetCategoriesTreeChild(long event_id, bool? demo)
        {
            SessionUser cuser = AppHelper.CurrentUser;
            return Content(CategoryRepository.GetCategoriesMapTreeJSON(event_id, (demo.HasValue) && (demo.Value && (cuser != null && cuser.IsAccessable))).ToString());
        }


        [Compress, NoCache]
        public JsonResult GetCategoriesTree(long? event_id, bool? demo)
        {
            SessionUser cuser = AppHelper.CurrentUser;
            long evnt_id = (event_id.HasValue) ? event_id.Value : EventRepository.GetCurrent().ID;
            return Json(CategoryRepository.GetCategoriesMapTreeJSON(evnt_id, (demo.HasValue) ? demo.Value && (cuser != null && cuser.IsAccessable) : false), JsonRequestBehavior.AllowGet);
        }

        [Compress, NoCache]
        public List<CategoryParentChild> GetCategoriesTree(long? event_id, bool? demo, string strChk)
        {
            SessionUser cuser = AppHelper.CurrentUser;
            long evnt_id = (event_id.HasValue) ? event_id.Value : EventRepository.GetCurrent().ID;

            List<CategoryParentChild> lnkCategoryParentChild = CategoryRepository.GetCategoriesMapTreeJSON(evnt_id, (demo.HasValue) ? demo.Value && (cuser != null && cuser.IsAccessable) : false, strChk);
            return lnkCategoryParentChild;
           // return new HtmlString(CategoryRepository.GetCategoriesMapTreeJSON(evnt_id, (demo.HasValue) ? demo.Value && (cuser != null && cuser.IsAccessable) : false, strChk).ToString());
        }
        //public static MvcHtmlString MenuItem(this HtmlHelper htmlHelper, string text, string action, string controller, string num, string icon)
        //{
        //    var routeData = htmlHelper.ViewContext.RouteData;
        //    var currentAction = routeData.GetRequiredString("action");
        //    var currentController = routeData.GetRequiredString("controller");
        //    bool isCurrent = string.Equals(currentAction, action, StringComparison.OrdinalIgnoreCase) && string.Equals(currentController, controller, StringComparison.OrdinalIgnoreCase);
        //    UrlHelper urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
        //    string url = urlHelper.Action(action, controller);
        //    StringBuilder html = new StringBuilder();
        //    TagBuilder span = new TagBuilder("span");
        //    span.AddCssClass("badge pull-right");
        //    span.InnerHtml = num;
        //    html.Append(span);
        //    TagBuilder i = new TagBuilder("i");
        //    i.AddCssClass(icon);
        //    html.Append(i);
        //    span = new TagBuilder("span");
        //    span.InnerHtml = text;
        //    html.Append(span);
        //    TagBuilder a = new TagBuilder("a");
        //    a.MergeAttribute("href", url);
        //    if (isCurrent)
        //    {
        //        a.AddCssClass("current");
        //    }
        //    a.InnerHtml = html.ToString();
        //    TagBuilder li = new TagBuilder("li");
        //    if (isCurrent)
        //    {
        //        li.AddCssClass("current");
        //    }
        //    li.InnerHtml = a.ToString();
        //    return MvcHtmlString.Create(li.ToString());
        //}

        #region Search
        //Search
        [HttpGet, Compress, NoCache]
        public ActionResult Search([ModelBinder(typeof(CustomItemBinder))]AuctionFilterParams param)
        {
            InitCurrentEvent();
            ViewData["Method"] = 0;
            ViewData["HideAdvancedSearchBlock"] = true;
            SetFilterParams(param);
            return View("SearchResult");
        }

        //pSearch
        private object pSearch(AuctionFilterParams param, byte method)
        {
            SessionUser cuser = AppHelper.CurrentUser;
            SetFilterParams(param);
            bool FullAccess = cuser != null && cuser.IsAccessable;
            if (method == 0)
            {
                if (string.IsNullOrEmpty(param.Description)) param.Description = "";
                ViewData["PageActionPath"] = "Search";
                param.Title = param.Description;
                return AuctionRepository.GetBySimpleCriterias(param, FullAccess, cuser == null ? -1 : cuser.ID);
            }
            ViewData["PageActionPath"] = "SearchResult";
            return (method == 1) ? AuctionRepository.GetByCriterias(param, FullAccess, cuser == null ? -1 : cuser.ID) : AuctionRepository.GetByAuctionID(param, FullAccess, cuser == null ? -1 : cuser.ID);
        }

        //pSearchUn
        [ChildActionOnly]
        public ActionResult pSearchUn(byte method, AuctionFilterParams param, int page)
        {
            return View("PartialAuctionGrid", pSearch(param, method));
        }
        //pSearch
        [ChildActionOnly]
        public ActionResult pSearch(byte method, AuctionFilterParams param, int page)
        {
            return View("PartialAuctionGrid", pSearch(param, method));
        }

        //SearchResult
        [HttpGet, Compress, NoCache]
        public ActionResult SearchResult([ModelBinder(typeof(CustomItemBinder))]AuctionFilterParams param)
        {
            InitCurrentEvent();
            ViewData["Method"] = 1;
            SetFilterParams(param);
            return View();
        }
        //SearchByAuctionID
        [HttpGet, Compress, NoCache]
        public ActionResult SearchByAuctionID([ModelBinder(typeof(CustomItemBinder))]AuctionFilterParams param)
        {
            InitCurrentEvent();
            ViewData["Method"] = 2;
            SetFilterParams(param);
            return View("SearchResult");
        }
        //AdvancedSearch
        [HttpGet, Compress, NoCache]
        public ActionResult AdvancedSearch()
        {
            InitCurrentEvent();
            return View();
        }
        #endregion

        #region edit content
        [HttpPost, NoCache]
        public JsonResult EditContent(string file, string content)
        {
            FileInfo fi = new FileInfo(Server.MapPath("~/Views/Home/" + file + ".ascx"));
            if (!fi.Exists)
                return JSON(new JsonExecuteResult(JsonExecuteResultTypes.ERROR, "The content file doesn't exist."));
            StreamWriter fs = fi.CreateText();
            fs.WriteLine("<%@ Control Language=\"C#\" Inherits=\"System.Web.Mvc.ViewUserControl\" %>");
            fs.Write(content);
            fs.Close();
            return JSON(new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS));
        }
        #endregion

        #region emails
        [HttpGet, Compress, NoCache]
        public ActionResult AnnoucementPreview(long? email_id, long? id, string t)
        {
            if (!id.HasValue || String.IsNullOrEmpty(t) || !email_id.HasValue) return RedirectToAction("Index", "Home");
            Email_Subscriber es = UserRepository.GetEmailSubscriber(id.GetValueOrDefault(0), t);
            if (es == null) return RedirectToAction("Index", "Home");
            ViewData["html_email"] = dataProvider.VariableRepository.GetAnnouncementEmail(email_id.GetValueOrDefault(0), es.ID, es.T, es.FullName, es.UserName);
            ViewData["title"] = dataProvider.VariableRepository.GetEmail(email_id.GetValueOrDefault(0)).Title;
            return View();
        }
        #endregion

        [NonAction]
        [VauctionAuthorize]
        public ActionResult Test()
        {
            return View();
        }

    }
}