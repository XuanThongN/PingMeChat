using PingMeChat.Shared.Enum;

namespace PingMeChat.Shared.Model.RegistrationForm
{
    public class TemporaryFormConfirmModel
    {
        public string FormTypeId { get; set; }
        public StatusProcessing Status { get; set; }
        public string StudentCode { get; set; }
    }
}
