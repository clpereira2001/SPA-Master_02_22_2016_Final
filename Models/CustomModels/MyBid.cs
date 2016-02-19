using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils;

namespace Vauction.Models
{
  [Serializable]
  public class MyBid
  {
    public decimal Amount { get; set; }
    public decimal MaxBid { get; set; }
    public decimal? CurrentBid_1 { get; set; }
    public decimal? CurrentBid_2 { get; set; }
    public DateTime DateMade { get; set; }    
    public bool IsWinner { get; set; }    
    public string ThubnailImage { get; set; }
    
    public LinkParams LinkParams { get; set; }

    public string CurrentBid
    {
      get { return HasBid?String.Format("{0}{1}{2}", CurrentBid_1.GetCurrency(), (CurrentBid_2.HasValue && CurrentBid_1 != CurrentBid_2.Value) ? " - " : String.Empty, (CurrentBid_2.HasValue && CurrentBid_1 != CurrentBid_2.Value) ? CurrentBid_2.GetValueOrDefault(0).GetCurrency() : String.Empty):string.Empty; }
    }

    public bool HasBid
    {
      get { return CurrentBid_1.HasValue && CurrentBid_1.Value>0; }
    }
  }
}
