using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace PingMeChat.Shared.Utils
{
    public static class HelperMethod
    {
        public static string MD5Hash(this string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
        public static string HashPassword(this string password)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var salt = new byte[16];
                rng.GetBytes(salt);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
                {
                    var hash = pbkdf2.GetBytes(20);
                    var hashBytes = new byte[36];
                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 20);
                    return Convert.ToBase64String(hashBytes);
                }
            }
        }
        public static bool VerifyPassword(string inputPassword, string storedPassword)
        {
            var hashBytes = Convert.FromBase64String(storedPassword);
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            using (var pbkdf2 = new Rfc2898DeriveBytes(inputPassword, salt, 10000))
            {
                var hash = pbkdf2.GetBytes(20);
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public static bool IsPasswordStrong(this string password)
        {
            if (password.Length < 8)
                return false;

            bool hasUpperCase = false;
            bool hasLowerCase = false;
            bool hasDigit = false;
            bool hasSpecialChar = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c))
                    hasUpperCase = true;
                else if (char.IsLower(c))
                    hasLowerCase = true;
                else if (char.IsDigit(c))
                    hasDigit = true;
                else if (!char.IsLetterOrDigit(c))
                    hasSpecialChar = true;
            }

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }
        public static bool IsValidEmail(this string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
        public static bool IsValidVietnamPhoneNumber(this string phoneNumber)
        {
            string pattern = @"^0\d{9}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }

        public static string FormatVietnamesePhoneNumber(this string phoneNumber)
        {
            // Loại bỏ mọi ký tự không phải số
            phoneNumber = Regex.Replace(phoneNumber, @"\D", "");

            // Kiểm tra độ dài số điện thoại
            if (phoneNumber.Length == 10)
            {
                // Định dạng số di động
                return Regex.Replace(phoneNumber, @"(\d{3})(\d{3})(\d{4})", "$1 $2 $3");
            }
            else if (phoneNumber.Length == 11)
            {
                // Định dạng số cố định
                return Regex.Replace(phoneNumber, @"(\d{2})(\d{3})(\d{4})(\d{2})", "$1 $2 $3 $4");
            }
            else
            {
                // Số điện thoại không hợp lệ
                return "Số điện thoại không hợp lệ";
            }
        }
    }
}
