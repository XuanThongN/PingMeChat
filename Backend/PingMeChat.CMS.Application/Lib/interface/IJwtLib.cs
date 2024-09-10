using PingMeChat.CMS.Entities;
using PingMeChat.CMS.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application;

public interface IJwtLib
{
    /// <summary>
    /// dùng các thuộc tính trong user để lưu vào claim => lưu vào jwt ở Payload jwt, tương tự cho phần role
    /// </summary>
    /// <param name="user"></param>
    /// <param name="roles"></param>
    /// <returns></returns>
    public string GenerateToken(Account model, List<string>? roles);
    public string GenerateToken(Account model);
    public string GenerateRefreshToken();

    /// <summary>
    /// lấy ra danh tính người dùng lưu trong jwt và lấy ra các thuộc tính trong TokenDto
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
}

