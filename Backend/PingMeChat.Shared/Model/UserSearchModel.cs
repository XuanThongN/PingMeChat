using PingMeChat.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared.Model
{
    public class UserSearchModel : RequestDataTable
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? Keyword { get; set; }
        public bool IsLocked { get; set; } = false;
        public PositionType? PositionType { get; set; } = Shared.Enum.PositionType.IsStudent;
    }
}
