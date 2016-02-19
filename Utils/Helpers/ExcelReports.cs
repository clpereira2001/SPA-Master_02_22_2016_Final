using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Drawing;
using Vauction.Models.CustomModels;

namespace Vauction.Utils.Helpers
{
  //public class ExcelReports
  //{
  //  public static void ConsignorSettlementReport(IEnumerable<ConsignorSettlement> ConsignorSettlementList, Vauction.Models.Event currentEvent, Vauction.Models.User currentUser )
  //  {
  //    Workbook xlWorkBook;
  //    Worksheet xlWorkSheet;

  //    xlWorkBook = new Application().Workbooks.Add(Missing.Value);
  //    xlWorkBook.Application.Visible = true;
  //    xlWorkSheet = (Worksheet)xlWorkBook.ActiveSheet;
  //    xlWorkSheet.Name = "Consignor Settlement Report";

  //    Range rg = xlWorkSheet.get_Range(xlWorkSheet.Cells[1, 1], xlWorkSheet.Cells[1, 10]);
  //    rg.MergeCells = true;
  //    rg.Font.Size = 16;
  //    rg.Font.Bold = true;
  //    rg.Font.Color = ColorTranslator.ToOle(Color.Brown);
  //    rg.Value2 = "Consignor Settlement Report";

  //    int rowIndex = 3;

  //    rg = xlWorkSheet.get_Range(xlWorkSheet.Cells[rowIndex, 1], xlWorkSheet.Cells[rowIndex, 10]);
  //    rg.MergeCells = true;      
  //    rg.Value2 = String.Format("Generate Date: {0}", DateTime.Now);

  //    rowIndex++;
  //    rg = xlWorkSheet.get_Range(xlWorkSheet.Cells[rowIndex, 1], xlWorkSheet.Cells[rowIndex, 10]);
  //    rg.MergeCells = true;      
  //    rg.Value2 = String.Format("Event: {0} ({1}-{2})", currentEvent.Title, currentEvent.DateStart, currentEvent.DateEnd);
      
  //    rowIndex++;
  //    if (currentUser != null)
  //    {
  //      rg = xlWorkSheet.get_Range(xlWorkSheet.Cells[rowIndex, 1], xlWorkSheet.Cells[rowIndex, 10]);
  //      rg.MergeCells = true;        
  //      rg.Value2 = String.Format("Consignor: {0} {1} {2} ({3})", currentUser.AddressCard.FirstName, currentUser.AddressCard.MiddleName, currentUser.AddressCard.LastName, currentUser.Login.ToUpper());
  //      rowIndex++;
  //    }

  //    rowIndex++;
  //    int colIndex = 1;

  //    if (currentUser == null)
  //      xlWorkSheet.Cells[rowIndex, colIndex++] = "Seller";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Lot#";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Title";      
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Status";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Cost";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Quantity";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Bid";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Ext";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Buyer's Premium";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Tax";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Insurance";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Shipping";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Total Due";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Discount";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Total Payment";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Unpaid";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Commission";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "To Consignor";
  //    xlWorkSheet.Cells[rowIndex, colIndex++] = "Commission Descr.";      
  //    xlWorkSheet.Cells[rowIndex, colIndex] = "Consignor ship";

  //    rg = xlWorkSheet.get_Range(xlWorkSheet.Cells[rowIndex, 1], xlWorkSheet.Cells[rowIndex, colIndex]);
  //    rg.Font.Bold = true;
  //    rg.Font.Color = System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.White);
  //    rg.Interior.Color = System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.Navy);

  //    int tableStartRow = rowIndex;
  //    int col;
  //    foreach (ConsignorSettlement cs in ConsignorSettlementList)
  //    {        
  //      rowIndex++;
  //      col = 1;
  //      if (currentUser==null)
  //        xlWorkSheet.Cells[rowIndex, col++] = cs.Seller;
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.Lot;
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.Description;        
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.IsSold;
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.Cost.GetCurrency();
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.Quantity;
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.WinningBid.GetCurrency();
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.Amount.GetCurrency();
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.BuyersPremium.GetCurrency();
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.Tax.GetCurrency();
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.Insurance.GetCurrency();
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.Shipping.GetCurrency();
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.TotalDue.GetCurrency();
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.Discount.GetCurrency();
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.TotalPayment.GetCurrency();
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.Unpaid.GetCurrency();
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.Commission.GetCurrency();
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.ToConsignor.GetCurrency();
  //      xlWorkSheet.Cells[rowIndex, col++] = cs.CommissionRateType;        
  //      xlWorkSheet.Cells[rowIndex, col] = (cs.IsConsignorShip)?"Yes":"No";

  //      rg = xlWorkSheet.get_Range(xlWorkSheet.Cells[rowIndex, 1], xlWorkSheet.Cells[rowIndex, col]);
  //      rg.Interior.Color = (cs.IsSold == "Unsold") ? ColorTranslator.ToOle(Color.LightYellow) : ColorTranslator.ToOle(Color.LightGreen);
  //    }

  //    rg = xlWorkSheet.get_Range(xlWorkSheet.Cells[tableStartRow, 1], xlWorkSheet.Cells[rowIndex, colIndex]);
  //    rg.Borders.LineStyle = XlLineStyle.xlContinuous;
  //    rg.Borders.Weight = XlBorderWeight.xlThin;

  //    rowIndex++;

  //    rg = xlWorkSheet.get_Range(xlWorkSheet.Cells[rowIndex, 1], xlWorkSheet.Cells[rowIndex, 3]);
  //    rg.Font.Bold = true;
  //    rg.MergeCells = true;
  //    rg.Value2 = "Grand Total:";

  //    rg = xlWorkSheet.get_Range(xlWorkSheet.Cells[rowIndex, ((currentUser == null) ? 8 : 7)], xlWorkSheet.Cells[rowIndex, ((currentUser == null) ? 18 : 17)]);
  //    rg.Borders.LineStyle = XlLineStyle.xlContinuous;
  //    rg.Borders.Weight = XlBorderWeight.xlThin;
  //    rg.Font.Bold = true;
  //    rg.Interior.Color = ColorTranslator.ToOle(Color.LightGray);

  //    for (int i = 0; i < 11; i++)
  //    {
  //      rg = xlWorkSheet.get_Range(xlWorkSheet.Cells[rowIndex, ((currentUser == null) ? 8 : 7) + i], xlWorkSheet.Cells[rowIndex, ((currentUser == null) ? 8 : 7) + i]);
  //      rg.Formula = String.Format("=SUM({0}{1}:{0}{2})", ((char)((int)(((currentUser == null) ? 'H' : 'G')) + i)).ToString(), tableStartRow, rowIndex - 1);
  //      rg.NumberFormat = "$0.00";
  //    }
      
      
  //    xlWorkSheet.Columns.AutoFit();

  //    GC.Collect();
  //  }
  //}
}
