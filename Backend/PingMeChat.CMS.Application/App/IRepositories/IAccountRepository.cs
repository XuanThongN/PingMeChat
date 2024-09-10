using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.CMS.Entities;
using PingMeChat.CMS.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace PingMeChat.CMS.Application.App.IRepositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        //Task<IdentityResult> SignUpAsync(Account dto, string password);
        //Task<SignInResult> SignInAsync(Account Account, string password); 
        //Task<Account> FindEmailAsync(string email); // sau khi tìm kiếm email của người dùng thành công => SignInAsync => CreateTokenAccount
        //Task<Account> FindByUserNameAsync(string userName); // tìm kiếm Accountname = positionCode ( mã định danh)
        //Task<Account?> FindByEmailOrUserName(string emailOrUserName); // tìm kiếm email hoặc Accountname
        //Task<TokenDto> CreateTokenAccount(string email);
        //Task<IdentityResult> RemoveFromRolesAsync(Account Account, IEnumerable<string> roles);
        //Task<IdentityResult> RemoveFromRoleAsync(Account Account, Role role);
        //Task<IdentityResult> AddToRolesAsync(Account Account, IEnumerable<string> roles);
        //Task<IdentityResult> AddToRole(Account Account, Role role);
        //Task<List<string>?> GetRoleAccount(Account Account); 
        //Task<List<string>?> GetRoleByEmail(string email);
        //bool IsEmailExits(string email);
        //Task<bool> UpdateAccount(Account Account);
        //Task<bool> Delete(string email);
        //void VerifyEmail(string token);
        //Task<bool> RevokeAll(); // thu hồi tất cả mã thông báo truy câp khi 1 người dùng đăng xuất hoặc bị đình chỉ
        //void RevokeToken(string token, string ipAddress);
        //TokenDto RefreshToken(string token, string ipAddress);
        //void ResetPassword(string token, string password, string email);
        //Task<bool> ChangePasswordAsync(string password, string emailOrUserName, string email);

    }
}
