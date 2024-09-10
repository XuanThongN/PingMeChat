using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Common.Dto
{
    public abstract class CreateDto
    {
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

    }
}
