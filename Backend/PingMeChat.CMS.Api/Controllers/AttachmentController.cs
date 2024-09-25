using AutoWrapper.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingMeChat.CMS.Api.Controllers;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.CMS.Application.Feature.Service.Attachments;
using PingMeChat.CMS.Application.Feature.Service.Attachments.Dto;
using PingMeChat.CMS.Application.Feature.Service.CallParticipants;
using PingMeChat.Shared.Utils;

namespace PingMeAttachment.CMS.Api.Controllers
{
    [Authorize]
    public class AttachmentController : BaseController
    {
        private readonly IAttachmentService _attachmentService;

        public AttachmentController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Attachment.UploadMultipleFilesRoute)]
        [ProducesResponseType(typeof(List<CloudinaryUploadResult>), StatusCodes.Status201Created)]
        public async Task<IActionResult> UploadMultipleFiles([FromForm] List<IFormFile> files)
        {
            // Kiểm tra nếu không có file nào được chọn
            if (files == null || !files.Any())
            {
                return Ok(new ApiResponse(Message.Error.CreateError, null, StatusCodes.Status400BadRequest));
            }

            // Kiểm tra giới hạn file (giả sử chỉ cho phép upload tối đa 5 file cùng lúc)
            if (files.Count > 5)
            {
                return BadRequest("Chỉ có thể upload tối đa 5 file một lần.");
            }

            var uploadResults = new List<CloudinaryUploadResult>();

            foreach (var file in files)
            {
                // Kiểm tra kích thước của từng file (20MB = 20 * 1024 * 1024 bytes)
                if (file.Length > 20 * 1024 * 1024)
                {
                    return BadRequest($"File {file.FileName} vượt quá giới hạn kích thước 20MB.");
                }

                try
                {
                    var uploadResult = await _attachmentService.UploadFileAsync(file);

                    // Kiểm tra nếu upload thất bại
                    if (uploadResult == null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, $"Không thể upload file {file.FileName}.");
                    }

                    uploadResults.Add(uploadResult);
                }
                catch (Exception ex)
                {
                    // Log lỗi nếu có
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Có lỗi xảy ra khi upload file {file.FileName}: {ex.Message}");
                }
            }

            // Trả về kết quả của tất cả các file đã được upload
            return Ok(new ApiResponse(Message.Success.CreateCompleted, uploadResults, StatusCodes.Status200OK));
        }


    }
}
