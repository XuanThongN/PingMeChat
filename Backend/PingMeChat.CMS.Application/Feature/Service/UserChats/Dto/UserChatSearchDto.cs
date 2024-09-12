using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.UserChats.Dto
{
    public  class UserChatSearchDto : RequestDataTable
    {
        public string? ChatId { get; set; }
        public string? UserId { get; set; }
    }
}
