using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Models;

namespace Vauction.Utils.Authorized.NET
{
  public class AN_Functions
  {
    public static string LastError { get; set; }

    private static void SetLastError(CustomerProfileWS.MessagesTypeMessage[] msgs)
    {
      Vauction.CustomerProfileWS.MessagesTypeMessage mtm = msgs.FirstOrDefault();
      LastError = mtm == null ? "Undefined error. Please try again." : String.Format("{0}: {1}", mtm.code, GetErrorDescription(mtm.code, mtm.text));
    }

    // CreateCustomerProfile    
    public static long CreateCustomerProfile(long user_id, string email, string lots)
    {
      LastError = String.Empty;
      CustomerProfileWS.CustomerProfileType m_new_cust = new CustomerProfileWS.CustomerProfileType();
      m_new_cust.merchantCustomerId = user_id.ToString();
      m_new_cust.email = email;
      m_new_cust.description = String.Format("Payment for lot#: {0} ({1})", lots, DateTime.Now.ToString());
      CustomerProfileWS.CreateCustomerProfileResponseType response = SoapAPIUtilities.Service.CreateCustomerProfile(SoapAPIUtilities.MerchantAuthentication, m_new_cust, CustomerProfileWS.ValidationModeEnum.none);
      if (response.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Error)
        SetLastError(response.messages);
      return response.customerProfileId;
    }

    // CreateCustomerProfileForDeposit    
    public static long CreateCustomerProfileForDeposit(long user_id, string email, string descr)
    {
      LastError = String.Empty;
      CustomerProfileWS.CustomerProfileType m_new_cust = new CustomerProfileWS.CustomerProfileType();
      m_new_cust.merchantCustomerId = user_id.ToString();
      m_new_cust.email = email;
      m_new_cust.description = descr;
      CustomerProfileWS.CreateCustomerProfileResponseType response = SoapAPIUtilities.Service.CreateCustomerProfile(SoapAPIUtilities.MerchantAuthentication, m_new_cust, CustomerProfileWS.ValidationModeEnum.none);
      if (response.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Error)
        SetLastError(response.messages);
      return response.customerProfileId;
    }
    
    // GetCustomerProfile
    public static CustomerProfileWS.CustomerProfileMaskedType GetCustomerProfile(long profile_id)
    {
      LastError = String.Empty;
      CustomerProfileWS.GetCustomerProfileResponseType response_type = SoapAPIUtilities.Service.GetCustomerProfile(SoapAPIUtilities.MerchantAuthentication, profile_id);
      if (response_type.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Error)
        SetLastError(response_type.messages);
      return response_type.profile;
    }
    
    // CreateCustomerPaymentProfile
    public static long CreateCustomerPaymentProfile(long profile_id, CreditCardInfo info)
    {
      LastError = String.Empty;
      CustomerProfileWS.CustomerPaymentProfileType new_payment_profile = new CustomerProfileWS.CustomerPaymentProfileType();
      CustomerProfileWS.PaymentType new_payment = new CustomerProfileWS.PaymentType();
      CustomerProfileWS.CreditCardType new_card = new CustomerProfileWS.CreditCardType();
      new_card.cardNumber = info.CardNumber;
      new_card.expirationDate = String.Format("{0:0000}-{1:00}", info.ExpirationYear, info.ExpirationMonth);
      new_card.cardCode = info.CardCode;
      new_payment.Item = new_card;
      new_payment_profile.payment = new_payment;
      CustomerProfileWS.CreateCustomerPaymentProfileResponseType response = SoapAPIUtilities.Service.CreateCustomerPaymentProfile(SoapAPIUtilities.MerchantAuthentication, profile_id, new_payment_profile, SoapAPIUtilities.IsTestingMode ? CustomerProfileWS.ValidationModeEnum.testMode : CustomerProfileWS.ValidationModeEnum.none);
      if (response.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Error)
        SetLastError(response.messages);
      return response.customerPaymentProfileId;
    }
    
