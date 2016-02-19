using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Controllers;

namespace Vauction.Utils.Authorized.NET
{
  public class SoapAPIUtilities
  { 
    private static CustomerProfileWS.MerchantAuthenticationType m_auth = null;
    private static CustomerProfileWS.Service service = null;
    public static bool IsTestingMode { get; set; }

    static SoapAPIUtilities()
    {
      //MERCHANT_NAME = bs.Config.GetProperty("AN_MerchantName");
      BaseController bs = new BaseController();
      IsTestingMode = Convert.ToBoolean(bs.Config.GetProperty("AN_TestingMode"));
    }

    public static CustomerProfileWS.MerchantAuthenticationType MerchantAuthentication
    {
      get
      {
        if (m_auth == null)
        {
          BaseController bs = new BaseController();
          m_auth = new CustomerProfileWS.MerchantAuthenticationType();
          m_auth.name = bs.Config.GetProperty("AN_MerchantName");
          m_auth.transactionKey = bs.Config.GetProperty("AN_TransactionKey");
        }
        return m_auth;
      }
    }

    public static CustomerProfileWS.Service Service
    {
      get
      {
        if (service == null)
        {
          BaseController bs = new BaseController();
          service = new CustomerProfileWS.Service();
          service.Url = bs.Config.GetProperty("AN_APIURL");
        }
        return service;
      }
    }
  }
}
