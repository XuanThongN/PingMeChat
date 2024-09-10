using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Entities.Users;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;

namespace PingMeChat.CMS.Application.App.Repositories
{
    public class AccountReponsitory : Repository<Account>, IAccountRepository
    {

        //private readonly IJwtLib _jwtLib;
        //private readonly IConfiguration _configuration;
        //private readonly AppDBContext _dbContext;

        //public AccountReponsitory(AppDBContext context, IJwtLib jwtLib, IConfiguration configuration) : base(context)
        //{
        //    _jwtLib = jwtLib;
        //    _configuration = configuration;
        //    _dbContext = context;
        //}

        //public async Task<TokenDto> CreateTokenAccount(string email)
        //{
        //    var account = await _dbContext.Set<Account>().Where(x => x.Email == email).FirstOrDefaultAsync();
        //    if (account == null)
        //    {
        //        Account = await _UserManager.FindByNameAsync(email);
        //        if (Account == null) return null;
        //    }
        //    // kiểm tra tài khoản đã bị khóa chưa
        //    if (Account.IsLocked == true || Account.IsDeleted == true) return null;

        //    var roles = await _UserManager.GetRolesAsync(Account) as List<string>;

        //    var token = _jwtLib.GenerateToken(Account, roles);
        //    var refreshToken = _jwtLib.GenerateRefreshToken();
        //    _ = int.TryParse(_configuration["JWT:RefreshTokenExpiresDays"], out int refreshTokenValidityInDays);
        //    _ = int.TryParse(_configuration["JWT:TokenExpiresMinutes"], out int tokenExpiresTime);

        //    Account.AccessToken = token;
        //    Account.RefreshToken = refreshToken;
        //    Account.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);
        //    await _UserManager.UpdateAsync(Account);

        //    string tokenExpiresMinutes = _configuration["JWT:TokenExpiresMinutes"];
        //    string refreshTokenExpiresDays = _configuration["JWT:RefreshTokenExpiresDays"];

        //    return new TokenDto
        //    {
        //        AccessToken = token,
        //        RefreshToken = refreshToken,
        //        TokenExpiresIn = DateTime.Now.AddMinutes(tokenExpiresTime),
        //        TokenExpiresMinutes = tokenExpiresMinutes,
        //        RefreshTokenExpiresMinutes = (60 * 24 * Int32.Parse(refreshTokenExpiresDays)).ToString()
        //    };
        //}

        //public async Task<Account> FindEmailAsync(string email)
        //{
        //    return await _UserManager.FindByEmailAsync(email);
        //}

        //public async Task<List<string>?> GetRoleAccount(Account Account)
        //{
        //    return await _UserManager.GetRolesAsync(Account) as List<string>;
        //}
        //public async Task<List<string>?> GetRoleByEmail(string email)
        //{
        //    var Account = await _UserManager.FindByEmailAsync(email);

        //    if (Account == null) return null;

        //    return await _UserManager.GetRolesAsync(Account) as List<string>;
        //}
        //public bool IsEmailExits(string email)
        //{
        //    return _UserManager.Users.Any(x => x.Email == email);
        //}

        //public async Task<bool> RevokeAll()
        //{
        //    try
        //    {
        //        var Accounts = await _UserManager.Users.ToListAsync();
        //        foreach (var Account in Accounts)
        //        {
        //            Account.RefreshToken = null;
        //            await _UserManager.UpdateAsync(Account);
        //        }

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //public async Task<SignInResult> SignInAsync(Account Account, string password)
        //{
        //    return await _signInManager.CheckPasswordSignInAsync(Account, password, false);
        //}

        //public Task<IdentityResult> SignUpByAdminAsync(RegisterByAdminDto dto, string email)
        //{
        //    var Account = new Account
        //    {
        //        Email = dto.Email,
        //        UserName = dto.PositionCode,
        //        DateRegistered = DateTime.Now,
        //        CreatedBy = email,
        //    };
        //    return _UserManager.CreateAsync(Account, dto.Password);
        //}

        //public async Task<bool> SignUpByAdminAsync(AuthRequestDto dto, string email)
        //{
        //    var Account = new Account
        //    {
        //        Email = dto.Email,
        //        UserName = dto.PositionCode,
        //        DateRegistered = DateTime.Now,
        //        CreatedBy = email,
        //    };

        //    var result = await _UserManager.CreateAsync(Account, dto.Password);

