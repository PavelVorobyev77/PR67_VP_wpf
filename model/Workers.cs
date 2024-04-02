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
    using System.ComponentModel.DataAnnotations;

    public partial class Workers
    {
        public int ID_Worker { get; set; }
        public Nullable<int> ID_Object { get; set; }
        public Nullable<int> ID_Role { get; set; }


        [Required(ErrorMessage = "Имя работника является обязательным полем.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Имя работника должно быть от 2 до 50 символов.")]
        public string WorkerName { get; set; }


        [Required(ErrorMessage = "Фамилия работника является обязательным полем.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Фамилия работника должна быть от 3 до 50 символов.")]
        public string WorkerSurname { get; set; }
        public string WorkerPatronymic { get; set; }

        [Required(ErrorMessage = "Номер телефона является обязательным полем.")]
        [RegularExpression(@"^(\+7|8)[0-9]{10}$", ErrorMessage = "Неверный формат номера телефона. Номер должен начинаться с +7 или 8 и содержать ровно 11 цифр.")]
        public string phoneNumber { get; set; }

        [Required(ErrorMessage = "Логин является обязательным полем.")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Логин должен содержать от 4 до 20 символов.")]
        [RegularExpression(@"^[\w-]+(?:\.[\w-]+)*@[\w-]+(?:\.[\w-]+)*$", ErrorMessage = "Логин должен содержать только буквы, цифры, символы: _, -, и должен иметь формат электронной почты.")]
        public string w_login { get; set; }

        //[Required(ErrorMessage = "Пароль является обязательным полем.")]
        public string w_pswd { get; set; }
        public Nullable<int> ID_Gender { get; set; }
        public Nullable<int> TwoFactorAuth { get; set; }
        public byte[] Image { get; set; }
        [Required(ErrorMessage = "Серия паспорта обязательна для заполнения")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Серия паспорта должна состоять из 4 цифр")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Серия паспорта должна состоять только из цифр")]
        public string serie_pass { get; set; }

        [Required(ErrorMessage = "Номер паспорта обязателен для заполнения")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Номер паспорта должен состоять из 6 цифр")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Номер паспорта должен состоять только из цифр")]
        public string number_pass { get; set; }

        public virtual Objects Objects { get; set; }
        public virtual Role Role { get; set; }
        public virtual Gender Gender { get; set; }
    }
}
