using System;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Text;
using Vauction.Models.CustomClasses;
using Vauction.Utils;
using Vauction.Utils.Autorization;
using Vauction.Utils.Lib;
using Vauction.Utils.Authorized.NET;
using Vauction.Utils.PayPal;
using Vauction.Utils.Helpers;
using Vauction.Utils.Perfomance;

namespace Vauction.Models
{
  public class InvoiceRepository : IInvoiceRepository
  {
    private VauctionDataContext dataContext;
    private ICacheDataProvider CacheRepository;

    public InvoiceRepository(VauctionDataContext dataContext, ICacheDataProvider cacheDataProvider)
    {
      this.dataContext = dataContext;
      CacheRepository = cacheDataProvider;
    }

    public bool SubmitChanges()
    {
      bool result = true;
      try
      {
        dataContext.SubmitChanges();
      }
      catch (ChangeConflictException e)
      {
        Logger.LogWarning(e.Message);
        foreach (ObjectChangeConflict occ in dataContext.ChangeConflicts)
        {
          try
          {
            occ.Resolve(RefreshMode.KeepCurrentValues);
          }
          catch
          {
            result = false;
          }
        }
      }
      return result;
    }

    //GetInvoice
    public Invoice GetInvoice(long invoice_id)
    {
      return dataContext.Invoices.SingleOrDefault(I => I.ID == invoice_id);
    }

    //GetUserInvoiceDOW
    public List<Invoice> GetUserInvoicesDOW(long user_id)
    {
      return dataContext.Invoices.Where(C => C.User_ID == user_id && C.Consignments_ID == null && C.UserInvoices_ID == null).ToList<Invoice>();
    }

    //GetPastUserInvoices
    public List<UserInvoice> GetPastUserInvoices(long user_id)
    {
      return dataContext.UserInvoices.Where(C => C.User_ID == user_id && (C.Invoices.Where(I2 => I2.Status == 1).Count() > 0 && C.Invoices.Sum(A => A.Amount) > 0) && C.Event.IsViewable).ToList().OrderByDescending(I => I.Event.DateEnd).ToList();
    }

    //GetUserInvoice
    public UserInvoice GetUserInvoice(long userinvoice_id)
    {
      return dataContext.UserInvoices.SingleOrDefault(C => C.ID == userinvoice_id);
    }

    //GetDepositAmount
    public decimal GetDepositAmount(long user_id, long event_id)
    {
      return dataContext.fUser_GetDeposit(user_id, event_id).GetValueOrDefault(0);
    }

    //AddDeposit
    public long AddDeposit(decimal amount, long auction_id, long event_id, long user_id, long paymentType_id, string TransactionId, Address billing, string CCNum)
    {
      long res = -1;
      try
      {
        PaymentType pt = dataContext.PaymentTypes.SingleOrDefault(type => type.ID == paymentType_id) ?? new PaymentType();
        Deposit dep = new Deposit
                        {
                          Amount = amount,
                          Auction_ID = auction_id,
                          DatePaid = DateTime.Now,
                          Event_ID = event_id,
                          User_ID = user_id,
                          PaymentType = String.Format("Payment: {0}", pt.Title),
                          AuthCode = TransactionId,
                          Addr = String.Format("{0} {1}", billing.Address_1, billing.Address_2),
                          City = billing.City,
                          Street = billing.State,
                          Zip = billing.Zip
                        };
        dep.AuthCode = TransactionId;
        dep.Num = CCNum;
        dep.IsRefunded = false;
        dep.PaymentType_Id = paymentType_id;
        dep.Country_Id = billing.Country_ID;
        dep.DepositAmount = dep.Amount;
        dataContext.Deposits.InsertOnSubmit(dep);
        res = SubmitChanges() ? dep.ID : -1;
      }
      catch (Exception ex)
      {
        Logger.LogException(String.Format("[user_id={0}; event_id={1}; amount={2}", user_id, event_id, auction_id), ex);
        res = -1;
      }
      return res;
    }

