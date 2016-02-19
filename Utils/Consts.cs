using System;
using System.Collections.Generic;
using System.Web;
using Vauction.Utils.Perfomance;

namespace Vauction.Utils
{
  public class EnumHelper<T, E>
  {
    public EnumHelper(T text, E value)
    {
      this.Text = text;
      this.Value = value;
    }

    public T Text { get; set; }
    public E Value { get; set; }
  }

  public class EnumFromString<T>
  {
    public static T GetValueByString(string value)
    {
      T retVal = (T)Enum.GetValues(typeof(T)).GetValue(0);
      foreach (T oneType in Enum.GetValues(typeof(T)))
      {
        if (oneType.ToString() == value)
          retVal = oneType;
      }
      return retVal;
    }
  }

  public static class Consts
  {
    public const int FormsAuthenticationTicketTime = 90;
    public static TimeSpan AuthorizeStatusCheckTime = new TimeSpan(1, 0, 0);

    public const int BottomDepositLimit = 2500;
    public const int TopDepositLimit = 6000;

    public const int BottomDepositLimitAmount = 2500;
    public const int TopDepositLimitAmount = 10000;

    public enum AuctonViewMode
    {
      List = 0,
      Grid = 1
    }

    public enum PaymentState
    {
      Declined = 1,
      Accepted = 2
    }

    public enum PaymentType
    {
      Undefined = 0,
      CreditCard = 1,
      Paypal = 2,
      WALKIN = 3,
      W_PayPal = 4,
      W_CreditCard = 5
    }

    public enum CategorySortFields
    {
      Title,
      Price,
      Description,
      Lot,
      CurrentBid,
      Bids,
      QTY
    }

    [Serializable]
    public enum OrderByValues
    {
      ascending,
      descending
    }

    public enum AuctionType
    {
      Normal = 1,
      Dutch = 2,
      DealOfTheWeek = 3
    }

    public enum AuctionStatus : byte
    {
      Pending = 1,
      Open = 2,
      Closed = 3,
      Locked = 4
    }

    public enum UserTypes : byte
    {
      Root = 1,
      Admin = 2,
      HouseBidder = 3,
      Buyer = 4,
      SellerBuyer = 5,
      Seller = 6,
      Reviewer = 7
    }

    public enum UserStatus : byte
    {
      Pending = 1, //-- User has not confirmed his email with us      
      Active = 2, // -- User had confirmed his email and has full access to his access level            
      Inactive = 3, // -- User is no longer in our system opted out
      Locked = 4 //-- Set by the admin / User did not pay his bill or any reason the admin chooses to lock him from using the system 
    }

    public enum DiscountAssignedType : byte
    {
      Undefined = 0,
      Shipping = 1,
      BuyerPremium = 2,
      TotalOrder = 9
    }

    public static byte DefaultCommissionRate
    {
      get { return 5; }
    }

    public static byte GetUserStatusByType(string status)
    {
      if (status == UserStatus.Locked.ToString())
        return Convert.ToByte(UserStatus.Locked);
      if (status == UserStatus.Active.ToString())
        return Convert.ToByte(UserStatus.Active);
      if (status == UserStatus.Locked.ToString())
        return Convert.ToByte(UserStatus.Locked);
      if (status == UserStatus.Pending.ToString())
        return Convert.ToByte(UserStatus.Pending);
      return Convert.ToByte(UserStatus.Pending);
    }
    public static byte GetStatusByType(string status)
    {
      if (status == AuctionStatus.Closed.ToString())
        return Convert.ToByte(AuctionStatus.Closed);
      if (status == AuctionStatus.Open.ToString())
        return Convert.ToByte(AuctionStatus.Open);
      if (status == AuctionStatus.Locked.ToString())
        return Convert.ToByte(AuctionStatus.Locked);
      if (status == AuctionStatus.Pending.ToString())
        return Convert.ToByte(AuctionStatus.Pending);
      return Convert.ToByte(AuctionStatus.Pending);
    }

    public static string GetPhonePartByFull(byte part, string phone)
    {
      if (phone.Length != 10 || phone.Length != 14)
      {
        return "";
      }
      if (phone.Length == 10)
      {
        phone += "    ";
      }
      switch (part)
      {
        case (1):
          return phone.Substring(0, 3);
        case (2):
          return phone.Substring(3, 3);
        case (3):
          return phone.Substring(6, 4);
        case (4):
          return phone.Substring(10, 4);

      }
      return "";
    }

