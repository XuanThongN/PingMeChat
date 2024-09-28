using AutoWrapper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingMeChat.CMS.Api.Controllers;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Config;
using PingMeChat.CMS.Application.Feature.Service.Chats;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Contacts;
using PingMeChat.CMS.Application.Feature.Service.Users;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.Shared.Enum;
using PingMeChat.Shared.Utils;
using System.Linq;
using System.Threading.Tasks;

[Authorize]
public class SearchController : BaseController
{
    private readonly IChatService _chatService;
    private readonly IUserService _userService;
    private readonly IContactService _contactService;

    public SearchController(IChatService chatService, IUserService userService, IContactService contactService)
    {
        _chatService = chatService;
        _userService = userService;
        _contactService = contactService;
    }

    [HttpGet]
    [Route(ApiRoutes.Feature.Search.SearchRoute)]
    [ProducesResponseType(typeof(SearchResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string keyword,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20
    )
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "search"), null, StatusCodes.Status200OK));
        }
        // Normalize keyword
        // var normalizedKeyword = keyword.NormalizeVietnamese().ToLower();    

        var contactIds = await _contactService.GetAllContactIds(userId);
        var users = await _userService.Pagination(
                pageNumber,
                pageSize,
                predicate: x =>
                        x.Id != userId &&
                       x.UserName.ToLower().Contains(keyword.ToLower()) ||
                        x.FullName!.ToLower().Contains(keyword.ToLower()) ||
                        x.Email!.ToLower().Contains(keyword.ToLower()) ||
                        x.PhoneNumber!.ToLower().Contains(keyword.ToLower()),
                orderBy: ord => ord.OrderBy(x => x.FullName));
        
        var userSearchDtos = new List<UserSearchDto>();
        foreach (var user in users.Data)
        {
            // Check if there is a private chat between the current user and the searched user
            var privateChat = await _chatService.Find(x =>
                    (x.UserChats.Any(u => u.UserId == userId) &&
                    x.UserChats.Any(u => u.UserId == user.Id)) &&
                    !x.IsGroup);
            var userSearchDto = new UserSearchDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,
                Status = contactIds.ContainsKey(user.Id) ? contactIds[user.Id] : ContactStatus.Stranger,
                ChatPrivateId = privateChat?.Id
            };
            userSearchDtos.Add(userSearchDto);
        }

        var groupChats = await _chatService.Pagination(
            pageNumber,
            pageSize,
            predicate: x =>
            x.IsGroup &&
            x.UserChats.Any(u => u.UserId == userId) &&
           x.Name!.ToLower().Contains(keyword.ToLower()),
           include: includes => includes.Include(x => x.UserChats).ThenInclude(x => x.User)
           );

        var groupChatDtos = groupChats.Data.Select(x => new GroupChatDto
        {
            Id = x.Id,
            Name = x.Name,
            IsGroup = x.IsGroup,
            AvatarUrl = x.AvatarUrl,
            UserChats = x.UserChats.Select(u => new ParticipantDto
            {
                Id = u.UserId,
                FullName = u.UserDto!.FullName,
                AvatarUrl = u.UserDto!.AvatarUrl,
            })
        }).ToList();

        var result = new SearchResult
        {
            Users = userSearchDtos,
            GroupChats = groupChatDtos
        };

        return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "search"), result, StatusCodes.Status200OK));
    }

}


public class SearchResult
{
    public List<UserSearchDto> Users { get; set; } = new List<UserSearchDto>();
    public List<GroupChatDto> GroupChats { get; set; } = new List<GroupChatDto>();
}


// Tạo class UserSearchDto
public class UserSearchDto
{
    public string Id { get; set; }
    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public string? ChatPrivateId { get; set; }
    public ContactStatus Status { get; set; }
}


// Tạo class GroupChatDto
public class GroupChatDto
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public bool IsGroup { get; set; }
    public string? AvatarUrl { get; set; }
    public IEnumerable<ParticipantDto> UserChats { get; set; }
}

public class ParticipantDto
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string? AvatarUrl { get; set; }
}