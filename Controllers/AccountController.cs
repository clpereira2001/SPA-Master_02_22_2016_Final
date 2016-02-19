using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using QControls.Validation;
using Vauction.Models;
using Vauction.Utils.Helpers;
using Vauction.Utils.Lib;
using Vauction.Utils.Paging;
using Vauction.Models.CustomModels;
using Relatives.Models.CustomBinders;
using Vauction.Utils;
using Vauction.Utils.Authorized.NET;
using Vauction.Utils.PayPal;
using Vauction.Utils.Autorization;
using System.Web.Routing;
using Vauction.Utils.Perfomance;

namespace Vauction.Controllers
{
    [HandleError, CrossSessionCheck]
    public class AccountController : BaseController
    {
        #region init
        IUserRepository UserRepository;
        IAuctionRepository AuctionRepository;
        IInvoiceRepository InvoiceRepository;
        IBidRepository BidRepository;
        ICountryRepository CountryRepository;
        IVariableRepository VariableRepository;
        ICategoryRepository CategoryRepository;
        IEventRepository EventRepository;
        public AccountController()
        {
            UserRepository = dataProvider.UserRepository;
            AuctionRepository = dataProvider.AuctionRepository;
            InvoiceRepository = dataProvider.InvoiceRepository;
            BidRepository = dataProvider.BidRepository;
            CountryRepository = dataProvider.CountryRepository;
            VariableRepository = dataProvider.VariableRepository;
            CategoryRepository = dataProvider.CategoryRepository;
            EventRepository = dataProvider.EventRepository;
        }

        public IFormsAuthenticationService FormsService { get; set; }
        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            base.Initialize(requestContext);
        }
        #endregion

