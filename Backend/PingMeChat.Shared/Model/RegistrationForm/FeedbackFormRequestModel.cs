using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.Shared.Model.RegistrationForm
{
    public class FeedbackFormRequestModel
    {
        public string FormId { get; set; }
        public string FormTypeId { get; set; }
        public string Content { get; set; } 
        
    }
}
