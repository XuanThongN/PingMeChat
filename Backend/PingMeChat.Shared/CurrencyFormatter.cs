using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared
{
    public static class CurrencyFormatter
    {
        public static string FormatToCurrency(this decimal amount)
        {
            CultureInfo culture = new CultureInfo("vi-VN");
            string formattedAmount = string.Format(culture, "{0:n0} đ", amount);
            return formattedAmount;
        }
        public static string FormatToCurrencyWithoutUnit(this decimal amount) // không có đơn vị tiền tệ
        {
            CultureInfo culture = new CultureInfo("vi-VN");
            string formattedAmount = string.Format(culture, "{0:n0}", amount);
            return formattedAmount;
        }
    }
}
