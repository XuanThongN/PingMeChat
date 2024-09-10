using PingMeChat.CMS.Entities.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Entities.Module
{
    public class ErrorLog : AuditableBaseEntity
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Exception { get; set; }
    }
}