    //GetInvoiceDetail
    public InvoiceDetail GetInvoiceDetail(long invoice_id)
    {
      return (from p in dataContext.spInvoice_Detail(invoice_id)
              select new InvoiceDetail
              {
                Amount = p.Amount,
                AuctionType = p.AuctionType_ID,
                BuyerPremium = p.BuyerPremium.GetValueOrDefault(0),
                DateCreated = p.DateCreated,
                Discount = p.Discount.GetValueOrDefault(0),
                Insurance = p.Insurance.GetValueOrDefault(0),
                Invoice_ID = p.Invoice_ID,
                LinkParams = new LinkParams { ID = p.Auction_ID, Lot = p.Lot, Title = p.Title, Event_ID = p.Event_ID, EventTitle = p.EventTitle, EventCategory_ID = p.EventCategory_ID, CategoryTitle = p.Category },
                Quantity = p.Quantity,
                Shipping = p.Shipping.GetValueOrDefault(0),
                Tax = p.Tax.GetValueOrDefault(0),
                Total = p.TotalDue.GetValueOrDefault(0),
                IsConsignorShip = p.IsConsignorShip,
                UserInvoice_ID = p.UserInvoices_ID
              }).FirstOrDefault();
    }

    //AddInvoicePayment
    public bool AddInvoicePayment(long auction_id, long user_id, int quantity, long event_id, string address, string ac, string cc, string city, string notes, long paymenttype_id, string shippingaddress, string description, string zip, string state, decimal buyprice, decimal bp, decimal shipping, decimal insurance, decimal tax, bool ispickup, bool isdow, ref long? invoice_id)
    {
      try
      {
        dataContext.CommandTimeout = 600000;
        if (!isdow)
          dataContext.spInvoice_AddInvoiceAndPaymentForITakeIt(auction_id, user_id, quantity, event_id, address, ac, cc, city, notes, paymenttype_id, shippingaddress, description, zip, state, buyprice, bp, shipping, insurance, tax, ispickup, Consts.UsersIPAddress, ref invoice_id);
        else
          dataContext.spInvoice_AddInvoiceAndPaymentForDOW(auction_id, user_id, quantity, event_id, address, ac, cc, city, notes, paymenttype_id, shippingaddress, description, zip, state, buyprice, bp, shipping, insurance, tax, ispickup, Consts.UsersIPAddress, ref invoice_id);
      }
      catch (Exception ex)
      {
        Logger.LogException(String.Format("Error while adding payment {0}, lot {1}, user_id {2}, quantity {3}, time {4}. Error: {5}", (!isdow) ? "ITakeIt" : "DOW", auction_id, user_id, quantity, DateTime.Now, ex.Message), ex);
        return false;
      }
      return true;
    }

    //GetEventUnpaidInvoicesForUser
    public List<InvoiceDetail> GetEventUnpaidInvoicesForUser(long user_id)
    {
      return (from p in dataContext.spInvoice_Detail_UserUnpaid(user_id)
              select new InvoiceDetail
              {
                Amount = p.Amount,
                AuctionType = p.AuctionType_ID,
                BuyerPremium = p.BuyerPremium.GetValueOrDefault(0),
                DateCreated = p.DateCreated,
                Discount = p.Discount.GetValueOrDefault(0),
                Insurance = p.Insurance.GetValueOrDefault(0),
                Invoice_ID = p.Invoice_ID,
                LinkParams = new LinkParams { ID = p.Auction_ID, Lot = p.Lot, Title = p.Title, Event_ID = p.Event_ID, EventTitle = p.EventTitle, EventCategory_ID = p.EventCategory_ID, CategoryTitle = p.Category },
                Quantity = p.Quantity,
                Shipping = p.Shipping.GetValueOrDefault(0),
                Tax = p.Tax.GetValueOrDefault(0),
                Total = p.TotalDue.GetValueOrDefault(0),
                IsConsignorShip = p.IsConsignorShip,
                UserInvoice_ID = p.UserInvoices_ID
              }).ToList();
    }

    //GetInvoiceDetailsByIDs
    public List<InvoiceDetail> GetEventInvoicesForUserByIDs(long user_id, List<long> invoice_ids)
    {
      //return dataContext.Invoices.Where(I => ListToPay.Contains(I.ID)).Cast<IInvoice>().ToList();    
      List<InvoiceDetail> result = GetEventUnpaidInvoicesForUser(user_id);
      return result.FindAll(I => invoice_ids.Contains(I.Invoice_ID));
    }

