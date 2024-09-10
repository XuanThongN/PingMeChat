using PingMeChat.Shared.Enum;

namespace PingMeChat.Shared.Model.RegistrationForm
{
    public class TemporaryFormSearchModel : RequestDataTable
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Keyword { get; set; }
        public StatusProcessing? Status { get; set; }
        public string? FormTypeId { get; set; }
       // public string? DateOSubmission { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }
}
