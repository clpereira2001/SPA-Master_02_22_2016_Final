using System;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  partial class PersonalShopperItem : IPersonalShopperItem
  {
    public string sDateExpires
    {
      get
      {
        return DateExpires.Text();
      }
      set
      {
        DateExpires = value.ToDate();
      }
    }
  }
}
