using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Excel;
using VkSchelude.Types;
using System.Text.RegularExpressions;

namespace VkSchelude
{
    public class Document
    {
        private string patternSingleDate = @"\d{1,2}\.\d{1,2}\.\d{1,2}";
        private string patternTrash = @"([-,\,,\.,\s])";
        private string patternFromTo = @"с \d{1,2}\.\d{1,2}\.\d{1,2}г\. по \d{1,2}\.\d{1,2}\.\d{1,2}г\.-[а-я,А-я,\,,\.]+";
        private string patternLesson = @"г.-[\,,\.,а-я,А-Я]+";
        private string patternMultiDate = @"(\d{1,2}\,){0,}\d{1,2}\.\d{1,2}\.\d{1,2}";
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
                if (i > 12)
                {
                }
                    #region---Prepare data
                    var ExcelCells = ((Excel.Range)Worksheet.Cells[i, j]);
                string mergedAddress = String.Empty;
                if (ExcelCells.MergeCells)
                {
                    mergedAddress = ExcelCells.MergeArea.get_Address(Type.Missing, Type.Missing, Excel.XlReferenceStyle.xlA1, Type.Missing, Type.Missing);
                }
                string day = String.Empty;
                int inew = i;
                do
                {
                    day = xlRange.Cells[inew, 1].Value2;
                    inew--;
                }
                while ((day == null) && (inew > 0) && String.IsNullOrEmpty(day));
                #endregion
                if (xlRange.Cells[i, j] != null && xlRange.Cells[i, j].Value2 != null)
                {
                    if (i == 18)
                    {

                    }
                    string dataString = xlRange.Cells[i, j].Value2.ToString();
                    if (dataString.Length < 10) continue;
                    int number = int.Parse(xlRange.Cells[i, 2].Value2.ToString());
                    string hall = xlRange.Cells[i, 4].Value2.ToString();
                    var a = ParseItemRow(dataString, day, number, hall, ExcelCells.MergeCells, mergedAddress);
                }
            }
            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        private List<LessonInfo> ParseItemRow(string DataString, string Day, int Number, string Hall, bool Merged, string MergedAddress = null)
        {
            if (DataString.Length < 10) return null;
            List<LessonInfo> result = new List<LessonInfo>();
            //List<LessonInfo> localResult
            if (DataString.Contains('\n'))
            {
                //foreach (string partData in DataString.Split('\n'))
                //{
                for(int i = 0; i< DataString.Split('\n').Count(); i++)
                {
                    string partData = DataString.Split('\n')[i];
                    if (Hall.Split('\n')[i].Equals("смен.об."))
                    {
                        Hall = Hall.Replace("\nсмен.об.", "");
                    }
                    result.AddRange(ParseItemRow(partData, Day, Number, Hall.Split('\n')[i], Merged, MergedAddress = null));
                    //return result; // проверить
                }
                return result;
            }
            string teacher = DataString.Split(':')[DataString.Split(':').Count() - 1];
            string dates = DataString.Substring(DataString.IndexOf(';') + 1);
            dates = dates.Substring(0, dates.LastIndexOf(':')).Trim();
            #region---Проверка "с...по"
            var itemsFromTo = Regex.Match(dates, patternFromTo);
            while (itemsFromTo.Captures.Count > 0)
            {
                var bb = Regex.Matches(itemsFromTo.Captures[0].Value, patternSingleDate);
                LessonInfo currentItem = new LessonInfo();
                currentItem.DateStart = DateTime.Parse(bb[0].Value);
                currentItem.DateEnd = DateTime.Parse(bb[1].Value);
                string type = Regex.Matches(itemsFromTo.Captures[0].Value, @"г.-[\,,\.,а-я,А-Я]+")[0]
                    .Value
                    .Replace("г.-", "");
                while (Regex.IsMatch(type, patternTrash + @"$")) // обрезаем мусор в конце
                {
                    type = type.Substring(0, type.Length - 1);
                }
                while (Regex.IsMatch(type, "^" + patternTrash)) // обрезаем мусор в начале
                {
                    type = type.Substring(1);
                }
                currentItem.Type = type;
                dates = dates.Replace(itemsFromTo.Captures[0].Value, "");
                itemsFromTo = Regex.Match(dates, patternFromTo);
                result.Add(currentItem);
            }
            itemsFromTo = Regex.Match(dates, patternMultiDate + patternLesson);
            while (itemsFromTo.Captures.Count > 0)
            {
                //парсинг остальных форматов записи даты
                itemsFromTo = Regex.Match(dates, patternMultiDate + patternLesson);
            }
            foreach(var item in result)
            {
                item.Day = Day;
                item.Teacher = teacher;
                item.Number = Number;
                item.Classroom = Hall;
            }
            #endregion
            Console.WriteLine($"{Day} - {DataString}");
            return result;
        }
    }
}