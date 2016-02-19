using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Models.CustomClasses;

namespace Vauction.Models
{
  public interface IInvoiceRepository
  {
    Invoice GetInvoice(long invoice_id);
    List<Invoice> GetUserInvoicesDOW(long user_id);
    List<UserInvoice> GetPastUserInvoices(long user_id);
    UserInvoice GetUserInvoice(long userinvoice_id);
    decimal GetDepositAmount(long user_id, long event_id);
    long AddDeposit(decimal amount, long auction_id, long event_id, long user_id, long paymentType_id, string TransactionId, Address billing, string CCNum);
    InvoiceDetail GetInvoiceDetail(long invoice_id);
    bool AddInvoicePayment(long auction_id, long user_id, int quantity, long event_id, string address, string ac, string cc, string city, string notes, long paymenttype_id, string shippingaddress, string description, string zip, string state, decimal buyprice, decimal bp, decimal shipping, decimal insurance, decimal tax, bool ispickup, bool isdow, ref long? invoice_id);
    List<InvoiceDetail> GetEventUnpaidInvoicesForUser(long user_id);
    List<InvoiceDetail> GetEventInvoicesForUserByIDs(long user_id, List<long> invoice_ids);
    decimal GetDepositAmount(long user_id);
    decimal GetDepositAmountExceptCurrent(long user_id);
    List<Deposit> SetDepositAmount(long user_id, ref decimal Amount);
    long AddPayment(decimal amount, long user_id, long userinvoice_id, long paymentType_id, string transactionId, string shaddress, Address billing, string CCNum, string notes);
    List<Invoice> GetInvoicesByIDetails(List<long> invoice_ids);
    int GetUnpaidInvoicesCount(long user_id);
    bool RefundingDeposits(long user_id);
    bool IsDiscountValide(string discount_code, long user_id, long? discount_id);
    JsonExecuteResult ValidateDiscount(string discount_code, long user_id, long? discount_id);
    Discount GetValidDiscount(long user_id, string couponCode, long? discount_id);
    bool ValidDiscountForUserID(long user_id);
    bool AddUsedDiscount(long user_id, long discount_id);
    bool SubmitChanges();






    // NOT DONE

    
    //IEnumerable<Invoice> GetList(int start, int count, string order, string dir, Int64? eventQuery, string userQuery, Int64? lotNumber);
    //int GetTotalCount(Int64? eventQuery, string userQuery, Int64? lotNumber);
    //void UpdateInvoices(string Login, long EventID, bool MarkPaid, bool DeleteShipping, decimal Discount, string DiscountReasonId);
    
    //int GetInvoicesForUserForEventCount(long EventID, string Login);
    //IEnumerable<Invoice> GetInvoicesForUserForEventList(long EventID, string Login);
    //Payment GetPaymentForCIPReport(string Login, long EventID);
    //void SetPaymentsForInvoices(long[] invoices);
    //
    //IEnumerable<Deposit> GetDepositList(long user_id);
    //void MakePaymentsForInvoices(string Login, long EventID, long[] Lots, decimal Discount, string DiscountReasonId);
    //int GetDiscountReasonQuantity();
    //IEnumerable<DiscountReason> GetDiscountReasonList();
    //void UpdateDOWInvoices();    
    
    //int GetDOWQuantityInInvoices(long AuctionID, long UserId);
    
    //void RefundingDeposits_WS(IUser user, string email_message, string siteURL, System.Diagnostics.EventLog el);
    //void DeleteInvoice(long invoice_id);
    //void CheckAndAddConsignment(long user_id, long event_id);
    //void PlaceDOWOrder(long id, int Quantity, long userid);
    //void RecalculateInvoiceForDOW(Invoice invoice, int quantity, User usr);    
    //List<UserInvoice> GetUserInvoices(long userinvoice_id);     
    //Invoice GetInvoice(long auction_id, long user_id);
  }
}