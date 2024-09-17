using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.Feature.ChatHubs
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            var result =  connection.User?.FindFirstValue("UserId");
            return result ?? string.Empty;
        }
    }
}
