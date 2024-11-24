using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectQuiz.AdminWindow.AdminRedactQuizzPages
{
	/// <summary>
	/// Логика взаимодействия для AddNewObject.xaml
	/// </summary>
	


	public partial class AddNewObject: Window
	{
		public object value { get; set; }
		public object parrentValue { get; set; }
		List<object> listValues { get; set; }
		StackPanel StackPanel { get; set; }

		AdminRedactQuizzes adminRedactQuizzes;

        public AddNewObject(List<Section> sectionList, StackPanel stackPanelSections, AdminRedactQuizzes adminRedactQuizzes)
		{
			Section section = new Section();
			value = section;
			StackPanel = stackPanelSections;
			this.adminRedactQuizzes = adminRedactQuizzes;
		}
		public AddNewObject(List<Quizzes> quizzesList, Section parrentSection, StackPanel stackPanelQuizzes, AdminRedactQuizzes adminRedactQuizzes)
		{
			Quizzes quizzes = new Quizzes();
			value = quizzes;
			parrentValue = parrentSection;
			StackPanel = stackPanelQuizzes;
            this.adminRedactQuizzes = adminRedactQuizzes;
        }
		public AddNewObject(List<Question> questionsList, Quizzes parrentQuizz,  StackPanel stackPanelQuizz, AdminRedactQuizzes adminRedactQuizzes)
		{
			Question question = new Question();
			value = question;
			parrentValue = parrentQuizz;
			StackPanel = stackPanelQuizz;
            this.adminRedactQuizzes = adminRedactQuizzes;
        }
		public AddNewObject(object addingObj, StackPanel stackPanel)
		{
			InitializeComponent();
		}

		private void Confirm_Click(object sender, RoutedEventArgs e)
		{
			if (textBox.Text == "")
				MessageBox.Show("Введите текст");
			else
			{
				if (value is Section section)
				{
					section.Title = textBox.Text;
					listValues.Add(section);

					Expander expander = new Expander { Header = section.Title + "\nID: " + section.ID };
					expander.Expanded += adminRedactQuizzes.RedactSectionOpen_Click;

                    StackPanel.Children.Add(expander);

					string addValueString = "INSERT INTO Sections(Name) VALUES (@Name)";
					try
					{
                        using (SqlCommand sqlCommand = new SqlCommand(addValueString, DataBaseConnection.Connection))
                        {
							sqlCommand.Parameters.AddWithValue("@Name", section.Title);
							sqlCommand.ExecuteNonQuery();
                        }
                    }
					catch(Exception ex) 
					{
						MessageBox.Show("Ошибка: " + ex.Message);
					}

				}
				else if (value is Quizzes quizzes)
				{
					((Section)parrentValue).Quizzes.Add(quizzes);
					listValues.Add(quizzes);

					Expander expander = new Expander { Content = quizzes.Title + "\nID: " + quizzes.ID };
					expander.Expanded += adminRedactQuizzes.RedactQuizzOpen_Click;


                    StackPanel.Children.Add(expander);
					quizzes.SectionID = ((Section)parrentValue).ID;


                    string addValueString = "INSERT INTO Quizzes(Title, SectionId) VALUES (@Name, @ID)";
                    try
                    {
                        using (SqlCommand sqlCommand = new SqlCommand(addValueString, DataBaseConnection.Connection))
                        {
                            sqlCommand.Parameters.AddWithValue("@Name", quizzes.Title);
                            sqlCommand.Parameters.AddWithValue("@ID", quizzes.SectionID);
                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
				else if (value is Question question)
                {
                    ((Quizzes)parrentValue).Questions.Add(question);
                    listValues.Add(question);

					Expander expander = new Expander { Content = question.Title + "\nID: " + question.ID };
					expander.Expanded += adminRedactQuizzes.RedactQuestionOpen_Click;

                    StackPanel.Children.Add(expander);
					question.QuizID = ((Quizzes)parrentValue).ID;



                    string addValueString = "INSERT INTO Questions(Text, QuizId) VALUES (@Name, @ID)";
                    try
                    {
                        using (SqlCommand sqlCommand = new SqlCommand(addValueString, DataBaseConnection.Connection))
                        {
                            sqlCommand.Parameters.AddWithValue("@Name", question.Title);
                            sqlCommand.Parameters.AddWithValue("@ID", question.QuizID);
                            sqlCommand.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка: " + ex.Message);
                    }
                }
				else
				{
					throw new Exception("Неустановленный тип! Файл AddNewObject");
				}
				this.Close();
			}
		}
		private void GalyaCancel_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
