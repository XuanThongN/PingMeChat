using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Contacts.Dto
{
    public  class ContactSearchDto : RequestDataTable
    {
        public string? UserId { get; set; }
        public string? FriendId { get; set; }
        public string? NickName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
    }
}