    //GetDepositAmount (user_id)
    public decimal GetDepositAmount(long user_id)
    {
      List<Deposit> deposits = dataContext.Deposits.Where(D => D.User_ID == user_id && (!D.IsRefunded.HasValue || !D.IsRefunded.Value)).ToList();
      return (deposits.Count() > 0) ? deposits.Sum(D => D.Amount).GetPrice() : 0;
    }

    //GetDepositAmountExceptCurrent
    public decimal GetDepositAmountExceptCurrent(long user_id)
    {
      List<Deposit> deposits = dataContext.Deposits.Where(D => D.User_ID == user_id && (!D.IsRefunded.HasValue || !D.IsRefunded.Value) && !D.Event.IsCurrent).ToList();
      return (deposits.Count() > 0) ? deposits.Sum(D => D.Amount).GetPrice() : 0;
    }

    //SetDepositAmount
    public List<Deposit> SetDepositAmount(long user_id, ref decimal depamount)
    {
      List<Deposit> affectedDeposits = new List<Deposit>();
      List<Deposit> query = dataContext.Deposits.Where(D => D.User_ID == user_id && (!D.IsRefunded.HasValue || !D.IsRefunded.Value) && !D.Event.IsCurrent).ToList();
      foreach (Deposit dep in query)
      {
        if (dep.Amount <= 0) continue;
        if (depamount <= 0) break;
        if (depamount - dep.Amount >= 0)
        {
          depamount -= dep.Amount;
          dep.Amount = 0;
        }
        else
        {
          dep.Amount -= depamount;
          depamount = 0;
        }
        affectedDeposits.Add(dep);
      }
      if (affectedDeposits.Count > 0)
        dataContext.SubmitChanges();
      return affectedDeposits;
    }

    //GetInvoicesByIDetails
    public List<Invoice> GetInvoicesByIDetails(List<long> invoice_ids)
    {
      return dataContext.Invoices.Where(I => invoice_ids.Contains(I.ID)).ToList();
    }

    //AddPayment
    public long AddPayment(decimal amount, long user_id, long userinvoice_id, long paymentType_id, string transactionId, string shaddress, Address billing, string CCNum, string notes)
    {
      PaymentType pt = dataContext.PaymentTypes.Where(P => P.ID == paymentType_id).SingleOrDefault();
      Payment newPayment = new Payment();
      newPayment.Amount = amount;
      newPayment.User_ID = user_id;
      newPayment.PaidDate = DateTime.Now;
      newPayment.PaymentType_ID = paymentType_id;
      newPayment.PostDate = DateTime.Now;
      newPayment.Description = "Payment for lots";
      newPayment.Notes = (String.IsNullOrEmpty(notes)) ? String.Format("Payment via {0}", pt.Title) : notes;
      newPayment.Address = String.Format("{0} {1}", billing.Address_1, billing.Address_2);
      newPayment.City = billing.City;
      newPayment.State = billing.State;
      newPayment.Zip = billing.Zip;
      newPayment.AuthCode = transactionId;
      newPayment.CCNum = CCNum;
      newPayment.ShippingAddress = shaddress;
      newPayment.UserInvoices_ID = userinvoice_id;
      dataContext.Payments.InsertOnSubmit(newPayment);
      SubmitChanges();
      return newPayment.ID;
    }

    //GetUnpaidInvoicesCount
    public int GetUnpaidInvoicesCount(long user_id)
    {
      return (from i in dataContext.Invoices
              join a in dataContext.Auctions on i.Auction_ID equals a.ID
              where i.User_ID == user_id && !i.IsPaid && a.AuctionType_ID != (long)Consts.AuctionType.DealOfTheWeek
              select i).Count();
    }

