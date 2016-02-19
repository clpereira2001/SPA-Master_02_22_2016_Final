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

using Vauction.Utils;

namespace Vauction.Models
{
    /// <summary>
    /// filter parametrs for cateoryes
    /// </summary>
    public class CategoryFilterParams : GeneralFilterParams
    {
      public override int GetHashCode()
      {
        return Id.GetHashCode() + Sortby.GetHashCode() + Orderby.GetHashCode() + ViewMode.GetHashCode() + page.GetHashCode() + PageSize.GetHashCode();
      }
        /// <summary>
        /// category is
        /// </summary>
        public int Id { get; set; }
        public new Consts.CategorySortFields? Sortby { get; set; }        
        public new Consts.OrderByValues? Orderby { get; set; }
    }
}