    // CreateCustomerShippingAddress
    public static long CreateCustomerShippingAddress(long profile_id, Address BillingAddress, string CountryTitle)
    {
      LastError = String.Empty;
      CustomerProfileWS.CustomerPaymentProfileType new_payment_profile = new CustomerProfileWS.CustomerPaymentProfileType();
      CustomerProfileWS.PaymentType new_payment = new CustomerProfileWS.PaymentType();
      CustomerProfileWS.CustomerAddressType cat = new CustomerProfileWS.CustomerAddressType();
      cat.firstName = BillingAddress.FirstName;
      cat.lastName = BillingAddress.LastName;
      cat.address = String.Format("{0} {1}", BillingAddress.Address_1, (String.IsNullOrEmpty(BillingAddress.Address_2) ? "" : BillingAddress.Address_2));
      cat.city = BillingAddress.City;
      cat.state = BillingAddress.State;
      cat.zip = BillingAddress.Zip;
      cat.country = BillingAddress.Country;
      cat.phoneNumber = BillingAddress.HomePhone;
      cat.faxNumber = BillingAddress.Fax;
      CustomerProfileWS.CreateCustomerShippingAddressResponseType response = SoapAPIUtilities.Service.CreateCustomerShippingAddress(SoapAPIUtilities.MerchantAuthentication, profile_id, cat);
      if (response.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Error)
        SetLastError(response.messages);
      return response.customerAddressId;
    }
    
    //UpdateCustomerPaymentProfile
    public static bool UpdateCustomerPaymentProfile(long profile_id, long payment_profile_id, CreditCardInfo info, string FirstName, string LastName)
    {
      LastError = String.Empty;
      CustomerProfileWS.CustomerPaymentProfileExType new_payment_profile = new CustomerProfileWS.CustomerPaymentProfileExType();
      new_payment_profile.customerPaymentProfileId = payment_profile_id;
      CustomerProfileWS.PaymentType new_payment = new CustomerProfileWS.PaymentType();
      CustomerProfileWS.CreditCardType new_card = new CustomerProfileWS.CreditCardType();
      new_card.cardNumber = info.CardNumber;
      new_card.expirationDate = String.Format("{0:0000}-{1:00}", info.ExpirationYear, info.ExpirationMonth);
      new_card.cardCode = info.CardCode;
      new_payment.Item = new_card;
      new_payment_profile.billTo = new CustomerProfileWS.CustomerAddressType();
      new_payment_profile.billTo.firstName = FirstName;
      new_payment_profile.billTo.lastName = LastName;
      new_payment_profile.billTo.address = String.Format("{0} {1}", info.Address1, (String.IsNullOrEmpty(info.Address2) ? "" : info.Address2));
      new_payment_profile.billTo.city = info.City;
      new_payment_profile.billTo.state = info.State;
      new_payment_profile.billTo.zip = info.Zip;
      new_payment_profile.billTo.country = info.CountryTitle;
      new_payment_profile.payment = new_payment;
      CustomerProfileWS.UpdateCustomerPaymentProfileResponseType response = SoapAPIUtilities.Service.UpdateCustomerPaymentProfile(SoapAPIUtilities.MerchantAuthentication, profile_id, new_payment_profile, SoapAPIUtilities.IsTestingMode ? CustomerProfileWS.ValidationModeEnum.testMode : CustomerProfileWS.ValidationModeEnum.none);
      if (response.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Error)
        SetLastError(response.messages);
      return response.resultCode == CustomerProfileWS.MessageTypeEnum.Ok; ;
    }
    
    // UpdateCustomerShippingAddress
    public static bool UpdateCustomerShippingAddress(long profile_id, long shipping_profile_id, Address address)
    {
      LastError = String.Empty;
      CustomerProfileWS.CustomerAddressExType cat = new CustomerProfileWS.CustomerAddressExType();
      cat.firstName = address.FirstName;
      cat.lastName = address.LastName;
      cat.address = String.Format("{0} {1}", address.Address_1, (String.IsNullOrEmpty(address.Address_2) ? "" : address.Address_2));
      cat.city = address.City;
      cat.state = address.State;
      cat.zip = address.Zip;
      cat.country = address.Country;
      cat.phoneNumber = address.HomePhone;
      cat.faxNumber = address.Fax;
      cat.customerAddressId = shipping_profile_id;
      CustomerProfileWS.UpdateCustomerShippingAddressResponseType response = SoapAPIUtilities.Service.UpdateCustomerShippingAddress(SoapAPIUtilities.MerchantAuthentication, profile_id, cat);
      if (response.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Error)
        SetLastError(response.messages);
      return response.resultCode == CustomerProfileWS.MessageTypeEnum.Ok;
    }
    
