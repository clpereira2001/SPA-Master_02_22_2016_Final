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
  [Serializable]
  public class SelectionCategory
  {
    public long id;
    public string Id;
    public string Title;
    public bool? Selected;

    public SelectionCategory()
    {
    }
    public SelectionCategory(long id, string title, bool sel)
    {
      this.id = id;
      Id = "select-" + id.ToString();
      Title = title;
      Selected = sel;
    }
  }
}