    public static decimal GetIncrement(decimal price)
    {
      //if ((price > 0) && (price < 100)) return Convert.ToDecimal(1.00);
      //if ((price >= 100) && (price < 500)) return Convert.ToDecimal(5.00);
      //if ((price >= 500) && (price < 1000)) return Convert.ToDecimal(10.00);
      //if ((price >= 1000) && (price < 5000)) return Convert.ToDecimal(25.00);
      //if ((price >= 5000) && (price < 10000)) return Convert.ToDecimal(50.00);
      //if ((price >= 10000) && (price < 50000)) return Convert.ToDecimal(200.00);
      //if ((price >= 50000) && (price < 100000)) return Convert.ToDecimal(500.00);
      //if (price >= 100000) return Convert.ToDecimal(1000.00);
      //return 1;
      if ((price > 0) && (price < 100)) return Convert.ToDecimal(1.00);
      if ((price >= 100) && (price < 200)) return Convert.ToDecimal(5.00);
      if ((price >= 200) && (price < 1000)) return Convert.ToDecimal(10.00);
      if ((price >= 1000) && (price < 5000)) return Convert.ToDecimal(25.00);
      if ((price >= 5000) && (price < 10000)) return Convert.ToDecimal(50.00);
      if ((price >= 10000) && (price < 50000)) return Convert.ToDecimal(200.00);
      if (price >= 50000) return Convert.ToDecimal(500.00);
      return 1;
    }

    [Serializable]
    public enum CommissionLimitType
    {
      SellingPrice = 0,
      InCommission = 1
    }

    public static string GetStringByCommissionLimitType(CommissionLimitType type)
    {
      string retVal = string.Empty;
      switch (type)
      {
        case CommissionLimitType.SellingPrice: retVal = "of Selling Price";
          break;
        case CommissionLimitType.InCommission: retVal = "in Commission";
          break;

      }
      return retVal;
    }

    public static List<string> GetListOfCommissionLimitType()
    {
      List<string> retVal = new List<string>();
      foreach (CommissionLimitType type in Enum.GetValues(typeof(CommissionLimitType)))
      {
        retVal.Add(GetStringByCommissionLimitType(type));
      }
      return retVal;
    }
    public static CommissionLimitType GetCommissionLimitTypeByString(string typeStr)
    {
      foreach (CommissionLimitType type in Enum.GetValues(typeof(CommissionLimitType)))
      {
        if (typeStr == GetStringByCommissionLimitType(type))
          return type;
      }
      return CommissionLimitType.SellingPrice;
    }

