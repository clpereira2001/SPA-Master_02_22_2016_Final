using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.ComponentModel;
using Vauction.Configuration;
using System.Text;
using System.Collections.Generic;
using Vauction.Utils;

namespace Vauction.Models.CustomModels
{
  /// <summary>
  /// filter parameter for auction
  /// </summary>
  public class AuctionFilterParams : GeneralFilterParams
  {
    public override int GetHashCode()
    {
      return page.GetHashCode()+ViewMode.GetHashCode() + ShortFilterString().GetHashCode() + ShortSimpleFilterString().GetHashCode() + ID.GetHashCode();
    }
    
    public new Consts.CategorySortFields? Sortby { get; set; }
    public new Consts.OrderByValues? Orderby { get; set; }
    /// <summary>
    /// create simple filter with ktitle and description
    /// </summary>
    /// <param name="keyword"></param>
    public AuctionFilterParams(string keyword)
    {
      Description = keyword;
      Title = keyword;
    }
    public AuctionFilterParams() { }
    /// <summary>
    /// auction id
    /// </summary>
    public int? ID { get; set; }
    /// <summary>
    /// lot number
    /// </summary>
    public int? LotNo { get; set; }
    /// <summary>
    /// start price
    /// </summary>
    public decimal? FromPrice { get; set; }
    /// <summary>
    /// finish price
    /// </summary>
    public decimal? ToPrice { get; set; }
    /// <summary>
    /// selectded category
    /// </summary>
    public long? SelectedCategory { get; set; }
    /// <summary>
    /// auction shood contains photo or not
    /// </summary>
    public bool? WithPhoto { get; set; }
    /// <summary>
    /// auction description
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// auction title
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// include auctions closed in 30 days
    /// </summary>
    public bool? ClosedAuctions { get; set; }

    /// <summary>
    /// create string with short description of filter, separated by comma
    /// </summary>
    /// <returns></returns>
    public string ShortFilterString()
    {
      StringBuilder sb = new StringBuilder();
      if (!string.IsNullOrEmpty(this.Title))
        sb.AppendFormat("Title:{0}, ", this.Title);
      if (!string.IsNullOrEmpty(this.Description))
        sb.AppendFormat("Description:{0}, ", this.Description);
      if (this.FromPrice.HasValue)
        sb.AppendFormat("Price from:{0}, ", this.FromPrice);
      if (this.ToPrice.HasValue)
        sb.AppendFormat("Price to:{0}, ", this.ToPrice);
      if (this.WithPhoto.HasValue)
        if (this.WithPhoto.Value)
          sb.Append("With photo");
        else
          sb.Append("Without photo");
      //else
      //    sb.Append("Without photo");
      return sb.ToString();
    }

    public string ShortSimpleFilterString()
    {
      StringBuilder sb = new StringBuilder();
      //sb.AppendFormat(LotNo.HasValue? " Lot: {0}" : " Title, Description: {0}", LotNo.HasValue?LotNo.Value.ToString() : Title ?? Description);      
      sb.AppendFormat(" Lot, Title, Description: {0}", Title ?? Description);      
      return sb.ToString();
    }
    #region private filds    
    private ICategoryRepository categoryRepositary = ((IVauctionConfiguration)ConfigurationManager.GetSection("Vauction")).DataProvider.GetInstance().CategoryRepository;    
    #endregion
  }
}
