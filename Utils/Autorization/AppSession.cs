using System;
using System.Web;
using Vauction.Models;
using Vauction.Configuration;
using Vauction.Utils.Perfomance;

namespace Vauction.Utils.Autorization
{
  #region SessionKeys
  public class SessionKeys
  {
    public const string User = "CurrentUser";    
  }
  #endregion

  #region SessionUser
  [Serializable]
  public class SessionUser
  {
    public long ID { get; set; }
    public string Login { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public byte UserType { get; set; }
    public byte Status { get; set; }
    public int CommRate_ID { get; set; }
    public bool IsConsignorShip { get; set; }
    public string SellerLocation { get; set; }
    public string IP { get; set; }

    public Address Address_Billing { get; set; }
    public Address Address_Shipping { get; set; }
    public bool IsAddressBillingLikeShipping { get; set; }
    public bool NotRecievingBidConfirmation { get; set; }
    public bool NotRecievingOutBidNotice { get; set; }
    public bool NotRecievingLotSoldNotice { get; set; }
    public bool NotRecievingLotClosedNotice { get; set; }
    public bool NotRecieveWeeklySpecials { get; set; }
    public bool NotRecieveNewsUpdates { get; set; }

    
    public bool IsSeller { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Seller; } }
    public bool IsSellerBuyer { get { return (Consts.UserTypes)UserType == Consts.UserTypes.SellerBuyer; } }
    public bool IsSellerType { get { return IsSeller || IsSellerBuyer; } }
    public bool IsBuyer { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Buyer; } }
    public bool IsBuyerType { get { return IsBuyer || IsSellerBuyer; } }
    public bool IsHouseBidder { get { return (Consts.UserTypes)UserType == Consts.UserTypes.HouseBidder; } }
    public bool IsBidder { get { return IsBuyerType || IsHouseBidder; } }
    public bool IsAdmin { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Admin; } }
    public bool IsRoot { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Root; } }
    public bool IsReviewer { get { return (Consts.UserTypes)UserType == Consts.UserTypes.Reviewer; } }
    public bool IsAdminType { get { return IsAdmin || IsRoot; } }
    public bool IsBackendUser { get { return IsAdmin || IsRoot || IsHouseBidder; } }
    public bool IsAccessable { get { return !IsBuyer; } }
    
    //SessionUser
    public SessionUser()
    {
      ID = CommRate_ID = 0;
      Login = FirstName = SellerLocation = Email = LastName= String.Empty;
      UserType = Status = 0;
      NotRecievingBidConfirmation = NotRecievingOutBidNotice = NotRecievingLotSoldNotice = NotRecievingLotClosedNotice = NotRecieveWeeklySpecials = NotRecieveNewsUpdates = IsConsignorShip = false;
    }

    //SessionUser
    public SessionUser(long id, string l, string fn, string ln, byte type, byte status, bool rbc, bool robn, bool rlsn, bool rlcn, bool rws, bool rnu, int commrate_id, bool isconsh, string slocation, string email, bool isbillinglikeshipping, AddressCard b, AddressCard s, string ip)
    {
      ID = id;
      Login = l;
      FirstName = fn;
      LastName = ln;
      UserType = type;
      Status = status;
      NotRecievingBidConfirmation = rbc;
      NotRecievingOutBidNotice = robn;
      NotRecievingLotSoldNotice = rlsn;
      NotRecievingLotClosedNotice = rlcn;
      NotRecieveWeeklySpecials = rws;
      NotRecieveNewsUpdates = rnu;
      CommRate_ID = commrate_id;
      IsConsignorShip = isconsh;
      SellerLocation = slocation;
      Email = email;
      IsAddressBillingLikeShipping = isbillinglikeshipping;
      Country country = ProjectConfig.Config.DataProvider.GetInstance().CountryRepository.GetCountryByID(b.Country_ID);
      Address_Billing = new Address { Address_1 = b.Address1, Address_2 = b.Address2, City = b.City, Country_ID = b.Country_ID, State_ID = b.State_ID, Zip = b.Zip, Country=country.Title, State=b.State, HomePhone=b.HomePhone, WorkPhone=b.WorkPhone, Fax=b.Fax, FirstName = b.FirstName, LastName=b.LastName, MiddleName=b.MiddleName };
      country = ProjectConfig.Config.DataProvider.GetInstance().CountryRepository.GetCountryByID(s.Country_ID);
      Address_Shipping = new Address { Address_1 = s.Address1, Address_2 = s.Address2, City = s.City, Country_ID = s.Country_ID, State_ID = s.State_ID, Zip = s.Zip, Country = country.Title, State = s.State, HomePhone = s.HomePhone, WorkPhone = s.WorkPhone, Fax = s.Fax, FirstName=s.FirstName, LastName=s.LastName, MiddleName=s.MiddleName };
      IP = ip;
    }

    //Create
    public static SessionUser Create(IUser user)
    {      
      AddressCard ac = ProjectConfig.Config.DataProvider.GetInstance().UserRepository.GetAddressCard(user.Billing_AddressCard_ID.GetValueOrDefault(-1), true);
      AddressCard ac2 = ProjectConfig.Config.DataProvider.GetInstance().UserRepository.GetAddressCard((user.BillingLikeShipping || !user.Shipping_AddressCard_ID.HasValue? user.Billing_AddressCard_ID:user.Shipping_AddressCard_ID).GetValueOrDefault(-1), true);        
      return new SessionUser(user.ID, user.Login, ac.FirstName, ac.LastName, user.UserType, user.Status, user.NotRecievingBidConfirmation, user.NotRecievingOutBidNotice, user.NotRecievingLotSoldNotice, user.NotRecievingLotClosedNotice, user.NotRecieveWeeklySpecials, user.NotRecieveNewsUpdates, user.CommRate_ID, user.IsConsignorShip, String.Format("{0}, {1}", ac.City, ac.State), user.Email,  user.BillingLikeShipping, ac, ac2, user.IP);
    }

    //Create
    public static SessionUser Create(UserNew user)
    {
      return new SessionUser
               {
                 Address_Billing = user.AddressBilling,
                 Address_Shipping = user.AddressShipping,
                 CommRate_ID = user.Commission_ID,
                 Email = user.Email,
                 FirstName = user.AddressBilling.FirstName,
                 ID = user.ID,
                 IP = user.IP,
                 IsAddressBillingLikeShipping = user.IsBillingLikeShipping,
                 IsConsignorShip = user.IsConsignorShip,
                 LastName = user.AddressBilling.LastName,
                 Login = user.Login,
                 NotRecieveNewsUpdates = user.NotRecieveNewsUpdates,
                 NotRecieveWeeklySpecials = user.NotRecieveWeeklySpecials,
                 NotRecievingBidConfirmation = user.NotRecievingBidConfirmation,
                 NotRecievingLotClosedNotice = user.NotRecievingLotClosedNotice,
                 NotRecievingLotSoldNotice = user.NotRecievingLotSoldNotice,
                 NotRecievingOutBidNotice = user.NotRecievingOutBidNotice,
                 SellerLocation = String.Format("{0}, {1}", user.AddressBilling.City, user.AddressBilling.State),
                 Status = user.Status,
                 UserType = user.Type
               };
    }


  }
  #endregion
  
  #region AppSession
  public class AppSession
  {
    //IsAuthenticated
    private static bool IsAuthenticated
    {
      get
      {
        return HttpContext.Current!=null && HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated;
      }
    }

    //CurrentUser
    public static SessionUser CurrentUser
    {
      get
      {
        try
        { 
          if (!IsAuthenticated) return null;
          SessionUser user = HttpContext.Current.Session[SessionKeys.User] as SessionUser;
          if (user == null)
          {
            VauctionPrincipal principal = (HttpContext.Current.User as VauctionPrincipal);
            if (principal != null)
            {
              VauctionIdentity identity = principal.UIdentity;
              UserNew usr = ProjectConfig.Config.DataProvider.GetInstance().UserRepository.GetUserActiveAndApproved(identity.ID, identity.Name);
              user = (usr != null) ? SessionUser.Create(usr) : null;
            }
            HttpContext.Current.Session[SessionKeys.User] = user;
          }
          //else
          //{
          //  #region added 2013-03-15
          //  HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
          //  if (authCookie == null || String.IsNullOrEmpty(authCookie.Value))
          //  {
          //    Logger.LogInfo(String.Format("[SESSION-ERROR] Type: NoCookie | SessionID:{0} | User_ID:{1}", HttpContext.Current.Session.SessionID, user.ID));
          //    HttpContext.Current.Session.Abandon();
          //    throw new UnauthorizedAccessException();
          //  }
          //  FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
          //  if (authTicket == null)
          //  {
          //    Logger.LogInfo(String.Format("[SESSION-ERROR] Type: NoFormAuthorizationTicket | SessionID:{0} | User_ID:{1}", HttpContext.Current.Session.SessionID, user.ID));
          //    HttpContext.Current.Session.Abandon();
          //    throw new UnauthorizedAccessException();
          //  }
          //  VauctionIdentity videntity = new VauctionIdentity(authTicket.Name, authTicket.UserData);
          //  if (videntity.ID != user.ID)
          //  {
          //    Logger.LogInfo(String.Format("[SESSION-ERROR]: Type:Mixed | SessionID:{0} | User_ID:{1} | CrossedUser_ID: {2}", HttpContext.Current.Session.SessionID, videntity.ID, user.ID));
          //    HttpContext.Current.Session.Abandon();
          //    throw new UnauthorizedAccessException();
          //  }
          //  #endregion

          //  #region added 2013-03-15
          //  if (user.IP != Consts.UsersIPAddress)
          //  {
          //    Logger.LogInfo(String.Format("[IP]: Type:DifferentIP | SessionID:{0} | user_id: {1} | User_IP:{2} | Current_ID: {3}", HttpContext.Current.Session.SessionID, user.ID, user.IP, Consts.UsersIPAddress));
          //    HttpContext.Current.Session.Abandon();
          //    throw new UnauthorizedAccessException();
          //  }
          //  #endregion
          //}
          return user;
        }
        catch
        {
          return null;
        }        
      }
      set
      {
        HttpContext.Current.Session[SessionKeys.User] = value;
      }
    }

  }
  #endregion

  #region AppApplication
  public class AppApplication
  {
    public static string CacheProviderKey;

    static AppApplication()
    {
      CacheProviderKey = "CacheProvider";
    }

    public static ICacheDataProvider CacheProvider
    {
      get
      {
        if (HttpContext.Current != null && HttpContext.Current.Application[CacheProviderKey] != null)
          return (ICacheDataProvider)HttpContext.Current.Application[CacheProviderKey];
        ICacheDataProvider cacheDataProvider;
        if (Consts.DataCachingTechnology == DataCacheTechnology.MEMORYOBJECT) cacheDataProvider = new CacheDataProvider();
        else
        {
          try
          {
            cacheDataProvider = new AFCDataProvider(Consts.ProductName);
          }
          catch
          {
            cacheDataProvider = new CacheDataProvider();
          }
        }
        return cacheDataProvider;
      }
      set { HttpContext.Current.Application[CacheProviderKey] = value; }
    }
  }
  #endregion
}