using hash_pswd;
using Microsoft.Win32;
using PR67_VP.model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using static PR67_VP.model.Workers;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace PR67_VP.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddWorker.xaml
    /// </summary>
    public partial class AddWorker : Page
    {
        private Entities1 context;

        public bool DialogResult { get; set; }

        public AddWorker()
        {
            InitializeComponent();
            context = new Entities1();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int twoFactorAuthValue = chbTwoFactorAuth.IsChecked == true ? 1 : 0; // Получаем значение для TwoFactorAuth: 1 если галочка установлена, иначе 0

            Workers newWorker = new Workers
            {
                WorkerName = txtWorkerName.Text,
                WorkerSurname = txtWorkerSurname.Text,
                phoneNumber = txtPhoneNumber.Text,
                w_login = txtLogin.Text,
                w_pswd = hash.HashPassword(txtPswd.Text),
                TwoFactorAuth = twoFactorAuthValue
            };

            /*
            * Обработка ошибок валидации:
            *  - Если присутствуют ошибки валидации:
            *      - Объединяются все сообщения об ошибках в одну строку.
            *      - Отображается сообщение об ошибке с объединенными сообщениями.
            */
            var validationContext = new ValidationContext(newWorker, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(newWorker, validationContext, validationResults, validateAllProperties: true);

            if (validationResults.Any())
            {
                string errorMessages = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                MessageBox.Show(errorMessages, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                context.Workers.Add(new Workers
                {
                    WorkerName = newWorker.WorkerName,
                    WorkerSurname = newWorker.WorkerSurname,
                    phoneNumber = newWorker.phoneNumber,
                    w_login = newWorker.w_login,
                    w_pswd = newWorker.w_pswd,
                    TwoFactorAuth = newWorker.TwoFactorAuth
                });

                context.SaveChanges();

                DialogResult = true;

                MessageBox.Show("Данные сохранены успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                //NavigationService.GoBack(); // Navigate back
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            txtWorkerName.Text = string.Empty;
            txtWorkerSurname.Text = string.Empty;
            txtWorkerPatronymic.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            txtLogin.Text = string.Empty;
            txtPswd.Text = string.Empty;
        }

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            // Создание диалогового окна для выбора изображения.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                // Установка изображения
                imgPhoto.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }
    }
}

