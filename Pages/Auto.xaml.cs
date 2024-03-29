﻿using System;
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

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            /* 
            * Логика входа, если все ввели правильно, то вход выполнен, иначе нет
            */
            // Проверяем разрешен ли доступ
            if (!IsAccessAllowed())
            {
                // Если доступ заблокирован, выводим сообщение об ошибке
                MessageBox.Show("Кнопка входа заблокирована. Подождите, пока не истечет время блокировки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();

            // Хэшируем введенный пароль
            string hashedPassword = hash.HashPassword(password);

            using (Entities1 db = new Entities1())
            {
                // Пытаемся найти пользователя в базе данных по введенному логину и хэшированному паролю
                Workers worker = db.Workers.FirstOrDefault(p => p.w_login == login && p.w_pswd == hashedPassword);

                if (worker != null)
                {
                    // Если пользователь найден, выводим персонализированное приветственное сообщение и загружаем форму
                    string welcomeMessage = GetWelcomeMessage(worker);
                    MessageBox.Show(welcomeMessage);
                    countUnsuccessful = 0;
                    LoadForm(worker.Role.RoleName, welcomeMessage);

                }
                else
                {
                    // Если пользователь не найден, увеличиваем счетчик неудачных попыток, генерируем CAPTCHA и выводим сообщение об ошибке
                    countUnsuccessful++;
                    GenerateCaptcha();
                    MessageBox.Show("Вы ввели неверный логин или пароль! Введите капчу для продолжения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                // Проверяем, достигнут ли лимит неудачных попыток для блокировки кнопки входа
                if (countUnsuccessful >= 3)
                {
                    BlockLoginButton();
                }

                if (worker != null)
                {
                    // Проверяем, включена ли у пользователя двухфакторная аутентификация
                    if (worker.TwoFactorAuth == 1)
                    {
                        string userEmail = db.Workers.FirstOrDefault(w => w.ID_Worker == worker.ID_Worker)?.w_login;
                        string confirmationCode = null;

                        if (userEmail != null)
                        {
                            if (userEmail.Contains("@mail.ru"))
                            {
                                // Отправляем код подтверждения на электронную почту
                                confirmationCode = MailRuMailSender.SendMailRu(userEmail);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Адрес электронной почты пользователя не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        // Открываем окно подтверждения и обрабатываем результат
                        ConfirmWindow confirmWindow = new ConfirmWindow(confirmationCode);
                        bool? result = confirmWindow.ShowDialog();

                        if (result == true)
                        {
                            GetWelcomeMessage(worker);
                        }
                        else
                        {
                            MessageBox.Show("Введенный код неверный. Пожалуйста, попробуйте еще раз!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    return;
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
            else if (currentHour >= 17 && currentHour <= 23)
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
            return currentHour >= 10 && currentHour < 23;
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
    }
}
