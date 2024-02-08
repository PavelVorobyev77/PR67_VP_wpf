//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PR67_VP.model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Workers
    {
        public int ID_Worker { get; set; }
        public Nullable<int> ID_Object { get; set; }
        public Nullable<int> ID_Role { get; set; }
        public string WorkerName { get; set; }
        public string WorkerSurname { get; set; }
        public string WorkerPatronymic { get; set; }
        public string phoneNumber { get; set; }
        public string w_login { get; set; }
        public string w_pswd { get; set; }
        public Nullable<int> ID_Gender { get; set; }
    
        public virtual Objects Objects { get; set; }
        public virtual Role Role { get; set; }
        public virtual Gender Gender { get; set; }

        public string Validate()
        {
            if (string.IsNullOrWhiteSpace(WorkerName) ||
                string.IsNullOrWhiteSpace(WorkerSurname) ||
                string.IsNullOrWhiteSpace(phoneNumber) ||
                string.IsNullOrWhiteSpace(w_login) ||
                string.IsNullOrWhiteSpace(w_pswd))
            {
                return "Заполните все обязательные поля.";
            }

            if (w_login.Length < 4 || w_login.Length > 20 || !IsLoginValid(w_login))
            {
                return "Логин должен содержать от 4 до 20 символов и содержать только буквы, цифры и символы: _, -";
            }

            if (!IsValidPhoneNumber(phoneNumber))
            {
                return "Неверный формат номера телефона. Номер должен начинаться с +7 или 8 и содержать ровно 11 цифр.";
            }

            return null; // Возвращаем null, если валидация прошла успешно
        }

        private bool IsLoginValid(string login)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(login, @"^[\w-]+$");
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^(\+7|8)[0-9]{10}$");
        }

        public string ValidateEdit()
        {
            if (string.IsNullOrWhiteSpace(WorkerName) ||
                string.IsNullOrWhiteSpace(WorkerSurname) ||
                string.IsNullOrWhiteSpace(phoneNumber) ||
                string.IsNullOrWhiteSpace(w_login))
            {
                return "Заполните все обязательные поля.";
            }

            if (w_login.Length < 4 || w_login.Length > 20 || !IsLoginValid(w_login))
            {
                return "Логин должен содержать от 4 до 20 символов и содержать только буквы, цифры и символы: _, -";
            }

            if (!IsValidPhoneNumber(phoneNumber))
            {
                return "Неверный формат номера телефона. Номер должен начинаться с +7 или 8 и содержать ровно 11 цифр.";
            }

            return null; // Возвращаем null, если валидация прошла успешно
        }
    }
}

