using hash_pswd;
using Microsoft.Win32;
using PR67_VP.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
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
using static PR67_VP.model.Workers;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace PR67_VP.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditWorkers.xaml
    /// </summary>
    public partial class EditWorkers : Window
    {
        public ObservableCollection<Workers> Workers { get; set; }
        private Entities1 context;
        private Workers Worker;

        public EditWorkers(Workers worker)
        {
            InitializeComponent();
            Worker = worker;
            context = new Entities1();
            DataContext = this;
            LoadData(Worker.ID_Worker); // Вызов метода LoadData() для загрузки данных выбранного сотрудника

        }

        // Загрузка данных для указанного сотрудника
        private void LoadData(int workerId)
        {
            using (var context = new Entities1())
            {
                Workers worker = context.Workers.Find(workerId);
                try
                {
                    if (worker == null)
                    {
                        // Сотрудник не найден, можно обработать этот сценарий
                        // Например, можно вывести сообщение или очистить поля ввода
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("fail");
                }
            }
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            txtWorkerName.Text = string.Empty;
            txtWorkerSurname.Text = string.Empty;
            txtWorkerPatronymic.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            txtLogin.Text = string.Empty;
            txtPassword.Text = string.Empty;
        }

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int twoFactorAuthValue = chbTwoFactorAuth.IsChecked == true ? 1 : 0; // Получаем значение для TwoFactorAuth: 1 если галочка установлена, иначе 0

            Workers editedWorker = new Workers()
            {
                WorkerName = txtWorkerName.Text,
                WorkerSurname = txtWorkerSurname.Text,
                WorkerPatronymic = txtWorkerPatronymic.Text,
                phoneNumber = txtPhoneNumber.Text,
                w_login = txtLogin.Text,
                w_pswd = txtPassword.Text,
                TwoFactorAuth = twoFactorAuthValue
            };

            // Получение ошибок валидации
            var validationContext = new ValidationContext(editedWorker, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(editedWorker, validationContext, validationResults, validateAllProperties: true);

            if (validationResults.Any())
            {
                string errorMessages = string.Join("\n", validationResults.Select(r => r.ErrorMessage));
                MessageBox.Show(errorMessages, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Workers selectedWorker = context.Workers.FirstOrDefault(w => w.ID_Worker == Worker.ID_Worker);

            if (selectedWorker != null)
            {
                selectedWorker.WorkerName = editedWorker.WorkerName;
                selectedWorker.WorkerSurname = editedWorker.WorkerSurname;
                selectedWorker.WorkerPatronymic = editedWorker.WorkerPatronymic;
                selectedWorker.phoneNumber = editedWorker.phoneNumber;
                selectedWorker.w_login = editedWorker.w_login;
                selectedWorker.TwoFactorAuth = editedWorker.TwoFactorAuth;

                if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    string hashedPassword = hash.HashPassword(txtPassword.Text);
                    selectedWorker.w_pswd = hashedPassword;
                }

                try
                {
                    context.SaveChanges();
                    MessageBox.Show("Данные сохранены успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить эту карточку?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                int workerId = Worker.ID_Worker;

                if (workerId > 0)
                {
                    Workers selectedWorker = context.Workers.FirstOrDefault(w => w.ID_Worker == workerId);

                    if (selectedWorker != null)
                    {
                        context.Workers.Remove(selectedWorker);
                        context.SaveChanges();

                        MessageBox.Show("Карточка удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }
    }
}
