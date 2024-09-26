using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.CMS.Entities.Feature;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Phím tắt để xoá các import không cần thiết
// 

namespace PingMeChat.CMS.Application.Feature.Service.Attachments.Dto
{
    public class AttachmentCreateDto : CreateDto
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string? MessageId { get; set; }

    }
}
