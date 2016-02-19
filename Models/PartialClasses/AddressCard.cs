using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Vauction.Models
{
  [Serializable]
  partial class AddressCard : IAddressCard
  {
    public string GetAddressText(string separator)
    {
      return String.Format("Addr1: {0}{6}Addr2: {1}{6}City: {2}{6}State: {3}{6}Country: {4}{6}Zip: {5}", Address1, Address2, City, State, (Country == null) ? String.Empty : Country.Title, Zip, separator);
    }
    public string GetAddressText2(string separator)
    {
      return String.Format("{0} {1}{6} {2}{6} {3}{6} {4}{6} {5}", Address1, Address2, City, State, (Country == null) ? String.Empty : Country.Title, Zip, separator);
    }

    public string FullAddress
    {
      get { return GetAddressText("|"); }
    }
    public string FullAddress2
    {
      get { return GetAddressText2("|"); }
    }

    public string FullName
    {
      get { return String.Format("{0} {1}{2}", FirstName, String.IsNullOrEmpty(MiddleName) ? "" : MiddleName + " ", LastName); }
    }
  }
}
