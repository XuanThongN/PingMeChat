using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared
{
    public static class DateHelper
    {   
        public static string ToVietNamFormat(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }
        // Hàm chuyển đổi từ DateTime sang chuỗi theo định dạng "yyyy-MM-dd HH:mm"
        public static string ConvertToMinuteFormat(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm");
        }

        // Hiển thị thời gian theo chỉ bằng giờ và phút
        public static string FormatDurationWithHourMinute(this TimeSpan duration)
        {
            int h = (int)duration.TotalHours;
            int m = duration.Minutes;
            return h == 0 ? $"{m} phút" :
                   m == 0 ? $"{h} giờ" :
                   $"{h} giờ {m:D2} phút";
        }



        public static string ToVietNamFormatDate(this DateTime dateTime)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), vietnamTimeZone);
            return vietnamDateTime.ToString("dd/MM/yyyy");
        }

        public static string ToVietNamFormatTime(this DateTime dateTime)
        {
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime vietnamDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), vietnamTimeZone);
            return vietnamDateTime.ToString("HH:mm");
        }

        public static DateTime ConvertToDateTime(string value)
        {
            DateTime result;
            DateTime.TryParseExact(value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out result);
            return result;
        }
        
        public static bool IsValidateTime(string value)
        {
            DateTime result;
            return DateTime.TryParseExact(value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out result);
            {
            }
        }
        public static string GetTimeAgoString(DateTime dateTime)
        {
            // TimeSpan: đối tượng để tính toán khoảng thời gian
            TimeSpan timeAgo = DateTime.Now - dateTime;

            if (timeAgo.TotalSeconds < 60)
            {
                return $"{Math.Floor(timeAgo.TotalSeconds)} giây trước";
            }
            else if (timeAgo.TotalMinutes < 60)
            {
                return $"{Math.Floor(timeAgo.TotalMinutes)} phút trước";
            }
            else if (timeAgo.TotalHours < 24)
            {
                return $"{Math.Floor(timeAgo.TotalHours)} giờ trước";
            }
            else if (timeAgo.TotalDays < 7)
            {
                return $"{Math.Floor(timeAgo.TotalDays)} ngày trước";
            }
            else if (timeAgo.TotalDays < 30)
            {
                int weeks = (int)Math.Floor(timeAgo.TotalDays / 7);
                return $"{weeks} tuần trước";
            }
            else if (timeAgo.TotalDays < 365)
            {
                int months = (int)Math.Floor(timeAgo.TotalDays / 30);
                return $"{months} tháng trước";
            }
            else
            {
                int years = (int)Math.Floor(timeAgo.TotalDays / 365);
                return $"{years} năm trước";
            }
        }

        public static string ConvertMinutesToTimeString(this int minutes)
        {
            int hours = minutes / 60;
            int remainingMinutes = minutes % 60;

            string result = "";

            if (hours > 0)
            {
                result += $"{hours} giờ";
                if (remainingMinutes > 0)
                {
                    result += " ";
                }
            }

            if (remainingMinutes > 0 || (hours == 0 && remainingMinutes == 0))
            {
                result += $"{remainingMinutes} phút";
            }

            return result.Trim();
        }

        //Hàm lấy thời gian chỉ tính tới phút
        public static DateTime RoundToMinute(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
        }
    }
}
