using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VkSchedule.AdminPanel
{
    public partial class Lessons : Form
    {
        public SqlConnection connection { get; set; }
        public int TypeOfPage { get; set; }
        public int Id { get; set; }
        public Lessons()
        {
            InitializeComponent();
        }
        public Lessons(int _TypeOfPage, LessonItem data, SqlConnection _Connection)
        {
            InitializeComponent();
            connection = _Connection;
            TypeOfPage = _TypeOfPage;
            getDataForComboBoxes("Title", "ref_NamesOfLessons", comboBox_Subject); //Список пользователей
            getDataForComboBoxes("Title", "tbl_Teachers", comboBox_Teacher);
            switch (TypeOfPage)
            {
                case 1:
                    Id = getMaxId() + 1;
                    break;
                case 2:
                    Id = data.Id;
                    textBox_Id.Text = data.Id.ToString();
                    textBox_DateFrom.Text = data.DateFrom;
                    textBox_DateTo.Text = data.DateTo;
                    textBox_Number.Text = data.Number.ToString();
                    textBox_TypeOfLesson.Text = data.TypeOfLesson;
                    textBox_Classroom.Text = data.Classroom;
                    comboBox_Teacher.Text = data.Teacher;
                    comboBox_Subject.Text = data.LessonName;
                    textBox_DayOfWeek.Text = data.DayOfWeek.ToString();
                    break;
            }
        }
        private int getMaxId()
        {
            var selectStatuses = "SELECT MAX(id) FROM tbl_Lessons";
            connection.Open();
            var cmd = new SqlCommand(selectStatuses, connection);
            int result = Convert.ToInt32(cmd.ExecuteScalar());
            connection.Close();
            return result;
        }
        private void getDataForComboBoxes(string column, string table, ComboBox combobox)
        {
            var selectString = String.Format("SELECT {0} FROM {1}", column, table);
            connection.Open();
            var cmd = new SqlCommand(selectString, connection);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                combobox.Items.Add(reader[0].ToString().Trim());
            }
            reader.Close();
            connection.Close();
        }
        private int getIdByTitle(string table, string value)
        {
            var selectStatuses = $"SELECT Id FROM {table} WHERE Title = '{value}'";
            connection.Open();
            var cmd = new SqlCommand(selectStatuses, connection);
            Int32 result = (Int32)cmd.ExecuteScalar();
            connection.Close();
            return (int)result;
        }
        private bool ValidateTask()
        {
            if (textBox_DateFrom.Text == String.Empty || textBox_DateTo.Text == String.Empty || textBox_Number.Text == String.Empty || textBox_TypeOfLesson.Text == String.Empty || textBox_Classroom.Text == String.Empty || textBox_DayOfWeek.Text == String.Empty || comboBox_Subject.Text == String.Empty || comboBox_Teacher.Text == String.Empty)
                return false;
            else
                return true;
        }
        private void button_Save_Click(object sender, EventArgs e)
        {
            var db = new DataDataContext();
            if (!ValidateTask())
            {
                MessageBox.Show("Не все поля заполнены");
                return;
            }
            var lesson = new tbl_Lessons();
            switch (TypeOfPage)
            {
                case 1:
                    lesson.Id = getMaxId() + 1;
                    lesson.DateFrom = Convert.ToDateTime(textBox_DateFrom.Text);
                    lesson.DateTo = Convert.ToDateTime(textBox_DateTo.Text);
                    lesson.Number = Convert.ToInt32(textBox_Number.Text);
                    lesson.TeacherId = getIdByTitle("tbl_Teachers", comboBox_Teacher.Text); ;
                    lesson.LessonNameId = getIdByTitle("ref_NamesOfLessons", comboBox_Subject.Text);
                    lesson.TypeOfLesson = textBox_TypeOfLesson.Text;
                    lesson.Classroom = textBox_Classroom.Text;
                    lesson.DayOfWeek = textBox_DayOfWeek.Text;
                    lesson.IsActive = true;
                    db.tbl_Lessons.InsertOnSubmit(lesson);
                    break;
                case 2:
                    lesson = db.tbl_Lessons.SingleOrDefault(i => i.Id == Id);
                    lesson.DateFrom = Convert.ToDateTime(textBox_DateFrom.Text);
                    lesson.DateTo = Convert.ToDateTime(textBox_DateTo.Text);
                    lesson.Number = Convert.ToInt32(textBox_Number.Text);
                    lesson.TeacherId = getIdByTitle("tbl_Teachers", comboBox_Teacher.Text); ;
                    lesson.LessonNameId = getIdByTitle("ref_NamesOfLessons", comboBox_Subject.Text);
                    lesson.TypeOfLesson = textBox_TypeOfLesson.Text;
                    lesson.Classroom = textBox_Classroom.Text;
                    lesson.DayOfWeek = textBox_DayOfWeek.Text;
                    break;
            }
            try
            {
                db.SubmitChanges();
                MessageBox.Show("Сохранение успешно");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show("Сохранение не удалось - " + error.Message);
            }
        }
    }
}
