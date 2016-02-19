using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models.Enums
{
    public class CategoryParentChild
    {
        public CategoryParentChild()
        {
            this.Childs = new List<CategoryChild>();
        }
        public string parentLinkName { get; set; }
        public string parentLinkUrl { get; set; }
        public int Parount { get; set; }
        public virtual List<CategoryChild> Childs { get; set; }

    }
    public class CategoryChild
    {
        public string ChildLinkName { get; set; }
        public string ChildLinkUrl { get; set; }
        public virtual CategoryParentChild CategoryParentChild { get; set; }
    }
}