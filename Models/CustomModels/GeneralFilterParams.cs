﻿using System.ComponentModel;
using Vauction.Utils;

namespace Vauction.Models
{
  public abstract class GeneralFilterParams
  {
    public GeneralFilterParams() { }
    [DefaultValue(Consts.CategorySortFields.Title)]
    public Consts.CategorySortFields Sortby { get; set; }
    [DefaultValue(Consts.OrderByValues.ascending)]
    public Consts.OrderByValues Orderby { get; set; }
    public int page { get; set; }
    [DefaultValue(21)]
    public int PageSize { get; set; }
    [DefaultValue((int)Consts.AuctonViewMode.List)]
    public int ViewMode { get; set; }
  }
}
