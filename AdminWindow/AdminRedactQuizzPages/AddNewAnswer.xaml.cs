using System;
using System.Collections.Generic;
using System.Data.Common;
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

namespace ProjectQuiz.AdminWindow.AdminRedactQuizzPages
{
    /// <summary>
    /// Логика взаимодействия для AddNewAnswer.xaml
    /// </summary>
    public partial class AddNewAnswer : Window
    {
        Answer newAnswer;
        Question parentQuestion;
        StackPanel answersStackPanel;
        List<Answer> answerList;
        AdminRedactQuizzes parentPage;
        public AddNewAnswer(Question parentQuestion , StackPanel answersStackPanel, List<Answer> answers, AdminRedactQuizzes parentPage)
        {
            newAnswer = new Answer()
            {
                QuestionID = parentQuestion.ID,
            };
            //((Question)questions.Where(ques => ques.ID == parentQuestion.ID)).Answers.Add(newAnswer);
            this.parentQuestion = parentQuestion;
            this.answersStackPanel = answersStackPanel;
            this.answerList = answers;
            this.parentPage = parentPage;

            InitializeComponent();
        }
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            if (textBox.Text == "")
                MessageBox.Show("Введите название");
            else
            {
                newAnswer.Text = textBox.Text;
                newAnswer.isCorrect = checkBox.IsChecked.Value;

                parentQuestion.Answers.Add(newAnswer);
                answerList.Add(newAnswer);

                string addValueString = $"INSERT INTO Answers(Text, QuestionId, IsCorrect) VALUES(@Text, @QuestionId, @Correct)";
                string getID = "SELECT AnswerId FROM Answers WHERE Text = @Text";
                try
                {
                    using (SqlCommand sqlCommand = new SqlCommand(addValueString, DataBaseConnection.Connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@Text", newAnswer.Text);
                        sqlCommand.Parameters.AddWithValue("@QuestionId", parentQuestion.ID);
                        sqlCommand.Parameters.AddWithValue("@Correct", newAnswer.isCorrect);
                        if (sqlCommand.ExecuteNonQuery() > 0)
                            MessageBox.Show("Ответ успешно добавлен");
                        else
                            MessageBox.Show("Произошла накладка в создании нового ответа");
                    }
                    using(SqlCommand sqlCommand = new SqlCommand(getID, DataBaseConnection.Connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@Text", newAnswer.Text);
                        newAnswer.ID = Convert.ToInt32(sqlCommand.ExecuteNonQuery());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }

                

                Button button = new Button { Content = newAnswer.Text + "\nID: " + newAnswer.ID };
                button.Click += parentPage.RedactAnswer_Click;
                answersStackPanel.Children.Add(button);

                this.Close();
            }
        }
        private void GalyaCancel_Click(object sender, RoutedEventArgs e)
        {
            newAnswer = null;
            this.Close();
        }
    }
}