    //RefundingDeposits
    public bool RefundingDeposits(long user_id)
    {
      List<Deposit> deposits = dataContext.Deposits.Where(D => D.User_ID == user_id && !D.Event.IsCurrent && (!D.IsRefunded.HasValue || !D.IsRefunded.Value) && D.Amount > 0 && D.PaymentType_Id != (byte)Consts.PaymentType.WALKIN).ToList();
      if (deposits == null || deposits.Count() == 0) return false;
      bool result = false;
      string refundTransactionID = String.Empty;
      bool res = true;
      foreach (Deposit deposit in deposits)
      {
        if (deposit.PaymentType_Id == (long)Consts.PaymentType.Paypal)
          result = PayPalRefund(deposit.Amount, "Refunding deposit", deposit.AuthCode, out refundTransactionID);
        else
          result = CreditCardRefund(deposit.Amount, deposit.AuthCode, deposit.Num, "Refunding deposit", out refundTransactionID);
        if (result)
        {
          deposit.IsRefunded = true;
          deposit.RefundedTransactionID = refundTransactionID;
          deposit.RefundedDate = DateTime.Now;
          SubmitChanges();
          IUserRepository userRepository = new UserRepository(dataContext, CacheRepository);
          User user = userRepository.GetUser(deposit.User_ID, true);
          AddressCard ac = userRepository.GetAddressCard(user.Billing_AddressCard_ID.GetValueOrDefault(-1), true);
          Mail.SendRefundingDepositConfirmation(ac.FirstName, ac.LastName, user.Email, deposit.Amount, deposit.AuthCode, deposit.RefundedTransactionID, deposit.DatePaid.Value.ToString(), deposit.PaymentType, HttpContext.Current.Server.MapPath(@"~\Templates\Mail\RefundingDeposit.txt"), AppHelper.GetSiteUrl());
        }
        else
        {
          Logger.LogWarning(string.Format("Refunding deposit failed. User:{0} | Type: {1} | TransactionID:{2}", deposit.User.Login, deposit.PaymentType, deposit.AuthCode));
          res = false;
        }
      }
      return res;
    }

    //PayPalRefund
    private bool PayPalRefund(decimal amount, string note, string transactionId, out string refundTransactionId)
    {
      bool res = false;
      refundTransactionId = String.Empty;
      string errormessage = String.Empty;
      try
      {
        StringBuilder requestString = PP_Functions.InitializeRequest("GetTransactionDetails");
        requestString.Append("&TRANSACTIONID=" + transactionId);
        decimal transactionAmount;
        if (!PP_Functions.GetTransactionDetails(requestString.ToString(), out transactionAmount)) return res;
        requestString = PP_Functions.InitializeRequest("RefundTransaction");
        requestString.Append("&TRANSACTIONID=" + transactionId);
        requestString.Append("&REFUNDTYPE=" + String.Format((amount == transactionAmount) ? "Full" : "Partial"));
        if (amount != transactionAmount)
          requestString.Append("&AMT=" + amount.GetPrice());
        requestString.Append("&CURRENCYCODE=USD");
        requestString.Append("&NOTE=" + note);
        string token, dummy, tmp;
        if (!PP_Functions.Post(requestString.ToString(), out token, out dummy, out refundTransactionId, out tmp, out errormessage))
        {
          refundTransactionId = String.Empty;
          throw new Exception();
        }
        else res = true;
      }
      catch (Exception ex)
      {
        Logger.LogException("Refunding deposit via PayPal: " + errormessage, ex);
      }
      return res;
    }

    //CreditCardRefund
    private bool CreditCardRefund(decimal amount, string transactionID, string CCNumber, string note, out string refundTransactionID)
    {
      SessionUser cuser = AppHelper.CurrentUser;
      bool res = false;
      refundTransactionID = string.Empty;
      try
      {
        long profile_id = AN_Functions.CreateCustomerProfile(cuser.ID, cuser.Email, String.Empty);
        if (profile_id <= 0)
          return res;

        refundTransactionID = AN_Functions.RefundTransaction(profile_id, amount, CCNumber.Substring(1, 4), transactionID, note);

        if (!AN_Functions.DeleteCustomerProfile(profile_id))
          return res;

        res = !String.IsNullOrEmpty(refundTransactionID);
      }
      catch (Exception ex)
      {
        Logger.LogException("Refunding deposit via Authorize.NET", ex);
      }
      return res;
    }

    //DiscountValidation
    public bool IsDiscountValide(string discount_code, long user_id, long? discount_id)
    {
      Discount discount = discount_id.HasValue ? GetDiscount(discount_id.GetValueOrDefault(-1)) : GetDiscount(discount_code);
      if (discount == null || !discount.IsActive) return false;
      if (!discount.UnlimitedTime && (DateTime.Now > discount.EndDate || DateTime.Now < discount.StartDate)) return false;
      List<DiscountUserLimitation> dul;
      bool result = true;
      switch (discount.Limitation_ID)
      {
        case 6: //Limitation "N Time Only"
          dul = dataContext.DiscountUserLimitations.Where(d => d.Discount_ID == discount.ID).ToList();
          result = dul.Count < discount.LimitationValue;
          break;
        case 8: //Limitation "N Time Per Customer"
          dul = dataContext.DiscountUserLimitations.Where(d => d.Discount_ID == discount.ID && d.User_ID == user_id).ToList();
          result = dul.Count < discount.LimitationValue;
          break;
        default: break;
      }
      return result;
    }

