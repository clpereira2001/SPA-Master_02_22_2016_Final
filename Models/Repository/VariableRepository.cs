using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Vauction.Utils;
using Vauction.Utils.Perfomance;
using Vauction.Utils.Lib;

namespace Vauction.Models
{
  public class VariableRepository : IVariableRepository
  {
    #region init
    private VauctionDataContext dataContext;
    private ICacheDataProvider CacheRepository;

    public VariableRepository(VauctionDataContext dataContext, ICacheDataProvider cacheDataProvider)
    {
      this.dataContext = dataContext;
      CacheRepository = cacheDataProvider;
    }
    #endregion

    //GetVariables
    private List<Variable> GetVariables()
    {
      DataCacheObject dco = new DataCacheObject(DataCacheType.REFERENCE, DataCacheRegions.COUNTRIES, "GETVARIABLES", new object[] { }, CachingExpirationTime.Days_01);
      List<Variable> result = CacheRepository.Get(dco) as List<Variable>;
      if (result != null) return result;
      result = dataContext.spSelect_Variable().ToList();
      if (result.Count()>0)
      {
        dco.Data = result;
        CacheRepository.Add(dco);
      }
      return result;
    }

    //GetInsurance
    public decimal GetInsurance()
    {
      return GetVariables().SingleOrDefault(V => V.Name == "InsuranceCoefficient").Value;
    }

    //GetSalesTaxRate
    public decimal GetSalesTaxRate()
    { 
      return GetVariables().SingleOrDefault(V => V.Name == "SalesTaxRate").Value; 
    }


    //TrackEmail
    public void TrackEmail(string IP, string event_id, long? user_id, string type)
    {
      try
      {
        dataContext.spTracking_EventEmail(IP, event_id, user_id, type);
      }
      catch (Exception ex)
      {
        Logger.LogException(String.Format("[Email tracking: event_id={0}, user_id={1}, type={2}, IP={3}]", event_id, user_id.GetValueOrDefault(-1), type, IP), ex);
      }
    }
    
    //TrackForwardingURL
    public void TrackForwardingURL(string IP, string event_id, string url, long? user_id, string type)
    {
      try
      {
        dataContext.spTracking_ForwardingURL(IP, event_id, url, user_id, type);
      }
      catch (Exception ex)
      {
        Logger.LogException(String.Format("F. URL tracking: event_id={0}, url={4}, user_id={1}, type={2}, IP={3}]", event_id, user_id.GetValueOrDefault(-1), type, IP, url), ex);
      }
    }

    //GetEmail
    public Email GetEmail(long email_id)
    {
      return dataContext.Emails.Where(e => e.ID == email_id).FirstOrDefault();
    }

    //GetAnnouncementEmail
    public string GetAnnouncementEmail(long email_id, long user_id, string type, string fullname, string username)
    {
      StringBuilder sb = new StringBuilder(File.ReadAllText(AppHelper.GetHTMLEmailTemplateFile(email_id)));
      sb.Replace("{{first_last_name}}", fullname);
      sb.Replace("{{user_name}}", username);
      sb.Replace("{{unsubscribe_link}}", String.Format("{0}/Home/EmailAlertsUnsubscribeSuccess?id={1}&t={2}", Consts.ProtocolSitePort, user_id, type));
      return sb.ToString();
    }

    //TestBiddingResult
    public void TestBiddingResult(long auction_id, BidCurrent current, BidCurrent opponent, BidCurrent winner, byte result, long etickes, long emilisec, ref long? parent_id)
    {
      if (opponent!=null)
      dataContext.spTest_BiddingResult(auction_id, current.ID, current.User_ID, current.Amount, current.MaxBid,
                                       current.IsProxy, current.DateMade, opponent.ID, opponent.User_ID, opponent.Amount,
                                       opponent.MaxBid, opponent.IsProxy, opponent.DateMade, result, winner.User_ID,
                                       winner.Amount, winner.MaxBid, etickes, emilisec, ref parent_id);
      else
        dataContext.spTest_BiddingResult(auction_id, current.ID, current.User_ID, current.Amount, current.MaxBid,
                                       current.IsProxy, current.DateMade, null, null, null, null, null, null, result, winner.User_ID,
                                       winner.Amount, winner.MaxBid, etickes, emilisec, ref parent_id);
    }
    
    //Test_ResetBiddingStatistics
    public void Test_ResetBiddingStatistics(long event_id)
    {
      dataContext.spTest_ResetBiddingResults(event_id);
    }

  }
}
