using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;

namespace ProjectQuiz
{
	/// <summary>
	/// Логика взаимодействия для MainWindowAdmin.xaml
	/// </summary>
	public partial class MainWindowAdmin : Window
	{
		Admin admin;
		public MainWindowAdmin(int adminID)
		{
			admin = new Admin();
			admin.AdminID = adminID;
			InitializeComponent();
			string queryForAdminName = $"SELECT Username FROM Users WHERE {adminID} = UserId";
			using (SqlCommand command = new SqlCommand(queryForAdminName, DataBaseConnection.Connection))
			{
				admin.AdminUserName = (string)command.ExecuteScalar();
			}
			UsersQury();
			admin.Sections = SectionsQury();
			MainFrameAdmin.Navigate(new AdminMenu(admin));

		}
		public void UsersQury()
		{
			string queryForAllUsers = "SELECT UserId, UserName, PasswordHash FROM Users";
			using (SqlCommand command = new SqlCommand(queryForAllUsers, DataBaseConnection.Connection))
			{
				using (SqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						User user = new User()
						{
							ID = reader.GetInt32(0),
							Name = reader.GetString(1),
							Password = reader.GetString(2)
						};
						admin.Users.Add(user);
					}
				}
			}
		}
		static public List<Section> SectionsQury()
		{
			List<Answer> answers = new List<Answer>();
			List<Question> questions = new List<Question>();
			List<Quizzes> quizzes = new List<Quizzes>();
			List<Section> sections = new List<Section>();
			try
			{
				string queryAnswers = "SELECT AnswerId, QuestionId, Text, isCorrect FROM Answers";
				using (SqlCommand command = new SqlCommand(queryAnswers, DataBaseConnection.Connection))
				using (SqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						answers.Add(new Answer
						{
							ID = reader.GetInt32(0),
							QuestionID = reader.GetInt32(1),
							Text = reader.GetString(2),
							isCorrect = reader.GetBoolean(3)
						});
					}
				}

				string queryQuestions = "SELECT QuestionId, QuizId, Text FROM Questions";
				using (SqlCommand command = new SqlCommand(queryQuestions, DataBaseConnection.Connection))
				using (SqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Question question = new Question
						{
							ID = reader.GetInt32(0),
							QuizID = reader.GetInt32(1),
							Title = reader.GetString(2)
						};

						question.Answers = answers.Where(answer => answer.QuestionID == question.ID).ToList();

                        //Select(answer => answer.QuestionID == question.ID).ToList();

                        questions.Add(question);
					}
				}

				string queryQuizzes = "SELECT QuizId, Title, SectionId FROM Quizzes";
				using (SqlCommand command = new SqlCommand(queryQuizzes, DataBaseConnection.Connection))
				using (SqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Quizzes quizz = new Quizzes
						{
							ID = reader.GetInt32(0),
							Title = reader.GetString(1),
							SectionID = reader.GetInt32(2)
						};

						quizz.Questions = questions.Where(question => question.QuizID == quizz.ID).ToList();

                        quizzes.Add(quizz);
                    }
				}

				string querySections = "SELECT SectionId, Name FROM Sections";
				using (SqlCommand command = new SqlCommand(querySections, DataBaseConnection.Connection))
				using (SqlDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						Section section = new Section
						{
							ID = reader.GetInt32(0),
							Title = reader.GetString(1),
						};

						section.Quizzes = quizzes.Where(quizz => quizz.SectionID == section.ID).ToList();

						sections.Add(section);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка: " + ex.Message);
			}
			
			return sections;
		}
		public void Exit()
		{
			LoginAndRegisterWindow LR = new LoginAndRegisterWindow();
			LR.Show();
			AdminWindow.Close();
		}
	}
}
