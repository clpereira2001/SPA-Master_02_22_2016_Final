using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Vauction.Controllers;
using Vauction.Models;
using System.Net;
using System.IO;
using Vauction.Utils.Lib;

namespace Vauction.Utils.PayPal
{
  public class PP_Functions
  {
    public static StringBuilder InitializeRequest(string method)
    { 
      StringBuilder requestString = new StringBuilder();
      requestString.Append("METHOD=" + HttpUtility.UrlEncode(method));
      requestString.Append("&USER=" + HttpUtility.UrlEncode(Consts.PayPalAPIUser));
      requestString.Append("&PWD=" + HttpUtility.UrlEncode(Consts.PayPalAPIPassword));
      requestString.Append("&SIGNATURE=" + HttpUtility.UrlEncode(Consts.PayPalAPISignature));
      requestString.Append("&VERSION=" + HttpUtility.UrlEncode(Consts.PayPalVersion));
      return requestString;
    }

    public static bool Post(string request, out string token, out string payerID)
    {
      string transactionId, transactionType, errormessage;
      return Post(request, out token, out payerID, out transactionId, out transactionType, new Address(), out errormessage);
    }

    public static bool Post(string request, out string token, out string payerID, Address shipping, out string errormessage)
    {
      string transactionId, transactionType;
      return Post(request, out token, out payerID, out transactionId, out transactionType, shipping, out errormessage);
    }

    public static bool Post(string request, out string token, out string payerID, out string transactionId, out string transactionType, out string errormessage)
    { 
      return Post(request, out token, out payerID, out transactionId, out transactionType, new Address(), out errormessage);
    }

    public static bool Post(string request, out string token, out string payerID, out string transactionId, out string transactionType, Address shipping, out string errormessage)
    {
      token = errormessage = payerID = transactionId = transactionType = String.Empty;
      HttpWebResponse webResponse;
      try
      {
        HttpWebRequest webRequest = WebRequest.Create(Consts.PayPalEndPointUrl) as HttpWebRequest;
        webRequest.Method = "POST";
        webRequest.ContentType = "application/x-www-form-urlencoded";
        webRequest.ContentLength = request.Length;
        StreamWriter writer = new StreamWriter(webRequest.GetRequestStream());
        writer.Write(request);
        writer.Close();
        webResponse = webRequest.GetResponse() as HttpWebResponse;
        if (!webRequest.HaveResponse || (webResponse.StatusCode != HttpStatusCode.OK && webResponse.StatusCode != HttpStatusCode.Accepted))
          throw new Exception();
      }
      catch(Exception ex)
      {
        Logger.LogException(ex);
        return false;
      }
      StreamReader reader = new StreamReader(webResponse.GetResponseStream());
      string responseString = reader.ReadToEnd();
      reader.Close();

      bool success = false;
      char[] ampersand = { '&' };
      string[] pairs = responseString.Split(ampersand);
      char[] equalsign = { '=' };
      for (int i = 0; i < pairs.Length; i++)
      {
        string[] pair = pairs[i].Split(equalsign);
        if (pair[0].ToLower() == "ack" && !HttpUtility.UrlDecode(pair[1]).ToLower().Contains("failure"))
        {
          success = true;
          continue;
        }
        if (pair[0].ToLower() == "token")
        {
          token = HttpUtility.UrlDecode(pair[1]);
          continue;
        }
        if (pair[0].ToLower() == "payerid")
        {
          payerID = HttpUtility.UrlDecode(pair[1]);
          continue;
        }
        if (pair[0].ToLower() == "transactionid")
        {
          transactionId = HttpUtility.UrlDecode(pair[1]);
          continue;
        }
        if (pair[0].ToLower() == "refundtransactionid")
        {
          transactionId = HttpUtility.UrlDecode(pair[1]);
          continue;
        }
        if (pair[0].ToLower() == "transactiontype")
        {
          transactionType = HttpUtility.UrlDecode(pair[1]);
          continue;
        }
        if (pair[0].ToLower() == "l_errorcode0")
        {
          errormessage = HttpUtility.UrlDecode(pair[1]);
          continue;
        }
        if (pair[0].ToLower() == "l_shortmessage0")
        {
          errormessage += String.IsNullOrEmpty(errormessage)?String.Empty:". " + HttpUtility.UrlDecode(pair[1]);
          continue;
        }
        if (pair[0].ToLower() == "l_longmessage0")
        {
          errormessage += String.IsNullOrEmpty(errormessage) ? String.Empty : ". " + HttpUtility.UrlDecode(pair[1]);
          continue;
        }
        if (pair[0].ToLower() == "shiptoname")
        {
          string[] snp = HttpUtility.UrlDecode(pair[1]).Split(' ');
          if (snp.Length > 0) shipping.FirstName = snp[0];
          if (snp.Length == 2)
            shipping.LastName = snp[1];
          else if (snp.Length == 3)
          {
            shipping.MiddleName = snp[1];
            shipping.LastName = snp[2];
          }
          continue;
        }
        if (pair[0].ToLower() == "shiptostreet")
        {
          shipping.Address_1 = HttpUtility.UrlDecode(pair[1]);
          continue;
        }
        if (pair[0].ToLower() == "shiptostreet2")
        {
          shipping.Address_2 = HttpUtility.UrlDecode(pair[1]);
          continue;
        }
        if (pair[0].ToLower() == "shiptocity")
        {
          shipping.City = HttpUtility.UrlDecode(pair[1]);
          continue;
        }
        if (pair[0].ToLower() == "shiptostate")
        {
          shipping.State = HttpUtility.UrlDecode(pair[1]);
          continue;
        }
        if (pair[0].ToLower() == "shiptozip")
        {
          shipping.Zip = HttpUtility.UrlDecode(pair[1]);
          continue;
        }
        if (pair[0].ToLower() == "shiptocountryname")
        {
          shipping.Country = HttpUtility.UrlDecode(pair[1]);
          continue;
        }
      }
      return success;
    }

    public static bool GetTransactionDetails(string request, out decimal amount)
    {
      HttpWebResponse webResponse;
      amount = 0;
      try
      {
        HttpWebRequest webRequest = WebRequest.Create(Consts.PayPalEndPointUrl) as HttpWebRequest;
        webRequest.Method = "POST";
        webRequest.ContentType = "application/x-www-form-urlencoded";
        webRequest.ContentLength = request.Length;
        StreamWriter writer = new StreamWriter(webRequest.GetRequestStream());
        writer.Write(request);
        writer.Close();
        webResponse = webRequest.GetResponse() as HttpWebResponse;
        if (!webRequest.HaveResponse ||
              (webResponse.StatusCode != HttpStatusCode.OK && webResponse.StatusCode != HttpStatusCode.Accepted))
        {
          throw new Exception();
        }
      }
      catch
      {
        return false;
      }
      StreamReader reader = new StreamReader(webResponse.GetResponseStream());
      string responseString = reader.ReadToEnd();
      reader.Close();

      bool success = false;
      char[] ampersand = { '&' };
      string[] pairs = responseString.Split(ampersand);
      char[] equalsign = { '=' };
      for (int i = 0; i < pairs.Length; i++)
      {
        string[] pair = pairs[i].Split(equalsign);
        if (pair[0].ToLower() == "ack" && HttpUtility.UrlDecode(pair[1]).ToLower() != "failure")
        {
          success = true;
          continue;
        }
        if (pair[0].ToLower() == "amt")
        {
          amount = Convert.ToDecimal(HttpUtility.UrlDecode(pair[1]));
          continue;
        }
      }
      return success;
    }
  }
}