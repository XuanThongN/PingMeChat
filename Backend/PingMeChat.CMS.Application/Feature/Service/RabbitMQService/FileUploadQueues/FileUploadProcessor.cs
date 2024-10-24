using Microsoft.AspNetCore.Http;
using PingMeChat.CMS.Application.Feature.ChatHubs;
using PingMeChat.CMS.Application.Feature.Service.Attachments;
using PingMeChat.CMS.Application.Feature.Service.Attachments.Dto;
using PingMeChat.CMS.Application.Feature.Service.Messages;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;
using PingMeChat.Shared.Enum;

namespace PingMeChat.CMS.Application.Feature.Services.RabbitMQServices.FileUploadQueues
{
    public class FileUploadProcessor
    {
        private readonly IAttachmentService _attachmentService;
        private readonly IChatHubService _chatHubService;

        public FileUploadProcessor(IAttachmentService attachmentService, IChatHubService chatHubService)
        {
            _attachmentService = attachmentService;
            _chatHubService = chatHubService;
        }

        public async Task ProcessUploadFileAsync(IFormFile file, FileUploadMessage message)
        {
            var uploadResult = await _attachmentService.UploadFileAsync(file);
            if (uploadResult != null)
            {
                // Update URL in database
                await _attachmentService.UpdateFileUrl(message.UploadId, message.FileName, uploadResult.Url);

                // Send realtime notification to client in chat
                await _chatHubService.NotifyUploadFileSuccessAsync(new AttachmentDto
                {
                    UploadId = message.UploadId,
                    FileName = message.FileName,
                    FilePath = uploadResult.Url,
                    FileType = FileTypeHelper.GetFileTypeFromMimeType(message.MimeType).GetDescription(),
                    FileSize = new FileInfo(message.FilePath).Length,
                    ChatId = message.ChatId,
                    MessageId = message.MessageId
                });
            }
        }
    }
}