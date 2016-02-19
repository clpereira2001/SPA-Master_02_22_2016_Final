namespace Vauction.Models
{
  public interface IVariableRepository
  {
    decimal GetInsurance();
    decimal GetSalesTaxRate();
    void TrackEmail(string IP, string event_id, long? user_id, string type);
    void TrackForwardingURL(string IP, string event_id, string url, long? user_id, string type);
    Email GetEmail(long email_id);
    string GetAnnouncementEmail(long email_id, long user_id, string type, string fullname, string username);

    void TestBiddingResult(long auction_id, BidCurrent current, BidCurrent opponent, BidCurrent winner, byte result,
                           long etickes, long emilisec, ref long? parent_id);
    void Test_ResetBiddingStatistics(long event_id);
  }
}
