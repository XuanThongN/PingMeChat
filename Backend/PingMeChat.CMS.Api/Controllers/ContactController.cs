using AutoWrapper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service.Chats.Dto;
using PingMeChat.CMS.Application.Feature.Service.Contacts;
using PingMeChat.CMS.Application.Feature.Service.Contacts.Dto;
using PingMeChat.Shared;
using PingMeChat.Shared.Utils;

namespace PingMeChat.CMS.Api.Controllers
{
    [Authorize]
    public class ContactController : BaseController
    {
        private readonly IContactService _contactService;
        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
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
    }
}
