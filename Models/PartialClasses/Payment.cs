using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Vauction.Models
{
  [Serializable]
  partial class Payment : IPayment
  {
    public string FullAddress
    {
      get { return Address + "|" + City + "|" + State + "|" + Zip; }
    }

    public string GetSubstring(int index)
    {
      if (String.IsNullOrEmpty(ShippingAddress)) return String.Empty;
      string[] str = ShippingAddress.Split('|');
      return (index < str.Length) ? str[index] : String.Empty;
    }

    public bool IsPickedUp
    {
      get { return String.Compare("PICK UP", ShippingAddress) == 0; }
    }

    public string Shipping_Address
    {
      get { return GetSubstring(1); }
    }
    public string Shipping_City
    {
      get { return GetSubstring(2); }
    }
    public string Shipping_State
    {
      get { return GetSubstring(3); }
    }
    public string Shipping_Country
    {
      get { return GetSubstring(4); }
    }
    public string Shipping_Zip
    {
      get { return GetSubstring(5); }
    }

    public string ShippingAddressComma
    {
      get
      {
        StringBuilder sb = new StringBuilder(ShippingAddress);
        sb.Replace(" |", ",");
        sb.Replace("|", ",");
        sb.Replace(" ,", ",");
        sb.Replace(",,", ",");
        return sb.ToString();
      }
    }
  }
}
