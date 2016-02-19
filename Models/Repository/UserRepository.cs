using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Data.Common;
using Vauction.Utils;
using Vauction.Utils.Lib;
using Vauction.Utils.Helpers;
using System.Linq.Dynamic;
using QControls.Security;
using System.Data.Linq;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
  public class UserRepository : IUserRepository
  {
    #region init
    private VauctionDataContext dataContext;
    private ICacheDataProvider CacheRepository;

    public UserRepository(VauctionDataContext dataContext, ICacheDataProvider cacheDataProvider)
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

    #region get user
    //GetUser
    public User GetUser(long User_ID, bool iscache)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETUSER", new object[] { User_ID }, CachingExpirationTime.Hours_01);
      User result = CacheRepository.Get(dco) as User;
      if (result != null && iscache) return result;
      result = dataContext.spSelect_User(User_ID, null, null, null, null, null).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }
    //GetUser
    public User GetUser(string login, bool iscache)
    {
      login = login.ToLower().Trim();
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETUSER", new object[] { login }, CachingExpirationTime.Hours_01);
      User result = CacheRepository.Get(dco) as User;
      if (result != null && iscache) return result;
      result = dataContext.spSelect_User(null, null, login, null, null, null).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }
    //GetUserByEmail
    public User GetUserByEmail(string email, bool iscache)
    {      
      email = email.ToLower().Trim();
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETUSERBYEMAIL", new object[] { email }, CachingExpirationTime.Hours_01);
      User result = CacheRepository.Get(dco) as User;
      if (result != null && iscache) return result;
      result = dataContext.spSelect_User(null, null, null, null, null, email).FirstOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }
    //UpdateUser
    private void UpdateUserCache(User user)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETUSER", new object[] { user.ID }, CachingExpirationTime.Hours_01, user);
      CacheRepository.Put(dco);
      dco.Params = new object[] { user.Login.ToLower() };
      CacheRepository.Put(dco);
      dco.Method = "GETUSERBYEMAIL";
      dco.Params = new object[] { user.Email.ToLower() };
      CacheRepository.Put(dco);
      //CacheRepository.Update(CacheDataKeys.USER_GETUSER, user, new object[] { user.ID });
      //CacheRepository.Update(CacheDataKeys.USER_GETUSER, user, new object[] { user.Login.ToLower() });
      //CacheRepository.Update(CacheDataKeys.USER_GETUSERBYEMAIL, user, new object[] {  });
    }
    //ClearUserCache
    public void ClearUserCache(long user_id)
    {
      User user = GetUser(user_id, true);
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETUSER", new object[] { user.ID }, CachingExpirationTime.Hours_01, user);
      CacheRepository.Remove(dco);
      dco.Params = new object[] { user.Login.ToLower() }; CacheRepository.Remove(dco);
      dco.Method = "GETUSERBYEMAIL";
      dco.Params = new object[] { user.Email.ToLower() };
      CacheRepository.Remove(dco);
    }
    #endregion

    #region login / validate user
    //GetUserActiveAndApproved
    public User GetUserActiveAndApproved(string login)
    {
      return dataContext.spSelect_User(null, null, login, true, (byte)Consts.UserStatus.Active, null).FirstOrDefault();      
    }

    //GetUserActiveAndApproved
    public UserNew GetUserActiveAndApproved(long user_id, string login)
    {
      //return dataContext.spSelect_User(user_id, null, login, true, (byte)Consts.UserStatus.Active, null).FirstOrDefault();
      //return dataContext.Users.Where(p => p.ID == user_id && p.Login == login && p.IsConfirmed && p.Status == (byte)Consts.UserStatus.Active).SingleOrDefault();
      return (from u in dataContext.spUser_GetUserActiveAndApproved(user_id, login)
                select new UserNew
                         {
                           AddressBilling = new Address{Address_1 = u.AB_Address1, Address_2 = u.AB_Address2, City = u.AB_City, Country = u.AB_Country, Country_ID = u.AB_CountryID.GetValueOrDefault(0), Fax = u.AB_Fax, FirstName = u.AB_First, HomePhone = u.AB_HomePhone, LastName = u.AB_LastName, MiddleName = u.AB_MiddleName, State = u.AB_State, State_ID = u.AB_StateID, WorkPhone = u.AB_WorkPhone, Zip = u.AB_Zip},
                           AddressShipping = new Address { Address_1 = u.AS_Address1, Address_2 = u.AS_Address2, City = u.AS_City, Country = u.AS_Country, Country_ID = u.AS_CountryID.GetValueOrDefault(0), Fax = u.AS_Fax, FirstName = u.AS_First, HomePhone = u.AS_HomePhone, LastName = u.AS_LastName, MiddleName = u.AS_MiddleName, State = u.AS_State, State_ID = u.AS_StateID, WorkPhone = u.AS_WorkPhone, Zip = u.AS_Zip },
                           Commission_ID = u.CommRate_ID,
                           ConfirmationCode = u.ConfirmationCode,
                           Email = u.Email,
                           FailedAttemps = u.FailedAttempts,
                           ID = u.ID,
                           IsBillingLikeShipping = u.BillingLikeShipping,
                           IsConsignorShip = u.IsConsignorShip,
                           IsEmailConfirmed = u.IsConfirmed,
                           IsModifyed = u.IsModifyed,
                           NotPasswordReset = u.NotPasswordReset,
                           NotDepositNeed = u.NotDepositNeed,
                           IsMaxDepositOnlyNeed = u.IsMaxDepositOnlyNeed,
                           NotRecievingLotClosedNotice = u.NotRecievingLotClosedNotice,
                           NotRecievingBidConfirmation = u.NotRecievingBidConfirmation,
                           NotRecievingLotSoldNotice = u.NotRecievingLotSoldNotice,
                           NotRecieveNewsUpdates = u.NotRecieveNewsUpdates,
                           NotRecievingOutBidNotice = u.NotRecievingOutBidNotice,
                           NotRecieveWeeklySpecials = u.NotRecieveWeeklySpecials,
                           LastAttempt = u.LastAttempt,
                           IP = u.IP,
                           Login = u.Login,
                           Notes = u.Notes,
                           Password = u.Password,
                           Reference = u.Reference,
                           DateIN = u.DateIN,
                           Status = u.Status,
                           Type = u.UserType
                         }).FirstOrDefault();
    }

    //TryToUpdateNormalAttempts
    public void TryToUpdateNormalAttempts(User usr)
    {
      try
      {
        if (usr == null) throw new Exception("The user doesn't exist");
        usr.IP = Consts.UsersIPAddress;
        usr.FailedAttempts = 0;
        usr.LastAttempt = DateTime.Now;
        dataContext.spUser_UpdateLoginInfo(usr.ID, usr.LastAttempt, usr.IP, usr.FailedAttempts);
      }
      catch (Exception ex)
      {
        Logger.LogException("[user_id=" + usr.ID.ToString(CultureInfo.InvariantCulture) + "]", ex);
      }
    }

    //ValidateLogin
    public bool ValidateLogin(string login, long ID)
    {      
      //return dataContext.spUser_Validate(ID, null, login.Trim()).FirstOrDefault() == null;
      var query = (from p in dataContext.Users where p.Login == login && p.ID != ID select p);
      return query.Count() == 0;
    }

    //ValidateEmail
    public bool ValidateEmail(string email, long ID)
    {
      //return dataContext.spUser_Validate(ID, email.Trim(), null).FirstOrDefault() == null;
      var query = (from p in dataContext.Users where p.Email.Trim().ToLower() == email.Trim().ToLower() && p.ID != ID select p);
      return query.Count() == 0;
    }

    //ValidateUser
    public User ValidateUser(string login, string password)
    {      
      return dataContext.spSelect_User(null, password, login, true, (byte)Consts.UserStatus.Active, null).FirstOrDefault();
    }
    #endregion

    #region outer subscription

    //GetOuterSubscription
    public OuterSubscription GetOuterSubscription(long id)
    {
      return dataContext.spSelect_OuterSubscription(id).FirstOrDefault();
    }

    // ValidateOuterSubscriptionEmail
    public bool ValidateOuterSubscriptionEmail(string email, long ID)
    {
      return dataContext.OuterSubscriptions.Where(p => p.Email.Trim().ToLower() == email.Trim().ToLower() && p.ID != ID).SingleOrDefault() == null;
    }

    // AddOuterSubscription
    public bool AddOuterSubscription(IOuterSubscription os)
    {
      bool res = true;
      try
      {
        dataContext.OuterSubscriptions.InsertOnSubmit(os as OuterSubscription);
        SubmitChanges();
      }
      catch (Exception e)
      {
        Logger.LogException(e);
        res = false;
      }
      return res;
    }

    //ActivateOuterSubscription
    public bool ActivateOuterSubscription(long? id)
    {
      bool res = true;
      try
      {
        if (!id.HasValue) throw new Exception("The code is not correct.");
        OuterSubscription os = dataContext.OuterSubscriptions.SingleOrDefault(OS => OS.ID == id.Value);
        if (os == null) throw new Exception("Outer subscription doesn't exist.");
        os.IsActive = true;
        SubmitChanges();
      }
      catch (Exception ex)
      {
        Logger.LogException("[ID=" + id + "]", ex);
        res = false;
      }
      return res;
    }

    //UnsubscribeFromEmail
    public bool UnsubscribeFromEmail(string email)
    {
      try
      {
        OuterSubscription sub = dataContext.OuterSubscriptions.Where(S => S.Email == email).SingleOrDefault();
        if (sub == null) return false; //throw new Exception("The email doesn't exist");
        dataContext.OuterSubscriptions.DeleteOnSubmit(sub);
        SubmitChanges();
      }
      catch // (Exception ex)
      {
        //Logger.LogException("[Email=" + email + "]", ex);
        return false;
      }
      return true;
    }

    //UnsubscribeRegisterUser
    public bool UnsubscribeRegisterUser(string email)
    {
      try
      {
        User user = GetUserByEmail(email, false);
        if (user == null) return false;// throw new Exception("User doesn't exist");
        user.NotRecieveNewsUpdates = user.NotRecieveWeeklySpecials = true;
        SubmitChanges();
      }
      catch //(Exception ex)
      {
        //Logger.LogException("[email=" + email + "]", ex);
        return false;
      }
      return true;
    }

    //UnsubscribeFromOuterSubscribtionByID
    public bool UnsubscribeFromOuterSubscribtionByID(long id)
    {
      try
      {
        OuterSubscription os = dataContext.OuterSubscriptions.Where(OS => OS.ID == id).SingleOrDefault();
        if (os == null) return false;// throw new Exception("Outer subscription doesn't exist.");
        dataContext.OuterSubscriptions.DeleteOnSubmit(os);
        SubmitChanges();
      }
      catch //(Exception ex)
      {
        //Logger.LogException("[id=" + id.ToString() + "]", ex);
        return false;
      }
      return true;
    }

    //SubscribeRegisterUser
    public bool SubscribeRegisterUser(User u)
    {
      try
      {        
        if (u == null) throw new Exception("The user doesn't exist");
        OuterSubscription os = dataContext.OuterSubscriptions.SingleOrDefault(OU => OU.Email == u.Email);
        if (os != null)
        {
          os.IsActive = true;
          os.IsRecievingUpdates = !u.NotRecieveNewsUpdates;
          os.IsRecievingWeeklySpecials = !u.NotRecieveWeeklySpecials;
          SubmitChanges();
          return false;
        }
        AddressCard acB = GetAddressCard(u.Billing_AddressCard_ID.GetValueOrDefault(-1), true);
        os = new OuterSubscription();
        os.Country = acB.Country.Title;
        os.Email = u.Email;
        os.EmailConfirm = u.Email;
        os.IsActive = true;
        os.IPAddress = Consts.UsersIPAddress;
        os.IsRecievingUpdates = !u.NotRecieveNewsUpdates;
        os.IsRecievingWeeklySpecials = !u.NotRecieveWeeklySpecials;
        os.FirstName = acB.FirstName;
        os.LastName = acB.LastName;
        os.State = acB.State;
        dataContext.OuterSubscriptions.InsertOnSubmit(os);
        SubmitChanges();
      }
      catch (Exception ex)
      {
        Logger.LogException("[user_id=" + u!=null?u.ID.ToString():"null" + "]", ex);
        return false;
      }
      return true;
    }

    //UnsubscribeRegisterUser
    public bool UnsubscribeRegisterUser(long user_id)
    {
      User user = GetUser(user_id, false);
      if (user == null) return false;
      try
      {
        user.NotRecieveNewsUpdates = user.NotRecieveWeeklySpecials = true;
        SubmitChanges();
        UpdateUserCache(user);
      }
      catch
      {
        return false;
      }
      return true;
    }

    //GetEmailSubscriber
    public Email_Subscriber GetEmailSubscriber(long user_id, string user_type)
    {
      return dataContext.Email_Subscribers.Where(e => e.ID == user_id && e.T == user_type).FirstOrDefault();
    }
    #endregion

    #region registration / profile
    //CheckEmailInDB
    public void CheckEmailInDB(string p, out bool EmailExistsConfirmed, out bool EmailExistsNonConfirmed)
    {
      EmailExistsConfirmed = EmailExistsNonConfirmed = false;
      try
      {
        User usr = GetUserByEmail(p, false);
        if (usr == null) return;
        EmailExistsConfirmed = usr.IsConfirmed;
        EmailExistsNonConfirmed = !usr.IsConfirmed;
      }
      catch (Exception ex)
      {
        Logger.LogException("[p=" + p + "]", ex);
      }
    }
    //ConfirmUser
    public User ConfirmUser(string confirmCode)
    {
      User user;
      try
      {
        user = dataContext.Users.Where(P => P.ConfirmationCode == confirmCode.Trim()).FirstOrDefault();
        if (user == null) throw new Exception("Confirmation code not valid");
        user.IsConfirmed = true;
        SubmitChanges();
        UpdateUserCache(user);
      }
      catch (Exception ex)
      {
        Logger.LogException("[confirmCode=" + confirmCode + "]", ex);
        return null;
      }
      return user;
    }
    //ActivateUser
    public bool ActivateUser(User user)
    {
      try
      {
        if (user == null) throw new Exception("The user doesn't exists");
        if (user.Status == (byte)Consts.UserStatus.Locked || user.Status == (byte)Consts.UserStatus.Inactive) throw new Exception("User is not pending");
        user.Status = (byte)Consts.UserStatus.Active;
        user.FailedAttempts = 0;
        SubmitChanges();
        UpdateUserCache(user);
      }
      catch (Exception ex)
      {
        Logger.LogException("[user_id=" + user != null ? user.ID.ToString() : "null" + "]", ex);
        return false;
      }
      return true;
    }

    //GetAddressCard
    public AddressCard GetAddressCard(long addresscard_id, bool iscache)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETADDRESSCARD", new object[] { addresscard_id }, CachingExpirationTime.Hours_01);
      AddressCard result = CacheRepository.Get(dco) as AddressCard;
      if (result != null && iscache) return result;
      result = dataContext.spSelect_AddressCard(addresscard_id).SingleOrDefault();
      if (result != null)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }
    //GetRegisterInfo
    public RegisterInfo GetRegisterInfo(long user_id)
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETREGISTERINFO", new object[] { user_id }, CachingExpirationTime.Minutes_30);
      RegisterInfo info = CacheRepository.Get(dco) as RegisterInfo;
      if (info != null) return info;
      info = new RegisterInfo();
      try
      {
        User user = GetUser(user_id, true);
        if (user == null) throw new Exception("The user doesn't exist");

        info.ID = user.ID;
        info.Login = user.Login;
        info.Email = user.Email;
        info.Password = user.Password;
        info.ConfirmPassword = user.Password;
        info.ConfirmEmail = user.Email;
        info.RecieveNewsUpdates = !user.NotRecieveNewsUpdates;
        info.RecieveWeeklySpecials = !user.NotRecieveWeeklySpecials;
        info.Reference = user.Reference;
        info.BillingLikeShipping = user.BillingLikeShipping;
        info.NotPasswordReset = user.NotPasswordReset;
        info.IsModifyed = user.IsModifyed;
        info.IsConsignorShip = user.IsConsignorShip;

        AddressCard BillingCard = user.Billing_AddressCard_ID.GetValueOrDefault(0)>0 ? GetAddressCard(user.Billing_AddressCard_ID.GetValueOrDefault(-1), true) : new AddressCard();
        info.BillingFirstName = BillingCard.FirstName;
        info.BillingLastName = BillingCard.LastName;
        info.BillingMIName = BillingCard.MiddleName;
        info.BillingFax = BillingCard.Fax;
        info.BillingAddress1 = BillingCard.Address1;
        info.BillingAddress2 = BillingCard.Address2;
        info.BillingCity = BillingCard.City;
        info.BillingZip = BillingCard.Zip;
        info.BillingState = BillingCard.State;
        info.BillingPhone = BillingCard.HomePhone;
        info.BillingWorkPhone = BillingCard.WorkPhone;
        info.BillingCountry = BillingCard.Country_ID;

        AddressCard ShippingCard = user.Shipping_AddressCard_ID.GetValueOrDefault(0)>0 ? GetAddressCard(user.Shipping_AddressCard_ID.GetValueOrDefault(-1), true) : new AddressCard();
        info.ShippingFirstName = (user.BillingLikeShipping) ? BillingCard.FirstName : ShippingCard.FirstName;
        info.ShippingLastName = (user.BillingLikeShipping) ? BillingCard.LastName : ShippingCard.LastName;
        info.ShippingMIName = (user.BillingLikeShipping) ? BillingCard.MiddleName : ShippingCard.MiddleName;
        info.ShippingFax = (user.BillingLikeShipping) ? BillingCard.Fax : ShippingCard.Fax;
        info.ShippingAddress1 = (user.BillingLikeShipping) ? BillingCard.Address1 : ShippingCard.Address1;
        info.ShippingAddress2 = (user.BillingLikeShipping) ? BillingCard.Address2 : ShippingCard.Address2;
        info.ShippingCity = (user.BillingLikeShipping) ? BillingCard.City : ShippingCard.City;
        info.ShippingZip = (user.BillingLikeShipping) ? BillingCard.Zip : ShippingCard.Zip;
        info.ShippingState = (user.BillingLikeShipping) ? BillingCard.State : ShippingCard.State;
        info.ShippingWorkPhone = (user.BillingLikeShipping) ? BillingCard.WorkPhone : ShippingCard.WorkPhone;
        info.ShippingPhone = (user.BillingLikeShipping) ? BillingCard.HomePhone : ShippingCard.HomePhone;
        info.ShippingCountry = (user.BillingLikeShipping) ? BillingCard.Country_ID : ShippingCard.Country_ID;

        dco.Data = info;
        CacheRepository.Add(dco);
      }
      catch (Exception ex)
      {
        Logger.LogException("[user_id=" + user_id.ToString() + "]", ex);
        return null;
      }
      return info;
    }
    //AddUser
    public User AddUser(RegisterInfo info)
    {
      User user = new User();
      try
      {
        ICountryRepository repCountry = new CountryRepository(dataContext, CacheRepository);

        State BillingState = repCountry.GetStateByCode(info.BillingState);
        State ShippingState;
        long state1 = (BillingState == null) ? 0 : BillingState.ID;
        long state2;
        if (!info.BillingLikeShipping)
        {
          ShippingState = repCountry.GetStateByCode(info.ShippingState.ToLower());
          state2 = (ShippingState == null) ? 0 : ShippingState.ID;
        }
        else state2 = state1;

        AddressCard BillingCard = new AddressCard();
        dataContext.AddressCards.InsertOnSubmit(BillingCard);
        BillingCard.Address1 = info.BillingAddress1;
        BillingCard.Address2 = info.BillingAddress2;
        BillingCard.Zip = info.BillingZip;
        BillingCard.City = info.BillingCity;
        BillingCard.FirstName = info.BillingFirstName;
        BillingCard.LastName = info.BillingLastName;
        BillingCard.MiddleName = info.BillingMIName;
        BillingCard.State = info.BillingState;
        BillingCard.Country_ID = info.BillingCountry;
        BillingCard.HomePhone = info.BillingPhone;
        BillingCard.WorkPhone = info.BillingWorkPhone;
        BillingCard.Fax = info.BillingFax;
        BillingCard.State_ID = state1;

        AddressCard ShippingCard = new AddressCard();
        dataContext.AddressCards.InsertOnSubmit(ShippingCard);
        ShippingCard.FirstName = (info.BillingLikeShipping) ? info.BillingFirstName : info.ShippingFirstName;
        ShippingCard.LastName = (info.BillingLikeShipping) ? info.BillingLastName : info.ShippingLastName;
        ShippingCard.MiddleName = (info.BillingLikeShipping) ? info.BillingMIName : info.ShippingMIName;
        ShippingCard.Address1 = (info.BillingLikeShipping) ? info.BillingAddress1 : info.ShippingAddress1;
        ShippingCard.Address2 = (info.BillingLikeShipping) ? info.BillingAddress2 : info.ShippingAddress2;
        ShippingCard.City = (info.BillingLikeShipping) ? info.BillingCity : info.ShippingCity;
        ShippingCard.State = (info.BillingLikeShipping) ? info.BillingState : info.ShippingState;
        ShippingCard.Country_ID = (info.BillingLikeShipping) ? info.BillingCountry : info.ShippingCountry;
        ShippingCard.HomePhone = (info.BillingLikeShipping) ? info.BillingPhone : info.ShippingPhone;
        ShippingCard.WorkPhone = (info.BillingLikeShipping) ? info.BillingWorkPhone : info.ShippingWorkPhone;
        ShippingCard.Fax = (info.BillingLikeShipping) ? info.BillingFax : info.ShippingFax;
        ShippingCard.Zip = (info.BillingLikeShipping) ? info.BillingZip : info.ShippingZip;
        ShippingCard.State_ID = (info.BillingLikeShipping) ? state1 : state2;

        dataContext.Users.InsertOnSubmit(user);
        user.Login = info.Login.Trim();
        user.Email = info.Email;
        user.ConfirmationCode = Guid.NewGuid().ToString().Replace("-", "");
        user.Password = info.Password;
        user.NotRecieveNewsUpdates = !info.RecieveNewsUpdates;
        user.NotRecieveWeeklySpecials = !info.RecieveWeeklySpecials;
        user.DateIN = DateTime.Now;
        user.UserType = (byte)Consts.UserTypes.Buyer;
        user.IsConfirmed = false;        
        user.Reference = info.Reference;
        user.BillingLikeShipping = info.BillingLikeShipping;
        user.Status = (byte)Consts.UserStatus.Pending;
        user.Notes = string.Empty;
        user.IsModifyed = true;
        user.CommRate_ID = Consts.DefaultCommissionRate;
        user.NotRecievingBidConfirmation = user.NotRecievingLotClosedNotice = user.NotRecievingLotSoldNotice = user.NotRecievingOutBidNotice = false;

        user.AddressCard_Billing = BillingCard;
        user.AddressCard_Shipping = ShippingCard;

        SubmitChanges();
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        user = null;
      }
      return user;
    }
    
    //UpdateUser
    public User UpdateUser(RegisterInfo info)
    {
      User usr = GetUser(info.ID, false);
      try
      {
        if (usr == null) throw new Exception("The user doesn't exist");
        if (usr.Email != info.Email)
          usr.ConfirmationCode = Guid.NewGuid().ToString().Replace("-", "");
        usr.Email = info.Email;
        usr.Password = info.Password;
        usr.NotRecieveNewsUpdates = !info.RecieveNewsUpdates;
        usr.NotRecieveWeeklySpecials = !info.RecieveWeeklySpecials;
        usr.Reference = info.Reference;
        usr.BillingLikeShipping = info.BillingLikeShipping;
        usr.IsModifyed = true;
        usr.NotPasswordReset = false;
        
        info.NotPasswordReset = false;
        info.IsModifyed = true;

        ICountryRepository repCountry = new CountryRepository(dataContext,CacheRepository);
        State BillingState = repCountry.GetStateByCode(info.BillingState);
        State ShippingState;
        long state1 = (BillingState == null) ? 0 : BillingState.ID;
        long state2;
        if (!info.BillingLikeShipping)
        {
          ShippingState = repCountry.GetStateByCode(info.ShippingState.ToLower());
          state2 = (ShippingState == null) ? 0 : ShippingState.ID;
        }
        else state2 = state1;

        IAddressCard acB = GetAddressCard(usr.Billing_AddressCard_ID.GetValueOrDefault(-1), false);
        if (acB == null)
        {
          acB = new AddressCard();
          dataContext.AddressCards.InsertOnSubmit(acB as AddressCard);
          usr.AddressCard_Billing = acB as AddressCard;
        }
        acB.FirstName = info.BillingFirstName;
        acB.LastName = info.BillingLastName;
        acB.MiddleName = info.BillingMIName;
        acB.Address1 = info.BillingAddress1;
        acB.Address2 = info.BillingAddress2;
        acB.City = info.BillingCity;
        acB.State = String.IsNullOrEmpty(info.BillingState) ? String.Empty : info.BillingState;
        acB.Zip = info.BillingZip;
        acB.Country_ID = info.BillingCountry;        
        acB.HomePhone = info.BillingPhone;
        acB.WorkPhone = info.BillingWorkPhone;
        acB.State_ID = state1;
        acB.Fax = info.BillingFax;

        IAddressCard acS = usr.Shipping_AddressCard_ID.HasValue ? GetAddressCard(usr.Shipping_AddressCard_ID.GetValueOrDefault(-1), false) : GetAddressCard(usr.Billing_AddressCard_ID.GetValueOrDefault(-1), false);
        if (acS == null)
        {
          acS = new AddressCard();
          dataContext.AddressCards.InsertOnSubmit(acS as AddressCard);
          usr.AddressCard_Shipping = acS as AddressCard;
        }
        acS.FirstName = (usr.BillingLikeShipping) ?info.BillingFirstName : info.ShippingFirstName;
        acS.LastName = (usr.BillingLikeShipping) ?info.BillingLastName : info.ShippingLastName;
        acS.MiddleName = (usr.BillingLikeShipping) ?info.BillingMIName : info.ShippingMIName;
        acS.Address1 = (usr.BillingLikeShipping) ? info.BillingAddress1 : info.ShippingAddress1;
        acS.Address2 = (usr.BillingLikeShipping) ? info.BillingAddress2 : info.ShippingAddress2;
        acS.City = (usr.BillingLikeShipping) ? info.BillingCity : info.ShippingCity;
        acS.State = (usr.BillingLikeShipping) ? info.BillingState : info.ShippingState;
        acS.State = String.IsNullOrEmpty(acS.State) ? String.Empty : acS.State;        
        acS.Zip = (usr.BillingLikeShipping) ? info.BillingZip : info.ShippingZip;
        acS.Country_ID = (usr.BillingLikeShipping) ? info.BillingCountry : info.ShippingCountry;
        acS.HomePhone = (usr.BillingLikeShipping) ? info.BillingPhone : info.ShippingPhone;
        acS.WorkPhone = (usr.BillingLikeShipping) ? info.BillingWorkPhone : info.ShippingWorkPhone;
        acS.State_ID = (usr.BillingLikeShipping) ? state1 : state2;
        acB.Fax = (usr.BillingLikeShipping) ? info.BillingFax : info.ShippingFax;

        SubmitChanges();
        UpdateUserCache(usr);

        DataCacheObject dco = new DataCacheObject(DataCacheType.ACTIVITY, DataCacheRegions.USERS, "GETREGISTERINFO", new object[] { usr.ID }, CachingExpirationTime.Minutes_30, info);
        CacheRepository.Put(dco);
        dco.Method = "GETADDRESSCARD";
        dco.Params = new object[] {acB.ID};
        dco.Data = acB;
        CacheRepository.Put(dco);
        dco.Params = new object[] { acS.ID };
        dco.Data = acS;
        CacheRepository.Put(dco);
        //CacheRepository.Update(CacheDataKeys.USER_GETREGISTERINFO, info, new object[] { usr.ID });
        //CacheRepository.Update(CacheDataKeys.USER_GETADDRESSCARD, acB, new object[] { acB.ID });
        //CacheRepository.Update(CacheDataKeys.USER_GETADDRESSCARD, acS, new object[] { acS.ID });
      }
      catch (Exception ex)
      {
        Logger.LogException("[registerinfo>login: "+info.Login+"]", ex);
        return null;
      }
      return usr;
    }


    #endregion

    #region forgot password / resend confirmation
    //GenerateNewConfirmationCode
    public bool GenerateNewConfirmationCode(User user)
    {      
      try
      {
        if (user == null) throw new Exception("The user doesn't exist");
        Guid newCode = Guid.NewGuid();
        user.ConfirmationCode = newCode.ToString().Replace("-", "");
        SubmitChanges();
        UpdateUserCache(user);
      }
      catch (Exception ex)
      {
        Logger.LogException("[user_id=" + user != null ? user.ID.ToString() : "null" + "]", ex);
        return false;
      }
      return true;
    }
    //SetNewUserPassword
    public User SetNewUserPassword(string email)
    {
      User user = GetUserByEmail(email,false);
      try
      {
        if (user == null) throw new Exception("The user doesn't exist");
        PasswordGenerator generator = new PasswordGenerator(6, 7);
        user.Password = generator.Generate();
        user.NotPasswordReset = true;
        SubmitChanges();
        UpdateUserCache(user);
      }
      catch (Exception ex)
      {
        Logger.LogException("[email=" + email + "]", ex);
        return null;
      }
      return user;
    }    
    //CheckChangePAssword
    public bool CheckChangePassword(long user_id, string new_password)
    {
      return dataContext.Users.Where(U => U.ID == user_id && U.Password == new_password && U.NotPasswordReset).Count() == 0;      
    }
    #endregion

    //UpdateEmailSettings
    public bool UpdateEmailSettings(long user_id, bool NotRecievingWeeklySpecials, bool NotRecievingNewsUpdates, bool NotRecievingBidConfirmation, bool NotRecievingOutBidNotice, bool NotRecievingLotSoldNotice, bool NotRecievingLotClosedNotice)
    {
      try
      {
        User user = GetUser(user_id, false);
        user.NotRecieveWeeklySpecials = NotRecievingWeeklySpecials;
        user.NotRecieveNewsUpdates = NotRecievingNewsUpdates;
        user.NotRecievingBidConfirmation = NotRecievingBidConfirmation;
        user.NotRecievingOutBidNotice = NotRecievingOutBidNotice;
        user.NotRecievingLotSoldNotice = NotRecievingLotSoldNotice;
        user.NotRecievingLotClosedNotice = NotRecievingLotClosedNotice;
        SubmitChanges();
        UpdateUserCache(user);
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        return false;
      }
      return true;
    }

    #region personal shopper
    //GetPersonalShopperForUser
    public PersonalShopperItem GetPersonalShopperForUser(long user_id)
    {
      return dataContext.PersonalShopperItems.Where(P => P.User_ID == user_id).SingleOrDefault();
    }
    //UpdatePersonalShopper
    public bool UpdatePersonalShopper(long id, DateTime date, bool IsActive, string key1, string key2, string key3, string key4, string key5, long? cat1, long? cat2, long? cat3, long? cat4, long? cat5, bool ishtml, long user_id)
    {
      try
      {
        PersonalShopperItem psi = GetPersonalShopperForUser(user_id);
        if (psi == null)
        {
          psi = new PersonalShopperItem();          
          dataContext.PersonalShopperItems.InsertOnSubmit(psi);
        }
        psi.Keyword1 = key1;
        psi.Keyword2 = key2;
        psi.Keyword3 = key3;
        psi.Keyword4 = key4;
        psi.Keyword5 = key5;
        psi.DateExpires = date;
        psi.IsActive = IsActive;
        psi.Category_ID1 = cat1;
        psi.Category_ID2 = cat2;
        psi.Category_ID3 = cat3;
        psi.Category_ID4 = cat4;
        psi.Category_ID5 = cat5;
        psi.User_ID = user_id;
        psi.IsHTML = ishtml;
        SubmitChanges();
      }
      catch (Exception ex)
      {
        Logger.LogException("[user_id=" + user_id.ToString() + "]", ex);
        return false;
      }
      return true;
    }   
    #endregion
  }
}
