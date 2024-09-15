using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.Service.Chats.Dto
{
    public class ChatCreateDto : CreateDto, IValidatableObject
    {
        public string? Name { get; set; } // Tên của đoạn chat hoặc tên của group chat (có thể null nếu là chat 1-1)

        [Required]
        public bool IsGroup { get; set; }

        public string? AvatarUrl { get; set; }

        [Required]
        public IEnumerable<string> UserIds { get; set; }

        // Custom validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsGroup && UserIds.Count() < 2)
            {
                yield return new ValidationResult("Group chat must have at least 2 other members.", new[] { nameof(UserIds) });
            }
            else if (!IsGroup && UserIds.Count() != 1)
            {
                yield return new ValidationResult("Private chat must have exactly one other member.", new[] { nameof(UserIds) });
            }
        }
    }
}
