using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
  [Serializable]
  public class Dow
  {
    public int Quantity { get; set; }
    public int WQuantity { get; set; }
    public decimal Amount { get; set; }
    public decimal BuyerPremium { get; set; }
    public decimal Shipping { get; set; }
    public decimal Insurance { get; set; }
    public decimal Tax { get; set; }
    public bool IsDOW { get; set; }
    
    public bool IsConsignorShip { get; set; }
    public decimal Price { get; set; }
    public decimal? BuyPrice {get; set;}

    public bool IsShipping { get; set; }

    public LinkParams LinkParams { get; set; }

    public decimal TotalCost
    {
      get { return Amount + BuyerPremium + Shipping + Insurance + Tax; }
    }
    
    public Dow(bool isdow)
    {
      Quantity = WQuantity = 0;
      Amount = BuyerPremium = Shipping = Insurance = Tax = 0;
      IsDOW = isdow;
    }
  }
}