using System;
using System.Collections.Generic;
using System.IO;
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

namespace HelpDesk
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        private Base_TitanEntities db;
        private int failedLogin = 0;
        private string captchaText;

        public AuthorizationWindow()
        {
            InitializeComponent();
            db = new Base_TitanEntities();
        }

        private void Button_Ok_Click(object sender, RoutedEventArgs e)
        {
            string login = tbLogin.Text;
            string password = pbPassword.Password;

            var user = db.Users.FirstOrDefault(u => u.Login == login);
            if (user != null)
            {
                if (string.IsNullOrEmpty(user.Salt))
                {
                    MessageBox.Show("Соль не найдена");
                    return;
                }

                // Проверяем пароль
                bool isValid = PasswordHelper.VerifyPassword(password, user.Password, user.Salt);
                if (isValid)
                {
                    // Сброс попыток при успешном входе
                    failedLogin = 0;

                    // Получаем роль пользователя
                    var role = db.Roles.FirstOrDefault(r => r.Role_ID == user.Role_ID);

                    // Проверяем роль пользователя
                    if (role != null)
                    {
                        if (role.Title_Role == "Полный доступ")
                        {
                            MainWindow mainWindow = new MainWindow();
                            mainWindow.Show();
                        }
                        else if (role.Title_Role == "Частичный доступ")
                        {
                            OutputOfTablesWindow outputOfTablesWindow = new OutputOfTablesWindow();
                            outputOfTablesWindow.Show();
                        }
                        else
                        {
                            MessageBox.Show("Нету такой роли");
                            return;
                        }

                        // Закрываем окно авторизации
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Роль не найдена");
                    }
                }
                else
                {
                    failedLogin++;
                    MessageBox.Show("Неверный пароль");

                    if (failedLogin >= 3)
                    {
                        using (var captchaImageStream = Captcha.GenerateCaptchaImage(out captchaText))
                        {
                            ShowCaptchaWindow(captchaImageStream);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Пользователь отсутствует");
            }
        }
        private void ShowCaptchaWindow(MemoryStream captchaImageStream)
        {
            CaptchaWindow captchaWindow = new CaptchaWindow(captchaImageStream, captchaText);
            bool? result = captchaWindow.ShowDialog();

            if (result == true)
            {
                failedLogin = 0;
            }
        }

        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow newUserWindow = new RegistrationWindow();
            newUserWindow.Show();
            this.Close();
        }
    }
}
