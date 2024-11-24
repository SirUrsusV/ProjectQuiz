using ProjectQuiz.UserWindow;
using System;
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

namespace ProjectQuiz
{
	/// <summary>
	/// Логика взаимодействия для MainWindowUser.xaml
	/// </summary>
	public partial class MainWindowUser : Window
	{
		User user;
		List<Section> sections;

		public MainWindowUser(int UserID)
		{
			user = new User();
			user.Results = new List<Result>();
			user.ID = UserID;
			InitializeComponent();
			try
			{
                string queryForUserName = $"SELECT Username FROM Users WHERE {UserID} = UserId";
                using (SqlCommand command = new SqlCommand(queryForUserName, DataBaseConnection.Connection))
                {
                    user.Name = (string)command.ExecuteScalar();
                }
                sections = MainWindowAdmin.SectionsQury();
                string qureForAllResults = "SELECT Score, MaxScore, DateTaken, NameQuizz FROM Results WHERE @UserId = UserId";
                using (SqlCommand command = new SqlCommand(qureForAllResults, DataBaseConnection.Connection))
                {
                    command.Parameters.AddWithValue("@UserId", UserID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Result result = new Result()
                            {
                                Score = reader.GetInt32(0),
                                MaxScore = reader.GetInt32(1),
                                DateTake = reader.GetDateTime(2),
								NameQuizzes = reader.GetString(3),
                            };
							user.Results.Add(result);
							AddResult(result);
                        }
                    }
                }
            }
			catch(Exception ex)
			{
				MessageBox.Show("Ошибка: " + ex.Message);
			}


			foreach (Section section in sections)
			{
				Expander expander = new Expander();
				StackPanel quizzPanel = new StackPanel();
				expander.Header = section.Title;
				expander.Content = quizzPanel;
				QuizzStackPanel.Children.Add(expander);
				Grid.SetRow(expander, 2);

				foreach (Quizzes quizz in section.Quizzes)
				{
					Button button = new Button();
					button.Margin = new Thickness(10);
					button.Content = quizz.Title;
					button.Click += StartQuizz_Click;
					quizz.Button = button;
					quizzPanel.Children.Add(button);
				}
			}
			HelloTextBlock.Text = $"Приветствуем {user.Name}!";

		}
		public void AddResult(Result result)
		{
			TextBox textBox = new TextBox()
			{
				Text = $"Викторина \"{result.NameQuizzes}\"\n{result.Score} из {result.MaxScore}\nПройдено {result.DateTake.ToLongDateString()}",
				Margin = new Thickness(20)
			};
			ResultsStackPanel.Children.Add(textBox);
			//if(!user.Results.Any(res => res == result))
			//{

			//}
		}

		private void StartQuizz_Click(object sender, RoutedEventArgs e)
		{
			Quizzes startQuiz = new Quizzes();
			foreach (Section section in sections)
			{
				foreach (Quizzes quizz in section.Quizzes)
					if (quizz.Button.Content == (((Button)sender)).Content)
					{
						startQuiz = quizz;
					}
			}
			//Quizzes startQuiz = quizzes.First(quizz => string.Equals(quizz.Title, ((Button)sender).Content.ToString()));// = quizzes.First(quizz => quizz.Title == (string)(((Button)sender).Content));


			QuizzRun quizzRun = new QuizzRun(startQuiz, user, this);
			quizzRun.Show();
		}
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			LoginAndRegisterWindow LR = new LoginAndRegisterWindow();
			LR.Show();
			this.Close();
		}
	}
}
