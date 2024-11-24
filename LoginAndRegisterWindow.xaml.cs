using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using ProjectQuiz;
using System.Data.Common;

namespace ProjectQuiz
{
	public partial class LoginAndRegisterWindow : Window
	{
		public LoginAndRegisterWindow()
		{
			InitializeComponent();
			////МАКСИМАЛЬНО ВРЕМЕННАЯ МЕРА. УДАЛИТЬ ПОСЛЕ ОКОНЬЧАНИЯ ТЕСТИРОВАНИЯ ВОЗМОЖНОСТЕЙ АДМИНА!!!!
			//LoginUsername.Text = "Ursus";
   //         LoginPassword.Password = "1234";
        }
		private void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			// Логика для входа
			string username = LoginUsername.Text;
			string password = LoginPassword.Password;
			SearchUser(username, password);
		}
		private void RegisterButton_Click(object sender, RoutedEventArgs e)
		{
			//Первый зарегестрированный будет админом
			// Логика для регистрации
			string username = RegisterUsername.Text;
			string password = RegisterPassword.Password;
			string confirmPassword = RegisterConfirmPassword.Password;
			if(username == "" || password == "")
			{
				MessageBox.Show("Не одно из полей не должно оставаться пустым!");
				return;
			}

			if (password == confirmPassword)
			{
				//connection.ChangeDatabase("Database with quiz");
				try
				{
					// Проверка на уникальность логина
					string checkLoginQuery = "SELECT COUNT(*) FROM Users WHERE Username = @Login";
					using (SqlCommand checkCommand = new SqlCommand(checkLoginQuery, DataBaseConnection.Connection))
					{
						checkCommand.Parameters.AddWithValue("@Login", username);

						int count = (int)checkCommand.ExecuteScalar();

						if (count > 0)
						{
							MessageBox.Show("Ошибка: Логин уже существует. Пожалуйста, выберите другой логин.");
							return; // Завершаем выполнение программы
						}
					}
					// Проверка на наличние админа в системе
					bool adminExist = false;
					string checkAdminQuery = "SELECT COUNT(*) FROM Users WHERE IsAdmin = 1";
					using (SqlCommand checkCommand = new SqlCommand(checkAdminQuery, DataBaseConnection.Connection))
					{
						if (adminExist = ((int)checkCommand.ExecuteScalar() == 0))
						{
							MessageBox.Show("Администратор не найден. Следующий пользователь будет администратором");
						}
					}
					// SQL-запрос для добавления логина и пароля
					// Если админ не найден, то новый пользователь будет соответсвующее проинициализирован/добавлен
					string query = string.Format("INSERT INTO Users (Username, PasswordHash, IsAdmin) VALUES (@Login, @Password, {0})", adminExist? '1' : '0');

					using (SqlCommand command = new SqlCommand(query, DataBaseConnection.Connection))
					{
						// Добавляем параметры для предотвращения SQL-инъекций
						command.Parameters.AddWithValue("@Login", username);
						command.Parameters.AddWithValue("@Password", password);

						// Выполняем запрос
						int result = command.ExecuteNonQuery();

						// Проверяем, был ли запрос успешным
						if (result > 0)
						{
							MessageBox.Show("Вы успешно зарегестрировались");
							SearchUser(username, password);
						}
						else
						{
							MessageBox.Show("Непредвиденная ошибка при отправке запроса.\nСообщите об этом администратору");
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Произошла ошибка: " + ex.Message);
				}
			}
			else
			{
				MessageBox.Show("Пароль не совпадает!");
			}
		}
		private void SearchUser(string username, string password)
		{
			try
			{
				// SQL-запрос для получения ID и Email
				string query = "SELECT UserId, IsAdmin FROM Users WHERE Username = @login AND PasswordHash = @password";

				using (SqlCommand command = new SqlCommand(query, DataBaseConnection.Connection))
				{
					// Добавление параметров для предотвращения SQL-инъекций
					command.Parameters.AddWithValue("@login", username);
					command.Parameters.AddWithValue("@password", password);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							// Вызов метода для дальнейшей инициализации Пользователя в клиенте
							int ID = reader.GetInt32(0);
							bool isAdmin = reader.GetBoolean(1);
                            reader.Close();
							InitializeUser(ID, isAdmin);
						}
						else
						{
							MessageBox.Show("Пароль или Логи некорректен");
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Произошла ошибка: " + ex.Message);
			}
		}
		private void InitializeUser(int _ID, bool isAdmin)
		{
			if(isAdmin)
			{
				MainWindowAdmin admin = new MainWindowAdmin(_ID);
				admin.Show();
				LRWindow.Close();
			}
			else
			{
				MainWindowUser user = new MainWindowUser(_ID);
				user.Show();
				LRWindow.Close();
			}
		}

	}
}
// Старые наработки
/*
// Это из входа в систему
//connection.Open();
//string queryIsAdmin = "SELECT UserId FROM Users WHERE Username = @login AND PasswordHash = @password AND isAdmin = 1";
//using (SqlCommand command = new SqlCommand(queryIsAdmin, connection))
//{
//	command.Parameters.AddWithValue("@login", username);
//	command.Parameters.AddWithValue("@password", password);
//	if ((int)command.ExecuteScalar() > 0)
//	{
//		MessageBox.Show("Вход для администратора выполнен");
//		InitializeUser(username, true);
//		return;
//	}
//}
//string query = "SELECT COUNT(*) FROM Users WHERE Username = @login AND PasswordHash = @password";
//using (SqlCommand command = new SqlCommand(query, connection))
//{
//	command.Parameters.AddWithValue("@login", username);
//	command.Parameters.AddWithValue("@password", password);
//	if ((int)command.ExecuteScalar() > 0)
//	{
//		MessageBox.Show("Вход выполнен");
//		InitializeUser(username, false);
//	}
//	else
//	{
//		MessageBox.Show("Логин или Пароль неправильный");
//		return;
//	}
//}

//Это было для считывания. Оказывается что пока один ридер на один connection открыт, другия открывать нелья

using (SqlCommand commandSection = new SqlCommand(queryAllSections, DataBaseConnection.Connection))
			using (SqlDataReader readerSection = commandSection.ExecuteReader())
			{
				List<Section> sections = new List<Section>();
				while (readerSection.Read())
				{
					Section section = new Section()
					{
						ID = readerSection.GetInt32(0),
						Title = readerSection.GetString(1),
					};
					string quryAllQuizzes = $"SELECT QuizId, Title FROM Quizzes WHERE SectionId = {readerSection.GetInt32(0)}";
					using (SqlCommand commandQuizz = new SqlCommand(quryAllQuizzes, DataBaseConnection.Connection))
					using (SqlDataReader readerQuizz = commandQuizz.ExecuteReader())
					{
						while (readerQuizz.Read())
						{
							Quizzes quizz = new Quizzes()
							{
								ID = readerQuizz.GetInt32(0),
								Title = readerQuizz.GetString(1),
								SectionID = readerSection.GetInt32(0)
							};

							string quryAllQuestions = $"SELECT QuestionId, [Text] FROM Quizzes WHERE QuizId = {readerQuizz.GetInt32(0)}";
							using (SqlCommand commandQuestion = new SqlCommand(quryAllQuizzes, DataBaseConnection.Connection))
							using (SqlDataReader readerQuestion = commandQuizz.ExecuteReader())
							{
								while (readerQuestion.Read())
								{
									Question question = new Question()
									{
										ID = readerQuestion.GetInt32(0),
										Title = readerQuestion.GetString(1),
									};

									string quryAllAnswers = $"SELECT AnswerId, [Text], isCorrect FROM Answers WHERE QuestionId = {readerQuestion.GetInt32(0)}";
									using (SqlCommand commandAnswer = new SqlCommand(quryAllAnswers, DataBaseConnection.Connection))
									using (SqlDataReader readerAnswer = commandQuizz.ExecuteReader())
									{
										while (readerAnswer.Read())
										{
											Answer answer = new Answer()
											{
												ID = readerAnswer.GetInt32(0),
												Text = readerAnswer.GetString(1),
												isCorrect = readerAnswer.GetBoolean(2)
											};
											question.Answers.Add(answer);
										}
									}
									quizz.Questions.Add(question);
								}
								section.Quizzes.Add(quizz);
							}
						}
					}
					sections.Add(section);
				}
				return sections;
			}
*/