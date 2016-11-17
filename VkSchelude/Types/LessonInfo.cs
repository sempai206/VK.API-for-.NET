using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkSchelude.Types
{
    public class LessonInfo
    {
        public string Day { get; set; }
        public int Number { get; set; }
        public string Lesson { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Classroom { get; set; }
        public string Type { get; set; }
        public string Teacher { get; set; }
        public string TeacherRank { get; set; }
        public string Group { get; set; }
    }
}