    // ValidateCustomerPaymentProfile
    public static bool ValidateCustomerPaymentProfile(long profile_id, long payment_profile_id, long shipping_profile_id, string cardcode)
    {
      LastError = String.Empty;
      CustomerProfileWS.ValidateCustomerPaymentProfileResponseType response = SoapAPIUtilities.Service.ValidateCustomerPaymentProfile(SoapAPIUtilities.MerchantAuthentication, profile_id, payment_profile_id, shipping_profile_id, cardcode, SoapAPIUtilities.IsTestingMode ? Vauction.CustomerProfileWS.ValidationModeEnum.testMode : Vauction.CustomerProfileWS.ValidationModeEnum.liveMode);
      if (response.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Error)
        SetLastError(response.messages);
      return response.resultCode == CustomerProfileWS.MessageTypeEnum.Ok;
    }
    
    //CreateTransaction
    public static string CreateTransaction(long profile_id, long payment_profile_id, long shipping_profile_id, decimal amount, string strInvoice, bool IsShipping, CreditCardInfo cci, string descr)
    {
      LastError = String.Empty;
      CustomerProfileWS.ProfileTransAuthCaptureType auth_capture = new CustomerProfileWS.ProfileTransAuthCaptureType();
      auth_capture.customerProfileId = profile_id;
      auth_capture.customerPaymentProfileId = payment_profile_id;
      auth_capture.amount = amount;
      auth_capture.order = new CustomerProfileWS.OrderExType();
      auth_capture.order.invoiceNumber = strInvoice;
      auth_capture.cardCode = cci.CardCode;
      auth_capture.order.description = descr;
      auth_capture.customerShippingAddressId = shipping_profile_id;
      auth_capture.customerShippingAddressIdSpecified = IsShipping;
      CustomerProfileWS.ProfileTransactionType trans = new CustomerProfileWS.ProfileTransactionType();
      trans.Item = auth_capture;
      CustomerProfileWS.CreateCustomerProfileTransactionResponseType response = SoapAPIUtilities.Service.CreateCustomerProfileTransaction(SoapAPIUtilities.MerchantAuthentication, trans, null);
      if (response.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Error)
        SetLastError(response.messages);
      string transactionID = String.Empty;
      if (response.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Ok)
      {
        string[] par = response.directResponse.Split(new char[] {'|', ','});
        transactionID = (par[6] != null) ? par[6] : String.Empty;
      }
      return transactionID;
    }

    //CreateTransactionForDeposit
    public static string CreateTransactionForDeposit(long profile_id, long payment_profile_id, long shipping_profile_id, decimal amount, CreditCardInfo cci, string descr)
    {
      LastError = String.Empty;
      CustomerProfileWS.ProfileTransAuthCaptureType auth_capture = new CustomerProfileWS.ProfileTransAuthCaptureType();
      auth_capture.customerProfileId = profile_id;
      auth_capture.customerPaymentProfileId = payment_profile_id;
      auth_capture.amount = amount;
      auth_capture.order = new CustomerProfileWS.OrderExType();      
      auth_capture.order.description = descr;
      auth_capture.cardCode = cci.CardCode;
      auth_capture.customerShippingAddressId = shipping_profile_id;      
      CustomerProfileWS.ProfileTransactionType trans = new CustomerProfileWS.ProfileTransactionType();
      trans.Item = auth_capture;
      CustomerProfileWS.CreateCustomerProfileTransactionResponseType response = SoapAPIUtilities.Service.CreateCustomerProfileTransaction(SoapAPIUtilities.MerchantAuthentication, trans, null);
      if (response.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Error)
        SetLastError(response.messages);
      string transactionID = String.Empty;
      if (response.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Ok)
      {
        string[] par = response.directResponse.Split(new char[] { '|', ',' });
        transactionID = (par[6] != null) ? par[6] : String.Empty;
      }
      return transactionID;
    }
   