    public static string GetImagePath(long auction_id, bool isweb)
    {
      return (string.Format((isweb) ? "{0}/{1}/{2}/" : @"{0}\{1}\{2}\", auction_id / 1000000, auction_id / 1000, auction_id));
    }

    public static string GetImagePathForUser(long user_id, long auctionlisting_id, bool isweb)
    {
      return (string.Format((isweb) ? "{0}/{1}" : @"{0}\{1}", user_id, auctionlisting_id));
    }

    public static bool IsSsl
    {
      get
      {
        string value = ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("IsSsl");
        return (!String.IsNullOrEmpty(value)) ? Convert.ToBoolean(value) : true;
      }
    }

    public static bool IsStandartPortsSsl
    {
      get
      {
        string value = ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("IsStandartPortsSsl");
        return (!String.IsNullOrEmpty(value)) ? Convert.ToBoolean(value) : true;
      }
    }

    public static string PortSsl
    {
      get
      {
        if (!IsSsl) return Port;
        string value = ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("PortSsl");
        return (!String.IsNullOrEmpty(value)) ? Convert.ToString(value) : "443";
      }
    }

    public static string Port
    {
      get
      {
        string value = ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("Port");
        return (!String.IsNullOrEmpty(value)) ? Convert.ToString(value) : "80";
      }
    }

    public static string SiteAddress
    {
      get
      {
        string value = ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("SiteAddress");
        return (!String.IsNullOrEmpty(value)) ? Convert.ToString(value) : "www.seizedpropertyauctions.com";
      }
    }

    public static int DropDownSize
    {
      get
      {
        string value = ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("DropdownSize");
        return (!String.IsNullOrEmpty(value)) ? Convert.ToInt32(value) : 104;
      }
    }

    public static bool IsShownOpenBidOne
    {
      get
      {
        string value = ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("IsShownOpenBidOne");
        return (!String.IsNullOrEmpty(value)) ? Convert.ToBoolean(value) : true;
      }
    }

    public static string Protocol
    {
      get
      {
        return IsSsl ? "https://" : "http://";
      }
    }

    public static string ProtocolSitePort
    {
      get { return IsStandartPortsSsl ? String.Format("{0}{1}", Protocol, SiteAddress) : String.Format("{0}{1}:{2}", Protocol, SiteAddress, PortSsl); }
    }

    public static int AuctionImageSize
    {
      get
      {
        string value = System.Configuration.ConfigurationManager.AppSettings["AuctionImageSize"];
        return (!String.IsNullOrEmpty(value)) ? Convert.ToInt32(value) : 700;
      }
    }

    public static int AuctionImageThumbnailSize
    {
      get
      {
        string value = System.Configuration.ConfigurationManager.AppSettings["AuctionImageThumbnailSize"];
        return (!String.IsNullOrEmpty(value)) ? Convert.ToInt32(value) : 96;
      }
    }

    public static string CompanyTitleName
    {
      get
      {
        string value = System.Configuration.ConfigurationManager.AppSettings["CompanyName"];
        return (!String.IsNullOrEmpty(value)) ? value : "---";
      }
    }

    public static string CompanyAddress
    {
      get
      {
        string value = System.Configuration.ConfigurationManager.AppSettings["CompanyAddress"];
        return (!String.IsNullOrEmpty(value)) ? value : "---";
      }
    }

    public static int PageSize
    {
      get
      {
        string value = ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("PageSize");
        return (!String.IsNullOrEmpty(value)) ? Convert.ToInt32(value) : 20;
      }
    }

    #region paypal constants
    public static string PayPalAPIUser
    {
      get { return ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("PayPalAPIUser"); }
    }
    public static string PayPalAPIPassword
    {
      get { return ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("PayPalAPIPassword"); }
    }
    public static string PayPalAPISignature
    {
      get { return ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("PayPalAPISignature"); }
    }
    public static string PayPalVersion
    {
      get { return ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("PayPalVersion"); }
    }
    public static string PayPalEndPointUrl
    {
      get { return ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("PayPalEndPointUrl"); }
    }
    public static string PayPalUrl
    {
      get { return ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("PayPalUrl"); }
    }
    #endregion

    //DataCachingTechnology
    public static DataCacheTechnology DataCachingTechnology
    {
      get
      {
        string result = ((Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("DataCachingTechnology");
        byte res = 0;
        return (!String.IsNullOrEmpty(result) && Byte.TryParse(result, out res)) ? (DataCacheTechnology)res : DataCacheTechnology.MEMORYOBJECT;
      }
    }

    //ProductName
    public static string ProductName
    {
      get
      {
        string result = ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("ProductName");
        return (!String.IsNullOrEmpty(result)) ? result : "DEFAULT";
      }
    }

    //ResourceHostName
    public static string ResourceHostName
    {
      get
      {
        string result = ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("ResourceHostName");
        return (!String.IsNullOrEmpty(result)) ? result : "";
      }
    }

    //ResourceHostName
    public static string ResourceSSLHostName
    {
      get
      {
        string result = ((Vauction.Configuration.IVauctionConfiguration)System.Configuration.ConfigurationManager.GetSection("Vauction")).GetProperty("ResourceSSLHostName");
        return (!String.IsNullOrEmpty(result)) ? result : "";
      }
    }

    //UsersIPAddress
    public static string UsersIPAddress
    {
      get
      {
        //try
        //{
        //  string[] ip = (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.UserHostAddress ?? String.Empty).Split(':');
        //  return ip[0];
        //}
        //catch (Exception ex)
        //{
        //  Lib.Logger.LogException("UsersIPAddress", ex);
        //  return HttpContext.Current.Request.UserHostAddress;
        //}
        return HttpContext.Current.Request.UserHostAddress;
      }
    }

    //DateTimeEnd
    public static string DateTimeEnd
    {
      get { return "EST."; }
    }

    //NotNeedToCheckIP
    //public static bool NeedToCheckIP
    //{
    //  get
    //  {
    //    return !(HttpContext.Current != null && HttpContext.Current.Request.Browser.Capabilities.Contains("extra") &&
    //           HttpContext.Current.Request.Browser.Capabilities["extra"].ToString().Contains("AOLBuild"));
    //  }
    //}
  }
}