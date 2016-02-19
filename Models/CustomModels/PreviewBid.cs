using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  [Serializable]
  public class PreviewBid
  {
    public bool IsProxy { get; set; }
    public decimal Amount { get; set; }
    public bool IsUpdate { get; set; }
    public bool IsOutBid { get; set; }
    public int Quantity { get; set; }
    public bool IsQuantityBig { get; set; }
    public decimal RealAmount { get; set; }
    public decimal PreviousAmount { get; set; }
    public decimal PreviousMaxBid { get; set; }
    public byte QuantityType { get; set; }
    public decimal DepositNeed { get; set; }
    public decimal PreviousDeposit { get; set; }
    public int PreviousQuantity { get; set; }
    public int WinnerBidsCount { get; set; }
    public int LoserBidsCount { get; set; }

    public LinkParams LinkParams { get; set; }

    public PreviewBid()
    {
      DepositNeed = PreviousDeposit = PreviousQuantity = WinnerBidsCount = LoserBidsCount = 0;
      IsUpdate = IsOutBid = IsQuantityBig = false;
      QuantityType = 255;
    }
    
    public decimal TotalRealAmount
    {
      get { return RealAmount * Quantity; }
    }

    public decimal TotalDeposit
    {
      get { return DepositNeed + PreviousDeposit; }
    }
  }
}