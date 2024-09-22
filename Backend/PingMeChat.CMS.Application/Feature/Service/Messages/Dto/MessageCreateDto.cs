using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
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
        [Required]
        public string ChatId { get; set; }
        public string SenderId { get; set; }
        [Required]
        public string? Content { get; set; }
        public string? AttachmentUrl { get; set; }
        public bool IsRead { get; set; }
    }
}
