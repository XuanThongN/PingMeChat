using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PingMeChat.Shared.Enum;

namespace PingMeChat.Shared.Enum
{
    public enum StatusNotification
    {
        [Display(Name = "Đã xem")]
        Watched = 1,
        [Display(Name = "Chưa xem")]
        NotSeen = 2,
        [Display(Name = "Đang xử lý")]
        Processing = 3,
        [Display(Name = "Thành công")]
        Processed = 4,
        [Display(Name = "Thất bại")]
        Error = 5,
    }

    public enum FileType
    {
        [Display(Name = "Ảnh")]
        Image = 1,
        [Display(Name = "Văn bản")]
        Document = 2,
        [Display(Name = "Video")]
        Video = 3,
        [Display(Name = "Âm thanh")]
        Audio = 4,

    }

    public enum LogEventLevel
    {
        [Display(Name = "Log hệ thống")]
        System = 1,
        [Display(Name = "Dữ liệu")]
        DataLog = 2,
    }

    public enum PositionType
    {
        [Display(Name = "Sinh viên")]
        IsStudent = 1,
        [Display(Name = "Cán bộ nhân viên")]
        IsStaff = 2,
    }

    public enum ContactStatus
    {
        [Display(Name = "Đang chờ")]
        Pending = 0,
        [Display(Name = "Đã chấp nhận")]
        Accepted = 1,
        [Display(Name = "Đã bị chặn")]
        Blocked = 2,
        [Display(Name = "Người lạ")]
        Stranger = 3
    }
}


public static class FileTypeHelper
{
    private static readonly Dictionary<string, FileType> MimeTypeToFileTypeMap = new Dictionary<string, FileType>
    {
        { "image/jpeg", FileType.Image },
        { "image/png", FileType.Image },
        { "image/gif", FileType.Image },
        { "image/bmp", FileType.Image },
        { "image/webp", FileType.Image },
        { "application/pdf", FileType.Document },
        { "application/msword", FileType.Document },
        { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", FileType.Document },
        { "text/plain", FileType.Document },
        { "video/mp4", FileType.Video },
        { "video/x-msvideo", FileType.Video },
        { "video/x-matroska", FileType.Video },
        { "video/webm", FileType.Video },
        { "audio/mpeg", FileType.Audio },
        { "audio/wav", FileType.Audio },
        { "audio/ogg", FileType.Audio },
        { "audio/webm", FileType.Audio },
    };

    public static FileType GetFileTypeFromMimeType(string mimeType)
    {
        if (MimeTypeToFileTypeMap.TryGetValue(mimeType, out var fileType))
        {
            return fileType;
        }
        return FileType.Document; // hoặc bạn có thể trả về một giá trị mặc định như FileType.Document
    }
}
