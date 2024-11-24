using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ProjectQuiz.UserWindow
{
	/// <summary>
	/// Логика взаимодействия для QuizzRun.xaml
	/// </summary>
	/// 



	public partial class QuizzRun : Window
	{
		Quizzes thisQuizz;
		StackPanel MainStackPanel;
		User User;
		int actualIndex;
		int maxIndex;
		int correctQuestion;
		Question currentQuestion;
		List<CheckBox> checkedListOnQuestion;
		MainWindowUser main;


        public QuizzRun(Quizzes quizz, User user, MainWindowUser main)
		{
			User = user;
			thisQuizz = quizz;
			this.main = main;
			//MainWindow.Title = $"Прохождение викторины под названием: {thisQuizz.Title}";
			actualIndex = 0;
			maxIndex = quizz.Questions.Count;
			InitializeComponent();
			loadQuestion();
		}
		void loadQuestion()
		{
			currentQuestion = thisQuizz.Questions[actualIndex];
            checkedListOnQuestion = new List<CheckBox>();
			QuestionPanel.Text = currentQuestion.Title;
			MainStackPanel = new StackPanel();
			MainSV.Content = MainStackPanel;

			foreach(Answer answer in currentQuestion.Answers)
			{
				Grid grid = new Grid();
				ColumnDefinition column1 = new ColumnDefinition();
				ColumnDefinition column2 = new ColumnDefinition();
				grid.ColumnDefinitions.Add(column1);
				grid.ColumnDefinitions.Add(column2);

				TextBlock textBlock1 = new TextBlock()
				{
					Text = answer.Text,
					HorizontalAlignment = HorizontalAlignment.Right,
					Margin = new Thickness(10)
				};
				grid.Children.Add(textBlock1);
				Grid.SetColumn(grid, 0);

				CheckBox checkBox = new CheckBox()
				{
					HorizontalAlignment = HorizontalAlignment.Left,
					Margin = new Thickness(10),
					IsChecked = false
				};
                checkedListOnQuestion.Add(checkBox);
				grid.Children.Add(checkBox);
                Grid.SetColumn(checkBox, 1);
				MainStackPanel.Children.Add(grid);
            }
		}
		void OutputResult()
		{

		}
		private void Continue_Click(object sender, RoutedEventArgs e)
		{
			if(actualIndex+1>0)
			{
				for(int i = 0; i < currentQuestion.Answers.Count; i++)
				{
					if (currentQuestion.Answers[i].isCorrect != checkedListOnQuestion[i].IsChecked)
					{
						break;
					}
					if (i == currentQuestion.Answers.Count - 1)
						correctQuestion++;
				}

			}
            if (actualIndex + 1 >= maxIndex)
				End();
			else
            {
                actualIndex++;
                loadQuestion();
            }

		}
		private void End()
		{
			MessageBox.Show($"Вы правильно ответели на {correctQuestion} вопросов");
			Result result = new Result
			{
				NameQuizzes = thisQuizz.Title,
				UserID = User.ID,
				Score = correctQuestion,
				MaxScore = thisQuizz.Questions.Count,
				DateTake = DateTime.Now
            };
			string addNewUserResult = "INSERT INTO Results (NameQuizz, UserId, Score, MaxScore, DateTaken) VALUES (@NameQuizz, @IdUser, @Score, @MaxScore, @DateTAken)";
			using (SqlCommand command = new SqlCommand(addNewUserResult,DataBaseConnection.Connection))
			{
				command.Parameters.AddWithValue("@NameQuizz", result.NameQuizzes);
				command.Parameters.AddWithValue("@IdUser", result.UserID);
				command.Parameters.AddWithValue("@Score", result.Score);
				command.Parameters.AddWithValue("@MaxScore", result.MaxScore);
				command.Parameters.AddWithValue("@DateTAken", result.DateTake);
				command.ExecuteNonQuery();
			}
			main.AddResult(result);

            this.Close();
		}
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
