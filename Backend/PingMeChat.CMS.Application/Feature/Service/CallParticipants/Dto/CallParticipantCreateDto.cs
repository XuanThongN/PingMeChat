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

namespace PingMeChat.CMS.Application.Feature.Service.CallParticipants.Dto
{
    public class CallParticipantCreateDto : CreateDto
    {
        public string CallId { get; set; }
        public string UserId { get; set; }
        public bool IsInitiator { get; set; }
        public DateTime JoinedTime { get; set; }
        public DateTime? LeftTime { get; set; }

    }
}
