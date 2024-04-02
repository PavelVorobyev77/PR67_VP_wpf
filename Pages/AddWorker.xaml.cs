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
using System.Xml.Linq;
using System.Net;
using System.IO;
using Microsoft.Office.Interop.Word;
using static PR67_VP.model.Workers;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using Page = System.Windows.Controls.Page;

namespace PR67_VP.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddWorker.xaml
    /// </summary>
    public partial class AddWorker : Page
    {
        private Entities1 context;
        private int? ID_Role;
        private string SelectedRoleName;

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
                WorkerPatronymic = txtWorkerPatronymic.Text,
                phoneNumber = txtPhoneNumber.Text,
                serie_pass = txtseriePass.Text,
                number_pass = txtnumPass.Text,
                w_login = txtLogin.Text,
                w_pswd = hash.HashPassword(txtPswd.Text),
                TwoFactorAuth = twoFactorAuthValue,
                ID_Role = (int)ID_Role


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
                context.Workers.Add(newWorker);

                context.SaveChanges();

                Contract(newWorker.serie_pass, newWorker.WorkerSurname, newWorker.WorkerName, newWorker.WorkerPatronymic, newWorker.number_pass, SelectedRoleName);

                DialogResult = true;

                MessageBox.Show("Данные сохранены успешно.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                //NavigationService.GoBack(); // Navigate back
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Contract(string serie_pass, string WorkerSurname, string WorkerName, string WorkerPatronymic, string number_pass, string roleName)
        {
            var items = new Dictionary<string, string>()
            {
                {"<city>", "Новосибирск"},
                {"<currentdate>", DateTime.Now.ToString("dd.MM.yyyy")},
                {"<olo>", "ООО"},
                {"<number>", "1"},
                {"<company>", "БОБ Строитель"},
                {"<dir>", "Воробьев П.А"},
                {"<worker>", "Воробьев П.А"},
                {"<address>", "Красный Проспект 121"},
                {"<kpp>", "123456789"},
                {"<zap>", "50000 рублей (50 тысяч рублей)" },
                {"<data>", "12"},
                {"<mvd>", "ГУ МВД России по Новосибирску"},
                {"<seriepass>", serie_pass},
                {"<name>", WorkerName},
                {"<surname>", WorkerSurname},
                {"<patronymic>", WorkerPatronymic},
                {"<numberpass>", number_pass},
                {"<role>", roleName}
            };

            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document wordDoc = null;
            try
            {
                wordApp = new Microsoft.Office.Interop.Word.Application();
                object missing = System.Reflection.Missing.Value;
                string fileName = @"C:\Users\pasch\Пашино\Колледж\3 КУРС\1 СЕМЕСТР\Программные модули\PR67_VP\Files\blank.docx";
                if (!File.Exists(fileName)) 
                {
                    MessageBox.Show("Файл не найден: " + fileName);
                    return;
                }
                wordDoc = wordApp.Documents.Open(fileName, ReadOnly: false, Visible: true);
                foreach (var item in items)
                {
                    object findText = item.Key;
                    object replaceText = item.Value;
                    Range myRange = wordDoc.Content;
                    myRange.Find.ClearFormatting();
                    myRange.Find.Execute(FindText: findText, ReplaceWith: replaceText, Replace: WdReplace.wdReplaceAll);
                }
                string newFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "contract_bob_theBuilder.docx");
                wordDoc.SaveAs2(newFilePath);
                MessageBox.Show("Документ успешно сохранен: " + newFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
            finally
            {
                wordDoc?.Close();
                wordApp?.Quit();
            }
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            txtWorkerName.Text = string.Empty;
            txtWorkerSurname.Text = string.Empty;
            txtWorkerPatronymic.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            txtseriePass.Text = string.Empty;
            txtnumPass.Text = string.Empty;     
            txtLogin.Text = string.Empty;
            txtPswd.Text = string.Empty;

            chbTwoFactorAuth.IsChecked = false;

            cb.SelectedIndex = -1; // Очищаем выбранное значение
        }

        private void PrintList_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() == true)
            {
                FlowDocument flowDoc = Doc.Document as FlowDocument;
                IDocumentPaginatorSource idp = flowdoc;
                pd.PrintDocument(idp.DocumentPaginator, "Title");
            }
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb.SelectedItem != null)
            {
                var selectedComboBoxItem = (ComboBoxItem)cb.SelectedItem;
                SelectedRoleName = selectedComboBoxItem.Content.ToString();
                int roleId;
                if (int.TryParse(selectedComboBoxItem.Tag.ToString(), out roleId))
                {
                    ID_Role = roleId;
                }
                else
                {
                    MessageBox.Show("Ошибка при получении ID роли.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

