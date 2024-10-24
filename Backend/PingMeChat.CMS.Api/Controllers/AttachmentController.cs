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
using PingMeChat.CMS.Application.Feature.Services.RabbitMQServices;
using PingMeChat.CMS.Application.Feature.Services.RabbitMQServices.FileUploadQueues;
using PingMeChat.Shared.Utils;
using Swashbuckle.AspNetCore.Annotations;

namespace PingMeAttachment.CMS.Api.Controllers
{
    [Authorize]
    public class AttachmentController : BaseController
    {
        private readonly IAttachmentService _attachmentService;
        private readonly string _tempDirectory;
        private readonly ILogger<AttachmentController> _logger;
        private readonly IRabbitMQService _rabbitMQService;

        public AttachmentController(IAttachmentService attachmentService,
        IConfiguration configuration,
        ILogger<AttachmentController> logger,
        IRabbitMQService rabbitMQService
        )
        {
            _attachmentService = attachmentService;
            _tempDirectory = configuration["TempUploadDirectory"] ?? Path.GetTempPath();
            _logger = logger;
            _rabbitMQService = rabbitMQService;
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Attachment.UploadChunkRoute)]
        [SwaggerOperation(Summary = "Uploading each chunk of file.", Description = "Then save in temp file until uploading all chunks of file")]
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

            // Tạo model để lưu thông tin file khi upload xong
            var uploadResult = new CloudinaryUploadResult();
            try
            {
                // Sử dụng using statement để đảm bảo stream được dispose
                using (var stream = new FileStream(chunkPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 4096, true))
                {
                    await model.Chunk.CopyToAsync(stream);
                }

                // Check if this is the last chunk
                if (model.ChunkIndex == model.TotalChunks - 1)
                {
                    var tempFilePath = Path.Combine(_tempDirectory, $"{Guid.NewGuid()}_{model.FileName}");

                    try
                    {
                        // Sử dụng buffer để đọc và ghi file
                        const int bufferSize = 81920; // 80KB buffer
                        using (var outputStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, true))
                        {
                            var chunks = Directory.GetFiles(uploadDir)
                                .OrderBy(f => int.Parse(Path.GetFileName(f).Split('_')[1]))
                                .ToList(); // Convert to list to avoid multiple enumerations

                            foreach (var chunk in chunks)
                            {
                                // Sử dụng separate try-catch cho mỗi chunk
                                try
                                {
                                    using (var inputStream = new FileStream(chunk, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize, true))
                                    {
                                        await inputStream.CopyToAsync(outputStream, bufferSize);
                                        inputStream.Close(); // Explicitly close the stream
                                    }
                                }
                                catch (IOException ex)
                                {
                                    _logger.LogError(ex, $"Error reading chunk file: {chunk}");
                                    // Thêm retry logic nếu cần
                                    await Task.Delay(100); // Wait briefly before retry
                                    using (var inputStream = new FileStream(chunk, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize, true))
                                    {
                                        await inputStream.CopyToAsync(outputStream, bufferSize);
                                        inputStream.Close();
                                    }
                                }
                            }

                            uploadResult.FileName = model.FileName;
                            uploadResult.FileSize = outputStream.Length;
                            uploadResult.FileType = model.MimeType;
                        }

                        // Generate temporary URL
                        var tempUrl = GenerateTemporaryUrl(tempFilePath);
                        uploadResult.Url = tempUrl;

                        // Send message to RabbitMQ to process the file upload to Cloudinary
                        _rabbitMQService.PublishMessage("file_upload_queue", new FileUploadMessage
                        {
                            FileName = model.FileName,
                            MimeType = model.MimeType,
                            FileSize = model.FileSize ?? 0,
                            ChatId = model.ChatId,
                            MessageId = model.MessageId,
                            FilePath = tempFilePath
                        });

                        // Clean up chunks safely
                        await CleanupChunksAsync(uploadDir);

                        return Ok(new ApiResponse("Upload completed successfully", uploadResult, StatusCodes.Status200OK));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error occurred while combining chunks");
                        await CleanupChunksAsync(uploadDir); // Cleanup on error
                        throw;
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while uploading chunk");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        private async Task CleanupChunksAsync(string directory)
        {
            int maxRetries = 3;
            int currentRetry = 0;

            while (currentRetry < maxRetries)
            {
                try
                {
                    if (Directory.Exists(directory))
                    {
                        // Ensure all file handles are released
                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        Directory.Delete(directory, true);
                    }
                    break;
                }
                catch (IOException)
                {
                    currentRetry++;
                    if (currentRetry == maxRetries)
                    {
                        _logger.LogWarning($"Failed to delete directory {directory} after {maxRetries} attempts");
                        break;
                    }
                    await Task.Delay(100 * currentRetry); // Exponential backoff
                }
            }
        }

        private string GenerateTemporaryUrl(string filePath)
        {
            // Implement logic to generate a temporary URL for the file
            var fileName = Path.GetFileName(filePath);
            return Url.Action("GetTemporaryFile", "Attachment", new { fileName }, Request.Scheme);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(ApiRoutes.Feature.Attachment.GetTemporaryFile)]
        public IActionResult GetTemporaryFile(string fileName)
        {
            var filePath = Path.Combine(_tempDirectory, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var mimeType = "application/octet-stream";
            return PhysicalFile(filePath, mimeType, fileName);
        }

        public class ChunkUploadModel
        {
            public string? UploadId { get; set; } = Guid.NewGuid().ToString();
            public int ChunkIndex { get; set; }
            public int TotalChunks { get; set; }
            public IFormFile Chunk { get; set; }
            public string? FileName { get; set; } // đính kèm với chunk cuối cùng
            public string? MimeType { get; set; } // đính kèm với chunk cuối cùng
            public long? FileSize { get; set; } // đính kèm với chunk cuối cùng

            public string? ChatId { get; set; }
            public string? MessageId { get; set; }
        }

    }
}
