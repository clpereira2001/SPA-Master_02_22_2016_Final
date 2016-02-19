using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
    public interface IMainPageLinkItem
    {
        Int64 ID { get; set; }
	    Int64 Category_ID { get; set; }
	    string Picture_URL { get; set; }
	    Int64 PositionNuber { get; set; }

        String CategoryName { get; }
        Int64 Position {get;}
    }
}
