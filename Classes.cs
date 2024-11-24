using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
// Отдельный файл для общих классов
namespace ProjectQuiz
{
    public class DataBaseConnection
    {
        static string connectionString = "Server=192.168.65.100;User id=Teacher;Password=shag39;Database=P23 Vakker Quizz DB;Trusted_Connection=True;";
		public static SqlConnection Connection {  get; private set; }
        static DataBaseConnection()
        {
			try
			{
				Connection = new SqlConnection(connectionString);
				Connection.Open();
			}
			catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message); }
        }
    }
    public class Answer
	{
		public int ID { get; set; }
		public int QuestionID { get; set; }
		public string Text { get; set; }
		public bool isCorrect { get; set; }
	}
	public class Question
	{
		public int ID { get; set; }
		public int QuizID { get; set; }
		public string Title { get; set; }
        public List<Answer> Answers { get; set; }
		public Question()
		{
			Answers = new List<Answer>();
		}

    }
	public class Quizzes
	{
		public int ID { get; set; }
		public int SectionID { get; set; }
		public string Title { get; set; }
        public List<Question> Questions { get; set; }
		public Button Button { get; set; }
		public Quizzes()
		{
			Questions = new List<Question>();
		}

    }
	public class Section
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public List<Quizzes> Quizzes { get; set; }
		public Section()
		{
			Quizzes = new List<Quizzes>();
		}
	}
	public class Result
	{
		public int ID { get; set; }
		public int UserID { get; set; }
		public string NameQuizzes {  get; set; }
		public int Score { get; set; }
		public int MaxScore { get; set; }
		public DateTime DateTake { get; set; }
	}
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public int ID { get; set; }
		public List<Result> Results { get; set; }
    }
    public class Admin
    {
        public int AdminID { get; set; }
        public string AdminUserName { get; set; }
        public List<User> Users {  get;  set; }
        public List<Section> Sections {  get; set; }
		public Admin()
		{
			Users = new List<User>();
            Sections = new List<Section>();
        }
    }
}
