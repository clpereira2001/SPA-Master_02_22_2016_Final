using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models.Enums
{
    public class CategoryChildLink : CategoryParentLink
    {
        public string ChildLinkName { get; set; }
        public string ChildLinkUrl { get; set; }
        
        // public int Child { get; set; } 
    }
}