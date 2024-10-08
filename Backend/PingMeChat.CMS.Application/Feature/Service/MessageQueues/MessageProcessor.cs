using PingMeChat.CMS.Application.Feature.Service.Messages;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;

namespace PingMeChat.CMS.Application.Feature.Services.RabbitMQServices
{
    public class MessageProcessor
    {
        private readonly IMessageService _messageService;

        public MessageProcessor(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task ProcessMessageAsync(MessageCreateDto messageDto)
        {
            await _messageService.SendMessageAsync(messageDto);
        }
    }
}