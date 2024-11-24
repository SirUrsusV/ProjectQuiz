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

namespace ProjectQuiz
{
    /// <summary>
    /// Логика взаимодействия для AdminMenu.xaml
    /// </summary>
    public partial class AdminMenu : Page
    {
        Admin admin;
        public AdminMenu(Admin admin)
        {
            InitializeComponent();
            HelloText.Text = $"Приветствуем Администратор\n{admin.AdminUserName}";
            this.admin = admin;
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            
            LoginAndRegisterWindow LR = new LoginAndRegisterWindow();
            LR.Show();
            Window.GetWindow(this).Close();
        }

        private void RedactUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminRedactUsers(admin.Users));
        }

        private void RedactQuizes_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminRedactQuizzes(admin.Sections));
        }
    }
}