    //ValidateDiscount
    public JsonExecuteResult ValidateDiscount(string discount_code, long user_id, long? discount_id)
    {
      Discount discount = discount_id.HasValue ? GetDiscount(discount_id.GetValueOrDefault(-1)) : GetDiscount(discount_code);
      string dscType = string.Empty;
      try
      {
        string errorMsg = "The Promo Code is unavailable.";
        if (discount == null || !discount.IsActive || (!discount.UnlimitedTime && (DateTime.Now > discount.EndDate || DateTime.Now < discount.StartDate)))
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, errorMsg);
        List<long> uu = (from d in dataContext.DiscountUserLimitations
                         where d.Discount_ID == discount.ID
                         orderby d.User_ID
                         select d.User_ID).Distinct().ToList();
        if (!uu.Contains(user_id) && discount.UserAmountLimitation>0 &&  uu.Count >= discount.UserAmountLimitation)
        {
          return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, errorMsg);
        }
        List<DiscountUserLimitation> dul;
        errorMsg = "The Promo Code is invalid.<br/>It has already been used.";
        switch (discount.Limitation_ID)
        {
          case 6: //Limitation "N Time Only"
            dul = dataContext.DiscountUserLimitations.Where(d => d.Discount_ID == discount.ID).ToList();
            if (dul.Count >= discount.LimitationValue) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, errorMsg);
            break;
          case 8: //Limitation "N Time Per Customer"
            dul = dataContext.DiscountUserLimitations.Where(d => d.Discount_ID == discount.ID && d.User_ID == user_id).ToList();
            if (dul.Count >= discount.LimitationValue) return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, errorMsg);
            break;
          default: break;
        }
        switch ((Consts.DiscountAssignedType) discount.Type_ID)
        {
          case Consts.DiscountAssignedType.Shipping:
            dscType = "Shipping and handling";
            break;
          case Consts.DiscountAssignedType.BuyerPremium:
            dscType = "Buyer's premium";
            break;
          default:
            dscType = "total order amount";
            break;
        }
      }
      catch (Exception ex)
      {
        return new JsonExecuteResult(JsonExecuteResultTypes.ERROR, ex.Message);
      }
      return new JsonExecuteResult(JsonExecuteResultTypes.SUCCESS, string.Format("The promo code is valid and will be applied at final checkout after pressing Continue.<br/>The {0} discount will be assigned to the {1}.", discount.DiscountValueText, dscType));
    }

    //GetValidDiscount
    public Discount GetValidDiscount(long user_id, string couponCode, long? discount_id)
    {
      Discount dicount = null;
      try
      {
        bool result = discount_id.HasValue ? IsDiscountValide(string.Empty, user_id, discount_id) : IsDiscountValide(couponCode, user_id, null);
        dicount = !result ? null : (discount_id.HasValue ? GetDiscount(discount_id.GetValueOrDefault(-1)) : GetDiscount(couponCode));
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }
      return dicount;
    }

    public bool ValidDiscountForUserID(long user_id)
    {
      try
      {
        bool res = dataContext.fExistValidDiscountForUserID(user_id).GetValueOrDefault(false);
        return res;
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
      }
      return false;
    }

    private Discount GetDiscount(string couponCode)
    {
      return dataContext.Discounts.SingleOrDefault(d => d.CouponCode == couponCode);
    }

    private Discount GetDiscount(long discount_id)
    {
      return dataContext.Discounts.SingleOrDefault(d => d.ID == discount_id);
    }

    public bool AddUsedDiscount(long user_id, long discount_id)
    {
      try
      {
        DiscountUserLimitation dul = new DiscountUserLimitation();
        dul.User_ID = user_id;
        dul.Discount_ID = discount_id;
        dataContext.DiscountUserLimitations.InsertOnSubmit(dul);
        SubmitChanges();
      }
      catch (Exception ex)
      {
        Logger.LogException(ex);
        throw ex;
      }
      return true;

    }
  }
}
