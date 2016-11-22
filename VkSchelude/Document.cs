using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Excel;
using VkSchelude.Types;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace VkSchelude
{
    public class Document
    {
        private string patternSingleDate = @"\d{1,2}\.\d{1,2}\.\d{1,2}";
        private string patternTrash = @"([-,\,,\.,\s])";
        private string patternFromTo = @"с \d{1,2}\.\d{1,2}\.\d{1,2} {0,1}г\. по \d{1,2}\.\d{1,2}\.\d{1,2} {0,1}г\.-[а-я,А-я,\,,\.,ё]+";
        private string patternLesson = @" {0,1}г.-[\,,\.,а-я,А-Я,ё]+";
        private string patternMultiDate = @"(\d{1,2}\,){0,}\d{1,2}\.\d{1,2}\.\d{1,2}";
        private string patternHardDate = @"\d{1,2}\.\d{1,2};\.\d{1,2}\.\d{1,2}";
        private string patternTeacher = @"[А-Я,а-я,ё,Ё]+\.[А-Я,а-я,ё,Ё]{1}\.";
        bool EnableConsole = false;
        public Document(bool _enableConsole = false)
        {
            EnableConsole = _enableConsole;
        }
        public List<LessonInfo> Parse(string excelDocPath)
        {
            if (!System.IO.File.Exists(excelDocPath))
            {
                Console.WriteLine("Файл отсутствует в директории " + excelDocPath); // временно. должна возвращаться ошибка, чтоб тут никакой текст не выводился
                return null;
            }
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(excelDocPath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Worksheet Worksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;
            List<LessonInfo> allLessons = new List<Types.LessonInfo>();
            int j = 3;
            int rowCount = xlRange.Rows.Count;
            for (int i = 1; i <= rowCount; i++)
            {
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
                    if (EnableConsole && i == 15)
                    {

                    }
                    string dataString = xlRange.Cells[i, j].Value2.ToString();
                    if (dataString.Length < 10) continue;
                    int number = int.Parse(xlRange.Cells[i, 2].Value2.ToString());
                    var itemhall = xlRange.Cells[i, 4];
                    string hall = String.Empty;
                    if (itemhall != null && itemhall.Value2 != null)
                        hall = xlRange.Cells[i, 4].Value2.ToString();
                    allLessons.AddRange(ParseItemRow(dataString, day, number, hall, ExcelCells.MergeCells, mergedAddress));
                }
            }
            if (EnableConsole)
            foreach (var item in allLessons)
            {
                Console.WriteLine($"{item.Day} - " +
                    $"{item.Number} - " +
                    $"{item.Lesson} - " +
                    String.Format("{0};", item.DateStart != null ? (((DateTime)item.DateStart).ToShortDateString()) : "null") +
                    String.Format("{0} - ", item.DateEnd != null ? (((DateTime)item.DateEnd).ToShortDateString()) : "null") +
                    $"{item.Type} - {item.TeacherRank} - {item.Teacher} - " +
                    $"{item.Classroom}");
            }
            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return allLessons;
        }
        private List<LessonInfo> ParseItemRow(string DataString, string Day, int Number, string Hall, bool Merged, string MergedAddress = null)
        {
            if (DataString.Length < 10) return null;
            List<LessonInfo> result = new List<LessonInfo>();
            if (DataString.Contains('\n'))
            {
                for (int i = 0; i < DataString.Split('\n').Count(); i++)
                {
                    string partData = DataString.Split('\n')[i];
                    if (Hall.Split('\n')[i].Equals("смен.об."))
                    {
                        Hall = Hall.Replace("\nсмен.об.", "");
                    }
                    result.AddRange(ParseItemRow(partData, Day, Number, Hall.Split('\n')[i], Merged, MergedAddress = null));
                }
                return result;
            }
            string teacher = DataString.Split(':')[DataString.Split(':').Count() - 1];
            string dates = DataString.Substring(DataString.IndexOf(';') + 1);
            if (Regex.IsMatch(dates, patternSingleDate)) // нет смысла парсить, если нет дат
            {
                dates = dates.Substring(0, dates.LastIndexOf(':')).Trim();
                #region---Проверка "с...по"
                var itemsFromTo = Regex.Match(dates, patternFromTo);
                while (itemsFromTo.Captures.Count > 0)
                {
                    var bb = Regex.Matches(itemsFromTo.Captures[0].Value, patternSingleDate);
                    LessonInfo currentItem = new LessonInfo();
                    currentItem.DateStart = DateTime.Parse(bb[0].Value);
                    currentItem.DateEnd = DateTime.Parse(bb[1].Value);
                    string type = Regex.Matches(itemsFromTo.Captures[0].Value, patternLesson)[0]
                        .Value
                        .Replace("г.-", "");
                    type = ClearTrim(type);
                    currentItem.Type = type;
                    dates = dates.Replace(itemsFromTo.Captures[0].Value, "");
                    itemsFromTo = Regex.Match(dates, patternFromTo);
                    result.Add(currentItem);
                }
                #endregion
                #region---Проверка дат в привычном формате
                itemsFromTo = Regex.Match(dates, patternMultiDate + patternLesson);
                while (itemsFromTo.Captures.Count > 0)
                {
                    //парсинг остальных форматов записи даты
                    var bb = Regex.Matches(itemsFromTo.Captures[0].Value, patternSingleDate);
                    string foundDate = bb[0].Value;
                    LessonInfo currentItem = new LessonInfo();
                    currentItem.DateStart = DateTime.Parse(foundDate);
                    currentItem.DateEnd = DateTime.Parse(foundDate);
                    string type = Regex.Matches(itemsFromTo.Captures[0].Value, patternLesson)[0]
                        .Value
                        .Replace("г.-", "");
                    type = ClearTrim(type);
                    dates = dates.Replace(foundDate, foundDate.Substring(foundDate.IndexOf('.')));
                    dates = dates.Replace(",.", ".");
                    itemsFromTo = Regex.Match(dates, patternMultiDate + patternLesson);
                    if(itemsFromTo.Captures.Count == 0)
                    {
                        var bb1 = Regex.Match(dates, patternHardDate);
                        if (bb1.Captures.Count > 0)
                        {
                            string problemDate = bb1.Captures[0].Value;
                            if (bb1.Captures.Count > 0)
                            {
                                string excessText = problemDate.Substring(problemDate.IndexOf(';'), problemDate.Length - bb1.Captures[0].Value.LastIndexOf('.') + 1);
                                dates = dates.Replace(excessText, "");
                                itemsFromTo = Regex.Match(dates, patternMultiDate + patternLesson);
                                // обрезать лишнее
                            }
                        }
                    }
                    currentItem.Type = type;
                    result.Add(currentItem);
                }
                #endregion
            }
            else
            {
                result.Add(new LessonInfo { Lesson = DataString.Split(';')[0] });
            }
            foreach (var item in result)
            {
                item.Lesson = DataString.Split(';')[0].Trim();
                item.Day = ClearTrim(Day);
                if (Regex.Matches(teacher, patternTeacher).Count > 0)
                {
                    item.Teacher = Regex.Matches(teacher, patternTeacher)[0].Value;
                    item.TeacherRank = ClearTrim(teacher.Replace(item.Teacher, ""));
                }
                else
                {
                    item.Teacher = teacher.Trim();
                }
                item.Number = Number;
                item.Classroom = ClearTrim(Hall);
            }
            return result;
        }
        private string ClearTrim(string input)
        {
            while (Regex.IsMatch(input, patternTrash + @"$")) // обрезаем мусор в конце
            {
                input = input.Substring(0, input.Length - 1);
            }
            while (Regex.IsMatch(input, "^" + patternTrash)) // обрезаем мусор в начале
            {
                input = input.Substring(1);
            }
            return input.Replace('\n', ' ');
        }
    }
}