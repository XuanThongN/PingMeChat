using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PingMeChat.CMS.Application.Common.Dto;
using PingMeChat.Shared.Enum;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;

namespace PingMeChat.CMS.Application.Feature.Service.Attachments.Dto
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }

}
