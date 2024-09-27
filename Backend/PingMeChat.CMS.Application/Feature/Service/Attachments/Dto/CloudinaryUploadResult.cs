using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System.ComponentModel.DataAnnotations;

namespace PingMeChat.CMS.Application.Feature.Service.Attachments.Dto
{
    public class CloudinaryUploadResult
    {
        public string PublicId { get; set; }
        public string FileName {get; set;}
        public string Url { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
    }


}
