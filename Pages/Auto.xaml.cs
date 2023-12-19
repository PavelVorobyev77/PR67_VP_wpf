using System;
using System.Collections.Generic;
using System.Linq;
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
using PR67_VP.model;
using PR67_VP.Pages;


namespace PR67_VP.Pages
{
    /// <summary>
    /// Логика взаимодействия для Auto.xaml
    /// </summary>
    public partial class Auto : Page
    {
        private int countUnsuccessful = 0;
        private string captcha = string.Empty;
        private bool isButtonBlocked = false;
        private int countdownDuration = 10;
        private DispatcherTimer countdownTimer;
        private int z = 0;

        public Auto()
        {
            InitializeComponent();

            txtCaptcha.Visibility = Visibility.Hidden;
            textBlockCaptcha.Visibility = Visibility.Hidden;

            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = new TimeSpan(0, 0, 1);
            countdownTimer.Tick += CountdownTimer_Tick;
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            int remainingSeconds = countdownDuration - z;
            if (remainingSeconds > 0)
            {
                seconds.Text = $"Вход заблокирован, попробуйте\nснова, через: {remainingSeconds} секунд";
                z++;


            }
            else
            {
                countdownTimer.Stop();
                isButtonBlocked = false;
                btnEnter.IsEnabled = true;
                btnEnterGuest.IsEnabled = true;
                txtLogin.IsEnabled = true;
                txtPassword.IsEnabled = true;
                txtCaptcha.IsEnabled = true;
                z = 0;
                seconds.Text = null;
            }
        }

        private void btnEnterGuest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client());
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            if (!IsAccessAllowed())
            {
                MessageBox.Show("Кнопка входа заблокирована. Подождите, пока не истечет время блокировки.");
                return;
            }

            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();

            // Хэшируем введенный пароль
            string hashedPassword = hash.HashPassword(password);

            using (Entities1 db = new Entities1())
            {
                Workers worker = db.Workers.FirstOrDefault(p => p.w_login == login && p.w_pswd == hashedPassword);

                if (worker != null)
                {
                    // Получение приветственного сообщения с указанием времени суток
                    string welcomeMessage = GetWelcomeMessage(worker);
                    MessageBox.Show(welcomeMessage);
                    countUnsuccessful = 0;
                    LoadForm(worker.Role.RoleName, welcomeMessage);

                }
                else
                {
                    countUnsuccessful++;
                    GenerateCaptcha();
                    MessageBox.Show("Вы ввели неверный логин или пароль! Введите капчу для продолжения.");
                }

                if (countUnsuccessful >= 3)
                {
                    BlockLoginButton();
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
            else if (currentHour >= 17 && currentHour <= 19)
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
            return currentHour >= 10 && currentHour < 19;
        }

        private void GenerateCaptcha()
        {
            textBlockCaptcha.Visibility = Visibility.Visible;
            txtCaptcha.Visibility = Visibility.Visible;

            Random random = new Random();
            int length = random.Next(5, 10);
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
    }
}
