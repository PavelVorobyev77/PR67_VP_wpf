using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace hash_pswd
{
    public class hash
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Преобразуем пароль в массив байтов
                byte[] sourceBytes = Encoding.UTF8.GetBytes(password);

                // Вычисляем хэш пароля
                byte[] hashedBytes = sha256Hash.ComputeHash(sourceBytes);

                // Преобразуем хэш в строку
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    stringBuilder.Append(hashedBytes[i].ToString("x2")); // Форматируем каждый байт хэша как двузначное шестнадцатеричное число
                }

                return stringBuilder.ToString();
            }
        }
    }
}
