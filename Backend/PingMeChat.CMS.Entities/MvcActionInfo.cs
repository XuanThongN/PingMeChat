using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities
{
    public class MvcActionInfo
    {
        public string Id => $"{ControllerId}:{Name}";

        public string Name { get; set; }

        public string? DisplayName { get; set; }

        public string ControllerId { get; set; }
    }
}
