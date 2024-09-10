using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemEnum = System.Enum;

namespace PingMeChat.Shared.Enum
{
    public static class EnumHelper
    {
        public static string GetDescription<T>(this T enumValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
                return null;

            var description = enumValue.ToString();
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), true);
                if (attrs != null && attrs.Length > 0)
                {
                    description = ((DisplayAttribute)attrs[0]).Name;
                }
            }

            return description;
        }

        public static List<EnumModel> GetEnumList<T>() where T : struct, SystemEnum
        {
            var list = new List<EnumModel>();
            try
            {
                foreach (T item in SystemEnum.GetValues(typeof(T)))
                {
                    list.Add(new EnumModel
                    {
                        Value = Convert.ToInt32(item),
                        Name = GetDescription(item)
                    });
                }
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc xử lý theo cách phù hợp
                Console.WriteLine($"Error in GetEnumList: {ex.Message}");
                return null;
            }

            return list;
        }

    }

    public class EnumModel
    {
        public int Value { get; set; }
        public string Name { get; set; }
    }
}
