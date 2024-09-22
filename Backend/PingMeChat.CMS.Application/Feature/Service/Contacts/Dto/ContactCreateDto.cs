using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Contacts.Dto
{
    public class ContactCreateDto : CreateDto
    {   
        public string? UserId { get; set; } // Id của người dùng hiện tại

        [Required(ErrorMessage = "Friend ID is required")]
        public string ContactUserId { get; set; } // Id của người dùng mà người dùng hiện tại muốn thêm vào danh sách liên hệ

        public string? NickName { get; set; } // Tên hiển thị của người dùng trong danh sách liên hệ
    }
}
