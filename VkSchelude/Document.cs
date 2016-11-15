using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Excel;
using VkSchelude.Types;

namespace VkSchelude
{
    public class Document
    {
        public void Parse(string excelDocPath)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(excelDocPath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Worksheet Worksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;
            int j = 3;
            int rowCount = xlRange.Rows.Count;
            for (int i = 1; i <= rowCount; i++)
            {
                // первый аргумент - номер строки
                // второй аргумент - номер столбца
                // основная работа - третий столбец
                //if (i == 18)
                //{
                var ExcelCells = ((Excel.Range)Worksheet.Cells[i, j]);
                if (ExcelCells.MergeCells)
                {
                    var c = ExcelCells.MergeArea.get_Address(Type.Missing, Type.Missing, Excel.XlReferenceStyle.xlA1, Type.Missing, Type.Missing);
                }
                if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                {
                    string a = xlRange.Cells[i, j].Value2.ToString();
                    Console.WriteLine(a);
                }
            }
            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        private LessonInfo ParseItemRow()
        {
            return null;
        }
    }
}