using PingMeChat.Shared.Enum;
using Newtonsoft.Json;
using System.Text;

namespace PingMeChat.CMS.Application.Common.Config
{
    public static class TextHelper
    {
         public static string NormalizeVietnamese(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            StringBuilder result = new StringBuilder(text);
            string[] vietnameseSigns = new string[]
            {
                "aAeEoOuUiIdDyY",
                "áàạảãâấầậẩẫăắằặẳẵ",
                "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
                "éèẹẻẽêếềệểễ",
                "ÉÈẸẺẼÊẾỀỆỂỄ",
                "óòọỏõôốồộổỗơớờợởỡ",
                "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
                "úùụủũưứừựửữ",
                "ÚÙỤỦŨƯỨỪỰỬỮ",
                "íìịỉĩ",
                "ÍÌỊỈĨ",
                "đ",
                "Đ",
                "ýỳỵỷỹ",
                "ÝỲỴỶỸ"
            };

            for (int i = 1; i < vietnameseSigns.Length; i++)
            {
                for (int j = 0; j < vietnameseSigns[i].Length; j++)
                    result.Replace(vietnameseSigns[i][j], vietnameseSigns[0][i - 1]);
            }

            return result.ToString();
        }
    }
}
