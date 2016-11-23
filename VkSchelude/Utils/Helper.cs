using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkSchelude.Types;

namespace VkSchelude.Utils
{
    public static class Helper
    {
        public static string GetAnswerString(int modelId, List<DCustom> inputData, DateTime? lessonsDate = null)
        {
            string answerString = "Запрос не дал результатов";
            switch (modelId)
            {
                case 2: // расписание
                    answerString = String.Empty;
                    if (inputData.Count == 0)
                        answerString = $"На указанный день ({((DateTime)lessonsDate).ToShortDateString()}) занятий не найдено";
                    else
                        answerString = $"Расписание на {((DateTime)lessonsDate).ToShortDateString()} ({inputData.First()["DayOfWeek"]}):\n";
                    foreach (var item in inputData)
                    {
                        answerString += $"{item["Number"]} - {item["NameOfLesson"]} ({item["TypeOfLesson"]}) - {item["TeacherName"]} ({item["Classroom"]})\n";
                    }
                    break;
                case 6: // преподы
                    answerString = String.Empty;
                    foreach (var item in inputData)
                    {
                        answerString += $"{item["FullName"]} ({item["Rank"]})\n";
                    }
                    break;
            }
            return answerString;
        }
    }
}
