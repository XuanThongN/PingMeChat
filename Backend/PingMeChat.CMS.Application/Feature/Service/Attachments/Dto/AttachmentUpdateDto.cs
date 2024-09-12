using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System.ComponentModel.DataAnnotations;

namespace PingMeChat.CMS.Application.Feature.Service.Attachments.Dto
{
    public class AttachmentUpdateDto : UpdateDto
    {
        public string? MessageId { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? FileType { get; set; }
        public long? FileSize { get; set; }
    }
}
