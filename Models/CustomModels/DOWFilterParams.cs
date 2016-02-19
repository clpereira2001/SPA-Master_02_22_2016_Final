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

namespace Vauction.Models.CustomModels
{
    /// <summary>
    /// filter parametrs for cateoryes
    /// </summary>
    public class DOWFilterParams : GeneralFilterParams
    {
        /// <summary>
        /// category is
        /// </summary>
        public long aaid { get; set; }
        public int Quantity { get; set; }
    }
}
