using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared
{
    public static class PasswordHelper
    {
        public static string GenerateRandomPassword()
        {
            string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string specialChars = "!@";
            string number = "0123456789";
            string allChars = "abcdefghijklmnopqrstuvwxyz" + uppercaseChars + specialChars + number;

            Random random = new Random();
            StringBuilder password = new StringBuilder();

            // Đảm bảo ít nhất một ký tự hoa
            password.Append(uppercaseChars[random.Next(uppercaseChars.Length)]);

            // Đảm bảo ít nhất một ký tự đặc biệt
            password.Append(specialChars[random.Next(specialChars.Length)]);

            // Đảm bảo ít nhất một ký tự số
            password.Append(number[random.Next(number.Length)]);

            // Tạo các ký tự còn lại ngẫu nhiên
            for (int i = 0; i < 6; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Trộn các ký tự trong mật khẩu
            string shuffledPassword = new string(password.ToString().ToCharArray().OrderBy(x => random.Next()).ToArray());

            return shuffledPassword;
        }
    }
}
