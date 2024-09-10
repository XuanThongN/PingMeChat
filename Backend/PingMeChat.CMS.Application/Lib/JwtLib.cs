using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PingMeChat.CMS.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using PingMeChat.CMS.Entities.Users;

namespace PingMeChat.CMS.Application.Lib
{
    public class JwtLib : IJwtLib
    {
        private readonly IConfiguration _configuration;

        public JwtLib(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // tạo ra 1 chuỗi json có độ dài 88 kỹ tự
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            // gán cho tường phần tử trong randomNumber mang 1 giá trị
            rng.GetBytes(randomNumber);
            //Với mỗi ký tự được mã hóa dưới dạng 8 bit, nên chuỗi kết quả sẽ có độ dài là 64 x 8 / 6 = 85,33 ký tự,
            //tuy nhiên vì các ký tự được mã hóa dưới dạng Base64 nên chuỗi kết quả sẽ có độ dài là 88 ký tự
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// B1: Tạo ra 1 danh sách chứa claim ( thông tin của user + danh sách role)
        /// B2: Tạo ra 1 key để mã hóa token ( key nào là key đưuọc khai báo trong file cấu hình)-khi lấy ra key này phải được mã  hóa
        /// B3: Tạo ra 1 đối tượng JwtSecurityToken ( token) và gán các giá trị của đối tượng đó cho phù hợp ( một số thông tin được lấy từ file cấu hình chung)
        /// đối tượng này sẽ được mã hóa
        /// B4: Trả về chuỗi JWT token được mã hóa từ đối tượng JwtSecurityToken bằng cách sử dụng JwtSecurityTokenHandler.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public string GenerateToken(Account model, List<string>? roles)
        {
            var authClaims = new List<Claim>
            {
              new Claim(ClaimTypes.Email, model.Email),
              new Claim(ClaimTypes.Name, model.UserName),
              new Claim("UserId", model.Id.ToString()),
              new Claim("Email", model.Email),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (roles != null)
            {
                foreach (var role in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            // SymmetricSecurityKey : đại diện cho khóa bí mật
            // khóa bí mật được tạo ra từ chuỗi bí mật
            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));
            // cố gắng chuyển đổi 1 giá trị được lấy từ file cấu hình, nếu có giá trị trả về nó sẽ gán cho tokenExpiresTimeMinutes
            // nếu không có giá trị thì tokenExpiresTimeMinutes = 0
            // _  : biểu hiện không cần quan tâm tới giá trị trả về
            _ = int.TryParse(_configuration["JWT:TokenExpiresMinutes"], out int tokenExpiresTimeMinutes);

            // đại diện cho một đối tượng token JWT, chứa các thông tin liên quan và đặc biệt phải chứa khóa( chữ ký) bí mật
            var token = new JwtSecurityToken(
              issuer: _configuration["JWT:ValidIssuer"],
              audience: _configuration["JWT:ValidAudience"],
              expires: DateTime.Now.AddMinutes(tokenExpiresTimeMinutes),
              claims: authClaims,
              //Sử dụng đối tượng SymmetricSecurityKey và thuật toán HmacSha512Signature để tạo ra credential cho token.
              // sử dụng khóa bị mật ( được khia báo trong file cấu hình) để tạo chữ ký cho JWT 
              // giải thích: nó mã hóa authenKey bằng thuật toán HmacSha512Signature
              // bảo đảm tính toàn vẹn của JWT và chỉ cho phép giải mã bến phía server
              signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
            );

            // đối tượng để xử lý token
            // Tạo, xác thực, đọc token
            // chuyển đổi json => token jwt và ngược lại
            // Ví dụ : 
            // var tokenHandler = new JwtSecurityTokenHandler();
            // var token = tokenHandler.CreateJwtSecurityToken(...);
            // var tokenString = tokenHandler.WriteToken(token);
            // var decodedToken = tokenHandler.ReadJwtToken(tokenString);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string GenerateToken(Account model)
        {
            var authClaims = new List<Claim>
            {
              new Claim(ClaimTypes.Email, model.Email),
              new Claim(ClaimTypes.Name, model.UserName),
              new Claim("UserId", model.Id.ToString()),
              new Claim("Email", model.Email),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

          

            // SymmetricSecurityKey : đại diện cho khóa bí mật
            // khóa bí mật được tạo ra từ chuỗi bí mật
            var authenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));
            // cố gắng chuyển đổi 1 giá trị được lấy từ file cấu hình, nếu có giá trị trả về nó sẽ gán cho tokenExpiresTimeMinutes
            // nếu không có giá trị thì tokenExpiresTimeMinutes = 0
            // _  : biểu hiện không cần quan tâm tới giá trị trả về
            _ = int.TryParse(_configuration["JWT:TokenExpiresMinutes"], out int tokenExpiresTimeMinutes);

            // đại diện cho một đối tượng token JWT, chứa các thông tin liên quan và đặc biệt phải chứa khóa( chữ ký) bí mật
            var token = new JwtSecurityToken(
              issuer: _configuration["JWT:ValidIssuer"],
              audience: _configuration["JWT:ValidAudience"],
              expires: DateTime.Now.AddMinutes(tokenExpiresTimeMinutes),
              claims: authClaims,
              //Sử dụng đối tượng SymmetricSecurityKey và thuật toán HmacSha512Signature để tạo ra credential cho token.
              // sử dụng khóa bị mật ( được khia báo trong file cấu hình) để tạo chữ ký cho JWT 
              // giải thích: nó mã hóa authenKey bằng thuật toán HmacSha512Signature
              // bảo đảm tính toàn vẹn của JWT và chỉ cho phép giải mã bến phía server
              signingCredentials: new SigningCredentials(authenKey, SecurityAlgorithms.HmacSha512Signature)
            );

            // đối tượng để xử lý token
            // Tạo, xác thực, đọc token
            // chuyển đổi json => token jwt và ngược lại
            // Ví dụ : 
            // var tokenHandler = new JwtSecurityTokenHandler();
            // var token = tokenHandler.CreateJwtSecurityToken(...);
            // var tokenString = tokenHandler.WriteToken(token);
            // var decodedToken = tokenHandler.ReadJwtToken(tokenString);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token), "Token cannot be null or empty.");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:ValidAudience"],
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!)),
                ValidateLifetime = false  // Quan trọng: Đặt thành false để cho phép xử lý token hết hạn
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            // Xác thực token
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            // Kiểm tra tính hợp lệ của token
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512Signature,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            // Kiểm tra xem token đã hết hạn chưa
            if (jwtSecurityToken.ValidTo < DateTime.UtcNow)
            {
                return null;  // Chỉ trả về null khi token đã hết hạn
            }

            return principal;
        }
    }
}
