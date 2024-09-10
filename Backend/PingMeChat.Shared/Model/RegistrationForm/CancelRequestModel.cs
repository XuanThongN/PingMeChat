using PingMeChat.Shared.Enum;

namespace PingMeChat.Shared.Model.RegistrationForm
{
    public class CancelRequestModel
    {
        public string FormTypeId { get; set; }
        public string StudentCode { get; set; }
        public string ReasonCancel { get; set; } // lý do hủy
    }
}