    // DeleteCustomerProfile
    public static bool DeleteCustomerProfile(long profile_id)
    {
      LastError = String.Empty;
      CustomerProfileWS.DeleteCustomerProfileResponseType response = SoapAPIUtilities.Service.DeleteCustomerProfile(SoapAPIUtilities.MerchantAuthentication, profile_id);
      if (response.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Error)
        SetLastError(response.messages);
      return (response.resultCode == CustomerProfileWS.MessageTypeEnum.Ok);
    }

    //RefundTransaction
    public static string RefundTransaction(long profile_id, decimal amount, string CCNumber, string tranID, string description)
    {
      LastError = String.Empty;
      CustomerProfileWS.ProfileTransRefundType auth_capture = new Vauction.CustomerProfileWS.ProfileTransRefundType();
      auth_capture.amount = amount;
      auth_capture.customerProfileId = profile_id;
      auth_capture.creditCardNumberMasked = "XXXX"+CCNumber;
      auth_capture.transId = tranID;
      auth_capture.order = new Vauction.CustomerProfileWS.OrderExType();
      auth_capture.order.description = description;      
      CustomerProfileWS.ProfileTransactionType trans = new CustomerProfileWS.ProfileTransactionType();
      trans.Item = auth_capture;      
      CustomerProfileWS.CreateCustomerProfileTransactionResponseType response = SoapAPIUtilities.Service.CreateCustomerProfileTransaction(SoapAPIUtilities.MerchantAuthentication, trans, null);
      if (response.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Error)
        SetLastError(response.messages);
      string transactionID = String.Empty;
      if (response.resultCode == Vauction.CustomerProfileWS.MessageTypeEnum.Ok)
      {
        string[] par = response.directResponse.Split(new char[] { '|', ',' });
        transactionID = (par[6] != null) ? par[6] : String.Empty;
      }
      return transactionID;
    }
    
