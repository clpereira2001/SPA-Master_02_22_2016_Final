using System;
using System.Globalization;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Text;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using Vauction.Models;

namespace Vauction.Utils.Helpers
{
  public class Mail
  {
    const string bidding_mail = "bidding";
    const string registration_mail = "registration";
    const string invoices_mail = "invoices";

    #region parse common data
    private static void ParseCommonData(UniMail.Template template)
    {
      template.Data.Add("{{copyrightDate}}", ConfigurationManager.AppSettings["copyrightDate"]);
      template.Data.Add("{{CompanyName}}", ConfigurationManager.AppSettings["CompanyName"]);
      template.Data.Add("{{CompanyAddress}}", ConfigurationManager.AppSettings["CompanyAddress"]);
      template.Data.Add("{{companyCity}}", ConfigurationManager.AppSettings["companyCity"]);
      template.Data.Add("{{companyState}}", ConfigurationManager.AppSettings["companyState"]);
      template.Data.Add("{{companyZip}}", ConfigurationManager.AppSettings["companyZip"]);
      template.Data.Add("{{siteName}}", ConfigurationManager.AppSettings["siteName"]);
      template.Data.Add("{{siteUrl}}", AppHelper.GetSiteUrl());
      template.Data.Add("{{siteEmail}}", ConfigurationManager.AppSettings["siteEmail"]);
    }
    private static void ParseCommonData(UniMail.Template template, string SiteURL)
    {
      template.Data.Add("{{copyrightDate}}", ConfigurationManager.AppSettings["copyrightDate"]);
      template.Data.Add("{{CompanyName}}", ConfigurationManager.AppSettings["CompanyName"]);
      template.Data.Add("{{CompanyAddress}}", ConfigurationManager.AppSettings["CompanyAddress"]);
      template.Data.Add("{{companyCity}}", ConfigurationManager.AppSettings["companyCity"]);
      template.Data.Add("{{companyState}}", ConfigurationManager.AppSettings["companyState"]);
      template.Data.Add("{{companyZip}}", ConfigurationManager.AppSettings["companyZip"]);
      template.Data.Add("{{siteName}}", ConfigurationManager.AppSettings["siteName"]);
      template.Data.Add("{{siteUrl}}", SiteURL);
      template.Data.Add("{{siteEmail}}", ConfigurationManager.AppSettings["siteEmail"]);
    }
    #endregion

    #region registration / profile / forgot password / resend confirmation
    // SendRegisterConfirmation
    public static void SendRegisterConfirmation(string FirstName, string LastName, string emailTo, string loginName, string confirmationUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\registerConfirm.txt"), registration_mail);

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      template.Data.Add("{{confUrl}}", confirmationUrl);
      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.Render(), registration_mail);
    }

