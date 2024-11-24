using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
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

namespace ProjectQuiz
{
	/// <summary>
	/// Логика взаимодействия для AdminRedactUsers.xaml
	/// </summary>
	public partial class AdminRedactUsers : Page
	{
		List<User> users;
		List<Button> buttons;
		User redacteringUser;
		Button redacteringUserButton;
		public AdminRedactUsers(List<User> users)
		{
			InitializeComponent();
			buttons = new List<Button>();
			foreach (User user in users)
			{
				Button button = new Button()
				{
					Content = $"{user.Name}" + $" ID: {user.ID}",
					Margin = new Thickness(5, 0, 5, 10),
					VerticalContentAlignment = VerticalAlignment.Center,
				};
				buttons.Add(button);
				button.Click += OpenRedact_Click;
				StackPanel.Children.Add(button);
			}
			this.users = users;
		}
		private void OpenRedact_Click(object sender, RoutedEventArgs e)
		{
			string keyword = " ID:";
			if (sender is Button button)
			{
				redacteringUserButton = button;
				var contentValue = button.Content;
				int index = ((string)contentValue).IndexOf(keyword);
				index += keyword.Length;
				string result = ((string)contentValue).Substring(index).Trim();

				IEnumerable<User> searchUser = from u in users
										 where u.ID == Convert.ToInt32(result)
										 select u;
				foreach (User item in searchUser)
					redacteringUser = item;
				UserNameRedact.Text = redacteringUser.Name;
				PasswordRedact.Text = redacteringUser.Password;
			}
		}
		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.GoBack();
		}
		private void DeliteUser_Click(object sender, RoutedEventArgs e)
		{
			if (redacteringUser == null)
				MessageBox.Show("Выберете пользователя");
			else
			{
				try
				{
					if (MessageBox.Show("Вы уверены что хотите удалить учётную запись пользователя?",
						"Удаление учётной записи",
						MessageBoxButton.YesNo,
						MessageBoxImage.Question) == MessageBoxResult.Yes)
					{
						// Удаление пользователя
						string deleteUserQury = $"DELETE From Users WHERE UserId = {redacteringUser.ID}";
						using(SqlCommand command = new SqlCommand(deleteUserQury, DataBaseConnection.Connection))
						{
							command.ExecuteNonQuery();
						}

                        // Удаление результатов
                        users.Remove(redacteringUser);
                        string deleteResQury = $"DELETE From Results WHERE UserId = {redacteringUser.ID}";
                        using (SqlCommand command = new SqlCommand(deleteResQury, DataBaseConnection.Connection))
                        {
                            command.ExecuteNonQuery();
                        }

                        if (sender is Button button)
						{ 
							buttons.Remove(button);
							StackPanel.Children.Remove(redacteringUserButton);
						}
						UserNameRedact.Text = "";
						PasswordRedact.Text= "";
						redacteringUser = null;
					}

				}
				catch(Exception ex)
				{
					MessageBox.Show("Произошла ошибка: " + ex.Message);
				}
			}
		}
		private void Confirm_Click(object sender, RoutedEventArgs e)
		{
			if (redacteringUser == null)
				MessageBox.Show("Выберете пользователя");
			else
			{
				try
				{
					if (UserNameRedact.Text == redacteringUser.Name || PasswordRedact.Text == redacteringUser.Password)
                        MessageBox.Show("Измените хотябы одну из строк");
                    else
						if (MessageBox.Show("Вы уверены что хотите удалить учётную запись пользователя?",
							"Удаление учётной записи",
							MessageBoxButton.YesNo,
							MessageBoxImage.Question) == MessageBoxResult.Yes)
						{
						    string redactUser = $"UPDATE Users SET Username = @login, PasswordHash = @password WHERE UserId = {redacteringUser.ID}";
						    using (SqlCommand command = new SqlCommand(redactUser, DataBaseConnection.Connection))
						    {
						        command.Parameters.AddWithValue("@login", UserNameRedact.Text);
						        command.Parameters.AddWithValue("@password", PasswordRedact.Text);
						        command.ExecuteNonQuery();
						    }
						    //redacteringUser.Name = UserNameRedact.Text;
						    //redacteringUser.Password = PasswordRedact.Text;
						    redacteringUserButton.Content = $"{UserNameRedact.Text} ID: {redacteringUser.ID}";
						    int index = users.FindIndex(s => s.ID == redacteringUser.ID);
						    users[index] = new User { ID = redacteringUser.ID, Name = UserNameRedact.Text, Password = PasswordRedact.Text };
						    UserNameRedact.Text = "";
						    PasswordRedact.Text = "";
						    redacteringUser = null;
						}


                }
				catch (Exception ex)
				{
					MessageBox.Show("Произошла ошибка: " + ex.Message);
				}
			}
		}
	}
}
