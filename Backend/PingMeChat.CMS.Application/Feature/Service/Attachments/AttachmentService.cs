using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using PingMeChat.CMS.Application.Feature.Service.Attachments.Dto;
using Npgsql.BackendMessages;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using System.Net;
using PingMeChat.CMS.Application.Common.Exceptions;

namespace PingMeChat.CMS.Application.Feature.Service.Attachments
{
    public interface IAttachmentService : IServiceBase<Attachment, AttachmentCreateDto, AttachmentUpdateDto, AttachmentDto, IAttachmentRepository>
    {
        Task<CloudinaryUploadResult> UploadFileAsync(IFormFile file);
    }
    public class AttachmentService : ServiceBase<Attachment, AttachmentCreateDto, AttachmentUpdateDto, AttachmentDto, IAttachmentRepository>, IAttachmentService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ILogErrorRepository _logErrorRepository;
        private readonly Cloudinary _cloudinary;
        public AttachmentService(IAttachmentRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUriService uriService,
        IAttachmentRepository attachmentRepository,
         ILogErrorRepository logErrorRepository,
         Cloudinary cloudinary) : base(repository, unitOfWork, mapper, uriService)
        {
            _attachmentRepository = attachmentRepository;
            _logErrorRepository = logErrorRepository;
            _cloudinary = cloudinary;
        }

        public async Task<CloudinaryUploadResult> UploadFileAsync(IFormFile file)
        {
            var uploadResult = new CloudinaryUploadResult();

            if (file.Length > 0)
            {
                // Kiểm tra kích thước file có vượt quá 25MB không
                if (file.Length > 25 * 1024 * 1024)
                {
                    throw new AppException($"File {file.FileName} vượt quá kích thước tối đa 25MB.");
                }

                using (var stream = file.OpenReadStream())
                {
                    // Xác định loại file (hình ảnh, video, hoặc file khác)
                    var fileType = file.ContentType.ToLower();
                    RawUploadParams uploadParams;

                    if (fileType.StartsWith("image"))
                    {
                        // Upload hình ảnh
                        uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.FileName, stream)
                        };
                    }
                    else if (fileType.StartsWith("video"))
                    {
                        // Upload video
                        uploadParams = new VideoUploadParams()
                        {
                            File = new FileDescription(file.FileName, stream)
                        };
                    }
                    else
                    {
                        // Upload file khác (raw)
                        uploadParams = new RawUploadParams()
                        {
                            File = new FileDescription(file.FileName, stream)
                        };
                    }

                    // Upload file lên Cloudinary
                    var result = await _cloudinary.UploadAsync(uploadParams);

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        // Lưu kết quả thành công
                        uploadResult.Url = result.SecureUrl.ToString();
                        uploadResult.PublicId = result.PublicId;
                        uploadResult.FileType = file.ContentType;  // Gán loại file
                        uploadResult.FileSize = file.Length;       // Gán kích thước file
                    }
                    else
                    {
                        throw new AppException($"Upload file {file.FileName} thất bại với mã lỗi {result.StatusCode}");
                    }
                }
            }

            return uploadResult;
        }



    }


}
