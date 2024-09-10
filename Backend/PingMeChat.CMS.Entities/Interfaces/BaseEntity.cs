using PingMeChat.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Interfaces
{
    public abstract class BaseEntity : IBaseEntity
    {
        public virtual string Id { get; set; } = Guid.NewGuid().ToString();
       
    }
}
