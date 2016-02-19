using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  [Serializable]
  public class BiddingHistory
  {
    public string Login { get; set; }
    public decimal Amount { get; set; }
    public DateTime DateMade { get; set; }
    public bool IsWinner { get; set; }
  }
}