        //    if (result.Succeeded)
        //    {
        //        await _UserManager.AddToRolesAsync(Account, dto.Roles);
        //        return true;
        //    }

        //    return false;
        //}
        //public async Task<bool> SignUpRangeAsync(List<RegisterByAdminDto> dtos, string email)
        //{
        //    var accounts = new List<Account>();
        //    dtos.ForEach(e =>
        //    {
        //        var account = new Account
        //        {
        //            Email = e.Email,
        //            UserName = e.PositionCode,
        //            DateRegistered = DateTime.Now,
        //            CreatedBy = email
        //        };

        //        accounts.Add(account);

        //    });
        //    await _context.Set<Account>().AddRangeAsync(accounts);
        //    return true;

        //}
        //public Task<IdentityResult> SignUpAsync(RegisterDto dto)
        //{
        //    var Account = new Account
        //    {
        //        Email = dto.Email,
        //        UserName = dto.PositionCode,
        //        DateRegistered = DateTime.Now,
        //        CreatedBy = dto.Email
        //    };
        //    return _UserManager.CreateAsync(Account, dto.Password);
        //}

        //public async Task<IdentityResult> SignUpAsync(Account dto, string password)
        //{
        //    var data = await _UserManager.CreateAsync(dto, password);
        //    return data;
        //}

        //public async Task<bool> UpdateAccount(Account Account)
        //{
        //    try
        //    {
        //        await _UserManager.UpdateAsync(Account);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }

        //}

        //public async Task Delete(string email)
        //{
        //    var Account = await _UserManager.FindByEmailAsync(email);

        //    if (Account == null)
        //        throw new AppException("Email không tồn tại", 404);

        //    Account.IsDeleted = true;
        //    Account.DeletedDate = DateTime.Now;
        //    Account.DeletedBy = email;

        //    await _UserManager.UpdateAsync(Account);
        //}

        //public void ResetPassword(ResetPasswordRequestDto resetPassword)
        //{
        //    throw new NotImplementedException();
        //}

        //public void ForgetPassword(ForgotPasswordRequestDto forgotPassword)
        //{
        //    throw new NotImplementedException();
        //}

        //public void VerifyEmail(string token)
        //{
        //    throw new NotImplementedException();
        //}

        //public void RevokeToken(string token, string ipAddress)
        //{
        //    throw new NotImplementedException();
        //}

        //public TokenDto RefreshToken(string token, string ipAddress)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<IdentityResult> RemoveFromRolesAsync(Account Account, IEnumerable<string> roles)
        //{
        //    return await _UserManager.RemoveFromRolesAsync(Account, roles);
        //}

        //public async Task<IdentityResult> AddToRolesAsync(Account Account, IEnumerable<string> roles)
        //{
        //    return await _UserManager.AddToRolesAsync(Account, roles);

        //}

        //public async Task<bool> ChangePasswordAsync(ChangePasswordDto request)
        //{
        //    try
        //    {
        //        var Account = await _UserManager.FindByEmailAsync(request.Email);

        //        var checkChangeP = await _UserManager.ChangePasswordAsync(Account, request.OldPassword, request.Password);
        //        if (checkChangeP != null && checkChangeP.Succeeded)
        //        {
        //            return true;

        //        }
        //        return false;
        //    }
        //    catch
        //    {
        //        return false;
        //    }

        //}

        //public async Task<Account> FindAccountNameAsync(string Accountname)
        //{
        //    return await _UserManager.FindByNameAsync(Accountname);

        //}

        //public async Task<Account?> FindByEmailOrAccountName(string Accountname)
        //{
        //    return await _context.Set<Account>().FirstOrDefaultAsync(x => x.UserName == Accountname || x.Email == Accountname);

        //}

        //public async Task<IdentityResult> AddToRole(Account Account, Role role)
        //{
        //    return await _UserManager.AddToRoleAsync(Account, role.Name);
        //}
        //public async Task<IdentityResult> RemoveFromRole(Account Account, Role role)
        //{
        //    return await _UserManager.RemoveFromRoleAsync(Account, role.Name);

        //}

        //public async Task<bool> Add(Account model, string password, List<string>? roles)
        //{

        //    {
        //        var result = await _UserManager.CreateAsync(model, password);

        //        if (result.Succeeded)
        //        {
        //            await _UserManager.AddToRolesAsync(model, roles);
        //            return true;
        //        }

        //        return false;
        //    }
        //}
        public AccountReponsitory(AppDBContext context) : base(context)
        {
        }
    }
}
