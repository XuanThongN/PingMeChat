using AutoWrapper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.Chats;
using PingMeChat.CMS.Application.Feature.Service.Users;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.Shared;
using PingMeChat.Shared.Utils;

namespace PingMeChat.CMS.Api.Controllers
{
    [Authorize]
    public class ChatController : BaseController
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;

        public ChatController(IChatService chatService, IUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
        }

        // [HttpGet(ApiRoutes.Feature.Chat.GetChatListRoute)]
        // public async Task<ApiResponse> GetChatList([FromQuery] PaginationQuery query)
        // {
        //     var chats = await _chatService.GetChatList(query);
        //     return ApiResponse.Success(chats);
        // }

        // [HttpGet(ApiRoutes.Feature.Chat.GetChatDetailRoute)]
        // public async Task<ApiResponse> GetChatDetail(string id)
        // {
        //     var chat = await _chatService.GetChatDetail(id);
        //     return ApiResponse.Success(chat);
        // }

        // [HttpPost(ApiRoutes.Feature.Chat.CreateChatRoute)]
        // public async Task<ApiResponse> CreateChat([FromBody] CreateChatDto dto)
        // {
        //     var chat = await _chatService.CreateChat(dto);
        //     return ApiResponse.Success(chat);
        // }

        // [HttpPut(ApiRoutes.Feature.Chat.UpdateChatRoute)]
        // public async Task<ApiResponse> UpdateChat(string id, [FromBody] UpdateChatDto dto)
        // {
        //     var chat = await _chatService.UpdateChat(id, dto);
        //     return ApiResponse.Success(chat);
        // }

        // [HttpDelete(ApiRoutes.Feature.Chat.DeleteChatRoute)]
        // public async Task<ApiResponse> DeleteChat(string id)
        // {
        //     await _chatService.DeleteChat(id);
        //     return ApiResponse.Success();
        // }
    }
}
