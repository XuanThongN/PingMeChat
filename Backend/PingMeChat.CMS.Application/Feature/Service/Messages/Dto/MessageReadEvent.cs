namespace PingMeChat.CMS.Application.Feature.Service.Messages.Dto
{
    public class MessageReadEvent
    {
        public string MessageId { get; set; }
        public string ReaderId { get; set; }
        public string ChatId { get; set; }
    }
}