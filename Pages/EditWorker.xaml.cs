using Microsoft.Win32;
using PR67_VP.model;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PR67_VP.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditWorker.xaml
    /// </summary>
    public partial class EditWorker : Page
    {
        private Entities1 context;
        private Workers Worker;

        public EditWorker(Workers worker)
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
                    if (worker != null)
                    {
                        txtId.Text = worker.ID_Worker.ToString();
                        txtWorkerName.Text = worker.WorkerName;
                        txtWorkerSurname.Text = worker.WorkerSurname;
                        txtWorkerPatronymic.Text = worker.WorkerPatronymic;
                        txtPhoneNumber.Text = worker.phoneNumber;
                        txtLogin.Text = worker.w_login;
                        txtPassword.Text = worker.w_pswd;
                        context.Entry(worker).State = EntityState.Modified;
                        MessageBox.Show("great");
                    }
                    else
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
            txtId.Text = string.Empty;
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
            if (string.IsNullOrWhiteSpace(txtId.Text) ||
                string.IsNullOrWhiteSpace(txtWorkerName.Text) ||
                string.IsNullOrWhiteSpace(txtWorkerSurname.Text) ||
                string.IsNullOrWhiteSpace(txtLogin.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Заполните все поля перед сохранением.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtId.Text, out int idWorker))
            {
                MessageBox.Show("Пожалуйста, введите корректное числовое значение для ID.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // Проверка, существует ли уже такой сотрудник
                Workers selectedWorker = context.Workers.FirstOrDefault(w => w.ID_Worker == idWorker);

                if (selectedWorker != null)
                {
                    // Обновление существующего сотрудника
                    selectedWorker.WorkerName = txtWorkerName.Text;
                    selectedWorker.WorkerSurname = txtWorkerSurname.Text;
                    selectedWorker.WorkerPatronymic = txtWorkerPatronymic.Text;
                    selectedWorker.phoneNumber = txtPhoneNumber.Text;
                    selectedWorker.w_login = txtLogin.Text;
                    selectedWorker.w_pswd = txtPassword.Text;

                    context.Entry(selectedWorker).State = EntityState.Modified;
                }
                else
                {
                    // Добавление нового сотрудника
                    Workers newWorker = new Workers
                    {
                        ID_Worker = idWorker,
                        WorkerName = txtWorkerName.Text,
                        WorkerSurname = txtWorkerSurname.Text,
                        WorkerPatronymic = txtWorkerPatronymic.Text,
                        phoneNumber = txtPhoneNumber.Text,
                        w_login = txtLogin.Text,
                        w_pswd = txtPassword.Text
                    };

                    context.Workers.Add(newWorker);
                }

                context.SaveChanges();

                MessageBox.Show("Данные сохранены успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить эту карточку?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                int workerId;

                if (int.TryParse(txtId.Text, out workerId))
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
