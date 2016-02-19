using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Vauction.Utils.Paging
{
    public class Pager
    {
        private ViewContext viewContext;
        private readonly int pageSize;
        private readonly int currentPage;
        private readonly int totalItemCount;
        private readonly RouteValueDictionary linkWithoutPageValuesDictionary;

        public Pager(ViewContext viewContext, int pageSize, int currentPage, int totalItemCount, RouteValueDictionary valuesDictionary)
        {
            this.viewContext = viewContext;
            this.pageSize = pageSize;
            this.currentPage = currentPage;
            this.totalItemCount = totalItemCount;
            this.linkWithoutPageValuesDictionary = valuesDictionary;
        }

        public string RenderHtml()
        {
            int pageCount = (int)Math.Ceiling(this.totalItemCount / (double)this.pageSize);
            int maxElemCount = 7;
            int minElem = 3;

            var sb = new StringBuilder();
            // pageCount = 4;
            if (pageCount == 1)
            {



                sb.Append("<nav class=\"pull-right\">");
                sb.Append("<ul class=\"pagination margin-vertical-xs\">");
                sb.Append("<li class=\"disabled text-xs\"><a aria-label=\"Previous\" ><span aria-hidden=\"true\"><</span></a></li>");
                sb.Append("<li class=\"disabled\"><span class=\"text-uppercase text-xs\">page</span></li>");
                sb.Append(String.Format("<li class=\"active text-xs\"> <a href='#'>{0}</a></li>", pageCount));
                sb.Append(String.Format("<li class=\"disabled text-xs\"><span class=\"text-uppercase text-xs\"> {0} of</span></li>", totalItemCount));
                sb.Append("<li class=\"text-xs\"><a aria-label=\"Next\" ><span aria-hidden=\"true\">></span></a></li>");
                sb.Append("</ul>");
                sb.Append("</nav>");



                //sb.Append("<span class=\"pager\">");
                //sb.Append(String.Format("<span class=\"page_box disabled\" style='vertical-align:top;'>{0} item(s)</span>", totalItemCount, pageCount));
                //sb.Append("</span>");
                return sb.ToString();
            }

            sb.Append("<nav class=\"pull-right\">");
            // sb.Append("<span class=\"pager\">");
            //sb.Append(String.Format("<span class=\"page_box disabled\" style='vertical-align:top;'>{0} item(s) in {1} page(s)</span>", totalItemCount, pageCount));
            sb.Append("<ul class=\"pagination margin-vertical-xs\">");
            // Previous
            sb.Append(this.currentPage > 1 ? GeneratePageLink("Previous", "<", this.currentPage - 1) : "<li class=\"disabled text-xs\"><a aria-label=\"Previous\" ><span aria-hidden=\"true\"><</span></a></li>");


            sb.Append("<li class=\"disabled\"><span class=\"text-uppercase text-xs\">page</span></li>");
            sb.Append(String.Format("<li class=\"active text-xs\"> <a href='#'>{0}</a></li>", this.currentPage));
            sb.Append(String.Format("<li class=\"disabled text-xs\"><span class=\"text-uppercase text-xs\">of {0} </span></li>", pageCount));





            //int start = 1;
            //int end = pageCount;
            //if (pageCount > maxElemCount)
            //{
            //    int middle = (int)Math.Ceiling(minElem / 2d) - 1;
            //    int below = (this.currentPage - middle);
            //    int above = (this.currentPage + middle);

            //    if (below < minElem)
            //    {
            //        above = minElem;
            //        below = 1;
            //    }
            //    else if (above > (pageCount - minElem + 1))
            //    {
            //        above = pageCount;
            //        below = (pageCount - minElem + 1);
            //    }
            //    start = below;
            //    end = above;
            //}

            //if (pageCount <= maxElemCount)
            //{
            //    for (int i = 1; i <= pageCount; i++)
            //        sb.Append(i == this.currentPage ? String.Format("<span class=\"page_box\" style='text-decoration:underline;color:#00C'>{0}</span>", i) : GeneratePageLink(i.ToString(), i));
            //}
            //else
            //{
            //    if (start >= minElem)
            //    {
            //        for (int i = 1; i <= (currentPage - minElem > minElem ? minElem : currentPage - minElem); i++)
            //            sb.Append(GeneratePageLink(i.ToString(), i));
            //        sb.Append("<span class=\"page_box disabled\" style='vertical-align:top;'>...</span>");
            //    }

            //    if (currentPage == pageCount - minElem + 1)
            //        sb.Append(GeneratePageLink((currentPage - 1).ToString(), (currentPage - 1)));

            //    for (int i = start; i <= end; i++)
            //        sb.Append(i == this.currentPage ? String.Format("<span class=\"page_box\" style='text-decoration:underline;color:#00C'>{0}</span>", i) : GeneratePageLink(i.ToString(), i));
            //    if (currentPage == minElem)
            //        sb.Append(GeneratePageLink((currentPage + 1).ToString(), (currentPage + 1)));

            //    if (pageCount - start >= minElem)
            //    {
            //        sb.Append("<span class=\"page_box disabled\" style='vertical-align:top;'>...</span>");
            //        for (int i = (pageCount - minElem > end ? pageCount - minElem + 1 : currentPage + minElem); i <= pageCount; i++)
            //            sb.Append(GeneratePageLink(i.ToString(), i));
            //    }
            //}

            // Next
            sb.Append(this.currentPage < pageCount ? GeneratePageLink("Next", ">", (this.currentPage + 1)) : "<li class=\"text-xs\"><a aria-label=\"Next\" ><span aria-hidden=\"true\">></span></a></li>");
            sb.Append("</ul>");
            sb.Append("</nav>");
            return sb.ToString();
        }

        private string GeneratePageLink(string labelLinkText, string linkText, int pageNumber)
        {
            var pageLinkValueDictionary = new RouteValueDictionary(this.linkWithoutPageValuesDictionary);

            if (!pageLinkValueDictionary.ContainsKey("page"))
            {
                pageLinkValueDictionary.Add("page", pageNumber);
            }
            else
                pageLinkValueDictionary["page"] = pageNumber;
            //var virtualPathData = this.viewContext.RouteData.Route.GetVirtualPath(this.viewContext, pageLinkValueDictionary);
            var tmpRequestContext = this.viewContext.RequestContext;
            if (this.viewContext.ViewData["PageActionPath"] != null && !String.IsNullOrEmpty(this.viewContext.ViewData["PageActionPath"].ToString()))
                tmpRequestContext.RouteData.Values["action"] = this.viewContext.ViewData["PageActionPath"].ToString();

            var virtualPathData = RouteTable.Routes.GetVirtualPath(tmpRequestContext, pageLinkValueDictionary);
            //var virtualPathData = RouteTable.Routes.GetVirtualPath(this.viewContext.RequestContext, pageLinkValueDictionary);

            if (virtualPathData != null)
            {
                string linkFormat = "<li class=\"text-xs\"><a aria-label='" + labelLinkText + "' class=\"page_box\" href=\"{0}\"><span aria-hidden=\"true\">{1}</span></a></li>";
                //string linkFormat = "<a aria-label=\"Previous\" href=\"{0}\"><span aria-hidden=\"true\"><</span></a>";
                //string linkFormat = "<a href=\"{0}\" aria-label=\"Previous\"><span aria-hidden=\"true\">{1}</span></a>";

                return String.Format(linkFormat, virtualPathData.VirtualPath, linkText);
            }
            else
            {
                return null;
            }
        }
    }
}