using PingMeChat.CMS.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities
{
    public partial class Media : AuditableBaseEntity
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int Size { get; set; }
        public bool IsAvatar { get; set; } // Đánh dấu hình ảnh có là ảnh đại diện hay không
    }
}
