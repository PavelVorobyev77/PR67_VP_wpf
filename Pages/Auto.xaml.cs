using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using hash_pswd;
using Newtonsoft.Json;
using PR67_VP.model;
using PR67_VP.Pages;


namespace PR67_VP.Pages
{
    /// <summary>
    /// Логика взаимодействия для Auto.xaml
    /// </summary>

    public partial class Auto : Page
    {
        private int countUnsuccessful = 0; // Счетчик неудачных попыток входа
        private string captcha = string.Empty; // Переменная для хранения капчи
        private bool isButtonBlocked = false; // блокировка кнопки
        private int countdownDuration = 10; // Длительность блокировки в секундах
        private DispatcherTimer countdownTimer; // Таймер для отсчета времени
        private int z = 0; // Переменная для отслеживания времени блокировки

        public Auto()
        {
            InitializeComponent();

            // Скрываем элементы для ввода капчи при инициализации
            txtCaptcha.Visibility = Visibility.Hidden;
            textBlockCaptcha.Visibility = Visibility.Hidden;

            // Инициализация таймера обратного отсчета
            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = new TimeSpan(0, 0, 1);
            countdownTimer.Tick += CountdownTimer_Tick;
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            /* 
            * Логика обновления отображаемого времени и разблокировки формы
            * Рассчитываем оставшееся время блокировки и обновляем отображаемое сообщение
            */
            int remainingSeconds = countdownDuration - z;
            if (remainingSeconds > 0)
            {
                // Если время блокировки еще не истекло, обновляем текстовое сообщение с обратным отсчетом
                seconds.Text = $"Вход заблокирован, попробуйте\nснова, через: {remainingSeconds} секунд";
                z++;


            }
            else
            {
                /* Если время блокировки истекло, останавливаем таймер, разблокируем элементы управления и сбрасываем состояние */
                countdownTimer.Stop();
                isButtonBlocked = false;
                btnEnter.IsEnabled = true;
                btnEnterGuest.IsEnabled = true;
                txtLogin.IsEnabled = true;
                txtPassword.IsEnabled = true;
                txtCaptcha.IsEnabled = true;
                z = 0;
                seconds.Text = null; // Сбрасываем текст сообщения
            }
        }

        private void btnEnterGuest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client());
        }


        private async void btnEnter_Click(object sender, RoutedEventArgs e)
        {


            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();

            //string hashedPassword = hash.HashPassword(password);

            // Создание объекта запроса
            var request = new LoginRequest { Login = login, Password = password };
            string jsonRequest = JsonConvert.SerializeObject(request);

            using (var client = new HttpClient())
            {
                try
                {
                    // Установка базового адреса вашего API
                    client.BaseAddress = new Uri("https://localhost:7235");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Отправка POST-запроса на конечную точку API
                    HttpResponseMessage response = await client.PostAsync("/api/Auto/login", new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                    // Обработка ответа API
                    if (response.IsSuccessStatusCode)
                    {
                        // Чтение и парсинг тела ответа
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        LoginResponse loginResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonResponse);

                        // Пример: отображение приветственного сообщения или загрузка формы
                        string welcomeMessage = $"Добро пожаловать, {loginResponse.Login}!";
                        MessageBox.Show(welcomeMessage);
                        countUnsuccessful = 0;
                        LoadForm(loginResponse.Role, welcomeMessage);
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Обработка неавторизованного доступа
                        MessageBox.Show("Вы ввели неверный логин или пароль! Введите капчу для продолжения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        countUnsuccessful++;
                        GenerateCaptcha();

                        // Проверка необходимости блокировки кнопки входа
                        if (countUnsuccessful >= 3)
                        {
                            BlockLoginButton();
                        }
                    }
                    else
                    {
                        // Обработка других кодов состояния HTTP
                        MessageBox.Show($"Ошибка входа: {response.StatusCode}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private string GetWelcomeMessage(Workers worker)
        {
            string greeting = "";

            // Определение времени суток
            int currentHour = DateTime.Now.Hour;
            if (currentHour >= 10 && currentHour <= 12)
            {
                greeting = "Доброе утро";
            }
            else if (currentHour >= 12 && currentHour <= 17)
            {
                greeting = "Добрый день";
            }
            else if (currentHour >= 17 && currentHour <= 24)
            {
                greeting = "Добрый вечер";
            }

            // Формирование фамилии и имени сотрудника
            string fullName;
            if (!string.IsNullOrEmpty(worker.WorkerPatronymic))
            {
                fullName = $"{worker.WorkerSurname} {worker.WorkerName} {worker.WorkerPatronymic}";
            }
            else
            {
                fullName = $"{worker.WorkerSurname} {worker.WorkerName}";
            }

            // Формирование приветственного сообщения с временем суток, приставкой (Mr/Mrs), и именем сотрудника
            string welcomeMessage = $"{greeting}, {GetSalutation(worker)} {fullName} ({worker.Role.RoleName})!";
            return welcomeMessage;
        }

        private string GetSalutation(Workers worker)
        {
            if (worker.ID_Gender == 1)
            {
                return "Mrs";
            }
            else
            {
                return "Mr";
            }
        }

        private bool IsAccessAllowed()
        {
            int currentHour = DateTime.Now.Hour;
            return currentHour >= 10 && currentHour < 2;
        }

        private void GenerateCaptcha()
        {
            textBlockCaptcha.Visibility = Visibility.Visible;
            txtCaptcha.Visibility = Visibility.Visible;

            Random random = new Random();
            int length = random.Next(5, 10); //Определяет желаемую длину кода капчи (от 5 до 10 символов)
            /*
            * Определяет строку, содержащую все возможные символы для кода капчи
            * Включает как строчные, так и прописные буквы
            */
            string captchaCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string generatedCaptcha = string.Empty;

            for (int i = 0; i < length; i++)
            {
                int randomIndex = random.Next(0, captchaCharacters.Length);
                generatedCaptcha += captchaCharacters[randomIndex];
            }

            captcha = generatedCaptcha;

            textBlockCaptcha.Text = captcha;
            textBlockCaptcha.TextDecorations = TextDecorations.Strikethrough;
        }

        private void BlockLoginButton()
        {
            txtLogin.IsEnabled = false;
            txtPassword.IsEnabled = false;
            txtCaptcha.IsEnabled = false;
            isButtonBlocked = true;
            btnEnter.IsEnabled = false;
            btnEnterGuest.IsEnabled = false;
            countdownTimer.Start();
        }

        private void LoadForm(string role, string welcomeMessage)
        {
            switch (role) 
            {
                case "Админ": //1
                    NavigationService.Navigate(new Admin { WelcomeMessage = welcomeMessage });
                    break;
                case "Прораб": //2
                    NavigationService.Navigate(new Pr { WelcomeMessage = welcomeMessage });
                    break;
                case "Инженер": //3
                    NavigationService.Navigate(new In { WelcomeMessage = welcomeMessage });
                    break;
                case "Клиент": //4
                    NavigationService.Navigate(new Client { WelcomeMessage = welcomeMessage });
                    break;
            }
        }

        private void btnforget_Click(object sender, RoutedEventArgs e)
        {
            PasswordRecoveryWindow passwordRecoveryWindow = new PasswordRecoveryWindow();
            passwordRecoveryWindow.Show();
        }

        public class LoginRequest
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        public class LoginResponse
        {
            public string Login { get; set; }
            public bool TwoFactorEnabled { get; set; }
            public string Role { get; set; }
        }
    }
}
