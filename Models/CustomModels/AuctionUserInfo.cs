﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  [Serializable]
  public class AuctionUserInfo
  {
    public bool IsRegisterForEvent { get; set; }
    public bool IsInWatchList { get; set; }
  }
}