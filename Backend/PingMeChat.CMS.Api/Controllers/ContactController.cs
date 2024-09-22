using AutoWrapper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Common.Pagination;
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
        public ContactController(IContactService ContactService)
        {
            _contactService = ContactService;
        }

        [HttpGet]
        [Route(ApiRoutes.Feature.Contact.GetAllByCurrentUserRoute)]
        [ProducesResponseType(typeof(ContactDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllByCurrentUser()
        {
            var currentUserId = GetUserId();
            var data = await _contactService.FindAll(u => u.UserId == currentUserId || u.ContactUserId == currentUserId);
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedCompleted, "liên hệ người dùng"), data, StatusCodes.Status200OK));
        }


        // Thêm liên hệ mới cho người dùng
        [HttpPost]
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
    }
}
