using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared.Authorization
{
    public static class Permissions
    {
        public static List<string> GeneralPermissionsForModule(string module)
        {
            return new List<string>(){
                $"Permissions.{module}.View",
                $"Permissions.{module}.Create",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete"
            };
        }
    }
}
