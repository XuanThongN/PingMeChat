using AutoWrapper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Octokit.Internal;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Contacts;
using PingMeChat.CMS.Application.Feature.Service.Contacts.Dto;
using PingMeChat.CMS.Application.Feature.Service.Users;
using PingMeChat.Shared;
using PingMeChat.Shared.Enum;
using PingMeChat.Shared.Utils;

namespace PingMeChat.CMS.Api.Controllers
{
    [Authorize]
    public class ContactController : BaseController
    {
        private readonly IContactService _contactService;
        private readonly IConfiguration _configuration;
        private readonly IUserService _accountService;
        private readonly IHttpClientFactory _httpClientFactory;
        public ContactController(IContactService contactService, IConfiguration configuration, IUserService userService, IHttpClientFactory httpClient)
        {
            _contactService = contactService;
            _configuration = configuration;
            _accountService = userService;
            _httpClientFactory = httpClient;
        }

        [HttpGet]
        [Route(ApiRoutes.Feature.Contact.GetAllByCurrentUserRoute)]
        [ProducesResponseType(typeof(IEnumerable<ContactDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllByCurrentUser()
        {
            var currentUserId = GetUserId();
            var data = await _contactService.GetUserContacts(currentUserId);
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedCompleted, "liên hệ người dùng"), data, StatusCodes.Status200OK));
        }


        // Thêm liên hệ mới cho người dùng
        [HttpPost]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Contact.AddRoute)]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Add([FromBody] ContactCreateDto dto)
        {
            var userId = GetUserId();
            dto.UserId = userId; // Lấy id của người dùng hiện tại
            // Nếu người dùng thêm liên hệ cho chính mình
            if (dto.ContactUserId == userId)
            {
                throw new AppException(string.Format(Message.Error.IsExisted, "Liên hệ"), StatusCodes.Status400BadRequest);
            }
            // Kiểm tra xem người dùng đã tồn tại trong danh sách liên hệ của người dùng hiện tại chưa
            var isExist = await _contactService.Find(u =>
                (u.UserId == userId && u.ContactUserId == dto.ContactUserId) ||
                (u.UserId == dto.ContactUserId && u.ContactUserId == userId)
            );

            if (isExist != null)
            {
                throw new AppException(string.Format(Message.Error.IsExisted, "Liên hệ"), StatusCodes.Status400BadRequest);
            }
            var data = await _contactService.Add(dto);
            return Ok(new ApiResponse(Message.Success.CreateCompleted, data, StatusCodes.Status200OK));
        }


        // Chấp nhận lời mời kết bạn
        [HttpPut]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Contact.AcceptFriendRequestRoute)]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> AcceptFriendRequest(string contactId)
        {
            var userId = GetUserId();
            var updatedContact = await _contactService.AcceptFriendRequest(userId, contactId);
            if (updatedContact == null)
            {
                throw new AppException("Không tìm thấy yêu cầu kết bạn hoặc không có quyền chấp nhận.", StatusCodes.Status404NotFound);
            }
            return Ok(new ApiResponse("Đã chấp nhận lời mời kết bạn.", updatedContact, StatusCodes.Status200OK));
        }

        // Hủy lời mời kết bạn
        [HttpDelete]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Contact.CancelFriendRequestRoute)]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> CancelFriendRequest(string contactId)
        {
            var userId = GetUserId();
            var result = await _contactService.CancelFriendRequest(userId, contactId);
            if (!result)
            {
                throw new AppException("Không tìm thấy yêu cầu kết bạn hoặc không có quyền hủy.", StatusCodes.Status404NotFound);
            }
            return Ok(new ApiResponse("Đã hủy lời mời kết bạn.", null, StatusCodes.Status200OK));
        }

        // Gửi lời mời kết bạn
        [HttpPost]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Contact.SendFriendRequestRoute)]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> SendFriendRequest([FromBody] ContactCreateDto contact)
        {
            var currentUserId = GetUserId();

            if (currentUserId == contact.ContactUserId)
            {
                throw new AppException("Không thể gửi lời mời kết bạn cho chính mình.", StatusCodes.Status400BadRequest);
            }

            var newContact = await _contactService.SendFriendRequest(currentUserId, contact.ContactUserId);
            return Created(string.Empty, new ApiResponse("Đã gửi lời mời kết bạn.", newContact, StatusCodes.Status201Created));
        }

        // Gợi ý kết bạn
        [HttpGet]
        [Route(ApiRoutes.Feature.Contact.RecommendFriendsRoute)]
        [ProducesResponseType(typeof(IEnumerable<AccountDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RecommendFriends()
        {
            var currentUserId = GetUserId();
            var recommendationApiUrl = _configuration["RecommendationApiUrl"];

            // Sử dụng HttpClient để gọi API khuyến nghị bạn bè
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"{recommendationApiUrl}?user_id={currentUserId}");
            var apiResponse = await response.Content.ReadFromJsonAsync<RecommendationResponseDto>();

            if (apiResponse == null || apiResponse.Recommendations == null || !apiResponse.Recommendations.Any())
            {
                return Ok(new ApiResponse("No recommendations found", new List<AccountDto>(), StatusCodes.Status200OK));
            }
            var recommendedAccountIds = apiResponse.Recommendations;
            // Lọc ra các tài khoản đã là bạn bè hoặc đã gửi lời mời kết bạn
            var contactIdsDictionary = await _contactService.GetAllContactIds(currentUserId);
            // Loại bỏ các tài khoản đã là bạn bè hoặc đã gửi lời mời kết bạn
            recommendedAccountIds = apiResponse.Recommendations
                .Where(r => !contactIdsDictionary.ContainsKey(r))
                .ToList();
            // lấy thông tin chi tiết của các tài khoản được khuyến nghị dựa theo id 
            // và vị trí thứ tự phải giữ nguyên với danh sách được khuyến nghị
            var recommendedAccounts = await _accountService.FindAll(x => recommendedAccountIds.Contains(x.Id));

            // Sắp xếp lại danh sách recommendedAccounts theo thứ tự của recommendedAccountIds
            var sortedRecommendedAccounts = recommendedAccountIds
                .Select(id => recommendedAccounts.FirstOrDefault(account => account.Id == id))
                .Where(account => account != null)
                .ToList();

            return Ok(new ApiResponse("Friend recommendations retrieved successfully", sortedRecommendedAccounts, StatusCodes.Status200OK));
        }

        // Lấy danh sách id bạn bè kèm trạng thái hoạt động
        [HttpGet]
        [Route(ApiRoutes.Feature.Contact.GetAllFriendsStatusRoute)]
        [ProducesResponseType(typeof(Dictionary<string, bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFriendIds()
        {
            var currentUserId = GetUserId();
            var contactIdsDictionary = await _contactService.GetAllFriendContactStatuses(currentUserId);
            return Ok(new ApiResponse("Friend statuses retrieved successfully", contactIdsDictionary, StatusCodes.Status200OK));
        }
    }
}
