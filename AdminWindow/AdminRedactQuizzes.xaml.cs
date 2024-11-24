using ProjectQuiz.AdminWindow.AdminRedactQuizzPages;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace ProjectQuiz
{
	/// <summary>
	/// Логика взаимодействия для AdminRedactQuizzes.xaml
	/// </summary>
	public partial class AdminRedactQuizzes : Page
	{
		List<Section> sections;
		List<Quizzes> quizzes;
		List<Question> questions;
		List<Answer> answers;

		List<Expander> expandersSection;
		List<Expander> expandersQuizz;
		List<Expander> expandersQuestion;
		List<Button> buttonsAnswers;

		TextBlock answerForAnswer;
		CheckBox answerIsCorrect;

		short level;
		object redactedOBJ;
		object redactedUI;
		string keyWordForID = "\nID:";

		bool isAnswer;

		public AdminRedactQuizzes(List<Section> sections)
		{
			try
			{
				expandersSection = new List<Expander>();
				expandersQuizz = new List<Expander>();
				expandersQuestion = new List<Expander>();
				buttonsAnswers = new List<Button>();

				this.sections = sections;
				quizzes = new List<Quizzes>();
				questions = new List<Question>();
				answers = new List<Answer>();

				level = 0;
				keyWordForID = " ID:";
				InitializeComponent();
				foreach (Section section in sections)
				{
					Expander expanderSection = new Expander()
					{
						Header = section.Title + "\nID: " + section.ID,
					};
					expanderSection.Expanded += RedactSectionOpen_Click;
					expanderSection.Collapsed += CloseExpander_Click;
					StackPanel stackPanelForQuizz = new StackPanel()
					{
						Margin = new Thickness(10, 0, 0, 5)
					};
					foreach (Quizzes quizz in section.Quizzes)
					{
						quizzes.Add(quizz);
						Expander expanderQuizz = new Expander()
						{
							Header = "  " + quizz.Title + " \n  ID: " + quizz.ID
						};
						expanderQuizz.Expanded += RedactQuizzOpen_Click;
						expanderQuizz.Collapsed += CloseExpander_Click;
						StackPanel stackPanelForQustion = new StackPanel()
						{
							Margin = new Thickness(10, 0, 0, 5)
						};
						foreach (Question question in quizz.Questions)
						{
							questions.Add(question);
							Expander expanderQuestion = new Expander()
							{
								Header = "  " + question.Title + " \n  ID: " + question.ID
							};
							expanderQuestion.Expanded += RedactQuestionOpen_Click;
							expanderQuestion.Collapsed += CloseExpander_Click;
							StackPanel stackPanelForAnswers = new StackPanel()
							{
								Margin = new Thickness(10, 0, 0, 5)
							};
							foreach (Answer answer in question.Answers)
							{
								answers.Add(answer);
								Button button = new Button()
								{
									Content = "  " + answer.Text + " \n  ID: " + answer.ID,
									HorizontalContentAlignment = HorizontalAlignment.Left,
									HorizontalAlignment = HorizontalAlignment.Left
								};
								button.Click += RedactAnswer_Click;
								stackPanelForAnswers.Children.Add(button);
								buttonsAnswers.Add(button);
							}
							expanderQuestion.Content = stackPanelForAnswers;
							expandersQuestion.Add(expanderQuestion);
							stackPanelForQustion.Children.Add(expanderQuestion);
							Button addAnswerButton = new Button()
							{
								Content = "Добавить новый ответ",
								Margin = new Thickness(5)
							};
							addAnswerButton.Click += AddNewAnswer_Click;
							stackPanelForAnswers.Children.Add(addAnswerButton);
						}
						expanderQuizz.Content = stackPanelForQustion;
						expandersQuizz.Add(expanderQuizz);
						stackPanelForQuizz.Children.Add(expanderQuizz);



						Button addQuestionButton = new Button()
						{
							Content = "Добавить новый вопрос",
							Margin = new Thickness(5)
						};
						addQuestionButton.Click += AddNewNoAnswer_Click;
						stackPanelForQustion.Children.Add(addQuestionButton);
					}
					expanderSection.Content = stackPanelForQuizz;
					expandersSection.Add(expanderSection);
					StackPanelXML.Children.Add(expanderSection);
					Button addQuizzButton = new Button()
					{
						Content = "Добавить новую викторину",
						Margin = new Thickness(5)
					};
					addQuizzButton.Click += AddNewNoAnswer_Click;
					stackPanelForQuizz.Children.Add(addQuizzButton);
				}
				Button addSectioButton = new Button()
				{
					Content = "Добавить новый раздел",
					Margin = new Thickness(5)
				};
				addSectioButton.Click += AddNewNoAnswer_Click;
				StackPanelXML.Children.Add(addSectioButton);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка: " + ex.Message);
			}
		}

		private void AddNewNoAnswer_Click(object sender, RoutedEventArgs e)
		{
            //AddNewObject addNewObject = new AddNewObject();
            //addNewObject.Show();
            Button button = sender as Button;
            Question parentQuetion = new Question();
            string contentValue = ((Expander)((StackPanel)(button.Parent)).Parent).Header.ToString();


            string keyword = "ID:";
            int index = contentValue.IndexOf(keyword);
            index += keyword.Length;
            string result = ((string)contentValue).Substring(index).Trim();

            IEnumerable<Question> searchParentQuestion = from q in questions
                                                         where q.ID == Convert.ToInt32(result)
                                                         select q;
            foreach (Question item in searchParentQuestion)
                parentQuetion = item;


            AddNewAnswer addNewAnswer = new AddNewAnswer(parentQuetion, ((StackPanel)(button.Parent)), answers, this);
            addNewAnswer.Show();
        }
		private void AddNewAnswer_Click(object sender, RoutedEventArgs e)
		{
			Button button = sender as Button;
			Question parentQuetion = new Question();
			string contentValue = ((Expander)((StackPanel)(button.Parent)).Parent).Header.ToString();


            string keyword = "ID:";
            int index = contentValue.IndexOf(keyword);
            index += keyword.Length;
			string result = ((string)contentValue).Substring(index).Trim();

			IEnumerable<Question> searchParentQuestion = from q in questions
										   where q.ID == Convert.ToInt32(result)
										   select q;
			foreach (Question item in searchParentQuestion)
				parentQuetion = item;


			AddNewAnswer addNewAnswer = new AddNewAnswer(parentQuetion, ((StackPanel)(button.Parent)), answers, this);
			addNewAnswer.Show();
		}
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GoBack();
		}
		private void Confirm_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (redactedOBJ == null)
					MessageBox.Show("Выберете объект");
				else
					if (MessageBox.Show("Вы уверены что хотите отредактировать этот объект?",
						"Изменеие викторины",
						MessageBoxButton.YesNo,
						MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						if(redactedOBJ is Answer answer)
						{
							answer.isCorrect = (bool)answerIsCorrect.IsChecked;
							answer.Text = textBox.Text;
							((Button)redactedUI).Content = answer.Text + $"\nID: {answer.ID}";

							string updateAnswer = $"UPDATE Answers SET Text = @Text, IsCorrect = @Correct WHERE AnswerId = {answer.ID}";
							using(SqlCommand command = new SqlCommand(updateAnswer, DataBaseConnection.Connection))
							{
								command.Parameters.AddWithValue("@Text", answer.Text);
								command.Parameters.AddWithValue("@Correct", answer.isCorrect);
								command.ExecuteNonQuery();
							}
						}
						else if(redactedOBJ is Section section)
						{
							section.Title = textBox.Text;
							((Expander)redactedUI).Header = textBox.Text;

							string updateAnswer = $"UPDATE Sections SET Name = @Text WHERE SectionId = {section.ID}";
							using (SqlCommand command = new SqlCommand(updateAnswer, DataBaseConnection.Connection))
							{
								command.Parameters.AddWithValue("@Text", section.Title);
								command.ExecuteNonQuery();
							}
						}
						else if (redactedOBJ is Quizzes quizz)
						{
							quizz.Title = textBox.Text;
							((Expander)redactedUI).Header = textBox.Text;

							string updateAnswer = $"UPDATE Quizzes SET Title = @Text WHERE QuizId = {quizz.ID}";
							using (SqlCommand command = new SqlCommand(updateAnswer, DataBaseConnection.Connection))
							{
								command.Parameters.AddWithValue("@Text", quizz.Title);
								command.ExecuteNonQuery();
							}
						}
						else if (redactedOBJ is Question question)
						{
							question.Title = textBox.Text;
							((Expander)redactedUI).Header = textBox.Text;

							string updateAnswer = $"UPDATE Questions SET Text = @Text WHERE QuestionId = {question.ID}";
							using (SqlCommand command = new SqlCommand(updateAnswer, DataBaseConnection.Connection))
							{
								command.Parameters.AddWithValue("@Text", question.Title);
								command.ExecuteNonQuery();
							}
						}

					}

				}
			

			catch ( Exception ex )
			{
				MessageBox.Show( "Ошибка: " + ex.Message );
			}
		}
		private void Delete_Click()
		{

		}
		private void CloseExpander_Click(object sender, RoutedEventArgs e)
		{
			level = 0;
			foreach (Expander item in expandersQuestion)
			{
				item.IsExpanded = false;
			}            
			foreach (Expander item in expandersQuizz)
			{
				item.IsExpanded = false;
			}           
			foreach (Expander item in expandersSection)
			{
				item.IsExpanded = false;
			}
			textBox.Text = "";
			if(isAnswer)
			{
				isAnswer = false;
				DeleteAnswerControl();
			}
			redactedOBJ = null;
		}
		public void RedactSectionOpen_Click(object sender, RoutedEventArgs e)
		{
			if(level == 0)
			{
				Expander expander = (Expander)sender;

				expander.IsExpanded = true;

				redactedUI = sender;
				int index = ((string)expander.Header).IndexOf("\nID:");
				index += keyWordForID.Length;
				string result = ((string)expander.Header).Substring(index);

				int ID = Convert.ToInt32(result.Trim());
				Section section = (Section)sections.First(sec => sec.ID == ID);
				level = 1;
				RedactObject(section);
			}
		}
        public void RedactQuizzOpen_Click(object sender, RoutedEventArgs e)
		{
			if (level == 1)
			{
				Expander expander = (Expander)sender;
				redactedUI = sender;
				int index = ((string)expander.Header).IndexOf(keyWordForID);
				index += keyWordForID.Length;
				int ID = Convert.ToInt32(((string)expander.Header).Substring(index).Trim());
				Quizzes quizz = (Quizzes)quizzes.First(quiz => quiz.ID == ID);
				level = 2;
				RedactObject(quizz);
			}
		}
        public void RedactQuestionOpen_Click(object sender, RoutedEventArgs e)
		{
			if (level == 2)
			{
				Expander expander = (Expander)sender;
				redactedUI = sender;
				int index = ((string)expander.Header).IndexOf(keyWordForID);
				index += keyWordForID.Length;
				int ID = Convert.ToInt32(((string)expander.Header).Substring(index).Trim());
				Question section = (Question)questions.First(ques => ques.ID == ID);
				level = 3;
				RedactObject(section);
			}
		}
        public void RedactAnswer_Click(object sender, RoutedEventArgs e)
		{
			Button bitton = (Button)sender;
			redactedUI = sender;
			int index = ((string)bitton.Content).IndexOf(keyWordForID);
			index += keyWordForID.Length;
			int ID = Convert.ToInt32(((string)bitton.Content).Substring(index).Trim());
			Answer answer = (Answer)answers.First(answ => answ.ID == ID);
			level = 4;
			RedactObject(answer);
		}
		private void RedactObject(object obj)
		{
			try
			{
				if (obj is Section section)
				{
					textBox.Text = section.Title;
					redactedOBJ = section;
					if (isAnswer)
					{
						isAnswer = false;
						DeleteAnswerControl();
					}
				}
				else if (obj is Quizzes quizz)
				{
					textBox.Text = quizz.Title;
					redactedOBJ = quizz;
					if (isAnswer)
					{
						isAnswer = false;
						DeleteAnswerControl();
					}
				}
				else if (obj is Question question)
				{
					textBox.Text = question.Title;
					redactedOBJ = question;
					if (isAnswer)
					{
						isAnswer = false;
						DeleteAnswerControl();
					}
				}
				else if (obj is Answer answer)
				{
					textBox.Text = answer.Text;
					redactedOBJ = answer;
					if(!isAnswer)
					{
						isAnswer = true;
						CreateAnswerControl();
					}

					answerIsCorrect.IsChecked = answer.isCorrect;
				}
				else
					throw new Exception("Для преоброзования не подходит не один из предлженных типов");
			}
			catch(Exception ex)
			{
				MessageBox.Show($"Ошибка: {ex.Message}");
			}

		}
		private void DeleteAnswerControl()
		{
			GridAnswerControl.Children.Remove( answerIsCorrect );
			GridAnswerControl.Children.Remove( answerForAnswer );
		}
		private void CreateAnswerControl()
		{
			answerForAnswer = new TextBlock()
			{
				Text = "Правилный ответ",
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				FontSize = 20
			};
			GridAnswerControl.Children.Add(answerForAnswer);
			Grid.SetRow(answerForAnswer, 1);
			answerIsCorrect = new CheckBox()
			{
				VerticalAlignment = VerticalAlignment.Center,
				IsChecked = ((Answer)redactedOBJ).isCorrect
			};
			GridAnswerControl.Children.Add(answerIsCorrect);
			Grid.SetColumn(answerIsCorrect, 1);
			Grid.SetRow(answerIsCorrect, 1);
			//< TextBlock Grid.Row = "2" Text =  VerticalAlignment = "Center" HorizontalAlignment = "Center" FontSize = "20" />
			//< CheckBox Grid.Row = "2" Grid.Column = "1" VerticalAlignment = "Center" />
		}
	}
}
