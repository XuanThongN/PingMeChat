using AutoWrapper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Attributes;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Feature.Service.Chats;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Messages;
using PingMeChat.CMS.Application.Feature.Service.Messages.Dto;
using PingMeChat.CMS.Application.Feature.Service.MessageStatuses;
using PingMeChat.Shared.Utils;

namespace PingMeChat.CMS.Api.Controllers
{
    //[Authorize]
    public class ChatController : BaseController
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Chat.CreateChatRoute)]
        [ProducesResponseType(typeof(ChatDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateChat([FromBody] ChatCreateDto chatCreateDto)
        {
            var userId = GetUserId();
            var result = await _chatService.CreateChatAsync(chatCreateDto, userId);
            return Ok(new ApiResponse(string.Format(Message.Success.CreateCompleted, "chat"), result, StatusCodes.Status201Created));
        }

        [HttpGet]
        [ChatAccess]
        [Route(ApiRoutes.Feature.Chat.GetChatDetailRoute)]
        [ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChatDetail([FromRoute] string chatId)
        {
            var userId = GetUserId();
            var result = await _chatService.GetChatDetailAsync(chatId, userId);
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedCompleted, "chat detail"), result, StatusCodes.Status200OK));
        }

        //[HttpGet]
        //[Route(ApiRoutes.Feature.Chat.GetChatListRoute)]
        //[ProducesResponseType(typeof(IEnumerable<ChatDto>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetChatList()
        //{
        //    var userId = GetUserId();
        //    var result = await _chatService.GetChatListAsync(userId);
        //    return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "chat list"), result, StatusCodes.Status200OK));
        //}

        [HttpGet]
        [Route(ApiRoutes.Feature.Chat.GetChatListRoute)]
        [ProducesResponseType(typeof(IEnumerable<ChatDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetChatList(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20
            )
        {
            var userId = GetUserId();
            //var result = await _chatService.GetChatListAsync(userId);
            var result = await _chatService.GetChatListAsync(
                userId,
                pageNumber,
                pageSize,
                route: Request.Path.Value
            );
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "chat list"), result, StatusCodes.Status200OK));
        }

        [HttpPost]
        [ChatAccess]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Chat.AddUserToChatRoute)]
        [ProducesResponseType(typeof(ChatDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddUserToChat(string chatId, [FromBody] string userId)
        {
            var currentUserId = GetUserId();
            var result = await _chatService.AddUserToChatAsync(chatId, userId, currentUserId);
            return Ok(new ApiResponse(string.Format(Message.Success.CreateCompleted, "user to chat"), result, StatusCodes.Status200OK));
        }

        [HttpDelete]
        [ChatAccess]
        [Route(ApiRoutes.Feature.Chat.RemoveUserFromChatRoute)]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveUserFromChat(string chatId, string userId)
        {
            var currentUserId = GetUserId();
            var result = await _chatService.RemoveUserFromChatAsync(chatId, userId, currentUserId);
            return Ok(new ApiResponse(string.Format(Message.Success.DeletedCompleted, "user from chat"), result, StatusCodes.Status200OK));
        }

    }
}
