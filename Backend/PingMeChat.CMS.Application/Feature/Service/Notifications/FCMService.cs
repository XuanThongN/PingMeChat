using System.Threading.Tasks;
using FirebaseAdmin.Messaging;

namespace PingMeChat.CMS.Application.Feature.Service.Notifications
{
    public interface IFCMService
    {
        Task SendNotificationAsync(string token, string title, string body, Dictionary<string, string> data);
    }

    public class FCMService : IFCMService
    {
        public async Task SendNotificationAsync(string token, string title, string body, Dictionary<string, string> data)
        {
            var message = new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                },
                Data = data
            };

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine($"Successfully sent message: {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }
    }
}
