using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Application.Feature.Service.Attachments.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Messages.Dto
{
    public class MessageCreateDto : CreateDto
    {
        public string? TempId { get; set; } // id tạm thời để xác định tin nhắn
        [Required]
        public string ChatId { get; set; }
        public string SenderId { get; set; }
        public string? Content { get; set; }
        public List<AttachmentCreateDto>? Attachments { get; set; }
        public bool? IsRead { get; set; }
    }
}
