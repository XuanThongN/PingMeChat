using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System.ComponentModel.DataAnnotations;

namespace PingMeChat.CMS.Application.Feature.Service.Messages.Dto
{
    public class MessageUpdateDto : UpdateDto
    {
        [Required]
        public string ChatId { get; set; }
        [Required]
        public string SenderId { get; set; }
        [Required]
        public string Content { get; set; }
        public string? AttachmentUrl { get; set; }
        public bool IsRead { get; set; }
    }
}
