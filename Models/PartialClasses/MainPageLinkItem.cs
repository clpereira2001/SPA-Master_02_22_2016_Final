using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  [Serializable]
  partial class MainPageLinkItem : IMainPageLinkItem
  {
    public string CategoryName
    {
      get { return Category.Title; }
    }

    public long Position
    {
      get { return PositionNuber; }
    }
  }
}
