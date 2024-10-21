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
        private readonly string _tempDirectory;
        private readonly ILogger<AttachmentController> _logger;

        public AttachmentController(IAttachmentService attachmentService, IConfiguration configuration, ILogger<AttachmentController> logger)
        {
            _attachmentService = attachmentService;
            _tempDirectory = configuration["TempUploadDirectory"] ?? Path.GetTempPath();
            _logger = logger;
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Attachment.UploadChunkRoute)]
        public async Task<IActionResult> UploadChunk([FromForm] ChunkUploadModel model)
        {
            if (model.Chunk == null || model.Chunk.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            if (model.Chunk.Length > 25 * 1024 * 1024) // 25MB limit
            {
                return BadRequest("File size exceeds the maximum limit of 25MB");
            }

            var uploadDir = Path.Combine(_tempDirectory, model.UploadId);
            Directory.CreateDirectory(uploadDir);

            var chunkPath = Path.Combine(uploadDir, $"chunk_{model.ChunkIndex}");

            try
            {
                using var stream = new FileStream(chunkPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
                await model.Chunk.CopyToAsync(stream);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading chunk");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Attachment.CompleteUploadRoute)]
        public async Task<IActionResult> CompleteUpload([FromBody] CompleteUploadModel model)
        {
            var uploadDir = Path.Combine(_tempDirectory, model.UploadId);
            if (!Directory.Exists(uploadDir))
            {
                return BadRequest("Upload ID not found");
            }

            var tempFilePath = Path.Combine(_tempDirectory, $"{Guid.NewGuid()}_{model.FileName}");

            try
            {
                await using (var outputStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                {
                    var chunks = Directory.GetFiles(uploadDir).OrderBy(f => int.Parse(Path.GetFileName(f).Split('_')[1]));
                    foreach (var chunk in chunks)
                    {
                        await using var inputStream = new FileStream(chunk, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
                        await inputStream.CopyToAsync(outputStream);
                    }
                }

                CloudinaryUploadResult uploadResult;
                await using (var fileStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read))
                {
                    var formFile = new FormFile(fileStream, 0, new FileInfo(tempFilePath).Length, model.FileName, model.FileName)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = model.MimeType
                    };

                    uploadResult = await _attachmentService.UploadFileAsync(formFile);
                }

                // Clean up
                Directory.Delete(uploadDir, true);
                System.IO.File.Delete(tempFilePath);

                if (uploadResult == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Upload failed");
                }

                return Ok(new ApiResponse("Upload completed successfully", uploadResult, StatusCodes.Status200OK));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while completing upload");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        public class ChunkUploadModel
        {
            public string? UploadId { get; set; }
            public int ChunkIndex { get; set; }
            public int TotalChunks { get; set; }
            public IFormFile Chunk { get; set; }
        }

        public class CompleteUploadModel
        {
            public string? UploadId { get; set; }
            public string? FileName { get; set; }
            public string? MimeType { get; set; }
            public long FileSize { get; set; }
        }
    }
}
