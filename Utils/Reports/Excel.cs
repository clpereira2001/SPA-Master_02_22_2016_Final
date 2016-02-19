using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Vauction.Models;

namespace Vauction.Utils.Reports
{
  public static class ExcelReport
  {
    //AddValue
    public static void AddValue(ExcelWorksheet ws, object value, int row, int col, int? fontSize, string fontName,
                                 Color? fontColor, bool? bold, ExcelFillStyle? fillStyle, Color? backgroundColor,
                                 ExcelHorizontalAlignment? horizontalAlignment,
                                 ExcelVerticalAlignment? verticalAlignment, ExcelBorderStyle? borderStyle, bool italic)
    {
      ws.Cells[row, col].Style.Font.Italic = italic;
      if (value != null) ws.Cells[row, col].Value = value;
      if (fontSize.HasValue) ws.Cells[row, col].Style.Font.Size = fontSize.Value;
      if (fontColor.HasValue) ws.Cells[row, col].Style.Font.Color.SetColor(fontColor.Value);
      ws.Cells[row, col].Style.Font.Bold = bold.GetValueOrDefault(false);
      if (!string.IsNullOrEmpty(fontName)) ws.Cells[row, col].Style.Font.Name = fontName;
      if (fillStyle.HasValue && backgroundColor.HasValue)
      {
        ws.Cells[row, col].Style.Fill.PatternType = fillStyle.Value;
        ws.Cells[row, col].Style.Fill.BackgroundColor.SetColor(backgroundColor.Value);
      }
      ws.Cells[row, col].Style.HorizontalAlignment = horizontalAlignment.HasValue
                                                       ? horizontalAlignment.Value
                                                       : ExcelHorizontalAlignment.Center;
      ws.Cells[row, col].Style.VerticalAlignment = verticalAlignment.HasValue
                                                     ? verticalAlignment.Value
                                                     : ExcelVerticalAlignment.Center;
      if (borderStyle.HasValue)
      {
        var border = ws.Cells[row, col].Style.Border;
        border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = borderStyle.Value;
      }
    }

    public static List<string> GetExportAuctionConsignorItemsFields()
    {
      return new List<string> {
        "ID", "LotID", "Event", "Auction", "Category", "Price", "Cost", "Shipping", "InternalID", "Start Date"
      };
    }

    //ExportAuctionConsignorItems
    public static void ExportAuctionConsignorItems(string filePath, List<spAuction_View_ConsignorItemsResult> items, string docTitle, string docAuthor, string docComments, string docCompany)
    {
      FileInfo newFile = new FileInfo(filePath);
      if (newFile.Exists)
      {
        newFile.Delete();
        newFile = new FileInfo(filePath);
      }
      List<string> exportFields = GetExportAuctionConsignorItemsFields();
      using (ExcelPackage package = new ExcelPackage(newFile))
      {
        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Consigned Items");
        for (int i = 0; i < exportFields.Count; i++)
        {
          AddValue(worksheet, exportFields[i], 1, i + 1, 11, "Arial", Color.White, true, ExcelFillStyle.Solid, Color.FromArgb(40, 65, 95),
                   ExcelHorizontalAlignment.Center, ExcelVerticalAlignment.Center, ExcelBorderStyle.Medium, false);
        }
        for (int i = 0; i < items.Count; i++)
        {
          var item = items[i];
          AddValue(worksheet, item.Auction_ID.ToString(), 2 + i, 1, 11, "Arial", Color.Black, false, ExcelFillStyle.None,
                   null, ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center,
                   ExcelBorderStyle.None, false);
          AddValue(worksheet, item.Lot, 2 + i, 2, 11, "Arial", Color.Black, false, ExcelFillStyle.None,
                   null, ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center,
                   ExcelBorderStyle.None, false);
          AddValue(worksheet, item.EventTitle, 2 + i, 3, 11, "Arial", Color.Black, false, ExcelFillStyle.None,
                   null, ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center,
                   ExcelBorderStyle.None, false);
          AddValue(worksheet, item.Title, 2 + i, 4, 11, "Arial", Color.Black, false, ExcelFillStyle.None,
                   null, ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center,
                   ExcelBorderStyle.None, false);
          AddValue(worksheet, item.FullCategoy, 2 + i, 5, 11, "Arial", Color.Black, false,
                   ExcelFillStyle.None, null, ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center,
                   ExcelBorderStyle.None, false);
          AddValue(worksheet, item.Price.GetCurrency(false), 2 + i, 6, 11, "Arial", Color.Black, false,
                   ExcelFillStyle.None, null, ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center,
                   ExcelBorderStyle.None, false);
          AddValue(worksheet, item.Cost.GetCurrency(false), 2 + i, 7, 11, "Arial", Color.Black, false,
                   ExcelFillStyle.None, null, ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center,
                   ExcelBorderStyle.None, false);
          AddValue(worksheet, item.Shipping.GetCurrency(false), 2 + i, 8, 11, "Arial", Color.Black, false,
                   ExcelFillStyle.None, null, ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center,
                   ExcelBorderStyle.None, false);
          AddValue(worksheet, item.InternalID, 2 + i, 9, 11, "Arial", Color.Black, false,
                             ExcelFillStyle.None, null, ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center,
                             ExcelBorderStyle.None, false);
          AddValue(worksheet, item.IsConsignorShip.GetValueOrDefault(false) ? "Yes" : "No", 2 + i, 10, 11, "Arial", Color.Black, false,
                             ExcelFillStyle.None, null, ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center,
                             ExcelBorderStyle.None, false);
          AddValue(worksheet, item.StartDate.ToString(), 2 + i, 11, 11, "Arial", Color.Black, false,
                             ExcelFillStyle.None, null, ExcelHorizontalAlignment.Left, ExcelVerticalAlignment.Center,
                             ExcelBorderStyle.None, false);
        }
        package.Workbook.Properties.Title = docTitle;
        package.Workbook.Properties.Author = docAuthor;
        package.Workbook.Properties.Comments = docComments;
        package.Workbook.Properties.Company = docCompany;
        package.Save();
      }
    }
  }
}