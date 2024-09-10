using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared
{
    public class Settings
    {
        public static IConfiguration Configuration;
        public static string ServiceUrl
        {
            get
            {
                var config = Configuration["ServiceUrl"];
                return config != null ? config : string.Empty;
            }
        }
        public static string JWT_Secret
        {
            get
            {
                var config = Configuration["JWT:Secret"];
                return config != null ? config : string.Empty;
            }
        }
        public static string JWT_ValidAudience
        {
            get
            {
                var config = Configuration["JWT:ValidAudience"];
                return config != null ? config : string.Empty;
            }
        }
        public static string JWT_ValidIssuer
        {
            get
            {
                var config = Configuration["JWT:ValidIssuer"];
                return config != null ? config : string.Empty;
            }
        }

        public static int JWT_TokenExpiresMinutes
        {
            get
            {
               return int.Parse(Configuration["JWT:TokenExpiresMinutes"]);
            }
        }

        public static int JWT_RefreshTokenExpiresDays
        {
            get
            {
                return int.Parse(Configuration["JWT:RefreshTokenExpiresDays"]);
            }
        }
    }

}
