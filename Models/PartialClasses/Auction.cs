using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils;
using System.Text;
using System.Text.RegularExpressions;

namespace Vauction.Models
{
  [Serializable]
  public partial class Auction : IAuction
  {
    public string DefaultThumb
    {
      get
      {
        Image img = Images.OrderByDescending(I => I.Default).FirstOrDefault();
        return (img == null) ? String.Empty : img.ThumbNailPath;
      }
    }

    public Category Category
    {
      get { return (EventCategory != null) ? EventCategory.CategoriesMap.Category : null; }
    }

    public string FullCategory
    {
      get { return (EventCategory != null) ? EventCategory.FullCategory : null; }
    }

    public string DescriptionWithoutTags
    {
      get
      {
        StringBuilder sb = new StringBuilder(Description);
        //sb.Replace("<", "&lt;");
        //sb.Replace(">", "&gt;");
        Regex regex = new Regex(@"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", RegexOptions.Singleline);
        MatchCollection matches = regex.Matches(sb.ToString());
        foreach (Match mch in matches)
          sb.Replace(mch.Value, "");
        return sb.ToString();
      }
    }

    public string DescriptionWithoutTagsAnd34
    {
      get { return DescriptionWithoutTags.Replace("\"", "&#34;"); }
    }

    public string DescriptionWithoutTagsShort
    {
      get
      {
        StringBuilder sb = new StringBuilder(DescriptionWithoutTags);
        if (sb.Length > 128) { sb.Remove(100, sb.Length - 100); sb.Append(" ... "); }
        return sb.ToString();
      }
    }

    public string LotTitle
    {
      get { return String.Format("Lot#{0}-{1}", Lot, Title.Replace("\"", "'")); }
    }

    public string UrlTitle
    {
      get { return Vauction.Utils.UrlParser.TitleToUrl(LotTitle); }
    }

    public string UrlCategory
    {
      get { return Vauction.Utils.UrlParser.TitleToUrl(FullCategory); }
    }
    public string StartTime
    {
      get { return String.Format("{0} ET", StartDate); }
    }
    public string EndTime
    {
      get { return String.Format("{0} ET", EndDate); }
    }
    public string StartEndTime
    {
      get { return String.Format("({0} to {1})", StartTime, EndTime); }
    }
  }
}
