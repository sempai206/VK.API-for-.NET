using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VkSchedule.AdminPanel
{
    public partial class Main : Form
    {
        public SqlConnection connection = new SqlConnection(getAuthData()["connectionString"]);
        public Main()
        {
            InitializeComponent();
            Refresh();
        }
        public override void Refresh()
        {
            connection.Open();
            var command = "SELECT tbl_Lessons.Id, tbl_Lessons.Number as [Номер пары], tbl_Lessons.DateFrom as С, tbl_Lessons.DateTo as По, tbl_Lessons.TypeOfLesson as [Тип пары], tbl_Lessons.Classroom as Аудитория, ref_NamesOfLessons.Title as Предмет, tbl_Teachers.Title AS Преподаватель, " + 
                                 "tbl_Lessons.DayOfWeek as [День недели]" + 
                          "FROM tbl_Lessons INNER JOIN " + 
                            "tbl_Teachers ON tbl_Lessons.TeacherId = tbl_Teachers.Id INNER JOIN " + 
                            "ref_NamesOfLessons ON tbl_Lessons.LessonNameId = ref_NamesOfLessons.Id " + 
                          "WHERE isActive = 1";
            var dt = new DataTable();
            var cmd = new SqlCommand(command, connection).ExecuteReader();
            dt.Load(cmd);
            dataGridView1.DataSource = dt;
            cmd.Close();
            connection.Close();
        }
        private static Dictionary<string, string> getAuthData()
        {
            if (!File.Exists("authData.txt"))
            {
                MessageBox.Show("Не найден файл authData.txt");
                Environment.Exit(0);
            }
            List<string> authData = File.ReadAllLines("authData.txt").ToList();
            Dictionary<string, string> authDictionary = new Dictionary<string, string>();
            foreach (var item in authData)
            {
                var parsedItem = item.Split(':');
                authDictionary.Add(parsedItem[0].Trim(), parsedItem[1].Trim());
            }
            return authDictionary;
        }
        private void dataGridView_Tasks_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!e.RowIndex.Equals(-1) && !e.ColumnIndex.Equals(-1) && e.Button.Equals(MouseButtons.Right))
            {
                dataGridView1.CurrentCell = dataGridView1[e.ColumnIndex, e.RowIndex];
                dataGridView1.CurrentRow.Selected = true;
            }
        }
        private void dataGridView_Tasks_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                if (e.Value != null)
                {
                    e.Value = e.Value.ToString().Trim();
                }
            }
        }

        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lesson = new Lessons(1, new LessonItem(), connection);
            lesson.ShowDialog();
            Refresh();
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lesson = new Lessons(2, new LessonItem()
            {
                Id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Id"].Value),
                DateFrom = dataGridView1.CurrentRow.Cells["С"].Value.ToString(),
                DateTo = dataGridView1.CurrentRow.Cells["По"].Value.ToString(),
                Number = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Номер пары"].Value),
                TypeOfLesson = dataGridView1.CurrentRow.Cells["Тип пары"].Value.ToString(),
                Teacher = dataGridView1.CurrentRow.Cells["Преподаватель"].Value.ToString(),
                LessonName = dataGridView1.CurrentRow.Cells["Предмет"].Value.ToString(),
                Classroom = dataGridView1.CurrentRow.Cells["Аудитория"].Value.ToString(),
                DayOfWeek = Convert.ToInt32(dataGridView1.CurrentRow.Cells["День недели"].Value)
            }, connection);
            lesson.ShowDialog();
            Refresh();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectStatuses = String.Format("DELETE FROM tbl_Lessons WHERE id = {0}", Convert.ToInt32(dataGridView1.CurrentRow.Cells["Id"].Value));
            connection.Open();
            var cmd = new SqlCommand(selectStatuses, connection);
            cmd.ExecuteNonQuery();
            connection.Close();
            MessageBox.Show("Запись удалена");
            Refresh();
        }

        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Refresh();
        }
    }
    public class LessonItem
    {
        public int Id { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int Number { get; set; }
        public string TypeOfLesson { get; set; }
        public string Teacher { get; set; }
        public string LessonName { get; set; }
        public string Classroom { get; set; }
        public int DayOfWeek { get; set; }
    }
}
