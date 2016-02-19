using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vauction.Models
{
    public interface IPersonalShopperItem
    {
        Int64 ID { get; set; }
		    Int64? Category_ID1 { get; set; }
        Int64? Category_ID2 { get; set; }
        Int64? Category_ID3 { get; set; }
        Int64? Category_ID4 { get; set; }
        Int64? Category_ID5 { get; set; }
		    Int64 User_ID { get; set; }
        string Keyword1 { get; set; }
        string Keyword2 { get; set; }
        string Keyword3 { get; set; }
        string Keyword4 { get; set; }
        string Keyword5 { get; set; }
        Boolean IsActive { get; set; }
        DateTime DateExpires { get; set; }
        string sDateExpires { get; }
        bool IsHTML { get; set; }
    }
}
