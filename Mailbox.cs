﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PR67_VP
{
    /*internal class YandexMailSender
    {
        public static string SendMailYandex(string userEmail)
        {
            string yandexAppPassword = "eqnzyuimfailzhjd";
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("egor18425@yandex.ru");
            mail.To.Add(userEmail);
            mail.Subject = "Код подтверждения";
            string confirmationCode = GenerateFourDigitCode();
            mail.Body = $"Ваш код подтверждения: {confirmationCode}";
            SmtpClient smtpClient = new SmtpClient("smtp.yandex.ru");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("egor18425@yandex.ru", yandexAppPassword);
            try
            {
                smtpClient.Send(mail);
                Console.WriteLine("Письмо успешно отправлено");
                return confirmationCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отправке письма: " + ex.Message);
                return null;
            }
        }

        private static string GenerateFourDigitCode()
        {
            Random random = new Random();
            int code = random.Next(1000, 10000);
            return code.ToString("D4");
        }
    }*/

    internal class MailRuMailSender
    {

        public static string SendMailRu(string userEmail)
        {
            string mailRuAppPassword = "AYevNHUt6sjzkqGJv7JE";
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("pascha27_05@mail.ru");
            mail.To.Add(userEmail);
            mail.Subject = "Код подтверждения";
            string confirmationCode = GenerateFourDigitCode();
            mail.Body = $"Ваш код подтверждения: {confirmationCode}";
            SmtpClient smtpClient = new SmtpClient("smtp.mail.ru");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("pascha27_05@mail.ru", mailRuAppPassword);
            try
            {
                smtpClient.Send(mail);
                Console.WriteLine("Письмо успешно отправлено");
                return confirmationCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отправке письма: " + ex.Message);
                return null;
            }
        }
        private static string GenerateFourDigitCode()
        {
            Random random = new Random();
            int code = random.Next(1000, 10000);
            return code.ToString("D4");
        }
    }

    /*internal class GmailSender
    {
        public static string SendGMail(string userEmail)
        {
            string gmailAppPassword = "navp wxkd uluc osja";
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("egorantonov1804@gmail.com");
            mail.To.Add(userEmail);
            mail.Subject = "Код подтверждения";
            string confirmationCode = GenerateFourDigitCode();
            mail.Body = $"Ваш код подтверждения: {confirmationCode}";
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential("egorantonov1804@gmail.com", gmailAppPassword);
            try
            {
                smtpClient.Send(mail);
                Console.WriteLine("Письмо успешно отправлено");
                return confirmationCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отправке письма: " + ex.Message);
                return null;
            }
        }
        private static string GenerateFourDigitCode()
        {
            Random random = new Random();
            int code = random.Next(1000, 10000);
            return code.ToString("D4");
        }
    }*/
}
