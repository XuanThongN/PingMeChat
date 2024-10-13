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

        public AttachmentController(IAttachmentService attachmentService, IConfiguration configuration)
        {
            _attachmentService = attachmentService;
            _tempDirectory = configuration["TempUploadDirectory"] ?? Path.GetTempPath();
        }

        // [HttpPost]
        // [ValidateUserAndModel]
        // [Route(ApiRoutes.Feature.Attachment.UploadMultipleFilesRoute)]
        // [ProducesResponseType(typeof(List<CloudinaryUploadResult>), StatusCodes.Status201Created)]
        // public async Task<IActionResult> UploadMultipleFiles([FromForm] List<IFormFile> files)
        // {
        //     // Kiểm tra nếu không có file nào được chọn
        //     if (files == null || !files.Any())
        //     {
        //         return Ok(new ApiResponse(Message.Error.CreateError, null, StatusCodes.Status400BadRequest));
        //     }

        //     // Kiểm tra giới hạn file (giả sử chỉ cho phép upload tối đa 5 file cùng lúc)
        //     if (files.Count > 5)
        //     {
        //         return BadRequest("Chỉ có thể upload tối đa 5 file một lần.");
        //     }

        //     var uploadResults = new List<CloudinaryUploadResult>();

        //     foreach (var file in files)
        //     {
        //         // Kiểm tra kích thước của từng file (20MB = 20 * 1024 * 1024 bytes)
        //         if (file.Length > 20 * 1024 * 1024)
        //         {
        //             return BadRequest($"File {file.FileName} vượt quá giới hạn kích thước 20MB.");
        //         }

        //         try
        //         {
        //             var uploadResult = await _attachmentService.UploadFileAsync(file);

        //             // Kiểm tra nếu upload thất bại
        //             if (uploadResult == null)
        //             {
        //                 return StatusCode(StatusCodes.Status500InternalServerError, $"Không thể upload file {file.FileName}.");
        //             }

        //             uploadResults.Add(uploadResult);
        //         }
        //         catch (Exception ex)
        //         {
        //             // Log lỗi nếu có
        //             return StatusCode(StatusCodes.Status500InternalServerError, $"Có lỗi xảy ra khi upload file {file.FileName}: {ex.Message}");
        //         }
        //     }

        //     // Trả về kết quả của tất cả các file đã được upload
        //     return Ok(new ApiResponse(Message.Success.CreateCompleted, uploadResults, StatusCodes.Status200OK));
        // }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Attachment.UploadChunkRoute)]
        public async Task<IActionResult> UploadChunk([FromForm] ChunkUploadModel model)
        {
            try
            {
                if (model.Chunk == null || model.Chunk.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                var uploadDir = Path.Combine(_tempDirectory, model.UploadId);
                Directory.CreateDirectory(uploadDir);

                var chunkPath = Path.Combine(uploadDir, $"chunk_{model.ChunkIndex}");

                using (var stream = new FileStream(chunkPath, FileMode.Create))
                {
                    await model.Chunk.CopyToAsync(stream);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine("Error occurred while uploading chunk: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(ApiRoutes.Feature.Attachment.CompleteUploadRoute)]
        public async Task<IActionResult> CompleteUpload([FromBody] CompleteUploadModel model)
        {
            try
            {
                var uploadDir = Path.Combine(_tempDirectory, model.UploadId);
                var chunks = Directory.GetFiles(uploadDir).OrderBy(f => int.Parse(Path.GetFileName(f).Split('_')[1]));

                var tempFilePath = Path.Combine(_tempDirectory, $"{Guid.NewGuid()}_{model.FileName}");

                using (var outputStream = new FileStream(tempFilePath, FileMode.Create))
                {
                    foreach (var chunk in chunks)
                    {
                        using var inputStream = new FileStream(chunk, FileMode.Open);
                        await inputStream.CopyToAsync(outputStream);
                    }
                }

                CloudinaryUploadResult uploadResult;
                using (var fileStream = new FileStream(tempFilePath, FileMode.Open))
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
                // Log the exception
                Console.WriteLine("Error occurred while completing upload: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
            }
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
