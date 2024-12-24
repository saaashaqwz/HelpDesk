using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HelpDesk
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private Base_TitanEntities db;
        public RegistrationWindow()
        {
            InitializeComponent();
            db = new Base_TitanEntities();
        }
        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (db.Users.Any(u => u.Login == tbLogin.Text))
                {
                    MessageBox.Show("Пользователь с таким логином уже существует");
                    return;
                }
                else
                {
                    string salt = PasswordHelper.GenerateSalt();
                    Users newUser = new Users
                    {
                        Login = tbLogin.Text,
                        Salt = salt,
                        Password = PasswordHelper.HashPassword(pbPassword.Password, salt),
                        Role_ID = 1
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();
                    MessageBox.Show("Регистрация прошла успешно!");

                    AuthorizationWindow authorizationWindow = new AuthorizationWindow();
                    authorizationWindow.Show();
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWindow authorizationWindow = new AuthorizationWindow();
            authorizationWindow.Show();
            this.Close();
        }
    }
}
