using PingMeChat.CMS.AdminPage.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PingMeChat.CMS.AdminPage.ViewComponents
{
    public class PageAlertViewComponent : ViewComponent
    {

        public PageAlertViewComponent()
        {
        }

        public IViewComponentResult Invoke(string filter)
        {
            List<Message> messages;
            if (ViewBag.PageAlerts == null)
            {
                messages = new List<Message>();
            }
            else
            {
                messages = new List<Message>(ViewBag.PageAlerts);
            }

            //if (TempData["PageAlerts"] == null)
            //{
            //    messages = new List<Message>();
            //}
            //else
            //{
            //    var tempData = JsonConvert.DeserializeObject<List<Message>>(TempData["PageAlerts"].ToString());;

            //    messages = new List<Message>(tempData);
            //}
            
            return View(messages);
        }
    }
}
