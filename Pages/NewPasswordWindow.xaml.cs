using hash_pswd;
using PR67_VP.model;
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
using System.Windows.Shapes;

namespace PR67_VP
{
    /// <summary>
    /// Логика взаимодействия для NewPasswordWindow.xaml
    /// </summary>
    public partial class NewPasswordWindow : Window
    {
        private string userEmail;
        public NewPasswordWindow(string userEmail)
        {
            InitializeComponent();
            this.userEmail = userEmail;
        }

        private void SaveNewPassword_Click(object sender, RoutedEventArgs e)
        {
            string newPassword = txtNewPassword.Password;
            string confirmPassword = txtConfirmPassword.Password;

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают. Пожалуйста, попробуйте снова!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            ChangePassword(userEmail, newPassword);
        }
        public void ChangePassword(string userEmail, string newPassword)
        {
            using (Entities1 bd = new Entities1())
            {
                try
                {
                    Workers worker = bd.Workers.FirstOrDefault(w => w.w_login == userEmail);

                    if (worker != null)
                    {
                        worker.w_pswd = hash.HashPassword(newPassword);
                        bd.SaveChanges();
                        MessageBox.Show("Пароль успешно изменен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Пользователь с таким email не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при изменении пароля: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