        #region log in & out
        //LogOn
        [HttpGet, RequireSslFilter, Compress, NoCache]
        public ActionResult LogOn()
        {
            InitCurrentEvent();
            ViewData["IsLogON"] = true;
            ViewData["ContentPageHeader"] = "1";

            return View();
        }
        //LogOn
        [HttpPost, RequireSslFilter, Compress, NoCache] //ValidateAntiForgeryToken
        public ActionResult LogOn(string login, string password, bool? rememberMe, string returnUrl)
        {
            if (Session["lstCategoryParentChild"] != null)
            {

            }
            ViewData["IsLogON"] = true;
            ViewData["ContentPageHeader"] = "1";
            if (ValidationCheck.IsEmpty(login))
            {
                InitCurrentEvent();
                ModelState.AddModelError("login", ErrorMessages.GetRequired("UserID"));
                return View();
            }
            if (String.IsNullOrEmpty(password))
            {
                InitCurrentEvent();
                ModelState.AddModelError("password", ErrorMessages.GetRequired("Password"));
                return View();
            }
            User trying = UserRepository.GetUser(login, false);

            if (trying == null || trying.Password.ToLower() != password.ToLower())
            {
                InitCurrentEvent();
                ModelState.AddModelError("login", "Incorrect UserID or password.");
                ViewData["wrongPass"] = true;
                ViewData["rememberMe"] = rememberMe.HasValue ? rememberMe.Value : false;
                return View();
            }
            if (trying.Status == (byte)Consts.UserStatus.Locked)
            {
                InitCurrentEvent();
                ViewData["isLocked"] = true;
                return View();
            }
            if (trying.Status == (byte)Consts.UserStatus.Pending || !trying.IsConfirmed)
            {
                InitCurrentEvent();
                ViewData["notConfirmed"] = true;
                return View();
            }

            UserRepository.TryToUpdateNormalAttempts(trying);
            FormsService.SignIn(login, rememberMe.HasValue && rememberMe.Value, trying);
            EventDetail ed = EventRepository.GetEventDetail(null);
            if (!ed.RegisterRequired && ed.IsClickable)
            {
                EventRepository.RegisterUserForEvent(ed.ID, trying.ID);
            }
            if (!trying.IsModifyed || trying.NotPasswordReset)
            {
                Session["redirectUrl"] = returnUrl;
                return RedirectToAction("Profile", "Account");
            }
            if (HttpContext.Request.IsUrlLocalToHost(returnUrl))
            {
                InitCurrentEvent();
                return Redirect(returnUrl);
            }

            if (Session["lstCategoryParentChild"] != null)
            {

            }
            return RedirectToAction("Index", "Home");
        }
        //LogOff
        [HttpGet, Compress, NoCache]
        public ActionResult LogOff()
        {
            InitCurrentEvent();
            FormsService.SignOut();
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region register / profile
        //LoadLinkedUserData
        [NonAction]
        private void LoadLinkedUserData(long? country, string zip)
        {
            ViewData["Countries"] = (country.HasValue) ? new SelectList(dataProvider.CountryRepository.GetCountryList(), "ID", "Title", country) : ViewData["Countries"] = new SelectList(dataProvider.CountryRepository.GetCountryList(), "ID", "Title");
            ViewData["States"] = (!string.IsNullOrEmpty(zip)) ? new SelectList(dataProvider.CountryRepository.GetStateList(null), "Code", "Code", zip) : ViewData["States"] = new SelectList(dataProvider.CountryRepository.GetStateList(null), "Code", "Code");
        }

        //Register (get)
        [RequireSslFilter, HttpGet, Compress, NoCache]
        public ActionResult Register()
        {
            InitCurrentEvent();
            LoadLinkedUserData(null, null);
            return View(new RegisterInfo());
        }
        //Register (post)
        [HttpPost, Compress, NoCache] //ValidateAntiForgeryToken,
        public ActionResult Register(string ConfirmCode)
        {
            InitCurrentEvent();
            RegisterInfo user = new RegisterInfo();
            string[] updateFields = new[] { "Agree", "Login", "Email", "ConfirmEmail", "Password", "ConfirmPassword", "RecieveWeeklySpecials", "RecieveNewsUpdates", "Fax", "Reference", "BillingLikeShipping", "BillingFirstName", "BillingMIName", "BillingLastName", "BillingAddress1", "BillingAddress2", "BillingCity", "BillingState", "BillingZip", "BillingPhone", "BillingWorkPhone", "BillingFax", "BillingCountry", "ShippingFirstName", "ShippingMIName", "ShippingLastName", "ShippingAddress1", "ShippingAddress2", "ShippingCity", "ShippingState", "ShippingZip", "ShippingPhone", "ShippingWorkPhone", "ShippingCountry", "ShippingFax" };
            if (TryUpdateModel(user, updateFields))
            {
                user.Validate(ModelState);
                if (ModelState.IsValid)
                {
                    User newUser = UserRepository.AddUser(user);
                    if (newUser == null)
                    {
                        LoadLinkedUserData(user.BillingCountry, user.BillingState);
                        return View(user);
                    }

                    Mail.SendRegisterConfirmation(user.BillingFirstName, user.BillingLastName, user.Email, user.Login, AppHelper.GetSiteUrl("/Account/RegisterFinish/" + newUser.ConfirmationCode));
                    return View("RegisterConfirm");
                }
                bool EmailExistsConfirmed = false;
                bool EmailExistsNonConfirmed = false;
                UserRepository.CheckEmailInDB(user.Email, out EmailExistsConfirmed, out EmailExistsNonConfirmed);
                ViewData["EmailExistsConfirmed"] = EmailExistsConfirmed;
                ViewData["EmailExistsNonConfirmed"] = EmailExistsNonConfirmed;
            }
            LoadLinkedUserData(user.BillingCountry, user.BillingState);
            return View(user);
        }
        //RegisterFinish
        [RequireSslFilter, HttpGet, Compress, NoCache]
        public ActionResult RegisterFinish(string id)
        {
            InitCurrentEvent();
            if (String.IsNullOrEmpty(id)) return View("RegisterConfirmFail");
            User user = UserRepository.ConfirmUser(id);
            if (user == null) return View("RegisterConfirmFail");
            if (!UserRepository.ActivateUser(user)) return View("RegisterConfirmFail");
            UserRepository.SubscribeRegisterUser(user);
            //FormsService.SignIn(user.Login, false, user);
            //UserRepository.TryToUpdateNormalAttempts(user);
            return View("RegisterConfirmSuccess");
        }

        [VauctionAuthorize, HttpGet, Compress, NoCache]
        public ActionResult Profile()
        {
            InitCurrentEvent();
            RegisterInfo user = UserRepository.GetRegisterInfo(AppHelper.CurrentUser.ID);
            if (user == null)
            {
                FormsService.SignOut();
                Session.Remove("redirectUrl");
                return RedirectToAction("LogOn");
            }
            LoadLinkedUserData(user.BillingCountry, user.BillingState);
            return View(user);
        }

        [VauctionAuthorize, HttpPost, Compress, NoCache] //ValidateAntiForgeryToken,
        public ActionResult Profile(string ConfirmCode)
        {
            SessionUser cuser = AppHelper.CurrentUser;
            InitCurrentEvent();
            RegisterInfo user = UserRepository.GetRegisterInfo(cuser.ID);
            string oldemail = user.Email;
            string[] updateFields = new[] { "Agree", "Login", "Email", "ConfirmEmail", "Password", "ConfirmPassword", "RecieveWeeklySpecials", "RecieveNewsUpdates", "Fax", "Reference", "BillingLikeShipping", "BillingFirstName", "BillingMIName", "BillingLastName", "BillingAddress1", "BillingAddress2", "BillingCity", "BillingState", "BillingZip", "BillingPhone", "BillingWorkPhone", "BillingFax", "BillingCountry", "ShippingFirstName", "ShippingMIName", "ShippingLastName", "ShippingAddress1", "ShippingAddress2", "ShippingCity", "ShippingState", "ShippingZip", "ShippingPhone", "ShippingWorkPhone", "ShippingCountry", "ShippingFax" };
            UpdateModel(user, updateFields);
            user.Validate(ModelState);
            if (!ModelState.IsValid)
            {
                LoadLinkedUserData(user.BillingCountry, user.BillingState);
                return View(user);
            }

            User usr = UserRepository.UpdateUser(user);
            if (usr == null)
            {
                LoadLinkedUserData(user.BillingCountry, user.BillingState);
                return View(user);
            }

            if (oldemail != usr.Email)
                Mail.EmailConfirmationCode(user.BillingFirstName, user.BillingLastName, user.Email, user.Login, AppHelper.GetSiteUrl("/Account/EmailConfirmation/" + usr.ConfirmationCode));

            if (cuser.ID == usr.ID)
                cuser = SessionUser.Create(usr as User);

            InitCurrentEvent();

            string url = String.Empty;
            if (Session["redirectUrl"] != null && !String.IsNullOrEmpty(url = Session["redirectUrl"].ToString()))
            {
                Session.Remove("redirectUrl");
                return Redirect(url);
            }
            return View("ProfileSaveMessage");
        }
        #endregion

        #region forgot password /resent confirmation / email confirmation
        //ResendConfirmationCode (get)
        [RequireSslFilter, HttpGet, Compress, NoCache]
        public ActionResult ResendConfirmationCode()
        {
            InitCurrentEvent();
            return View(new ResendConfirmationCode());
        }
        //ResendConfirmationCode (post)
        [HttpPost, Compress, NoCache] //ValidateAntiForgeryToken,
        public ActionResult ResendConfirmationCode(string Email)
        {
            InitCurrentEvent();
            ResendConfirmationCode data = new ResendConfirmationCode();
            UpdateModel(data, new[] { "Email" });
            data.Validate(ModelState);
            if (ModelState.IsValid && !String.IsNullOrEmpty(Email))
            {
                User usr = UserRepository.GetUserByEmail(Email, false);
                if (usr == null)
                {
                    ModelState.AddModelError("Email", "The user with this email doesn't exist in the system");
                    return View(data);
                }
                if (String.IsNullOrEmpty(usr.ConfirmationCode) || (usr.ConfirmationCode.Length < 20))
                    if (!UserRepository.GenerateNewConfirmationCode(usr)) return View(data);
                AddressCard ac = UserRepository.GetAddressCard(usr.Billing_AddressCard_ID.GetValueOrDefault(-1), true);
                Mail.ResendConfirmationCode(ac.FirstName, ac.LastName, usr.Email, usr.Login, AppHelper.GetSiteUrl("/Account/RegisterFinish/" + usr.ConfirmationCode));
                InitCurrentEvent();
                return View("ResendConfirmationCodeUpdate");
            }
            return View(data);
        }

        //ForgotPassword (get)
        [RequireSslFilter, HttpGet, Compress, NoCache]
        public ActionResult ForgotPassword()
        {
            return View(new ForgotPassword());
        }
        //ForgotPassword (post)
        [HttpPost, Compress, NoCache] //ValidateAntiForgeryToken, 
        public ActionResult ForgotPassword(string Email)
        {
            ForgotPassword data = new ForgotPassword();
            UpdateModel(data, new[] { "Email" });
            data.Validate(ModelState);
            if (!ModelState.IsValid) return View(data);
            User user = UserRepository.SetNewUserPassword(data.Email);
            if (user != null)
            {
                AddressCard ac = UserRepository.GetAddressCard(user.Billing_AddressCard_ID.GetValueOrDefault(-1), true);
                Mail.ForgotPassword(ac.FirstName, ac.LastName, user.Email, user.Login, user.Password);
                InitCurrentEvent();
                return View("ForgotPasswordUpdate");
            }
            return View(data);
        }

        //EmailConfirmation
        [RequireSslFilter, HttpGet, Compress, NoCache]
        public ActionResult EmailConfirmation(string id)
        {
            InitCurrentEvent();
            if (String.IsNullOrEmpty(id)) return View(false);
            User user = UserRepository.ConfirmUser(id);
            if (user == null) return View(false);
            SessionUser cuser = AppHelper.CurrentUser;
            if (cuser != null && cuser.ID == user.ID) cuser.Email = user.Email;
            return View(true);
        }

        #endregion

        #region my account
        //MyAccount
        [VauctionAuthorize, RequireSslFilter, HttpGet, Compress, NoCache]
        public ActionResult MyAccount()
        {
            InitCurrentEvent();
            return View();
        }

        #region auctions you have participated in
        //PastAuction
        [VauctionAuthorize, RequireSslFilter, NoCache]
        public ActionResult PastAuction()
        {
            InitCurrentEvent();
            SessionUser cuser = AppHelper.CurrentUser;
            if (cuser != null)
            {
                List<Event> lstEvent = BidRepository.GetPastEventBiddingHistory(cuser.ID);
                ViewData["lstEvent"] = "";
                ViewData["lstEvent"] = lstEvent;
            }

            return View();
        }
        //pPastAuction
        [ChildActionOnly]
        public ActionResult pPastAuction(long event_id, long user_id)
        {
            //List<Event> lstEvent = BidRepository.GetPastEventBiddingHistory(user_id);
            //Session["lstEvent"] = "";
            //Session["lstEvent"] = lstEvent;
            return View("pPastAuction", BidRepository.GetPastEventBiddingHistory(user_id));
        }

        [VauctionAuthorize, RequireSslFilter, HttpGet, Compress, NoCache]
        public ActionResult AuctionsParticipated([ModelBinder(typeof(CustomItemBinder))]AuctionFilterParams param)
        {
            if (!param.ID.HasValue) return RedirectToAction("PastAuction");
            Event ev = EventRepository.GetEventByID(param.ID.Value);
            if (ev == null || ev.DateEnd >= DateTime.Now) return RedirectToAction("PastAuction");
            ViewData["Event"] = ev;
            SetFilterParams(param);
            return View();
        }

        //pAuctionsParticipated
        [ChildActionOnly]
        public ActionResult pAuctionsParticipated(long event_id, long user_id, AuctionFilterParams param, int page)
        {
            SetFilterParams(param);
            ViewData["PageActionPath"] = "AuctionsParticipated";
            return View("pAuctionsParticipated", BidRepository.GetPastUsersWatchList(event_id, user_id, page, param.PageSize));
        }
        #endregion

        #region past auction invoices
        //PastInvoices
        [VauctionAuthorize, RequireSslFilter, HttpGet, Compress]
        public ActionResult PastInvoices()
        {
            InitCurrentEvent();
            SessionUser cuser = AppHelper.CurrentUser;
            //long id = 241672;
            List<Invoice> invoices = InvoiceRepository.GetUserInvoicesDOW(cuser.ID);
            ViewData["isDOW"] = invoices.Count() > 0;
            return View(InvoiceRepository.GetPastUserInvoices(cuser.ID));
        }
        //InvoiceDetailed
        [VauctionAuthorize, RequireSslFilter(IsRequired = "False"), HttpGet, Compress]
        public ActionResult InvoiceDetailed(long? id)
        {
            
            SessionUser cuser = AppHelper.CurrentUser;
            ViewData["UserInvoice"] = InvoiceRepository.GetUserInvoice(id.HasValue ? id.Value : -1);
            return View();
        }

        [VauctionAuthorize, RequireSslFilter(IsRequired = "False"), HttpGet, Compress]
        public ActionResult InvoiceDetailedDOW(long? id)
        {
            
               //id = 241672;
            if (!id.HasValue || id.Value != AppHelper.CurrentUser.ID)
                return RedirectToAction("PastInvoices");
            ViewData["UserInvoice"] = InvoiceRepository.GetUserInvoicesDOW(id.HasValue ? id.Value : -1);
            return View();
        }
        #endregion

        #region email settings
        //EditMailSettings (get)
        [VauctionAuthorize, HttpGet, Compress]
        public ActionResult EditMailSettings()
        {
            SessionUser cuser = AppHelper.CurrentUser;
            ViewData["IsRecievingBidConfirmation"] = cuser.NotRecievingBidConfirmation;
            ViewData["IsRecievingLotClosedNotice"] = cuser.NotRecievingLotClosedNotice;
            ViewData["IsRecievingLotSoldNotice"] = cuser.NotRecievingLotSoldNotice;
            ViewData["IsRecievingOutBidNotice"] = cuser.NotRecievingOutBidNotice;
            ViewData["IsRecievingWeeklySpecials"] = cuser.NotRecieveWeeklySpecials;
            ViewData["IsRecievingNewsUpdates"] = cuser.NotRecieveNewsUpdates;
            return View();
        }
        //EditMailSettings (post)
        [VauctionAuthorize, HttpPost, Compress] //, ValidateAntiForgeryToken
        public ActionResult EditMailSettings(string IsRecievingWeeklySpecials, string IsRecievingNewsUpdates, string IsRecievingBidConfirmation, string IsRecievingOutBidNotice, string IsRecievingLotSoldNotice, string IsRecievingLotClosedNotice)
        {
            bool NotRecievingWeeklySpecials = Convert.ToBoolean(IsRecievingWeeklySpecials);
            bool NotRecievingNewsUpdates = Convert.ToBoolean(IsRecievingNewsUpdates);
            bool NotRecievingBidConfirmation = Convert.ToBoolean(IsRecievingBidConfirmation);
            bool NotRecievingOutBidNotice = Convert.ToBoolean(IsRecievingOutBidNotice);
            bool NotRecievingLotSoldNotice = Convert.ToBoolean(IsRecievingLotSoldNotice);
            bool NotRecievingLotClosedNotice = Convert.ToBoolean(IsRecievingLotClosedNotice);
            SessionUser cuser = AppHelper.CurrentUser;
            if (UserRepository.UpdateEmailSettings(cuser.ID, NotRecievingWeeklySpecials, NotRecievingNewsUpdates, NotRecievingBidConfirmation, NotRecievingOutBidNotice, NotRecievingLotSoldNotice, NotRecievingLotClosedNotice))
            {
                //ViewData["IsRecievingBidConfirmation"] = NotRecievingBidConfirmation;
                //ViewData["IsRecievingLotClosedNotice"] = NotRecievingLotClosedNotice;
                //ViewData["IsRecievingLotSoldNotice"] = NotRecievingLotSoldNotice;
                //ViewData["IsRecievingOutBidNotice"] = NotRecievingOutBidNotice;
                //ViewData["IsRecievingWeeklySpecials"] = NotRecievingWeeklySpecials;
                //ViewData["IsRecievingNewsUpdates"] = NotRecievingNewsUpdates;
                //ViewData["DataSaved"] = true;
                cuser.NotRecievingBidConfirmation = NotRecievingBidConfirmation;
                cuser.NotRecievingLotClosedNotice = NotRecievingLotClosedNotice;
                cuser.NotRecievingLotSoldNotice = NotRecievingLotSoldNotice;
                cuser.NotRecievingOutBidNotice = NotRecievingOutBidNotice;
                cuser.NotRecieveWeeklySpecials = NotRecievingWeeklySpecials;
                cuser.NotRecieveNewsUpdates = NotRecievingNewsUpdates;
            }
            return RedirectToAction("MyAccount");
        }
        #endregion

        #region edit your personal shopper
        //EditPersonalShopper
        [VauctionAuthorize, RequireSslFilter, HttpGet, Compress]
        public ActionResult EditPersonalShopper()
        {
            InitCurrentEvent();
            ViewData["category"] = CategoryRepository.GetCategoryLeafs(null);
            return View(UserRepository.GetPersonalShopperForUser(AppHelper.CurrentUser.ID) ?? new PersonalShopperItem());
        }
        //EditPersonalShopper
        [VauctionAuthorize, HttpPost, Compress] //, ValidateAntiForgeryToken
        public ActionResult EditPersonalShopper(long? psi_id, DateTime? PersonalDate, bool ddlIsCur, string Keyword1, string Keyword2, string Keyword3, string Keyword4, string Keyword5, long? ddlCategory1, long? ddlCategory2, long? ddlCategory3, long? ddlCategory4, long? ddlCategory5, bool ddlIsHTML)
        {
            SessionUser cuser = AppHelper.CurrentUser;
            bool res = UserRepository.UpdatePersonalShopper(psi_id.HasValue ? psi_id.Value : 0, PersonalDate.HasValue ? PersonalDate.Value : DateTime.Now.AddDays(7), ddlIsCur, Keyword1, Keyword2, Keyword3, Keyword4, Keyword5, ddlCategory1, ddlCategory2, ddlCategory3, ddlCategory4, ddlCategory5, ddlIsHTML, cuser.ID);
            if (!res)
            {
                ViewData["category"] = CategoryRepository.GetCategoryLeafs(null);
                return View(new PersonalShopperItem { IsActive = ddlIsCur, DateExpires = PersonalDate.HasValue ? PersonalDate.Value : DateTime.Now.AddDays(7), Keyword1 = Keyword1, Keyword2 = Keyword2, Keyword3 = Keyword3, Keyword4 = Keyword4, Keyword5 = Keyword5, Category_ID1 = ddlCategory1, Category_ID2 = ddlCategory2, Category_ID3 = ddlCategory3, Category_ID4 = ddlCategory4, Category_ID5 = ddlCategory5, User_ID = cuser.ID });
            }
            return RedirectToAction("MyAccount");
        }

        //ReceivePersonalShopperUpdate
        [VauctionAuthorize, HttpGet, Compress]
        public ActionResult ReceivePersonalShopperUpdate([ModelBinder(typeof(CustomItemBinder))]AuctionFilterParams param)
        {
            SessionUser cuser = AppHelper.CurrentUser;
            InitCurrentEvent();
            PersonalShopperItem psi = UserRepository.GetPersonalShopperForUser(cuser.ID);
            if (psi == null)
                return View(new PagedList<AuctionShort>(new List<AuctionShort>(), 0, 0, 0));

            var auc = AuctionRepository.GetFutureEventForPersShopper(cuser.ID, param.page, param.PageSize);
            if (auc.Count() > 0)
                Mail.SendFutureEventsPerShopper(cuser.Email, cuser.FirstName, cuser.LastName, cuser.Login, auc.ToList(), psi.IsHTML);
            ViewData["PageActionPath"] = "ReceivePersonalShopperUpdate";
            SetFilterParams(param);
            return View(auc);
        }

        #endregion

        #region watch list
        //WatchBid
        [VauctionAuthorize, RequireSslFilter(IsRequired = "false"), HttpGet, Compress]
        public ActionResult WatchBid()
        {
            InitCurrentEvent();
            EventDetail evnt = ViewData["CurrentEvent"] as EventDetail;
            if (evnt == null) return RedirectToAction("Index", "Home");
            if (evnt.DateEnd < DateTime.Now) return RedirectToAction("AuctionsParticipated", new { id = evnt.ID });
            return View(BidRepository.GetBidWatchForUser(AppHelper.CurrentUser.ID, evnt.ID));
        }

        //RemoveBid
        [VauctionAuthorize, HttpGet, Compress]
        public ActionResult RemoveBid(long id)
        {
            AuctionRepository.RemoveAuctionFromWatchList(AppHelper.CurrentUser.ID, id);
            return RedirectToAction("WatchBid");
        }

        [VauctionAuthorize, HttpPost, Compress]
        public JsonResult UpdateWatchListPage()
        {
            InitCurrentEvent();
            EventDetail evnt = ViewData["CurrentEvent"] as EventDetail;
            if (evnt == null || evnt.DateEnd < DateTime.Now) return JSON(new { });
            return JSON(BidRepository.UpdateWatchListPage(AppHelper.CurrentUser.ID, evnt.ID));
        }
        #endregion

        #endregion

        #region payments

        #region deposit
        //PayDeposit
        [VauctionAuthorize, RequireSslFilter, Compress, HttpPost, NoCache] //, ValidateAntiForgeryToken
        public ActionResult PayDeposit()
        {
            ViewData["LeftUserControlVisibility"] = false;

            SessionUser cuser = AppHelper.CurrentUser;
            PreviewBid pb = Session["PreviewBid"] as PreviewBid;
            if (pb == null) return RedirectToAction("Index", "Home");
            if (pb.DepositNeed == 0) return RedirectToAction("AuctionDetail", "Auction", new { id = pb.LinkParams.ID });
            CreditCardInfo cci = new CreditCardInfo();
            cci.Address1 = cuser.Address_Billing.Address_1;
            cci.Address2 = cuser.Address_Billing.Address_2;
            cci.CardCode = cci.CardNumber = String.Empty;
            cci.City = cuser.Address_Billing.City;
            cci.Country = cuser.Address_Billing.Country_ID;
            cci.CountryTitle = cuser.Address_Billing.Country;
            cci.ExpirationMonth = DateTime.Now.Month;
            cci.ExpirationYear = DateTime.Now.Year;
            cci.State = cuser.Address_Billing.State;
            cci.Zip = cuser.Address_Billing.Zip;
            ViewData["Countries"] = new SelectList(dataProvider.CountryRepository.GetCountryList(), "ID", "Title", cci.Country);
            return View(cci);
        }

        //ConfirmDeposit
        [NonAction]
        private void ConfirmDeposit(decimal amount, long auction_id, long event_id, bool IsPayPal, string transactionId, string transactionType, Address billing, string CCNum)
        {
            SessionUser cuser = AppHelper.CurrentUser;
            InvoiceRepository.AddDeposit(amount, auction_id, event_id, cuser.ID, (long)((IsPayPal) ? Consts.PaymentType.Paypal : Consts.PaymentType.CreditCard), transactionId, billing, CCNum);
            Mail.SendDepositPaymentConfirmation(cuser.FirstName, cuser.LastName, cuser.Email, amount, transactionId, "Paid " + ((IsPayPal) ? "via PayPal" : "by Credit Card"));
        }

        //CreditCardResult
        [VauctionAuthorize, HttpPost, RequireSslFilter, Compress, NoCache] //, ValidateAntiForgeryToken
        public ActionResult CreditCardDeposit(CreditCardInfo cci)
        {
            if (!ModelState.IsValid)
            {
                ViewData["IsCreditCard"] = true;
                ViewData["Countries"] = new SelectList(dataProvider.CountryRepository.GetCountryList(), "ID", "Title", cci.Country);
                return View("PayDeposit", cci);
            }
            SessionUser cuser = AppHelper.CurrentUser;
            PreviewBid pb = Session["PreviewBid"] as PreviewBid;
            if (pb == null) return RedirectToAction("Index", "Home");
            if (pb.DepositNeed == 0) return RedirectToAction("AuctionDetail", "Auction", new { id = pb.LinkParams.ID });

            long profile_id = 0;
            try
            {
                profile_id = AN_Functions.CreateCustomerProfileForDeposit(cuser.ID, cuser.Email, String.Format("Title: {0}. Description: Good Faith Deposit (Fully Refundable). Total Amount: {1}. Date: {2}", pb.LinkParams.EventTitle, pb.DepositNeed, DateTime.Now.ToString()));
                if (profile_id <= 0) throw new Exception(AN_Functions.LastError);//("Creating customer profile faild");
                CustomerProfileWS.CustomerProfileMaskedType profile = AN_Functions.GetCustomerProfile(profile_id);
                if (profile == null) throw new Exception(AN_Functions.LastError);//("Unable to locate customer's profile");

                Address billingAddress = cuser.Address_Billing;
                billingAddress.Address_1 = cci.Address1;
                billingAddress.Address_2 = cci.Address2;
                billingAddress.City = cci.City;
                billingAddress.Zip = cci.Zip;
                billingAddress.State = cci.State;
                billingAddress.Country_ID = cci.Country.HasValue ? cci.Country.Value : 0;
                if (String.IsNullOrEmpty(cci.CountryTitle))
                    billingAddress.Country = CountryRepository.GetCountryByID(billingAddress.Country_ID).Title;

                long shipping_profile_id = AN_Functions.CreateCustomerShippingAddress(profile_id, billingAddress, cci.CountryTitle);
                if (shipping_profile_id <= 0) throw new Exception(AN_Functions.LastError);//("Unable to add customer's shipping address");

                long payment_profile_id = AN_Functions.CreateCustomerPaymentProfile(profile_id, cci);
                if (payment_profile_id <= 0) throw new Exception(AN_Functions.LastError);//("Unable to add customer's payment profile");

                if ((!AN_Functions.UpdateCustomerShippingAddress(profile_id, shipping_profile_id, billingAddress)) || (!AN_Functions.UpdateCustomerPaymentProfile(profile_id, payment_profile_id, cci, billingAddress.FirstName, billingAddress.LastName)))
                    throw new Exception(AN_Functions.LastError);//("Unable to update customer's payment profile");

                profile = AN_Functions.GetCustomerProfile(profile_id);
                if (profile == null) throw new Exception(AN_Functions.LastError);//("Unable to locate customer's profile");

                if (!AN_Functions.ValidateCustomerPaymentProfile(profile_id, payment_profile_id, shipping_profile_id, cci.CardCode))
                    throw new Exception(AN_Functions.LastError);//("Unable to validate customer's payment profile");

                string TransactionID = AN_Functions.CreateTransactionForDeposit(profile_id, payment_profile_id, shipping_profile_id, pb.DepositNeed, cci, "Good Faith Deposit (Fully Refundable)");

                if (String.IsNullOrEmpty(TransactionID)) throw new Exception("The transaction was unsuccessful.");

                ConfirmDeposit(pb.DepositNeed, pb.LinkParams.ID, pb.LinkParams.Event_ID, false, TransactionID, String.Empty, billingAddress, cci.CardNumber.Substring(cci.CardNumber.Length - 5));
                return RedirectToAction("PlaceBid", "Auction");
            }
            catch (Exception ex)
            {
                string error = (pb != null) ? String.Format("[auction_id={0}, deposit={1}]", pb.LinkParams.ID, pb.DepositNeed) : String.Empty;
                Logger.LogException(error, ex);
                ViewData["ErrorMessage"] = ex.Message;
            }
            finally
            {
                try
                {
                    if (!AN_Functions.DeleteCustomerProfile(profile_id))
                        throw new Exception(AN_Functions.LastError);//("Unable to delete customer's profile");
                }
                catch (Exception ex)
                {
                    string error = (pb != null) ? String.Format("[auction_id={0}, deposit={1}]", pb.LinkParams.ID, pb.DepositNeed) : String.Empty;
                    Logger.LogException(error, ex);
                }
            }
            InitCurrentEvent();
            return View("PayDepositFailed", 1);
        }

        //PayPalDeposit
        [VauctionAuthorize, HttpPost, RequireSslFilter, Compress, NoCache] //, ValidateAntiForgeryToken
        public ActionResult PayPalDeposit()
        {
            ViewData["LeftUserControlVisibility"] = false;
            PreviewBid pb = Session["PreviewBid"] as PreviewBid;
            if (pb == null) return RedirectToAction("Index", "Home");
            if (pb.DepositNeed == 0) return RedirectToAction("AuctionDetail", "Auction", new { id = pb.LinkParams.ID });
            try
            {
                StringBuilder requestString = PP_Functions.InitializeRequest("SetExpressCheckout");
                requestString.Append("&RETURNURL=" + AppHelper.GetSiteUrl("/Account/PayPalConfirmDeposit"));
                requestString.Append("&CANCELURL=" + pb.LinkParams.AuctionDetailUrl);
                requestString.Append("&PAYMENTACTION=authorization");
                requestString.AppendFormat("&L_NAME0={0}", "Good Faith Deposit");
                requestString.AppendFormat("&L_DESC0={0}", "Fully Refundable");
                requestString.AppendFormat("&L_QTY0={0}", 1);
                requestString.AppendFormat("&L_AMT0={0}", pb.DepositNeed.GetPrice().ToString());
                requestString.AppendFormat("&AMT={0}", pb.DepositNeed.GetPrice().ToString());

                string token;
                string dummy;
                if (!PP_Functions.Post(requestString.ToString(), out token, out dummy)) throw new Exception("Payment via PayPal failed");
                Response.Redirect("https://" + Consts.PayPalUrl + "/cgi-bin/webscr?cmd=_express-checkout&token=" + token);
                return Content(String.Empty);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                string error = (pb != null) ? String.Format("[auction_id={0}, deposit={1}]", pb.LinkParams.ID, pb.DepositNeed) : String.Empty;
                Logger.LogException(error, ex);
            }
            InitCurrentEvent();
            return View("PayDepositFailed", 0);
        }

        //PayPalConfirmDeposit
        [VauctionAuthorize, RequireSslFilter, Compress, NoCache]
        public ActionResult PayPalConfirmDeposit()
        {
            PreviewBid pb = Session["PreviewBid"] as PreviewBid;
            if (pb == null) return RedirectToAction("Index", "Home");
            if (pb.DepositNeed == 0) return RedirectToAction("AuctionDetail", "Auction", new { id = pb.LinkParams.ID });
            try
            {
                string errormessage = String.Empty;
                if (Request.QueryString.Get("token") == null) throw new Exception("Express Checkout Failed");
                string token, payerID, transactionID, transactionType;
                StringBuilder requestString = PP_Functions.InitializeRequest("GetExpressCheckoutDetails");
                requestString.Append("&TOKEN=" + HttpUtility.UrlEncode(Request.QueryString.Get("token")));
                Address shipping = new Address();
                if (PP_Functions.Post(requestString.ToString(), out token, out payerID, shipping, out errormessage))
                {
                    requestString = PP_Functions.InitializeRequest("DoExpressCheckoutPayment");
                    requestString.Append("&TOKEN=" + HttpUtility.UrlEncode(token));
                    requestString.Append("&AMT=" + HttpUtility.UrlEncode(pb.DepositNeed.ToString("f")));
                    requestString.Append("&PAYMENTACTION=Sale");
                    requestString.Append("&PAYERID=" + HttpUtility.UrlEncode(payerID));

                    requestString.AppendFormat("&L_NAME0={0}", "Deposit");
                    requestString.AppendFormat("&L_QTY0={0}", 1);
                    requestString.AppendFormat("&L_DESC0={0}", "Event# " + pb.LinkParams.EventTitle);
                    requestString.AppendFormat("&L_AMT0={0}", pb.DepositNeed.GetPrice().ToString());
                    requestString.AppendFormat("&ITEMAMT={0}", pb.DepositNeed.GetPrice().ToString());
                    requestString.AppendFormat("&AMT={0}", pb.DepositNeed.GetPrice().ToString());

                    if (PP_Functions.Post(requestString.ToString(), out token, out payerID, out transactionID, out transactionType, out errormessage))
                    {
                        ConfirmDeposit(pb.DepositNeed, pb.LinkParams.ID, pb.LinkParams.Event_ID, true, transactionID, transactionType, AppHelper.CurrentUser.Address_Billing, String.Empty);
                        return RedirectToAction("PlaceBid", "Auction");
                    }
                    throw new Exception(String.IsNullOrEmpty(errormessage) ? "Express Checkout Failed" : errormessage);
                }
                throw new Exception(String.IsNullOrEmpty(errormessage) ? "Express Checkout Failed" : errormessage);
            }
            catch (Exception ex)
            {
                ViewData["ErrorMessage"] = ex.Message;
                string error = (pb != null) ? String.Format("[auction_id={0}, deposit={1}]", pb.LinkParams.ID, pb.DepositNeed) : String.Empty;
                Logger.LogException(error, ex);
            }
            return View("PayDepositFailed", 0);
        }
        #endregion

        #region Illtakeit
        //ITakeIt
        [VauctionAuthorize, RequireSslFilter, Compress, HttpPost, NoCache] //, ValidateAntiForgeryToken
        public ActionResult ITakeIt(long auction_id, int quantity, bool? isDOW)
        {
            bool isdow = (isDOW.HasValue && isDOW.Value);
            SessionUser cuser = AppHelper.CurrentUser;
            AuctionDetail auction = (!isdow) ? AuctionRepository.GetAuctionDetail(auction_id, true) : AuctionRepository.GetDowDetail(auction_id, cuser.ID);
            if (auction == null) return RedirectToAction("Index", "Home");
            if (!isdow && (auction.AuctionType == (byte)Consts.AuctionType.DealOfTheWeek || (!auction.IsViewable && !auction.IsClickable && !auction.IsPrivate) || auction.EndDate.CompareTo(DateTime.Now) < 0 || auction.Status != (byte)Consts.AuctionStatus.Open || quantity == 0 || auction.Quantity <= 0 || auction.BuyPrice.GetValueOrDefault(0) == 0)) return RedirectToAction("AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = auction.LinkParams.ID });
            else if (isdow && (auction.AuctionType != (byte)Consts.AuctionType.DealOfTheWeek || auction.EndDate.CompareTo(DateTime.Now) < 0 || auction.Status != (byte)Consts.AuctionStatus.Open || quantity == 0 || auction.Quantity <= 0 || auction.Price <= 0)) return RedirectToAction("Index", "Home");

            if (isdow)
                AuctionRepository.RemoveReservedAuctionQuantityForAuction(cuser.ID, auction_id);

            decimal Shipping, Insurance, Tax, BuyerPremium, Amount, coefInsurance, coefTax;

            coefInsurance = VariableRepository.GetInsurance();
            coefTax = VariableRepository.GetSalesTaxRate();

            decimal price = (!isdow) ? auction.BuyPrice.Value : auction.Price;

            int wquantity = quantity;
            quantity = (auction.Quantity < quantity) ? auction.Quantity : quantity;

            Amount = quantity * price;
            BuyerPremium = (!isdow) ? Amount * (auction.BuyerFee / 100) : 0;
            Shipping = quantity * auction.Shipping;
            Insurance = (Shipping > 0) ? (quantity * (coefInsurance * Convert.ToInt32(price / 100) + coefInsurance)) : 0;
            Tax = (cuser.Address_Billing.State == "FL" && auction.IsTaxable) ? quantity * (coefTax * price) : 0;

            Dow dow = new Dow(isdow);
            dow.LinkParams = auction.LinkParams;
            dow.Quantity = quantity;
            dow.WQuantity = wquantity;
            dow.Amount = Amount;
            dow.BuyerPremium = BuyerPremium;
            dow.Shipping = Shipping;
            dow.Insurance = Insurance;
            dow.Tax = Tax;
            dow.Price = auction.Price;
            dow.BuyPrice = auction.BuyPrice;
            dow.IsConsignorShip = auction.IsConsignorShip;

            Session["DOW_" + auction_id.ToString()] = dow;
            ViewData["auction_id"] = auction_id;

            CreditCardInfo cci = new CreditCardInfo();
            cci.Address1 = cuser.Address_Billing.Address_1;
            cci.Address2 = cuser.Address_Billing.Address_2;
            cci.CardCode = cci.CardNumber = String.Empty;
            cci.City = cuser.Address_Billing.City;
            cci.Country = cuser.Address_Billing.Country_ID;
            cci.CountryTitle = cuser.Address_Billing.Country;
            cci.ExpirationMonth = DateTime.Now.Month;
            cci.ExpirationYear = DateTime.Now.Year;
            cci.State = cuser.Address_Billing.State;
            cci.Zip = cuser.Address_Billing.Zip;
            ViewData["Countries"] = new SelectList(dataProvider.CountryRepository.GetCountryList(), "ID", "Title", cci.Country);
            return View(cci);
        }

        //ConfirmPaymentITakeIt
        private PaymentConfirmation ConfirmPaymentITakeIt(Dow dow, bool ishipping, Address billing, Address shipping, string transactionID, string transactionType, string CCNum, bool ispaypal)
        {
            long? invoice_id = -1;
            SessionUser cuser = AppHelper.CurrentUser;
            if (!InvoiceRepository.AddInvoicePayment(dow.LinkParams.ID, cuser.ID, dow.Quantity, dow.LinkParams.Event_ID, billing.Address_1 + ", " + billing.Address_2, transactionID, CCNum, billing.City, "Payment " + ((ispaypal) ? "via PayPal" : "by Credit Card"), (long)((ispaypal) ? Consts.PaymentType.Paypal : Consts.PaymentType.CreditCard), (!ishipping) ? "PICK UP" : String.Format("{0} | {1}", shipping.FullName, shipping.FullAddress2), "Payment for lot# " + dow.LinkParams.Lot + (!dow.IsDOW ? " (I'll Take It)" : " (DOW)"), billing.Zip, billing.State, dow.Amount, dow.BuyerPremium, dow.Shipping, dow.Insurance, dow.Tax, !ishipping, dow.IsDOW, ref invoice_id)) return null;
            PaymentConfirmation pc = new PaymentConfirmation();
            pc.PaymentConfirmationType = (dow.IsDOW) ? PaymentConfirmationType.DOW : PaymentConfirmationType.ILLTAKEIT;
            pc.TransactionID = transactionID;
            pc.TransactionType = transactionType;
            pc.Address_Billing = billing;
            pc.Address_Shipping = shipping;
            pc.IsShipping = ishipping;
            pc.Invoices.Add(InvoiceRepository.GetInvoiceDetail(invoice_id.Value));
            pc.PaymentType = ispaypal ? Consts.PaymentType.Paypal : Consts.PaymentType.CreditCard;

            AuctionRepository.GetAuctionDetail(dow.LinkParams.ID, false); // refresh item's cache

            Mail.SendInvoicePaymentConfirmation(cuser.Email, cuser.Login, pc);
            return pc;
        }

        //CreditCardITakeIt
        [VauctionAuthorize, HttpPost, RequireSslFilter, Compress, NoCache] //, ValidateAntiForgeryToken
        public ActionResult CreditCardITakeIt(long auction_id, bool ishipping, CreditCardInfo cci)
        {
            ViewData["auction_id"] = auction_id;
            if (!ModelState.IsValid)
            {
                ViewData["ishipping"] = !ishipping;
                ViewData["IsCreditCard"] = true;
                ViewData["Countries"] = new SelectList(dataProvider.CountryRepository.GetCountryList(), "ID", "Title", cci.Country);
                return View("ITakeIt", cci);
            }
            Dow dow = Session["DOW_" + auction_id.ToString()] as Dow;
            if (dow == null) return RedirectToAction("Index", "Home");
            dow.IsShipping = ishipping;
            SessionUser cuser = AppHelper.CurrentUser;
            AuctionDetail auction = (!dow.IsDOW) ? AuctionRepository.GetAuctionDetail(auction_id, false) : AuctionRepository.GetDowDetail(auction_id, cuser.ID);
            if (auction == null) return RedirectToAction("Index", "Home");
            if (!dow.IsDOW && (auction.AuctionType == (byte)Consts.AuctionType.DealOfTheWeek || (!auction.IsViewable && !auction.IsClickable && !auction.IsPrivate) || auction.EndDate.CompareTo(DateTime.Now) < 0 || auction.Status != (byte)Consts.AuctionStatus.Open || dow.Quantity == 0 || auction.Quantity <= 0 || auction.BuyPrice.GetValueOrDefault(0) == 0)) return RedirectToAction("AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = auction.LinkParams.ID });
            else if (dow.IsDOW && (auction.AuctionType != (byte)Consts.AuctionType.DealOfTheWeek || auction.EndDate.CompareTo(DateTime.Now) < 0 || auction.Status != (byte)Consts.AuctionStatus.Open || dow.Quantity == 0 || auction.Quantity <= 0 || auction.Price <= 0)) return RedirectToAction("Index", "Home");

            decimal coefInsurance = VariableRepository.GetInsurance();

            dow.Shipping = (ishipping || dow.IsConsignorShip) ? dow.Shipping : 0;
            dow.Insurance = (dow.Shipping > 0) ? dow.Insurance : 0;

            long profile_id = 0;
            try
            {
                if (dow.IsDOW)
                {
                    AuctionRepository.RemoveReservedAuctionQuantityForAuction(cuser.ID, auction_id);
                    if (!AuctionRepository.ReserveAuctionQuantity(cuser.ID, auction_id, dow.Quantity))
                        throw new Exception("Operation failed. Probably the ordered quantity in unavailable. Please try again.");
                }

                profile_id = AN_Functions.CreateCustomerProfile(cuser.ID, cuser.Email, dow.LinkParams.Lot.ToString());
                if (profile_id <= 0) throw new Exception(AN_Functions.LastError);
                CustomerProfileWS.CustomerProfileMaskedType profile = AN_Functions.GetCustomerProfile(profile_id);
                if (profile == null) throw new Exception(AN_Functions.LastError);

                Address bAddr = cuser.Address_Billing;
                bAddr.Address_1 = cci.Address1;
                bAddr.Address_2 = cci.Address2;
                bAddr.City = cci.City;
                bAddr.Zip = cci.Zip;
                bAddr.State = cci.State;
                bAddr.Country_ID = cci.Country.HasValue ? cci.Country.Value : 0;
                bAddr.Country = (String.IsNullOrEmpty(cci.CountryTitle)) ? CountryRepository.GetCountryByID(bAddr.Country_ID).Title : cci.CountryTitle;

                long shipping_profile_id = AN_Functions.CreateCustomerShippingAddress(profile_id, bAddr, cci.CountryTitle);
                if (shipping_profile_id <= 0) throw new Exception(AN_Functions.LastError);//("Unable to add customer's shipping address");

                long payment_profile_id = AN_Functions.CreateCustomerPaymentProfile(profile_id, cci);
                if (payment_profile_id <= 0) throw new Exception(AN_Functions.LastError);//("Unable to add customer's payment profile");

                if ((!AN_Functions.UpdateCustomerShippingAddress(profile_id, shipping_profile_id, bAddr)) || (!AN_Functions.UpdateCustomerPaymentProfile(profile_id, payment_profile_id, cci, bAddr.FirstName, bAddr.LastName)))
                    throw new Exception(AN_Functions.LastError);//("Unable to update customer's payment profile");

                profile = AN_Functions.GetCustomerProfile(profile_id);
                if (profile == null) throw new Exception(AN_Functions.LastError);//("Unable to locate customer's profile");

                if (!AN_Functions.ValidateCustomerPaymentProfile(profile_id, payment_profile_id, shipping_profile_id, cci.CardCode))
                    throw new Exception(AN_Functions.LastError);//("Unable to validate customer's payment profile");

                string TransactionID = AN_Functions.CreateTransaction(profile_id, payment_profile_id, shipping_profile_id, dow.TotalCost, String.Empty, ishipping, cci, "Lot#: " + dow.LinkParams.Lot.ToString());

                if (String.IsNullOrEmpty(TransactionID)) throw new Exception("The transaction was unsuccessful.");

                PaymentConfirmation pc = ConfirmPaymentITakeIt(dow, ishipping, bAddr, cuser.Address_Shipping, TransactionID, String.Empty, cci.CardNumber.Substring(cci.CardNumber.Length - 5), false);
                if (pc == null) throw new Exception(AN_Functions.LastError);

                Session.Remove("DOW_" + auction_id.ToString());

                return View("PaymentConfirmation", pc);
            }
            catch (Exception ex)
            {
                string error = (dow != null) ? String.Format("[auction_id={0}, user_id={1}, quantity={2}]", dow.LinkParams.ID, cuser.ID, dow.Quantity) : String.Empty;
                Logger.LogException(error, ex);
                ViewData["ErrorMessage"] = ex.Message;
            }
            finally
            {
                try
                {
                    if (!AN_Functions.DeleteCustomerProfile(profile_id))
                        throw new Exception(AN_Functions.LastError);//("Unable to delete customer's profile");
                }
                catch (Exception ex)
                {
                    string error = (dow != null) ? String.Format("[auction_id={0}, user_id={1}, quantity={2}]", dow.LinkParams.ID, cuser.ID, dow.Quantity) : String.Empty;
                    Logger.LogException(error, ex);
                }
            }
            InitCurrentEvent();
            return View("PayIlltakeItFailed");
        }

        //PayPalITakeIt
        [VauctionAuthorize, HttpPost, RequireSslFilter, Compress, NoCache] //, ValidateAntiForgeryToken
        public ActionResult PayPalITakeIt(long? auction_id, bool ishipping)
        {
            if (!auction_id.HasValue) return RedirectToAction("Index", "Home");
            Dow dow = Session["DOW_" + auction_id.ToString()] as Dow;
            if (dow == null) return RedirectToAction("Index", "Home");
            SessionUser cuser = AppHelper.CurrentUser;
            try
            {
                dow.IsShipping = ishipping;
                AuctionDetail auction = (!dow.IsDOW) ? AuctionRepository.GetAuctionDetail(auction_id.Value, false) : AuctionRepository.GetDowDetail(auction_id.Value, cuser.ID);
                if (auction == null) return RedirectToAction("Index", "Home");
                if (!dow.IsDOW && (auction.AuctionType == (byte)Consts.AuctionType.DealOfTheWeek || (!auction.IsViewable && !auction.IsClickable && !auction.IsPrivate) || auction.EndDate.CompareTo(DateTime.Now) < 0 || auction.Status != (byte)Consts.AuctionStatus.Open || dow.Quantity == 0 || auction.Quantity <= 0 || auction.BuyPrice.GetValueOrDefault(0) == 0)) return RedirectToAction("AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = auction.LinkParams.ID });
                else if (dow.IsDOW && (auction.AuctionType != (byte)Consts.AuctionType.DealOfTheWeek || auction.EndDate.CompareTo(DateTime.Now) < 0 || auction.Status != (byte)Consts.AuctionStatus.Open || dow.Quantity == 0 || auction.Quantity <= 0 || auction.Price <= 0)) return RedirectToAction("Index", "Home");

                if (auction.Quantity <= 0) throw new Exception("You can't buy this lot, because it is already sold.");

                if (dow.IsDOW)
                {
                    AuctionRepository.RemoveReservedAuctionQuantityForAuction(cuser.ID, auction_id.Value);
                    if (!AuctionRepository.ReserveAuctionQuantity(cuser.ID, auction_id.Value, dow.Quantity))
                        throw new Exception("Operation failed. Probably the ordered quantity in unavailable. Please try again.");
                }

                StringBuilder requestString = PP_Functions.InitializeRequest("SetExpressCheckout");
                requestString.Append("&RETURNURL=" + AppHelper.GetSiteUrl("/Account/PayPalConfirmITakeIt"));
                requestString.Append("&CANCELURL=" + (dow.IsDOW ? AppHelper.GetSiteUrl("/Home/Index") : dow.LinkParams.AuctionDetailUrl));
                requestString.Append("&PAYMENTACTION=authorization");

                decimal coefInsurance = VariableRepository.GetInsurance();

                int number = 0;
                dow.Shipping = (ishipping || auction.IsConsignorShip) ? dow.Shipping : 0;
                dow.Insurance = dow.Shipping > 0 ? dow.Insurance : 0;

                StringBuilder sb = new StringBuilder();
                sb.Append(dow.Price > 0 ? "| Price: " + dow.Amount.GetCurrency() : String.Empty);
                sb.Append(dow.BuyerPremium > 0 ? " | Buyer's premium: " + dow.BuyerPremium.GetCurrency() : String.Empty);
                sb.Append(dow.Shipping > 0 ? " | Shipping: " + dow.Shipping.GetCurrency() : String.Empty);
                sb.Append(dow.Insurance > 0 ? " | Insurance: " + dow.Insurance.GetCurrency() : String.Empty);
                sb.Append(dow.Tax > 0 ? " | Tax: " + dow.Tax.GetCurrency() : String.Empty);

                requestString.AppendFormat("&L_NAME{0}={1}", number, auction.LinkParams.Title);
                requestString.AppendFormat("&L_NUMBER{0}={1}", number, auction.LinkParams.Lot);
                requestString.AppendFormat("&L_DESC{0}={1}", number, sb.ToString());
                requestString.AppendFormat("&L_QTY{0}={1}", number, 1);
                requestString.AppendFormat("&L_AMT{0}={1}", number, dow.TotalCost.GetPrice()); //price.GetPrice());

                requestString.AppendFormat("&ITEMAMT={0}", dow.TotalCost.GetPrice());
                requestString.AppendFormat("&AMT={0}", dow.TotalCost.GetPrice());

                string dummy, token;
                if (!PP_Functions.Post(requestString.ToString(), out token, out dummy)) throw new Exception("Payment via PayPal failed");
                Session.Remove("DOW_" + auction_id.Value.ToString());
                Session["DOW"] = dow;
                Response.Redirect("https://" + Consts.PayPalUrl + "/cgi-bin/webscr?cmd=_express-checkout&token=" + token);
                return Content(String.Empty);
            }
            catch (Exception ex)
            {
                string error = (dow != null) ? String.Format("[auction_id={0}, user_id={1}, quantity={2}]", dow.LinkParams.ID, cuser.ID, dow.Quantity) : String.Empty;
                Logger.LogException(error, ex);
                ViewData["ErrorMessage"] = ex.Message;
            }
            InitCurrentEvent();
            return View("PayIlltakeItFailed");
        }

        //PayPalConfirmITakeIt
        [VauctionAuthorize, RequireSslFilter, Compress, NoCache]
        public ActionResult PayPalConfirmITakeIt()
        {
            Dow dow = Session["DOW"] as Dow;
            if (dow == null) return RedirectToAction("Index", "Home");
            SessionUser cuser = AppHelper.CurrentUser;
            try
            {
                if (Request.QueryString.Get("token") == null) throw new Exception("Express Checkout Failed");
                string errormessage = String.Empty;
                string token, payerID, transactionID, transactionType;
                StringBuilder requestString = PP_Functions.InitializeRequest("GetExpressCheckoutDetails");
                requestString.Append("&TOKEN=" + HttpUtility.UrlEncode(Request.QueryString.Get("token")));
                Address shipping = new Address();
                if (PP_Functions.Post(requestString.ToString(), out token, out payerID, shipping, out errormessage))
                {
                    AuctionDetail auction = (!dow.IsDOW) ? AuctionRepository.GetAuctionDetail(dow.LinkParams.ID, false) : AuctionRepository.GetDowDetail(dow.LinkParams.ID, cuser.ID);
                    if (auction == null) return RedirectToAction("Index", "Home");
                    int qty = AuctionRepository.GetAuctionQuantityReserveForUser(cuser.ID, auction.LinkParams.ID);
                    if (!dow.IsDOW && (auction.AuctionType == (byte)Consts.AuctionType.DealOfTheWeek || (!auction.IsViewable && !auction.IsClickable && !auction.IsPrivate) || auction.EndDate.CompareTo(DateTime.Now) < 0 || auction.Status != (byte)Consts.AuctionStatus.Open || dow.Quantity == 0 || auction.Quantity <= 0 || auction.BuyPrice.GetValueOrDefault(0) == 0)) return RedirectToAction("AuctionDetail", new { controller = "Auction", action = "AuctionDetail", id = auction.LinkParams.ID });
                    else if (dow.IsDOW && (auction.AuctionType != (byte)Consts.AuctionType.DealOfTheWeek || auction.EndDate.CompareTo(DateTime.Now) < 0 || auction.Status != (byte)Consts.AuctionStatus.Open || dow.Quantity == 0 || (qty <= 0 && auction.Quantity <= 0) || auction.Price <= 0)) return RedirectToAction("Index", "Home");

                    requestString = PP_Functions.InitializeRequest("DoExpressCheckoutPayment");
                    requestString.Append("&TOKEN=" + HttpUtility.UrlEncode(token));
                    requestString.Append("&AMT=" + HttpUtility.UrlEncode(dow.TotalCost.GetPrice().ToString("f")));
                    requestString.Append("&PAYMENTACTION=Sale");
                    requestString.Append("&PAYERID=" + HttpUtility.UrlEncode(payerID));

                    StringBuilder sb = new StringBuilder();
                    sb.Append(dow.Price > 0 ? "| Price: " + dow.Amount.GetCurrency() : String.Empty);
                    sb.Append(dow.BuyerPremium > 0 ? " | Buyer's premium: " + dow.BuyerPremium.GetCurrency() : String.Empty);
                    sb.Append(dow.Shipping > 0 ? " | Shipping: " + dow.Shipping.GetCurrency() : String.Empty);
                    sb.Append(dow.Insurance > 0 ? " | Insurance: " + dow.Insurance.GetCurrency() : String.Empty);
                    sb.Append(dow.Tax > 0 ? " | Tax: " + dow.Tax.GetCurrency() : String.Empty);
                    int number = 0;
                    requestString.AppendFormat("&L_NAME{0}={1}", number, auction.LinkParams.Title);
                    requestString.AppendFormat("&L_NUMBER{0}={1}", number, auction.LinkParams.Lot);
                    requestString.AppendFormat("&L_DESC{0}={1}", number, sb.ToString());
                    requestString.AppendFormat("&L_QTY{0}={1}", number, 1);
                    requestString.AppendFormat("&L_AMT{0}={1}", number, dow.TotalCost.GetPrice());
                    requestString.AppendFormat("&ITEMAMT={0}", dow.TotalCost.GetPrice());
                    requestString.AppendFormat("&AMT={0}", dow.TotalCost.GetPrice());

                    if (PP_Functions.Post(requestString.ToString(), out token, out payerID, out transactionID, out transactionType, out errormessage))
                    {
                        PaymentConfirmation pc = ConfirmPaymentITakeIt(dow, dow.IsShipping, cuser.Address_Billing, shipping, transactionID, transactionType, String.Empty, true);
                        if (pc == null) throw new Exception(AN_Functions.LastError);
                        Session.Remove("DOW");
                        return View("PaymentConfirmation", pc);
                    }
                    else throw new Exception(String.IsNullOrEmpty(errormessage) ? "Express Checkout Failed" : errormessage);
                }
                else throw new Exception(String.IsNullOrEmpty(errormessage) ? "Express Checkout Failed" : errormessage);
            }
            catch (Exception ex)
            {
                string error = (dow != null) ? String.Format("[auction_id={0}, user_id={1}, quantity={2}]", dow.LinkParams.ID, cuser.ID, dow.Quantity) : String.Empty;
                Logger.LogException(error, ex);
                ViewData["ErrorMessage"] = ex.Message;
                Session.Remove("DOW");
            }
            InitCurrentEvent();
            return View("PayIlltakeItFailed");
        }

        #endregion

        #region auction items
        //PayForItems
        [VauctionAuthorize, RequireSslFilter, HttpGet, Compress, NoCache]
        public ActionResult PayForItems()
        {
            SessionUser cuser = AppHelper.CurrentUser;
 //           long id = 257689;
            ViewData["vDiscount"] = InvoiceRepository.ValidDiscountForUserID(cuser.ID);
            return View(InvoiceRepository.GetEventUnpaidInvoicesForUser(cuser.ID));
        }

        //CalculaterDiscountForInvoices
        private void CalculaterDiscountForInvoices(List<InvoiceDetail> invoices, decimal discountAmount)
        {
            if (invoices.Count() == 0) return;
            decimal res = discountAmount / invoices.Count;
            if (res <= invoices.FirstOrDefault().TotalCost)
            {
                invoices.ForEach(I => I.TotalOrderDiscount = res);
                return;
            }
            invoices.First().TotalOrderDiscount = invoices.First().TotalCost;
            CalculaterDiscountForInvoices(invoices.Skip(1).ToList(), discountAmount - invoices.First().TotalCost);
        }

        //MakePayment
        [VauctionAuthorize, RequireSslFilter, Compress, HttpPost, NoCache] //, ValidateAntiForgeryToken
        public ActionResult MakePayment()
        {
            List<long> ListToPay = new List<long>();
            if (Request.Form.GetValues("chkPickup") == null || Request.Form.GetValues("chkPickup").Length == 0) return RedirectToAction("PayForItems");
            bool isShipping = !Convert.ToBoolean(Request.Form.GetValues("chkPickup")[0]);
            try
            {
                foreach (string o in Request.Form.AllKeys)
                {
                    if (o.Contains("sel_") && Request.Form.GetValues(o)[0] == "true")
                        ListToPay.Add(Int64.Parse(o.Replace("sel_", "")));
                }
            }
            catch
            {
                return RedirectToAction("PayForItems");
            }
            if (ListToPay.Count == 0) return RedirectToAction("PayForItems");
            SessionUser cuser = AppHelper.CurrentUser;
            //cuser.ID = 257689;
            PaymentConfirmation pc = new PaymentConfirmation();

            List<InvoiceDetail> invoices = InvoiceRepository.GetEventInvoicesForUserByIDs(cuser.ID, ListToPay);
            invoices.ForEach(I => I.Shipping = (isShipping || I.IsConsignorShip) ? I.Shipping : 0);
            invoices.ForEach(I => I.Insurance = I.Shipping > 0 ? I.Insurance : 0);
            string promoCode = Request.Form["tbxCouponCode"];
            Discount discount = (!String.IsNullOrEmpty(promoCode))
                                  ? InvoiceRepository.GetValidDiscount(cuser.ID, promoCode, null)
                                  : null;
            if (discount != null)
            {
                pc.CouponDiscountValue = discount.DiscountValueText;
                pc.DiscountAssignedType = (Consts.DiscountAssignedType)discount.Type_ID;
                pc.DiscountCoupon_ID = discount.ID;
                switch (pc.DiscountAssignedType)
                {
                    case Consts.DiscountAssignedType.Shipping:
                        invoices.ForEach(I => I.RealSh = I.Shipping);
                        invoices.ForEach(I => I.Shipping = discount.IsPercent ? Math.Max(I.Shipping * (1 - discount.DiscountValue / 100), 0) : Math.Max(I.Shipping - discount.DiscountValue, 0));
                        break;
                    case Consts.DiscountAssignedType.BuyerPremium:
                        invoices.ForEach(I => I.RealBP = I.BuyerPremium);
                        invoices.ForEach(I => I.BuyerPremium = discount.IsPercent ? Math.Max(I.BuyerPremium * (1 - discount.DiscountValue / 100), 0) : Math.Max(I.BuyerPremium - discount.DiscountValue, 0));
                        break;
                    default:
                        invoices = invoices.OrderBy(i => i.TotalCost).ToList();
                        invoices.ForEach(I => I.RealDiscount = I.Discount);
                        if (discount.IsPercent)
                            invoices.ForEach(I => I.TotalOrderDiscount = I.TotalCost * discount.DiscountValue / 100);
                        else
                            CalculaterDiscountForInvoices(invoices, discount.DiscountValue);
                        invoices.ForEach(I => I.Discount += I.TotalOrderDiscount.GetValueOrDefault(0));
                        break;
                }
                invoices.ForEach(I => I.Discount_ID = discount.ID);
            }
            invoices.ForEach(I => I.Total = I.TotalCost);

            pc.Deposit = pc.DepositLimit = InvoiceRepository.GetDepositAmountExceptCurrent(cuser.ID);
            pc.Invoices.AddRange(invoices);
            pc.IsShipping = isShipping;

            Session["PayForItems"] = pc;

            List<InvoiceDetail> ids = InvoiceRepository.GetEventUnpaidInvoicesForUser(cuser.ID);
            ViewData["IsBack"] = (ids.Count() > 0 && pc.Deposit < ids.Sum(I => I.TotalCost) && pc.TotalCost <= pc.Deposit);

            CreditCardInfo cci = new CreditCardInfo();
            cci.Address1 = cuser.Address_Billing.Address_1;
            cci.Address2 = cuser.Address_Billing.Address_2;
            cci.CardCode = cci.CardNumber = String.Empty;
            cci.City = cuser.Address_Billing.City;
            cci.Country = cuser.Address_Billing.Country_ID;
            cci.CountryTitle = cuser.Address_Billing.Country;
            cci.ExpirationMonth = DateTime.Now.Month;
            cci.ExpirationYear = DateTime.Now.Year;
            cci.State = cuser.Address_Billing.State;
            cci.Zip = cuser.Address_Billing.Zip;
            ViewData["Countries"] = new SelectList(dataProvider.CountryRepository.GetCountryList(), "ID", "Title", cci.Country);
            return View(cci);
        }

        //MakePaymentFromDeposit
        private long MakePaymentFromDeposit(ref decimal amount, string shippingAddress, Address ba, long userinvoice_id)
        {
            if (amount <= 0) return -1;
            SessionUser cuser = AppHelper.CurrentUser;
            decimal am, amountpaid = amount;
            StringBuilder depositsId;
            Address billing = new Address();
            List<Deposit> deposits = InvoiceRepository.SetDepositAmount(cuser.ID, ref amountpaid);
            depositsId = new StringBuilder();
            depositsId.Append("Deposit#: ");
            foreach (Deposit dep in deposits)
                depositsId.AppendFormat("{0}, ", dep.ID);
            depositsId.Remove(depositsId.Length - 2, 2);
            Deposit deposit = deposits[deposits.Count - 1];
            billing = new Address();
            billing.FirstName = ba.FirstName;
            billing.LastName = ba.LastName;
            billing.MiddleName = ba.MiddleName;
            billing.Address_1 = deposit.Addr;
            billing.City = deposit.City;
            billing.State = deposit.Street;
            billing.Zip = deposit.Zip;
            billing.Country_ID = deposit.Country_Id;
            am = amount - amountpaid;
            amount = amountpaid;
            return InvoiceRepository.AddPayment(am, cuser.ID, userinvoice_id, deposit.PaymentType_Id, String.Empty, shippingAddress, billing, String.Empty, depositsId.ToString());
        }

        //ConfirmPayment    
        private PaymentConfirmation ConfirmPayment(bool isPayPal, Address billing, Address shipping, string transactionID, string transactionType, string CCNum, PaymentConfirmation pc)
        {
            decimal depositAmount, deposit = pc.Deposit;
            depositAmount = deposit;
            string shippingAddress = (!pc.IsShipping) ? "PICK UP" : String.Format("{0} | {1}", shipping.FullName, shipping.FullAddress2);
            decimal amount, am;
            List<Invoice> invoices = InvoiceRepository.GetInvoicesByIDetails(pc.Invoices.Select(I => I.Invoice_ID).ToList());
            long payment_id, deposit_id; long? discount_id = null;
            var grInv = invoices.GroupBy(I2 => I2.UserInvoices_ID);
            StringBuilder sb;
            InvoiceDetail id;
            SessionUser cuser = AppHelper.CurrentUser;
            foreach (var c in grInv)
            {
                amount = 0;
                sb = new StringBuilder();
                foreach (Invoice inv in c.ToList())
                {
                    if ((id = pc.Invoices.Where(I => I.Invoice_ID == inv.ID).FirstOrDefault()) == null) continue;
                    if (pc.HasDiscount)
                    {
                        discount_id = id.Discount_ID;
                        inv.Discount_ID = id.Discount_ID;
                        inv.Real_Sh = id.RealSh;
                        inv.Real_BP = id.RealBP;
                        inv.Real_Discount = id.RealDiscount;
                        inv.BuyerPremium = id.BuyerPremium;
                        inv.Shipping = id.Shipping;
                        inv.Discount = id.Discount;
                    }
                    inv.Insurance = id.Insurance;
                    inv.IsPaid = true;
                    inv.IsPickUp = !pc.IsShipping;
                    sb.AppendFormat("{0},", id.LinkParams.Lot);
                    amount += id.TotalCost;
                }
                if (sb.Length > 0) sb.Remove(sb.Length - 1, 1);

                deposit_id = -1;
                if (deposit >= 0)
                {
                    am = (deposit >= amount) ? amount : deposit;
                    if ((deposit_id = MakePaymentFromDeposit(ref am, shippingAddress, billing, c.Key.Value)) != -1)
                        if (deposit >= amount)
                        {
                            deposit -= amount;
                            foreach (Invoice inv in c.ToList())
                                inv.PaymentDeposit_Id = deposit_id;
                            continue;
                        }
                        else
                        {
                            amount -= deposit;
                            deposit = 0;
                        }
                }
                payment_id = (isPayPal) ? InvoiceRepository.AddPayment(amount, cuser.ID, c.Key.Value, (int)Consts.PaymentType.Paypal, transactionID, shippingAddress, billing, String.Empty, "Payment for lot(s)# " + sb.ToString()) : InvoiceRepository.AddPayment(amount, cuser.ID, c.Key.Value, (int)Consts.PaymentType.CreditCard, transactionID, shippingAddress, billing, CCNum, "Payment for lot(s)# " + sb.ToString());
                foreach (Invoice inv in c.ToList())
                {
                    inv.Payment_ID = payment_id;
                    if (deposit_id != -1) inv.PaymentDeposit_Id = deposit_id;
                }
                if (discount_id.HasValue) InvoiceRepository.AddUsedDiscount(cuser.ID, discount_id.GetValueOrDefault(-1));
            }
            InvoiceRepository.SubmitChanges();

            if (InvoiceRepository.GetUnpaidInvoicesCount(cuser.ID) == 0)
                pc.IsDepositRefunded = InvoiceRepository.RefundingDeposits(cuser.ID);

            pc.PaymentConfirmationType = PaymentConfirmationType.ITEMS;
            pc.TransactionID = transactionID;
            pc.TransactionType = transactionType;
            pc.Address_Billing = billing;
            pc.Address_Shipping = shipping;
            pc.PaymentType = isPayPal ? Consts.PaymentType.Paypal : Consts.PaymentType.CreditCard;
            pc.Deposit = depositAmount;
            pc.DepositLimit = InvoiceRepository.GetDepositAmount(cuser.ID);

            Mail.SendInvoicePaymentConfirmation(cuser.Email, cuser.Login, pc);
            return pc;
        }

        //CreditCardResult
        [VauctionAuthorize, HttpPost, RequireSslFilter, Compress, NoCache] //, ValidateAntiForgeryToken
        public ActionResult CreditCardResult(CreditCardInfo cci)
        {
            if (!ModelState.IsValid)
            {
                ViewData["IsCreditCard"] = true;
                ViewData["Countries"] = new SelectList(dataProvider.CountryRepository.GetCountryList(), "ID", "Title", cci.Country);
                return View("MakePayment", cci);
            }
            PaymentConfirmation pc = Session["PayForItems"] as PaymentConfirmation;
            SessionUser cuser = AppHelper.CurrentUser;
            if (pc == null || pc.Invoices == null || pc.Invoices.Count() == 0 || (pc.HasDiscount && !InvoiceRepository.IsDiscountValide(null, cuser.ID, pc.DiscountCoupon_ID))) return RedirectToAction("PayForItems", "Account");

            StringBuilder sbInvoices = new StringBuilder();
            StringBuilder sbLots = new StringBuilder();

            foreach (InvoiceDetail id in pc.Invoices)
            {
                sbInvoices.Append(id.Invoice_ID.ToString() + ", ");
                sbLots.Append(id.LinkParams.Lot + ", ");
            }
            if (sbInvoices.Length > 0)
                sbInvoices.Remove(sbInvoices.Length - 2, 2);
            if (sbLots.Length > 0)
                sbLots.Remove(sbLots.Length - 2, 2);

            int month = Convert.ToInt32(Request.Form["ExpirationMonth"]);
            int year = Convert.ToInt32(Request.Form["ExpirationYear"]);

            long profile_id = 0;
            try
            {
                profile_id = AN_Functions.CreateCustomerProfile(cuser.ID, cuser.Email, sbLots.ToString());
                if (profile_id <= 0) throw new Exception(AN_Functions.LastError);
                CustomerProfileWS.CustomerProfileMaskedType profile = AN_Functions.GetCustomerProfile(profile_id);
                if (profile == null) throw new Exception(AN_Functions.LastError);

                Address bAddr = cuser.Address_Billing;
                bAddr.Address_1 = cci.Address1;
                bAddr.Address_2 = cci.Address2;
                bAddr.City = cci.City;
                bAddr.Zip = cci.Zip;
                bAddr.State = cci.State;
                bAddr.Country_ID = cci.Country.HasValue ? cci.Country.Value : 0;
                bAddr.Country = (String.IsNullOrEmpty(cci.CountryTitle)) ? CountryRepository.GetCountryByID(bAddr.Country_ID).Title : cci.CountryTitle;

                long shipping_profile_id = AN_Functions.CreateCustomerShippingAddress(profile_id, bAddr, cci.CountryTitle);
                if (shipping_profile_id <= 0) throw new Exception(AN_Functions.LastError);

                long payment_profile_id = AN_Functions.CreateCustomerPaymentProfile(profile_id, cci);
                if (payment_profile_id <= 0) throw new Exception(AN_Functions.LastError);

                if ((!AN_Functions.UpdateCustomerShippingAddress(profile_id, shipping_profile_id, bAddr)) || (!AN_Functions.UpdateCustomerPaymentProfile(profile_id, payment_profile_id, cci, bAddr.FirstName, bAddr.LastName)))
                    throw new Exception(AN_Functions.LastError);

                profile = AN_Functions.GetCustomerProfile(profile_id);
                if (profile == null) throw new Exception(AN_Functions.LastError);

                if (!AN_Functions.ValidateCustomerPaymentProfile(profile_id, payment_profile_id, shipping_profile_id, cci.CardCode))
                    throw new Exception(AN_Functions.LastError);

                string TransactionID = AN_Functions.CreateTransaction(profile_id, payment_profile_id, shipping_profile_id, (pc.TotalCost - pc.Deposit).GetPrice(), sbInvoices.ToString(), pc.IsShipping, cci, "Lot#: " + sbLots.ToString());

                if (String.IsNullOrEmpty(TransactionID)) throw new Exception("The transaction was unsuccessful.");

                pc = ConfirmPayment(false, bAddr, cuser.Address_Shipping, TransactionID, String.Empty, cci.CardNumber.Substring(cci.CardNumber.Length - 5), pc);
                if (pc == null) throw new Exception(AN_Functions.LastError);
                Session.Remove("PayForItems");
                return View("PaymentConfirmation", pc);
            }
            catch (Exception ex)
            {
                Logger.LogException(String.Format("[user_id={0}]", cuser.ID), ex);
                ViewData["ErrorMessage"] = ex.Message;
            }
            finally
            {
                try
                {
                    if (!AN_Functions.DeleteCustomerProfile(profile_id))
                        throw new Exception(AN_Functions.LastError);
                }
                catch (Exception ex)
                {
                    Logger.LogException(String.Format("[user_id={0}]", cuser.ID), ex);
                }
            }
            InitCurrentEvent();
            return View("PaymentFailed");
        }

        //PayPalResult
        [VauctionAuthorize, HttpPost, RequireSslFilter, Compress, NoCache] //, ValidateAntiForgeryToken
        public ActionResult PayPalResult()
        {
            PaymentConfirmation pc = Session["PayForItems"] as PaymentConfirmation;
            SessionUser cuser = AppHelper.CurrentUser;
            if (pc == null || pc.Invoices == null || pc.Invoices.Count() == 0 || (pc.HasDiscount && !InvoiceRepository.IsDiscountValide(null, cuser.ID, pc.DiscountCoupon_ID))) return RedirectToAction("PayForItems", "Account");
            try
            {
                StringBuilder requestString = PP_Functions.InitializeRequest("SetExpressCheckout");
                requestString.Append("&RETURNURL=" + AppHelper.GetSiteUrl("/Account/PayPalConfirm"));
                requestString.Append("&CANCELURL=" + AppHelper.GetSiteUrl("/Account/PayForItems"));
                requestString.Append("&PAYMENTACTION=authorization");

                requestString.Append(PayPalDetails(pc));

                string token;
                string dummy;
                if (PP_Functions.Post(requestString.ToString(), out token, out dummy))
                {
                    Response.Redirect("https://" + Config.GetProperty("PayPalUrl") + "/cgi-bin/webscr?cmd=_express-checkout&token=" + token);
                    return Content("ok");
                }
                else throw new Exception("Payment via PayPal failed");
            }
            catch (Exception ex)
            {
                Logger.LogException(String.Format("[user_id={0}]", cuser.ID), ex);
                ViewData["ErrorMessage"] = ex.Message;
            }
            InitCurrentEvent();
            return View("PaymentFailed");
        }

        //PrepairTitleForPayPal
        private string PrepairTitleForPayPal(string title)
        {
            return title.Replace("½", " 1/2").Replace("⅔", " 2/3").Replace("⅕", " 1/3").Replace("⅖", " 2/5").Replace("⅗", " 3/5").Replace("⅘", " 4/5").Replace("⅙", " 1/6").Replace("⅚", " 5/6").Replace("⅛", " 1/8").Replace("⅜", " 3/8").Replace("⅝", " 5/8").Replace("⅞", " 7/8");
        }

        //PayPalDetails
        private string PayPalDetails(PaymentConfirmation pc)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbresult = new StringBuilder();
            int number = -1;
            foreach (InvoiceDetail id in pc.Invoices)
            {
                number++;
                sb = new StringBuilder();
                sb.Append(id.Amount > 0 ? "| Price: " + id.Amount.GetCurrency() : String.Empty);
                sb.Append(id.BuyerPremium > 0 ? " | Buyer's premium: " + id.BuyerPremium.GetCurrency() : String.Empty);
                sb.Append(id.Shipping > 0 ? " | Shipping: " + id.Shipping.GetCurrency() : String.Empty);
                sb.Append(id.Insurance > 0 ? " | Insurance: " + id.Insurance.GetCurrency() : String.Empty);
                sb.Append(id.Tax > 0 ? " | Tax: " + id.Tax.GetCurrency() : String.Empty);
                sb.Append(id.Discount > 0 ? " | Discount: " + id.Discount.GetCurrency() : String.Empty);

                sbresult.AppendFormat("&L_NAME{0}={1}", number, PrepairTitleForPayPal(id.LinkParams.Title));
                sbresult.AppendFormat("&L_NUMBER{0}={1}", number, id.LinkParams.Lot);
                sbresult.AppendFormat("&L_DESC{0}={1}", number, sb.ToString());
                sbresult.AppendFormat("&L_QTY{0}={1}", number, 1);
                sbresult.AppendFormat("&L_AMT{0}={1}", number, id.TotalCost.GetPrice());
            }
            if (pc.Deposit > 0)
            {
                number++;
                sbresult.AppendFormat("&L_NAME{0}={1}", number, "--- Deposit / Credit");
                sbresult.AppendFormat("&L_QTY{0}={1}", number, 1);
                sbresult.AppendFormat("&L_DESC{0}={1}", number, String.Empty);
                sbresult.AppendFormat("&L_AMT{0}={1}", number, -pc.Deposit.GetPrice());
            }
            sbresult.AppendFormat("&ITEMAMT={0}", (pc.TotalCost - pc.Deposit).GetPrice());
            sbresult.AppendFormat("&AMT={0}", (pc.TotalCost - pc.Deposit).GetPrice());
            return sbresult.ToString();
        }

        //PayPalConfirm
        [VauctionAuthorize, RequireSslFilter, Compress, NoCache]
        public ActionResult PayPalConfirm()
        {
            SessionUser cuser = AppHelper.CurrentUser;
            try
            {
                PaymentConfirmation pc = Session["PayForItems"] as PaymentConfirmation;
                if (pc == null || pc.Invoices == null || pc.Invoices.Count() == 0) throw new Exception("Express Checkout Failed");

                if (Request.QueryString.Get("token") == null) throw new Exception("Express Checkout Failed");
                string token;
                string payerID;
                string transactionID, transactionType;
                StringBuilder requestString = PP_Functions.InitializeRequest("GetExpressCheckoutDetails");
                requestString.Append("&TOKEN=" + HttpUtility.UrlEncode(Request.QueryString.Get("token")));
                Address shipping = new Address();
                string errormessage = String.Empty;
                if (PP_Functions.Post(requestString.ToString(), out token, out payerID, shipping, out errormessage))
                {
                    requestString = PP_Functions.InitializeRequest("DoExpressCheckoutPayment");
                    requestString.Append("&TOKEN=" + HttpUtility.UrlEncode(token));
                    requestString.Append("&AMT=" + HttpUtility.UrlEncode((pc.TotalCost - pc.Deposit).ToString("f")));
                    requestString.Append("&PAYMENTACTION=Sale");
                    requestString.Append("&PAYERID=" + HttpUtility.UrlEncode(payerID));

                    requestString.Append(PayPalDetails(pc));

                    if (PP_Functions.Post(requestString.ToString(), out token, out payerID, out transactionID, out transactionType, out errormessage))
                    {
                        pc = ConfirmPayment(true, cuser.Address_Billing, shipping, transactionID, transactionType, String.Empty, pc);
                        if (pc == null) throw new Exception("Express Checkout Failed");
                        Session.Remove("PayForItems");
                        return View("PaymentConfirmation", pc);
                    }
                    else throw new Exception(String.IsNullOrEmpty(errormessage) ? "Express Checkout Failed" : errormessage);
                }
                else throw new Exception(String.IsNullOrEmpty(errormessage) ? "Express Checkout Failed" : errormessage);

            }
            catch (Exception ex)
            {
                Logger.LogException(String.Format("[user_id={0}]", cuser.ID), ex);
                ViewData["ErrorMessage"] = ex.Message;
            }
            InitCurrentEvent();
            return View("PaymentFailed");
        }

        //PayFromDeposit
        [VauctionAuthorize, HttpPost, RequireSslFilter, NoCache] //, ValidateAntiForgeryToken
        public ActionResult PayFromDeposit()
        {
            PaymentConfirmation pc = Session["PayForItems"] as PaymentConfirmation;
            SessionUser cuser = AppHelper.CurrentUser;
            if (pc == null || pc.Invoices == null || pc.Invoices.Count() == 0 || (pc.HasDiscount && !InvoiceRepository.IsDiscountValide(null, cuser.ID, pc.DiscountCoupon_ID))) return RedirectToAction("PayForItems", "Account");
            string shippingAddress = (!pc.IsShipping) ? String.Empty : String.Format("{0} | {1}", cuser.Address_Shipping.FullName, cuser.Address_Shipping.FullAddress);
            decimal amount = 0;
            InvoiceDetail id;
            StringBuilder isConsignorShipItems = new StringBuilder();
            List<Invoice> invoices = InvoiceRepository.GetInvoicesByIDetails(pc.Invoices.Select(I => I.Invoice_ID).ToList());
            foreach (Invoice inv in invoices)
            {
                if ((id = pc.Invoices.Where(I => I.Invoice_ID == inv.ID).FirstOrDefault()) == null) continue;
                if (pc.HasDiscount)
                {
                    inv.Discount_ID = id.Discount_ID;
                    inv.Real_Sh = id.RealSh;
                    inv.Real_BP = id.RealBP;
                    inv.Real_Discount = id.RealDiscount;
                    inv.BuyerPremium = id.BuyerPremium;
                    inv.Shipping = id.Shipping;
                    inv.Discount = id.Discount;
                }
                inv.Shipping = id.Shipping;
                inv.Insurance = id.Insurance;
                inv.IsPaid = true;
                inv.IsPickUp = !pc.IsShipping;
                amount = id.TotalCost;
                inv.PaymentDeposit_Id = MakePaymentFromDeposit(ref amount, shippingAddress, cuser.Address_Billing, inv.UserInvoices_ID.GetValueOrDefault(-1));
                if (id.IsConsignorShip) isConsignorShipItems.Append(id.LinkParams.Lot.ToString() + ",");
            }
            InvoiceRepository.SubmitChanges();
            if (pc.HasDiscount) InvoiceRepository.AddUsedDiscount(cuser.ID, pc.Invoices[0].Discount_ID.GetValueOrDefault(-1));
            if (isConsignorShipItems.Length > 0) isConsignorShipItems.Remove(isConsignorShipItems.Length - 1, 1);

            pc.PaymentConfirmationType = PaymentConfirmationType.ITEMS;
            pc.TransactionID = String.Empty;
            pc.TransactionType = String.Empty;
            pc.Address_Billing = cuser.Address_Billing;
            pc.Address_Shipping = cuser.Address_Shipping;
            pc.DepositLimit = InvoiceRepository.GetDepositAmount(cuser.ID);
            pc.Deposit = pc.TotalCost;
            if (InvoiceRepository.GetUnpaidInvoicesCount(cuser.ID) == 0)
                pc.IsDepositRefunded = InvoiceRepository.RefundingDeposits(cuser.ID);

            Mail.SendInvoicePaymentConfirmation(cuser.Email, cuser.Login, pc);

            Session.Remove("PayForItems");
            return View("PaymentConfirmation", pc);
        }

        #endregion

        #region discount
        //ValidateDiscount
        [VauctionAuthorize, HttpPost, NoCache]
        public JsonResult ValidateDiscount(string discount_code)
        {
            return ((string.IsNullOrEmpty(discount_code) || discount_code.Equals("Promo Code")) ? null : JSON(InvoiceRepository.ValidateDiscount(discount_code, AppHelper.CurrentUser.ID, null)));
        }
        #endregion //Discount

        #endregion

        #region print invoice
        [VauctionAuthorize, RequireSslFilter, Compress, NoCache]
        public ActionResult PrintInvoice()
        {
            PaymentConfirmation pc = Session["PayForItems"] as PaymentConfirmation;
            return View(pc);
        }
        #endregion

    }
}