    //GetErrorDescription
    private static string GetErrorDescription(string code, string msg)
    {      
      int code_id;
      if (!Int32.TryParse(code.Remove(0, 1), out code_id)) return msg;      
      string desc = msg;
      switch (code_id)
      {
        case 1:
          desc = "This transaction has been approved.";
          break;
        case 2:
        case 3:
          desc = "This transaction has been declined.";
          break;
        case 4:
          desc = "This transaction has been declined. The code returned from the processor indicating that the card used needs to be picked up.";
          break;
        case 5:
          desc = "A valid amount is required. The value submitted in the amount field did not pass validation for a number.";
          break;
        case 6:
          desc = "The credit card number is invalid.";
          break;
        case 7:
          desc = "The credit card expiration date is invalid. The format of the date submitted was incorrect.";
          break;
        case 8:
          desc = "The credit card has expired.";
          break;
        case 9:
          desc = "The ABA code is invalid.";
          break;
        case 10:
          desc = "The account number is invalid.";
          break;
        case 11:
          desc = "A duplicate transaction has been submitted. A transaction with identical amount and credit card information was submitted two minutes prior.";
          break;        
        case 12:
          desc = "An authorization code is required but not present.";
          break;
        case 13:
          desc = "The merchant API Login ID is invalid or the account is inactive.";
          break;
        case 14:
          desc = "The Referrer or Relay Response URL is invalid. The Relay Response or Referrer URL does not match the merchant’s configured value(s) or is absent. Applicable only to SIM and WebLink APIs.";
          break;
        case 15:
          desc = "The transaction ID is invalid. The transaction ID value is non-numeric or was not present for a transaction that requires it (i.e., VOID, PRIOR_AUTH_CAPTURE, and CREDIT).";
          break;
        case 16:
          desc = "The transaction was not found. The transaction ID sent in was properly formatted but the gateway had no record of the transaction.";
          break;
        case 17:
          desc = "The merchant does not accept this type of credit card. The merchant was not configured to accept the credit card submitted in the transaction.";
          break;
        case 18:
          desc = "ACH transactions are not accepted by this merchant. The merchant does not accept electronic checks.";
          break;
        case 19:
        case 20:
        case 21:
        case 22:
        case 23:
        case 25:
        case 26:
        case 57:
        case 58:
        case 59:
        case 60:
        case 61:
        case 62:
        case 63:
          desc = "An error occurred during processing. Please try again in 5 minutes.";
          break;
        case 24:
          desc = "The Nova Bank Number or Terminal ID is incorrect. Call Merchant Service Provider.";
          break;
        case 27:
          desc = "Your transaction resulted in an error. The billing address or secure code provided did not match the information of the cardholder.";
          break;
        case 28:
          desc = "The merchant does not accept this type of credit card. The Merchant ID at the processor was not configured to accept this card type.";
          break;
        case 29:
          desc = "The Paymentech identification numbers are incorrect. Call Merchant Service Provider.";
          break;
        case 30:
          desc = "The configuration with the processor is invalid. Call Merchant Service Provider.";
          break;
        case 31:
          desc = "The FDC Merchant ID or Terminal ID is incorrect. Call Merchant Service Provider. The merchant was incorrectly set up at the processor.";
          break;
        case 32:
          desc = "This reason code is reserved or not applicable to this API.";
          break;
        case 33:
          desc = "FIELD cannot be left blank. The word FIELD will be replaced by an actual field name. This error indicates that a field the merchant specified as required was not filled in. Please see the Form Fields section of the Merchant Integration Guide for details.";
          break;
        case 34:
          desc = "The VITAL identification numbers are incorrect. Call Merchant Service Provider. The merchant was incorrectly set up at the processor.";
          break;
        case 35:
          desc = "An error occurred during processing. Call Merchant Service Provider. The merchant was incorrectly set up at the processor.";
          break;
        case 36:
          desc = "The authorization was approved, but settlement failed.";
          break;
        case 37:
          desc = "The credit card number is invalid.";
          break;
        case 38:
          desc = "The Global Payment System identification numbers are incorrect. Call Merchant Service Provider. The merchant was incorrectly set up at the processor.";
          break;
        case 40:
          desc = "This transaction must be encrypted.";
          break;
        case 41:
          desc = "This transaction has been declined. Only merchants set up for the FraudScreen.Net service would receive this decline. This code will be returned if a given transaction’s fraud score is higher than the threshold set by the merchant.";
          break;
        case 43:
          desc = "The merchant was incorrectly set up at the processor. Call your Merchant Service Provider. The merchant was incorrectly set up at the processor.";
          break;
        case 44:
          desc = "This transaction has been declined. The card code submitted with the transaction did not match the card code on file at the card issuing bank and the transaction was declined.";
          break;
        case 45:
          desc = "This transaction has been declined. This error would be returned if the transaction received a code from the processor that matched the rejection criteria set by the merchant for both the AVS and Card Code filters.";
          break;
        case 46:
          desc = "Your session has expired or does not exist. You must log in to continue working.";
          break;
        case 47:
          desc = "The amount requested for settlement may not be greater than the original amount authorized. This occurs if the merchant tries to capture funds greater than the amount of the original authorization-only transaction.";
          break;
        case 48:
          desc = "This processor does not accept partial reversals. The merchant attempted to settle for less than the originally authorized amount.";
          break;
        case 49:
          desc = "A transaction amount greater than $[amount] will not be accepted. The transaction amount submitted was greater than the maximum amount allowed.";
          break;
        case 50:
          desc = "This transaction is awaiting settlement and cannot be refunded. Credits or refunds can only be performed against settled transactions. The transaction against which the credit/refund was submitted has not been settled, so a credit cannot be issued.";
          break;        
        case 51:
          desc = "The sum of all credits against this transaction is greater than the original transaction amount.";
          break;
        case 52:
          desc = "The transaction was authorized, but the client could not be notified; the transaction will not be settled.";
          break;
        case 53:
          desc = "The transaction type was invalid for ACH transactions.";
          break;
        case 54:
          desc = "The referenced transaction does not meet the criteria for issuing a credit.";
          break;
        case 55:
          desc = "The sum of credits against the referenced transaction would exceed the original debit amount. The transaction is rejected if the sum of this credit and prior credits exceeds the original debit amount.";
          break;
        case 56:
          desc = "This merchant accepts ACH transactions only; no credit card transactions are accepted. The merchant processes eCheck.Net transactions only and does not accept credit cards.";
          break;
        case 65:
          desc = "This transaction has been declined. The transaction was declined because the merchant configured their account through the Merchant Interface to reject transactions with certain values for a Card Code mismatch.";
          break;
        case 66:
          desc = "This transaction cannot be accepted for processing. The transaction did not meet gateway security guidelines.";
          break;
        case 68:
          desc = "The version parameter is invalid.";
          break;
        case 69:
          desc = "The transaction type is invalid.";
          break;
        case 70:
          desc = "The transaction method is invalid.";
          break;
        case 71:
          desc = "The bank account type is invalid.";
          break;
        case 72:
          desc = "The authorization code is invalid.";
          break;
        case 73:
          desc = "The driver’s license date of birth is invalid.";
          break;
        case 74:
        case 76:
          desc = "The duty amount is invalid.";
          break;
        case 75:
          desc = "The freight amount is invalid.";
          break;
        case 77:
          desc = "The SSN or tax ID is invalid.";
          break;
        case 78:
          desc = "The Card Code (CVV2/CVC2/CID) is invalid.";
          break;
        case 79:
          desc = "The driver’s license number is invalid.";
          break;
        case 80:
          desc = "The driver’s license state is invalid.";
          break;
        case 81:
          desc = "The requested form type is invalid.";
          break;
        case 82:
          desc = "Scripts are only supported in version 2.5.";
          break;
        case 83:
        case 84:
        case 85:
        case 86:
        case 87:
        case 88:
        case 89:
        case 90:
          desc = "This reason code is reserved or not applicable to this API.";
          break;
        case 91:
          desc = "Version 2.5 is no longer supported.";
          break;
        case 92:
          desc = "The gateway no longer supports the requested method of integration.";
          break;
        case 97:
          desc = "This transaction cannot be accepted. Applicable only to SIM API. Fingerprints are only valid for a short period of time. If the fingerprint is more than one hour old or more than 15 minutes into the future, it will be rejected. This code indicates that the transaction fingerprint has expired.";
          break;
        case 98:
          desc = "This transaction cannot be accepted. Applicable only to SIM API. The transaction fingerprint has already been used.";
          break;
        case 99:
          desc = "This transaction cannot be accepted.";
          break;
        case 100:
          desc = "The eCheck.Net type is invalid.";
          break;
        case 101:
          desc = "The given name on the account and/or the account type does not match the actual account. Applicable only to eCheck.Net. The specified name on the account and/or the account type do not match the NOC record for this account.";
          break;
        case 102:
          desc = "This request cannot be accepted. A password or Transaction Key was submitted with this WebLink request. This is a high security risk.";
          break;
        case 103:
          desc = "This transaction cannot be accepted. A valid fingerprint, Transaction Key, or password is required for this transaction.";
          break;
        case 104:
          desc = "This transaction is currently under review. Applicable only to eCheck.Net. The value submitted for country failed validation.";
          break;
        case 105:
          desc = "This transaction is currently under review. Applicable only to eCheck.Net. The values submitted for city and country failed validation.";
          break;
        case 106:
          desc = "This transaction is currently under review. Applicable only to eCheck.Net. The value submitted for company failed validation.";
          break;
        case 107:
          desc = "This transaction is currently under review. Applicable only to eCheck.Net. The value submitted for bank account name failed validation.";
          break;
        case 108:
          desc = "This transaction is currently under review. Applicable only to eCheck.Net. The values submitted for first name and last name failed validation.";
          break;
        case 109:
          desc = "This transaction is currently under review. Applicable only to eCheck.Net. The values submitted for first name and last name failed validation.";
          break;
        case 110:
          desc = "This transaction is currently under review. Applicable only to eCheck.Net. The value submitted for bank account name does not contain valid characters.";
          break;
        case 116:
          desc = "The authentication indicator is invalid. This error is only applicable to Verified by Visa and MasterCard SecureCode transactions. The ECI value for a Visa transaction; or the UCAF indicator for a MasterCard transaction submitted in the x_authentication_indicator field is invalid.";
          break;
        case 117:
          desc = "The cardholder authentication value is invalid. This error is only applicable to Verified by Visa and MasterCard SecureCode transactions. The CAVV for a Visa transaction; or the AVV/UCAF for a MasterCard transaction is invalid.";
          break;
        case 118:
          desc = "The combination of authentication indicator and cardholder authentication value is invalid. This error is only applicable to Verified by Visa and MasterCard SecureCode transactions. The combination of authentication indicator and cardholder authentication value for a Visa or MasterCard transaction is invalid. For more information, see the “Cardholder Authentication” section of this document.";
          break;
        case 119:
          desc = "Transactions having cardholder authentication values cannot be marked as recurring.";
          break;
        case 120:
          desc = "An error occurred during processing. Please try again. The system-generated void for the original timed-out transaction failed. (The original transaction timed out while waiting for a response from the authorizer.)";
          break;
        case 121:
          desc = "An error occurred during processing. Please try again. The system-generated void for the original errored transaction failed. (The original transaction experienced a database error.)";
          break;
        case 122:
          desc = "An error occurred during processing. Please try again. The system-generated void for the original errored transaction failed. (The original transaction experienced a processing error.)";
          break;
        case 123:
          desc = "This account has not been given the permission(s) required for this request. The transaction request must include the API Login ID associated with the payment gateway account.";
          break;
        case 127:
          desc = "The transaction resulted in an AVS mismatch. The address provided does not match billing address of cardholder. The system-generated void for the original AVS-rejected transaction failed.";
          break;
        case 128:
          desc = "This transaction cannot be processed. The customer’s financial institution does not currently allow transactions for this account.";
          break;
        case 130:
          desc = "This payment gateway account has been closed. IFT: The payment gateway account status is Blacklisted.";
          break;
        case 131:
          desc = "This transaction cannot be accepted at this time. IFT: The payment gateway account status is Suspended-STA.";
          break;
        case 132:
          desc = "This transaction cannot be accepted at this time. IFT: The payment gateway account status is Suspended-Blacklist.";
          break;
        case 141:
          desc = "This transaction has been declined. The system-generated void for the original FraudScreen-rejected transaction failed.";
          break;
        case 145:
          desc = "This transaction has been declined. The system-generated void for the original card code-rejected and AVS-rejected transaction failed.";
          break;
        case 152:
          desc = "The transaction was authorized, but the client could not be notified; the transaction will not be settled. The system-generated void for the original transaction failed. The response for the original transaction could not be communicated to the client.";
          break;
        case 165:
          desc = "This transaction has been declined. The system-generated void for the original card code-rejected transaction failed.";
          break;
        case 170:
          desc = "An error occurred during processing. Please contact the merchant. Concord EFS – Provisioning at the processor has not been completed.";
          break;
        case 171:
          desc = "An error occurred during processing. Please contact the merchant. Concord EFS – This request is invalid.";
          break;
        case 172:
          desc = "An error occurred during processing. Please contact the merchant. Concord EFS – The store ID is invalid.";
          break;
        case 173:
          desc = "An error occurred during processing. Please contact the merchant. Concord EFS – The store key is invalid.";
          break;
        case 174:
          desc = "An error occurred during processing. Please contact the merchant. Concord EFS – This transaction type is not accepted by the processor.";
          break;
        case 175:
          desc = "The processor does not allow voiding of credits. Concord EFS – This transaction is not allowed. The Concord EFS processing platform does not support voiding credit transactions. Please debit the credit card instead of voiding the credit.";
          break;
        case 180:
          desc = "An error occurred during processing. Please try again. The processor response format is invalid.";
          break;
        case 181:
          desc = "An error occurred during processing. Please try again. The system-generated void for the original invalid transaction failed. (The original transaction included an invalid processor response format.)";
          break;
        case 185:
          desc = "This reason code is reserved or not applicable to this API.";
          break;
        case 193:
          desc = "The transaction is currently under review. The transaction was placed under review by the risk management system.";
          break;
        case 200:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The credit card number is invalid.";
          break;
        case 201:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The expiration date is invalid.";
          break;
        case 202:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The transaction type is invalid.";
          break;
        case 203:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The value submitted in the amount field is invalid.";
          break;
        case 204:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The department code is invalid.";
          break;
        case 205:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The value submitted in the merchant number field is invalid.";
          break;
        case 206:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The merchant is not on file.";
          break;
        case 207:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The merchant account is closed.";
          break;
        case 208:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The merchant is not on file.";
          break;
        case 209:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. Communication with the processor could not be established.";
          break;
        case 210:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The merchant type is incorrect.";
          break;
        case 211:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The cardholder is not on file.";
          break;
        case 212:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The bank configuration is not on file.";
          break;
        case 213:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The merchant assessment code is incorrect.";
          break;
        case 214:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. This function is currently unavailable.";
          break;
        case 215:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The encrypted PIN field format is invalid.";
          break;
        case 216:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The ATM term ID is invalid.";
          break;
        case 217:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. This transaction experienced a general message format problem.";
          break;
        case 218:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The PIN block format or PIN availability value is invalid.";
          break;
        case 219:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The ETC void is unmatched.";
          break;
        case 220:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The primary CPU is not available.";
          break;
        case 221:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. The SE number is invalid.";
          break;
        case 222:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. Duplicate auth request (from INAS).";
          break;
        case 223:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. This transaction experienced an unspecified error.";
          break;
        case 224:
          desc = "This transaction has been declined. This error code applies only to merchants on FDC Omaha. Please re-enter the transaction.";
          break;
        case 243:
          desc = "Recurring billing is not allowed for this eCheck.Net type.";
          break;
        case 244:
          desc = "This eCheck.Net type is not allowed for this Bank Account Type.";
          break;
        case 245:
          desc = "This eCheck.Net type is not allowed when using the payment gateway hosted payment form.";
          break;
        case 246:
          desc = "This eCheck.Net type is not allowed. The merchant’s payment gateway account is not enabled to submit the eCheck.Net type.";
          break;
        case 247:
          desc = "This eCheck.Net type is not allowed.";
          break;
        case 248:
          desc = "The check number is invalid. Invalid check number. Check number can only consist of letters and numbers and not more than 15 characters.";
          break;
        case 250:
          desc = "This transaction has been declined. This transaction was submitted from a blocked IP address.";
          break;
        case 251:
          desc = "This transaction has been declined. The transaction was declined as a result of triggering a Fraud Detection Suite filter.";
          break;
        case 254:
          desc = "Your transaction has been declined. The transaction was declined after manual review.";
          break;
        case 261:
          desc = "An error occurred during processing. The transaction experienced an error during sensitive data encryption and was not processed. Please try again.";
          break;
        case 270:
          desc = "The line item [item number] is invalid.";
          break;
        case 271:
          desc = "The number of line items submitted is not allowed. A maximum of 30 line items can be submitted. The number of line items submitted exceeds the allowed maximum of 30.";
          break;
        case 288:
          desc = "Merchant is not registered as a Cardholder Authentication participant. This transaction cannot be accepted. The merchant has not indicated participation in any Cardholder Authentication Programs in the Merchant Interface.";
          break;
        case 289:
          desc = "This processor does not accept zero dollar authorization for this card type. Your credit card processing service does not yet accept zero dollar authorizations for Visa credit cards. You can find your credit card processor listed on your merchant profile.";
          break;
        case 290:
          desc = "One or more required AVS values for zero dollar authorization were not submitted. When submitting authorization requests for Visa, the address and zip code fields must be entered.";
          break;
        case 296:
          desc = "The specified SplitTenderId is not valid.";
          break;
        case 297:
          desc = "A Transaction ID and a Split Tender ID cannot both be used in a single transaction request.";
          break;
        case 300:
          desc = "The device ID is invalid.";
          break;
        case 301:
          desc = "The device batch ID is invalid.";
          break;
        case 302:
          desc = "The reversal flag is invalid.";
          break;
        case 303:
          desc = "The device batch is full. Please close the batch. The current device batch must be closed manually from the POS device.";
          break;
        case 304:
          desc = "The original transaction is in a closed batch. The original transaction has been settled and cannot be reversed.";
          break;
        case 305:
          desc = "The merchant is configured for auto-close. This merchant is configured for auto-close and cannot manually close batches.";
          break;
        case 306:
          desc = "The batch is already closed.";
          break;
        case 309:
          desc = "The device has been disabled.";
          break;
        case 315:
          desc = "The credit card number is invalid. This is a processor-issued decline.";
          break;
        case 316:
          desc = "The credit card expiration date is invalid. This is a processor-issued decline.";
          break;
        case 317:
          desc = "The credit card has expired. This is a processor-issued decline.";
          break;
        case 318:
          desc = "A duplicate transaction has been submitted. This is a processor-issued decline.";
          break;
        case 319:
          desc = "The transaction cannot be found. This is a processor-issued decline.";
          break;
        default:         
          break;
      }
      return desc;
    }
  }
}