    //ResendConfirmationCode
    public static void ResendConfirmationCode(string FirstName, string LastName, string emailTo, string loginName, string confirmationUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\resendConfirmationCode.txt"), registration_mail);

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{confUrl}}", confirmationUrl);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);

      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.Render(), registration_mail);
    }

    //ForgotPassword
    public static void ForgotPassword(string FirstName, string LastName, string emailTo, string loginName, string password)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\passwordReset.txt"), registration_mail);
      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{login}}", loginName);
      template.Data.Add("{{password}}", password);
      template.Data.Add("{{FirstName}}", FirstName);
      template.Data.Add("{{LastName}}", LastName);
      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.Render(), registration_mail);
    }

    //EmailConfirmationCode
    public static void EmailConfirmationCode(string FirstName, string LastName, string emailTo, string loginName, string confirmationUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\emailConfirmationCode.txt"), registration_mail);
      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{confUrl}}", confirmationUrl);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.Render(), registration_mail);
    }

    #endregion

    #region sending personal shopper
    public static void SendFutureEventsPerShopper(string email, string firstname, string lastname, string login, List<AuctionShort> auctions, bool ishtml)
    {
      try
      {
        if (ishtml)
        {
          UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\ReceivePersonalShopperUpdate.txt"));
          template.Encoding = System.Text.Encoding.UTF8;
          template.ToEmail = email;

          StringBuilder sb = new StringBuilder();
          string str, image_path;
          str = image_path = String.Empty;
          bool line = true;
          LinkedResource lr;
          int imgNumber = 0;
          FileInfo fi;

          //fi = new FileInfo(HttpContext.Current.Server.MapPath(String.Format(@"~/public/images/logo_1.jpg")));
          //if (fi.Exists)
          //{
          //  lr = new LinkedResource(fi.FullName);
          //  lr.ContentId = "logo";
          //  lr.TransferEncoding = TransferEncoding.Base64;
          //  template.LinkedResourceCollection.Add(lr);
          //}
          //fi = new FileInfo(HttpContext.Current.Server.MapPath(String.Format(@"~/public/images/logo_text_tr.gif")));
          //if (fi.Exists)
          //{
          //  lr = new LinkedResource(fi.FullName);
          //  lr.ContentId = "logo_text";
          //  lr.TransferEncoding = TransferEncoding.Base64;
          //  template.LinkedResourceCollection.Add(lr);
          //}

          foreach (AuctionShort a in auctions)
          {
            image_path = a.ThumbnailPath;
            str = String.Empty;
            if (!String.IsNullOrEmpty(image_path))
            {
              fi = new FileInfo(HttpContext.Current.Server.MapPath(AppHelper.AuctionImage(a.LinkParams.ID, image_path)));
              if (fi.Exists)
              {
                lr = new LinkedResource(fi.FullName);
                lr.ContentId = "img" + (++imgNumber).ToString();
                lr.TransferEncoding = TransferEncoding.Base64;
                template.LinkedResourceCollection.Add(lr);
                str = "<img src=\"cid:img" + imgNumber.ToString() + "\">";
              }
              else str = "&nbsp;";
            }
            sb.AppendFormat("<tr {0}><td colspan=\"2\"><hr /></td></tr>", (line) ? "style='background-color:#d9e9f0'" : String.Empty);
            sb.AppendFormat("<tr {0}><td align=\"left\" rowspan=\"3\" style='padding:5px'>{3}</td><td style='text-align:left;'><a href=\"{4}\"><b>{1}</b></a></td></tr><tr {0}><td>{2}</td></tr><tr {0}><td><i>{5}</i></td></tr>", (line) ? "style='background-color:#d9e9f0'" : String.Empty, a.LinkParams.Title, /*a.DescriptionWithoutTagsShort*/ String.Empty, str, AppHelper.GetSiteUrl(String.Format("/Auction/AuctionDetail/{0}/{1}/{2}/{3}", a.LinkParams.ID, a.LinkParams.EventUrl, a.LinkParams.CategoryUrl, a.LinkParams.LotTitleUrl)), String.Format("({0} ET to {1} ET)", a.StartDate, a.EndDate));
            line = !line;
          }

          template.Data.Add("{{info}}", sb.ToString());
          template.Data.Add("{{firstname}}", firstname);
          template.Data.Add("{{lastname}}", lastname);
          template.Data.Add("{{login}}", login);

          ParseCommonData(template);

          UniMail.Mailer.Enqueue(template.RenderHTML());
        }
        else
        {
          UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\ReceivePersonalShopperUpdateText.txt"));

          template.Encoding = System.Text.Encoding.UTF8;
          template.ToEmail = email;

          StringBuilder sb = new StringBuilder();

          foreach (AuctionShort a in auctions)
          {
            sb.AppendLine("Lot/Title: " + a.LinkParams.Lot + " - " + a.LinkParams.Title);
            sb.AppendLine("" + String.Format("({0} ET to {1} ET)", a.StartDate, a.EndDate));
            sb.AppendLine(AppHelper.GetSiteUrl(String.Format("/Auction/AuctionDetail/{0}/{1}/{2}/{3}", a.LinkParams.ID, a.LinkParams.EventUrl, a.LinkParams.CategoryUrl, a.LinkParams.LotTitleUrl)));
            sb.AppendLine();
          }

          template.Data.Add("{{info}}", sb.ToString());
          template.Data.Add("{{firstname}}", firstname);
          template.Data.Add("{{lastname}}", lastname);
          template.Data.Add("{{login}}", login);

          ParseCommonData(template);
          UniMail.Mailer.Enqueue(template.Render());
        }
      }
      catch (Exception ex)
      {
        Vauction.Utils.Lib.Logger.LogInfo("SendFutureEventsPerShopper error: " + ex.Message);
      }
    }
    #endregion

    #region payment
    //SendDepositPaymentConfirmation
    public static void SendDepositPaymentConfirmation(string FirstName, string LastName, string emailTo, decimal DepositAmount, string TransactionID, string PaymentMethod)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\DepositPaymentConfirmation.txt"), invoices_mail);
      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;

      template.Data.Add("{{TransactionID}}", TransactionID);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      template.Data.Add("{{PaymentType}}", PaymentMethod);
      template.Data.Add("{{amount}}", DepositAmount.GetCurrency());

      ParseCommonData(template);

      UniMail.Mailer.Enqueue(template.Render(), invoices_mail);
    }

    //SendInvoicePaymentConfirmation
    public static void SendInvoicePaymentConfirmation(string email, string login, PaymentConfirmation pc)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\InvoicePaymentConfirmation.txt"));
      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = email;

      StringBuilder table = new StringBuilder();
      if (pc.IsShipping)
        table.AppendFormat("We will be shipping out your item{0} and sending you a shipping confirmation email within 8 business days.", pc.Quantity > 1 ? "s" : "");
      else
        table.AppendFormat("Your item{0} will be held at will call and will be available Monday - Friday, 9AM - 4PM<br /><b>{1}</b><br /><b>{2}</b><br />", pc.Quantity > 1 ? "s" : "", Consts.CompanyTitleName, Consts.CompanyAddress);
      template.Data.Add("{{text_title}}", table.ToString());

      table = new StringBuilder();

      if (!String.IsNullOrEmpty(pc.TransactionID))
      {
        table.Append("<span style='color:Navy'><strong>Transaction information</strong></span><br />");
        table.AppendFormat("{0} Transaction ID# <strong>{1}</strong>", (pc.PaymentType == Consts.PaymentType.Paypal ? "Express Checkout Payment" : "Paid by Credit Card"), pc.TransactionID);
      }
      else
      {
        table.Append("<span style='color:Navy'><strong>Paid from the Account Balance</strong></span>");
      }

      template.Data.Add("{{Transaction}}", table.ToString());

      #region Billing & Shipping
      table = new StringBuilder();
      table.Append("<colgroup>");
      table.AppendLine("<col width=\"355px\" /><col width=\"360px\" />");
      table.Append("</colgroup>");
      table.AppendLine("<tr>");
      table.AppendFormat("<td style='background-color:#002868;color:#FFFFFF;font-weight:bold;border:0px solid black;padding-left:10px;padding: 5px 0px 5px 10px;'>Billing information</td>");
      table.AppendFormat("<td style='background-color:#002868;color:#FFFFFF;font-weight:bold;border:0px solid black;padding-left:10px;padding: 5px 0px 5px 10px;'>Shipping information</td>");
      table.AppendLine("</tr>");
      table.AppendLine("<tr>");
      table.AppendFormat("<td style='border-top:solid 1px #DDD;border-left:solid 1px #DDD;border-right:solid 1px #DDD;padding: 5px 0px 5px 10px;background-color:#FFF;'>{0} (<b>{1}</b>)</td>", pc.Address_Billing.FullName, login.ToUpper());
      if (pc.IsShipping)
        table.AppendFormat("<td style='border-top: solid 1px #DDD;border-right:solid 1px #DDD; padding: 5px 0px 5px 10px; background-color: White;'>{0} (<b>{1}</b>)</td>", pc.Address_Shipping.FullName, login.ToUpper());
      else
      {
        table.AppendFormat("<td style='border-top: solid 1px #DDD;border-right:solid 1px #DDD; padding-left: 10px;padding-right:10px; background-color: White;vertical-align:top' rowspan='{0}'>", ((!String.IsNullOrEmpty(pc.Address_Billing.WorkPhone) || (pc.Address_Shipping != null && !String.IsNullOrEmpty(pc.Address_Shipping.WorkPhone)))) ? "8" : "7");
        table.AppendFormat("Your item{0} will be held at will call and will be available Monday - Friday, 9AM - 4PM <br />", pc.Quantity > 1 ? "s" : "");
        table.AppendFormat("<b>{0}</b><br />", Consts.CompanyTitleName);
        table.AppendFormat("<b>{0}</b></td>", Consts.CompanyAddress);
      }
      table.AppendLine("</tr>");
      table.AppendLine("<tr>");
      table.AppendFormat("<td style='border-top:solid 1px #DDD;border-left:solid 1px #DDD;border-right:solid 1px #DDD;padding: 5px 0px 5px 10px;background-color:#FFF;'>{0}&nbsp;</td>", pc.Address_Billing.Address_1);
      if (pc.IsShipping)
        table.AppendFormat("<td style='border-top: solid 1px #DDD;border-right:solid 1px #DDD; padding: 5px 0px 5px 10px; background-color: White;'>{0}&nbsp;</td>", pc.Address_Shipping.Address_1);
      table.AppendLine("</tr>");
      table.AppendLine("<tr>");
      table.AppendFormat("<td style='border-top:solid 1px #DDD;border-left:solid 1px #DDD;border-right:solid 1px #DDD;padding: 5px 0px 5px 10px;background-color:#FFF;'>{0}&nbsp;</td>", pc.Address_Billing.Address_2);
      if (pc.IsShipping)
        table.AppendFormat("<td style='border-top: solid 1px #DDD;border-right:solid 1px #DDD;padding: 5px 0px 5px 10px;background-color: White;'>{0}&nbsp;</td>", pc.Address_Shipping.Address_2);
      table.AppendLine("</tr>");
      table.AppendLine("<tr>");
      table.AppendFormat("<td style='border-top:solid 1px #DDD;border-left:solid 1px #DDD;border-right:solid 1px #DDD;padding: 5px 0px 5px 10px;background-color:#FFF;'>{0},&nbsp;{1}</td>", pc.Address_Billing.City, pc.Address_Billing.State);
      if (pc.IsShipping)
        table.AppendFormat("<td style='border-top: solid 1px #DDD;border-right:solid 1px #DDD; padding: 5px 0px 5px 10px; background-color: White;'>{0},&nbsp;{1}</td>", pc.Address_Shipping.City, pc.Address_Shipping.State);
      table.AppendLine("</tr>");
      table.AppendLine("<tr>");
      table.AppendFormat("<td style='border-top:solid 1px #DDD;border-left:solid 1px #DDD;border-right:solid 1px #DDD;padding: 5px 0px 5px 10px;background-color:#FFF;'>{0}&nbsp;</td>", pc.Address_Billing.Zip);
      if (pc.IsShipping)
        table.AppendFormat("<td style='border-top: solid 1px #DDD;border-right:solid 1px #DDD; padding: 5px 0px 5px 10px; background-color: White;'>{0}&nbsp;</td>", pc.Address_Shipping.Zip);
      table.AppendLine("</tr>");
      table.AppendLine("<tr>");
      table.AppendFormat("<td style='border-top:solid 1px #DDD;border-left:solid 1px #DDD;border-right:solid 1px #DDD;padding: 5px 0px 5px 10px;background-color:#FFF;'>{0}&nbsp;</td>", pc.Address_Billing.Country);
      if (pc.IsShipping)
        table.AppendFormat("<td style='border-top: solid 1px #DDD;border-right:solid 1px #DDD; padding: 5px 0px 5px 10px; background-color: White;'>{0}</td>", pc.Address_Shipping.Country);
      table.AppendLine("</tr>");
      if (!String.IsNullOrEmpty(pc.Address_Billing.WorkPhone) || (pc.Address_Shipping != null && !String.IsNullOrEmpty(pc.Address_Shipping.WorkPhone)))
      {
        table.AppendLine("<tr>");
        table.AppendFormat("<td style='border-top:solid 1px #DDD;border-left:solid 1px #DDD;border-right:solid 1px #DDD;padding: 5px 0px 5px 10px;;background-color:#FFF;'>{0}&nbsp;</td>", pc.Address_Billing.HomePhone);
        if (pc.IsShipping)
          table.AppendFormat("<td style='border-top: solid 1px #DDD;border-right:solid 1px #DDD; padding: 5px 0px 5px 10px; background-color: White;'>{0}&nbsp;</td>", pc.Address_Shipping.HomePhone);
        table.AppendLine("</tr>");
      }
      else
      {
        table.AppendLine("<tr>");
        table.AppendFormat("<td style='border-top: solid 1px #DDD;border-right:solid 1px #DDD;border-left:solid 1px #DDD;border-bottom:solid 1px #DDD;padding: 5px 0px 5px 10px;background-color:#FFF;'>{0}&nbsp;</td>", pc.Address_Billing.HomePhone);
        if (pc.IsShipping)
          table.AppendFormat("<td style='border-top: solid 1px #DDD;border-right: solid 1px #DDD;border-bottom: solid 1px #DDD;padding: 5px 0px 5px 10px; background-color: White;'>{0}&nbsp;</td>", pc.Address_Shipping.HomePhone);
        table.AppendLine("</tr>");
      }
      if (!String.IsNullOrEmpty(pc.Address_Billing.WorkPhone) || (pc.Address_Shipping != null && !String.IsNullOrEmpty(pc.Address_Shipping.WorkPhone)))
      {
        table.AppendLine("<tr>");
        table.AppendFormat("<td style='border-top: solid 1px #DDD;border-right:solid 1px #DDD;border-left:solid 1px #DDD;border-bottom:solid 1px #DDD;padding: 5px 0px 5px 10px;background-color:#FFF;'>{0}&nbsp;</td>", pc.Address_Billing.WorkPhone);
        if (pc.IsShipping)
          table.AppendFormat("<td style='border-top: solid 1px #DDD;border-right: solid 1px #DDD;border-bottom: solid 1px #DDD;padding: 5px 0px 5px 10px; background-color: White;'>{0}&nbsp;</td>", pc.Address_Shipping.WorkPhone);
        table.AppendLine("</tr>");
      }
      template.Data.Add("{{billing_info}}", table.ToString());
      #endregion

      #region Invoices
      table = new StringBuilder();
      string tableBL = "border-left:solid 1px #DDD; border-bottom:solid 1px #DDD;padding-left:10px;background-color:White;";
      string tableBLR = "border-right:solid 1px #DDD;border-left:solid 1px #DDD;border-bottom:solid 1px #DDD;padding-left:10px;background-color:White;";
      foreach (InvoiceDetail invoice in pc.Invoices)
      {
        table.AppendLine("<tr>");
        table.AppendFormat("<td style='font-size:14px;font-weight:bold;padding: 5px 0px 5px 10px;background-color:#F2F2F2;border: solid 1px #e2e2e2'>{0}</td>", invoice.LinkParams.Lot);
        table.AppendFormat("<td style='font-size:14px;font-weight:bold;padding: 5px 0px 5px 10px;background-color:#F2F2F2;border: solid 1px #e2e2e2'>{0}</td>", invoice.DateCreated.ToShortDateString());
        table.AppendFormat("<td style='font-size:14px;font-weight:bold;padding: 5px 0px 5px 10px;background-color:#F2F2F2;border: solid 1px #e2e2e2'>{0}</td>", invoice.LinkParams.Title);
        table.AppendFormat("<td style='font-size:14px;font-weight:bold;padding: 5px 0px 5px 10px;background-color:#F2F2F2;border: solid 1px #e2e2e2'>{0}</td>", invoice.Quantity);
        table.AppendFormat("<td style='font-size:14px;font-weight:bold;font-weight:bold;padding: 5px 0px 5px 10px;background-color:#F2F2F2;border: solid 1px #e2e2e2'>{0}</td>", invoice.TotalCostWithoutCouponDiscount.GetCurrency());
        table.AppendLine("</tr>");

        table.AppendLine("<tr><td colspan='5' style='font-size:14px;background-color:#DEEBF7;padding:5px'><table style='margin: 0px; table-layout: fixed;' cellpadding='0' cellspacing='0'><colgroup><col width='245px' /><col width='365px' /><col width='120px' /></colgroup>");
        table.AppendFormat("<tr><td>&nbsp;</td><td style='border-top:solid 1px #DDDDDD;border-left:solid 1px #DDDDDD;border-bottom:solid 1px #DDDDDD;padding-left:10px;background-color:White;'>Winning Bid {0}</td><td style='border:solid 1px #DDDDDD;padding-left:10px;background-color:White;'>{1}</td></tr>", (invoice.Quantity > 1) ? String.Format("<span style='display: table-row-group; float: right;'>{0} x {1} = </span>", invoice.Quantity, ((decimal)(invoice.Amount / invoice.Quantity)).GetCurrency()) : String.Empty, invoice.Amount.GetCurrency());
        if (invoice.AuctionType != (long)Consts.AuctionType.DealOfTheWeek && (invoice.BuyerPremium > 0 || invoice.RealBP.HasValue))
          table.AppendFormat("<tr><td>&nbsp;</td><td style='{0}'>Buyer's premium</td><td style='{1}'>{2}</td></tr>", tableBL, tableBLR, invoice.RealBP.GetValueOrDefault(invoice.BuyerPremium).GetCurrency());
        if (invoice.Shipping > 0 || invoice.RealSh.HasValue)
        {
          table.AppendFormat("<tr><td>&nbsp;</td><td style='{0}'>Shipping and handling</td><td style='{1}'>{2}</td></tr>", tableBL, tableBLR, invoice.RealSh.GetValueOrDefault(invoice.Shipping).GetCurrency());
          table.AppendFormat("<tr><td>&nbsp;</td><td style='{0}'>Insurance</td><td style='{1}'>{2}</td></tr>", tableBL, tableBLR, invoice.Insurance.GetCurrency());
        }
        if (invoice.Tax > 0)
          table.AppendFormat("<tr><td>&nbsp;</td><td style='{0}'>Tax</td><td style='{1}'>{2}</td></tr>", tableBL, tableBLR, invoice.Tax.GetCurrency());
        if (invoice.Discount > 0 && invoice.TotalOrderDiscount.GetValueOrDefault(0) != invoice.Discount)
          table.AppendFormat("<tr><td>&nbsp;</td><td style='{0}'>Tax</td><td style='{1}'>{2}</td></tr>", tableBL, tableBLR, (invoice.Discount - invoice.TotalOrderDiscount.GetValueOrDefault(0)).GetCurrency());
        table.AppendLine("</table>");
        table.AppendLine("</td>");
        table.AppendLine("</tr>");
      }
      template.Data.Add("{{invoices_info}}", table.ToString());
      #endregion

      if (pc.HasDiscount)
      {
        table = new StringBuilder();
        table.AppendFormat("<br /><tr><td style='background-color:#01347E;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:14px;' colspan=4>{0}&nbsp;Paid:&nbsp;</td><td style='font-weight:bold;background-color:#01347E;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:14px;'>{1}</td></tr>", "Subtotal", pc.TotalCostWithoutDiscount.GetCurrency());
        table.AppendFormat("<br /><tr><td style='font-weight:bold;background-color:#400000;border:solid 1px #400000;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:14px;text-align:right' colspan=4>{0}:&nbsp;</td><td style='font-weight:bold;background-color:#400000;border:solid 1px #400000;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:14px;'>{1}</td></tr>", "Promo Discount", pc.DiscountAmount.GetCurrency());
        template.Data.Add("{{discount_info}}", table.ToString());
      }
      else template.Data.Add("{{discount_info}}", String.Empty);

      template.Data.Add("{{TotalDue}}", pc.TotalCost.GetCurrency());

      if (pc.Deposit > 0)
      {
        table = new StringBuilder();
        table.AppendFormat("<br /><tr><td style='font-weight:bold;background-color:#400000;border:solid 1px #400000;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:14px;text-align:right' colspan=4>{0}:&nbsp;</td><td style='font-weight:bold;background-color:#400000;border:solid 1px #400000;color:#FFFFFF;padding: 5px 0px 5px 10px;font-size:14px;'>{1}</td></tr>", (pc.TotalCost - pc.Deposit > 0) ? "Deposit" : "Paid from Account Balance", (pc.TotalCost - pc.Deposit < 0) ? pc.TotalCost.GetCurrency() : pc.Deposit.GetCurrency());
        if (pc.TotalCost - pc.Deposit > 0)
        {
          table.AppendFormat("<br /><tr><td style='font-weight:bold;background-color:#DEEBF7;border:solid 1px #e2e2e2;color:#333333;padding: 5px 0px 5px 10px;font-size:14px;text-align:right' colspan=4>{0}&nbsp;Paid:&nbsp;</td><td style='font-weight:bold;background-color:#DEEBF7;border:solid 1px #e2e2e2;color:#333333;padding: 5px 0px 5px 10px;font-size:14px;'>{1}</td></tr>", (pc.Deposit == 0) ? "Total" : "Balance", (pc.TotalCost - pc.Deposit < 0) ? "(" + (pc.Deposit - pc.TotalCost).GetCurrency() + ")" : (pc.TotalCost - pc.Deposit).GetCurrency());
        }
        else if (pc.DepositLimit >= 0)
        {
          table.AppendFormat("<br /><tr><td style='font-weight:bold;background-color:#DEEBF7;border:solid 1px #e2e2e2;color:#333333;padding: 5px 0px 5px 10px;font-size:14px;text-align:right' colspan=4>{0}&nbsp;</td><td style='font-weight:bold;background-color:#DEEBF7;border:solid 1px #e2e2e2;color:#333333;padding: 5px 0px 5px 10px;font-size:14px;'>{1}</td></tr>", !pc.IsDepositRefunded ? "Account Balance:" : "Refunded Amount:", pc.DepositLimit.GetCurrency(false));
        }
        template.Data.Add("{{deposits}}", table.ToString());
      }
      else template.Data.Add("{{deposits}}", String.Empty);

      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.RenderHTML());
    }

    #endregion

    public static void SendMessageFromConsignor(string emailTo, ConsignNowForm form)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\MessageFromConsignor.txt"));
      template.Encoding = Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{firstname}}", form.FirstName);
      template.Data.Add("{{lastname}}", form.LastName);
      template.Data.Add("{{email}}", form.Email);
      template.Data.Add("{{phone}}", form.Phone ?? string.Empty);
      if (!string.IsNullOrWhiteSpace(form.Description))
      {
        template.Data.Add("{{descriptionTitle}}", "Item(s) description:");
        template.Data.Add("{{description}}", form.Description);
      }
      else
      {
        template.Data.Add("{{descriptionTitle}}", string.Empty);
        template.Data.Add("{{description}}", string.Empty);
      }
      template.Data.Add("{{acquire}}", form.Acquire ?? string.Empty);
      template.Data.Add("{{finance}}", form.Finance ? "Yes" : "No");
      template.Data.Add("{{subscribe}}", form.Subscribe ? "Yes" : "No");
      if (form.FileLinks.Any())
      {
        StringBuilder sb = new StringBuilder("Image(s):<br />");
        form.FileLinks.ForEach(t => sb.AppendFormat("<a href=\"{0}\" target=\"_blank\"><img src=\"{0}\" style=\"max-width:220px;max-height:220px;\"></a>", t)); template.Data.Add("{{images}}", sb.ToString());
      }
      else
      {
        template.Data.Add("{{images}}", string.Empty);
      }

      form.Attachments.ForEach(t => template.AttachItem.Add(new Attachment(t, MediaTypeNames.Image.Jpeg)));
      ParseCommonData(template);
      UniMail.Mailer.Send(template.RenderHTML());
    }
    // NOT DONE



    public static void SendOutBidLetter(string FirstName, string LastName, string emailTo, long auctionID, string loginName, string auctionName, string currentSuccessfulBid, string auctionEndDate, string newBidUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\outBid.txt"), bidding_mail);

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{auctionID}}", auctionID.ToString());
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{currentSuccessfulBid}}", currentSuccessfulBid);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{newBidUrl}}", newBidUrl);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);

      ParseCommonData(template);

      UniMail.Mailer.Enqueue(template.Render(), bidding_mail);
    }

    public static void SendOutBidLetter(string emailTo, long auctionID, string loginName, string auctionName, string currentSuccessfulBid, string auctionEndDate, string newBidUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\outBid_login.txt"), bidding_mail) { Encoding = Encoding.UTF8, ToEmail = emailTo };

      template.Data.Add("{{auctionID}}", auctionID.ToString(CultureInfo.InvariantCulture));
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{currentSuccessfulBid}}", currentSuccessfulBid);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{newBidUrl}}", newBidUrl);
      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.Render(), bidding_mail);
    }

    public static void SendOutBidMultipleLetter(string FirstName, string LastName, string emailTo, long auctionID, string loginName, string auctionName, /*string currentSuccessfulBid,*/ string auctionEndDate, string curwinquanty, string quantity, string newBidUrl/*, bool full*/)
    {
      //UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath((full) ? @"~\Templates\Mail\outBidMultiple.txt" : @"~\Templates\Mail\outBidMultipleNotTotaly.txt"));
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\outBidMultiple.txt"), bidding_mail);
      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{auctionID}}", auctionID.ToString());
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{auctionName}}", auctionName);
      //template.Data.Add("{{currentSuccessfulBid}}", currentSuccessfulBid);
      template.Data.Add("{{quantity}}", quantity);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{newBidUrl}}", newBidUrl);
      //if (!full) 
      template.Data.Add("{{curwin}}", curwinquanty);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.Render(), bidding_mail);
    }
    public static void SendOutBidMultipleLetter(string emailTo, long auctionID, string loginName, string auctionName, /*string currentSuccessfulBid,*/ string auctionEndDate, string curwinquanty, string quantity, string newBidUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\outBidMultiple_login.txt"), bidding_mail);
      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{auctionID}}", auctionID.ToString());
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{quantity}}", quantity);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{newBidUrl}}", newBidUrl);
      template.Data.Add("{{curwin}}", curwinquanty);
      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.Render(), bidding_mail);
    }

    public static void SendFreeEmailRegisterConfirmation(string emailTo, string firstName, string lastName, string url)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\registerFreeEmailConfirm.txt"), registration_mail);

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{FirstName}}", firstName);
      template.Data.Add("{{LastName}}", lastName);
      template.Data.Add("{{Url}}", url);

      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.Render(), registration_mail);
    }

    public static void SendSuccessfulMultipleBidLetter(string FirstName, string LastName, string emailTo, long auctionID, string loginName, string auctionName, string auctionEndDate, long quantity, string usersbid, string curwinquanty, string newBidUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\successfulBidMultiple.txt"), bidding_mail);

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{auctionID}}", auctionID);
      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{quantity}}", quantity);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{usersbid}}", usersbid);
      template.Data.Add("{{curwin}}", curwinquanty);
      template.Data.Add("{{newBidUrl}}", newBidUrl);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);

      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.Render(), bidding_mail);
    }
    public static void SendSuccessfulMultipleBidLetter(string emailTo, long auctionID, string loginName, string auctionName, string auctionEndDate, long quantity, string usersbid, string curwinquanty, string newBidUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\successfulBidMultiple_login.txt"), bidding_mail);
      template.Encoding = Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{auctionID}}", auctionID);
      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{loginName}}", loginName);
      template.Data.Add("{{quantity}}", quantity);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{usersbid}}", usersbid);
      template.Data.Add("{{curwin}}", curwinquanty);
      template.Data.Add("{{newBidUrl}}", newBidUrl);

      ParseCommonData(template);
      UniMail.Mailer.Enqueue(template.Render(), bidding_mail);
    }

    public static void SendSuccessfulBidLetter(string FirstName, string LastName, string emailTo, long auctionID, string loginName, string auctionName, string usersbid, string usersmaxbid, string auctionEndDate, string newBidUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\successfulBid.txt"), bidding_mail);

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{auctionID}}", auctionID);
      template.Data.Add("{{loginName}}", loginName);

      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{usersbid}}", usersbid);
      template.Data.Add("{{usersmaxbid}}", usersmaxbid);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{newBidUrl}}", newBidUrl);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      ParseCommonData(template);

      UniMail.Mailer.Enqueue(template.Render(), bidding_mail);
    }

    public static void SendSuccessfulBidLetter(string emailTo, long auctionID, string loginName, string auctionName, string usersbid, string usersmaxbid, string auctionEndDate, string newBidUrl)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\successfulBid_login.txt"), bidding_mail) { Encoding = Encoding.UTF8, ToEmail = emailTo };

      template.Data.Add("{{auctionID}}", auctionID);
      template.Data.Add("{{loginName}}", loginName);

      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{usersbid}}", usersbid);
      template.Data.Add("{{usersmaxbid}}", usersmaxbid);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{newBidUrl}}", newBidUrl);

      ParseCommonData(template);

      UniMail.Mailer.Enqueue(template.Render(), bidding_mail);
    }

    public static void SendSuccessfulBidUpdateLetter(string FirstName, string LastName, string emailTo, long auctionID, string loginName, string auctionName, string usersbid, string usersmaxbid, string auctionEndDate, string newBidUrl, bool IsMaxBidOrAmount)
    {
      UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\bidUpdate.txt"), bidding_mail);

      template.Encoding = Encoding.UTF8;
      template.ToEmail = emailTo;
      template.Data.Add("{{auctionID}}", auctionID);
      template.Data.Add("{{loginName}}", loginName);

      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{usersbid}}", usersbid);
      template.Data.Add("{{usersmaxbid}}", usersmaxbid);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{newBidUrl}}", newBidUrl);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      if (IsMaxBidOrAmount)
      {
        template.Data.Add("{{change_maxbid}}", "maximum bid was raised to");
        template.Data.Add("{{was_not_change_maxbid}}", "");
        template.Data.Add("{{change_bid}}", "current bid in the amount of");
        template.Data.Add("{{was_not_change_bid}}", "was not changed");
      }
      else
      {
        template.Data.Add("{{change_maxbid}}", "current maximum bid in the amount of");
        template.Data.Add("{{was_not_change_maxbid}}", "was not change");
        template.Data.Add("{{change_bid}}", "bid was raised to");
        template.Data.Add("{{was_not_change_bid}}", "");
      }
      ParseCommonData(template);

      UniMail.Mailer.Enqueue(template.Render(), bidding_mail);
    }

    public static void SendSuccessfulBidUpdateLetter(string emailTo, long auctionID, string loginName, string auctionName, string usersbid, string usersmaxbid, string auctionEndDate, string newBidUrl, bool IsMaxBidOrAmount)
    {
      UniMail.Template template =
        new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\bidUpdate_login.txt"), bidding_mail) { Encoding = Encoding.UTF8, ToEmail = emailTo };

      template.Data.Add("{{auctionID}}", auctionID);
      template.Data.Add("{{loginName}}", loginName);

      template.Data.Add("{{auctionName}}", auctionName);
      template.Data.Add("{{usersbid}}", usersbid);
      template.Data.Add("{{usersmaxbid}}", usersmaxbid);
      template.Data.Add("{{auctionEndDate}}", auctionEndDate);
      template.Data.Add("{{newBidUrl}}", newBidUrl);

      if (IsMaxBidOrAmount)
      {
        template.Data.Add("{{change_maxbid}}", "maximum bid was raised to");
        template.Data.Add("{{was_not_change_maxbid}}", "");
        template.Data.Add("{{change_bid}}", "current bid in the amount of");
        template.Data.Add("{{was_not_change_bid}}", "was not changed");
      }
      else
      {
        template.Data.Add("{{change_maxbid}}", "current maximum bid in the amount of");
        template.Data.Add("{{was_not_change_maxbid}}", "was not change");
        template.Data.Add("{{change_bid}}", "bid was raised to");
        template.Data.Add("{{was_not_change_bid}}", "");
      }
      ParseCommonData(template);

      UniMail.Mailer.Enqueue(template.Render(), bidding_mail);
    }

    //public static void SendEndOfAuctionLetter(string FirstName, string LastName, string emailTo, long auctionID, string loginName, string auctionName, string auctionEndDate, string newBidUrl)
    //{
    //  UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(@"~\Templates\Mail\endOfAuction.txt"));

    //  template.Encoding = System.Text.Encoding.UTF8;
    //  template.ToEmail = emailTo;
    //  template.Data.Add("{{auctionID}}", auctionID);
    //  template.Data.Add("{{loginName}}", loginName);

    //  template.Data.Add("{{auctionName}}", auctionName);
    //  template.Data.Add("{{auctionEndDate}}", auctionEndDate);
    //  template.Data.Add("{{newBidUrl}}", newBidUrl);
    //  template.Data.Add("{{firstName}}", FirstName);
    //  template.Data.Add("{{lastName}}", LastName);
    //  ParseCommonData(template);

    //  UniMail.Mailer.Enqueue(template.Render());
    //}

    //public static void SendWinningLetter(string FirstName, string LastName, string emailTo, long auctionID, string loginName, string auctionName, bool isMultiple, string usersbid, string usersmaxbid, string curwin, string quantity, string auctionEndDate, string newBidUrl)
    //{
    //  UniMail.Template template = new UniMail.Template(HttpContext.Current.Server.MapPath(String.Format(@"~\Templates\Mail\{0}", (isMultiple) ? "winningBidMultiple.txt" : "winningBid.txt")));

    //  template.Encoding = System.Text.Encoding.UTF8;
    //  template.ToEmail = emailTo;
    //  template.Data.Add("{{auctionID}}", auctionID);
    //  template.Data.Add("{{loginName}}", loginName);
    //  template.Data.Add("{{quantity}}", quantity);
    //  template.Data.Add("{{curwin}}", curwin);
    //  template.Data.Add("{{auctionName}}", auctionName);
    //  template.Data.Add("{{usersbid}}", usersbid);
    //  template.Data.Add("{{usersmaxbid}}", usersmaxbid);
    //  template.Data.Add("{{auctionEndDate}}", auctionEndDate);
    //  template.Data.Add("{{newBidUrl}}", newBidUrl);
    //  template.Data.Add("{{firstName}}", FirstName);
    //  template.Data.Add("{{lastName}}", LastName);
    //  ParseCommonData(template);

    //  UniMail.Mailer.Enqueue(template.Render()); ;
    //}

    public static void SendRefundingDepositConfirmation(string FirstName, string LastName, string emailTo, decimal DepositAmount, string PrevTransactionID, string RefundedTransactionID, string DateFrom, string PaymentMethod, string templatePath, string SiteURL)
    {
      UniMail.Template template = new UniMail.Template(templatePath);

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;

      template.Data.Add("{{prevTransactionID}}", PrevTransactionID);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      template.Data.Add("{{dateFrom}}", DateFrom);
      template.Data.Add("{{paymentMethod}}", PaymentMethod);
      template.Data.Add("{{transactionID}}", RefundedTransactionID);
      template.Data.Add("{{amount}}", DepositAmount.GetCurrency());

      ParseCommonData(template, SiteURL);

      UniMail.Mailer.Enqueue(template.Render()); ;
    }

    public static void SendRefundingDepositConfirmation(string FirstName, string LastName, string emailTo, decimal DepositAmount, string PrevTransactionID, string RefundedTransactionID, string DateFrom, string PaymentMethod, StringBuilder email_template, string SiteURL)
    {
      UniMail.Template template = new UniMail.Template(email_template);

      template.Encoding = System.Text.Encoding.UTF8;
      template.ToEmail = emailTo;

      template.Data.Add("{{prevTransactionID}}", PrevTransactionID);
      template.Data.Add("{{firstName}}", FirstName);
      template.Data.Add("{{lastName}}", LastName);
      template.Data.Add("{{dateFrom}}", DateFrom);
      template.Data.Add("{{paymentMethod}}", PaymentMethod);
      template.Data.Add("{{transactionID}}", RefundedTransactionID);
      template.Data.Add("{{amount}}", DepositAmount.GetCurrency());

      ParseCommonData(template, SiteURL);

      UniMail.Mailer.Enqueue(template.Render()); ;
    }




  }
}