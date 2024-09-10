using PingMeChat.Shared.Enum;
using Newtonsoft.Json;

namespace PingMeChat.CMS.Application.Common.Config
{
    public static class JsonHelper
    {
        public static object? TryConvertJsonToObject(string jsonString, FormTypeEnum formTypeCode)
        {
            //switch (formTypeCode)
            //{
            //    case FormTypeEnum.PingMeChat_DXCBD: // đơn xin cấp bảng điểm
            //        return JsonConvert.DeserializeObject<PingMeChat_DDKHP>(jsonString);
            //    case FormTypeEnum.PingMeChat_DDKHP: // đơn đăng ký học phần
            //        return JsonConvert.DeserializeObject<PingMeChat_DXCBD>(jsonString);
            //}
            return null;
        }

        //public static AbstractForm CreateForm(string jsonString, FormTypeEnum formTypeCode)
        //{
        //    switch (formTypeCode)
        //    {
        //        case FormTypeEnum.PingMeChat_DXCBD: // đơn xin cấp bảng điểm
        //            return JsonConvert.DeserializeObject<TranscriptRequest>(jsonString);
        //        case FormTypeEnum.PingMeChat_DDKHP: // đơn đăng ký học phần
        //            return JsonConvert.DeserializeObject<CourseRegistrationForm>(jsonString);
        //    }
        //    return null;
        //}
    }
}
