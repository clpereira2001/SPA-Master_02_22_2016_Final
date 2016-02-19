using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models.Enums
{
    public class CategoryParentLink
    {
        public string parentLinkName { get; set; }
        public string parentLinkUrl { get; set; }
        public int Parount { get; set; }
        public List<CategoryChildLink> CategoryChildLink { get; set; }
    }
}