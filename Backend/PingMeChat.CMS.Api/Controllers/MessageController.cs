using AutoWrapper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.Chats;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Messages;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;
using PingMeChat.CMS.Application.Feature.Service.MessageStatuses;
using PingMeChat.Shared;
using PingMeChat.Shared.Utils;

namespace PingMeChat.CMS.Api.Controllers
{
    [Authorize]
    public class MessageController : BaseController
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        [ValidateUserAndModel]
        [ChatAccess]
        [Route(ApiRoutes.Feature.Message.SendMessageRoute)]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> SendMessage(string chatId, [FromBody] string content)
        {
            var userId = GetUserId();
            var result = await _messageService.SendMessageAsync(chatId, userId, content);
            return Ok(new ApiResponse(string.Format(Message.Success.CreateCompleted, "message"), result, StatusCodes.Status201Created));
        }

        // Get messages
        [HttpGet]
        [ChatAccess]
        [Route(ApiRoutes.Feature.Message.GetChatMessagesRoute)]
        [ProducesResponseType(typeof(PagedResponse<List<MessageDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChatMessages(
            [FromRoute] string chatId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _messageService.GetChatMessagesAsync(
                chatId,
                pageNumber,
                pageSize,
                route: Request.Path.Value
            );
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "chat messages"), result, StatusCodes.Status200OK));
        }

    }
}
