using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Indentity.Auth.Dto;
using PingMeChat.CMS.Application.Feature.Indentity.Email;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities;
using PingMeChat.CMS.Entities.Users;
using PingMeChat.Shared.Utils;
using System.Linq.Expressions;

namespace PingMeChat.CMS.Application.Feature.Indentity.Auth
{
    public interface IAuthService
    {
        Task<TokenDto> Login(LoginDto model, string deviceInfo, string ipAddress);
        Task<bool> Register(RegisterDto model, string email);
        Task<bool> Register(RegisterDto model);
        Task<bool> Logout(string accessToken);
        Task<bool> ChangePassword(PasswordChangeRequestDto model, string email);
        Task<bool> ChangePassword(ChangePasswordDto model, string email);
        Task<bool> LockAccount(string emailUser, string email);
        Task<bool> LockAccountById(string id, string email);
        Task<bool> UnlockAccount(string userId, string email);
        Task<AccountDto> Find(Expression<Func<Account, bool>> match);
        Task<bool> UpdateInfoAndPassword(UpdateAccountInfoDto model, string email);
        Task<IEnumerable<SelectListItem>> GetSelectListUser();
        Task<bool> VerifyPassword(string email, string password);
        Task<bool> VerifyCode(string email, string code);
        Task<bool> ResendVerificationCode(string email);
        Task<bool> ForgotPassword(string email);
        Task<bool> VerifyResetCode(string email, string code);
        Task<bool> ResetPassword(string email, string newPassword);
        //Task<bool> IsExistEmail(string email);
        //Task<AccountDto> GetByEmail(string email);
        //Task<bool> VerifyPassword(string email, string password);
        //Task<AccountDto> GetMe(string email);

        //Task<bool> AssignRoleToUser(string userId, string roleId);
        //Task<bool> RemoveRoleFromUser(string userId, string roleId);
        //Task<bool> AssignMenuToUser(string userId, string menuId);
        //Task<bool> RemoveMenuFromUser(string userId, string menuId);
        //Task<IEnumerable<MenuDto>> GetUserMenus(string userId);
        //Task<bool> ChangeStatus(string id, string email);

    }
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUsersMenuRepository _usersMenuRepository;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IMapper _mapper;
        private readonly IJwtLib _jwtLib;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;


        public AuthService(IUnitOfWork unitOfWork,
            IAccountRepository accountRepository,
            IRoleRepository roleRepository,
            IUsersMenuRepository usersMenuRepository,
            IUserSessionRepository userSessionRepository,
            IMapper mapper,
            IJwtLib jwtLib,
            IConfiguration configuration,
            ITokenService tokenService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _usersMenuRepository = usersMenuRepository;
            _userSessionRepository = userSessionRepository;
            _mapper = mapper;
            _jwtLib = jwtLib;
            _configuration = configuration;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<TokenDto> Login(LoginDto model, string deviceInfo, string ipAddress)
        {
            var userNameOrEmail = model.UserName.ToLower();
            var user = await _accountRepository.Find(x => x.UserName.ToLower() == userNameOrEmail
                                                    || x.Email.ToLower() == userNameOrEmail);
            if (user == null || !HelperMethod.VerifyPassword(model.Password, user.Password))
            {
                throw new AppException("Tài khoản hoặc mật khẩu không hợp lệ", 404);

            }
            if (user.IsLocked) throw new AppException("Tài khoản đã bị khóa", 400);
            if (!user.IsVerified) throw new AppException("Account is not verified", 400);

            var accessToken = _jwtLib.GenerateToken(user);
            var refreshToken = _jwtLib.GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWT:RefreshTokenExpiresDays"], out int refreshTokenValidityInDays);
            _ = int.TryParse(_configuration["JWT:TokenExpiresMinutes"], out int tokenExpiresTime);

            string tokenExpiresMinutes = _configuration["JWT:TokenExpiresMinutes"];
            string refreshTokenExpiresDays = _configuration["JWT:RefreshTokenExpiresDays"];

            var userSession = new UserSession
            {
                AccountId = user.Id,
                RefreshToken = new RefreshToken
                {
                    TokenValue = refreshToken,
                    DeviceInfo = deviceInfo,
                    IPAddress = ipAddress,
                    ExpiryDate = DateTime.UtcNow.AddDays(7),
                    AccountId = user.Id
                },
                AccessToken = accessToken,
                LoginTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow
            };

            await _userSessionRepository.Add(userSession);
            await _unitOfWork.SaveChangeAsync();

            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenExpiresIn = DateTime.Now.AddMinutes(tokenExpiresTime),
                TokenExpiresMinutes = tokenExpiresMinutes,
                RefreshTokenExpiresMinutes = (60 * 24 * Int32.Parse(refreshTokenExpiresDays)).ToString(),
                FullName = user.FullName,
                Email = user.Email,
                UserId = user.Id,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                AvatarUrl = user.AvatarUrl
            };
        }

        public async Task<bool> Register(RegisterDto model, string email)
        {
            var user = await _accountRepository.Find(x => x.UserName == model.UserName || x.Email == model.Email);
            if (user != null)
            {
                if (user.UserName == model.UserName)
                {
                    throw new AppException("Tài khoản đã tồn tại trong hệ thống", 400);
                }
                else if (user.Email == model.Email)
                {
                    throw new AppException("Email đã tồn tại trong hệ thống", 400);
                }
            }

            Account account = new Account
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password.HashPassword(),
                IsLocked = false,
                UpdatedBy = email,
                CreatedBy = email
            };

            await _accountRepository.Add(account);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }

        public async Task<bool> Register(RegisterDto model)
        {
            var user = await _accountRepository.Find(x => x.UserName == model.UserName || x.Email == model.Email);
            if (user != null)
            {
                if (user.UserName == model.UserName)
                {
                    throw new AppException(string.Format(Message.Error.IsExisted, "Account"), 400);
                }
                else if (user.Email == model.Email)
                {
                    throw new AppException(string.Format(Message.Error.IsExisted, "Email"), 400);
                }
                else if (user.PhoneNumber == model.Phone)
                {
                    throw new AppException(string.Format(Message.Error.IsExisted, "Phone number"), 400);
                }
            }

            Account account = new Account
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password.HashPassword(),
                FullName = model.FullName,
                PhoneNumber = model.Phone,
                IsLocked = false,
                IsVerified = false, // chưa xác thực
                VerificationCode = new Random().Next(100000, 999999).ToString(), // Random 6 số
                CodeExpiryTime = DateTime.UtcNow.AddMinutes(10),
            };

            await _accountRepository.Add(account);
            await _unitOfWork.SaveChangeAsync();

            // Send email logic here (use any email service)
            await SendVerificationEmail(account.Email, account.VerificationCode, account.FullName);

            return true;
        }

        private async Task SendVerificationEmail(string email, string code, string fullName)
        {
            string subject = "Welcome to PingMeChat - Verify Your Account";
            string body = $@"
        <html>
        <body style='font-family: Arial, sans-serif; color: #333;'>
            <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px;'>
                <h1 style='color: #4a4a4a; text-align: center;'>Welcome to PingMeChat!</h1>
                <p>Dear {fullName},</p>
                <p>Thank you for joining PingMeChat. To complete your registration, please use the following verification code:</p>
                <h2 style='text-align: center; color: #007bff; font-size: 24px; padding: 10px; background-color: #f8f9fa; border-radius: 4px;'>{code}</h2>
                <p>This code will expire in 10 minutes.</p>
                <p>If you didn't request this code, please ignore this email.</p>
                <p>Best regards,<br>The PingMeChat Team</p>
            </div>
        </body>
        </html>";

            await _emailService.SendEmailAsync(email, subject, body);
        }
        public async Task<bool> Logout(string accessToken)
        {
            var userSession = await _userSessionRepository.Find(x => x.AccessToken == accessToken);
            if (userSession == null)
            {
                throw new AppException(string.Format(Message.Error.NotFound, "AccessToken"), StatusCodes.Status404NotFound);
            }


            var executionStrategy = _unitOfWork.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        userSession.LogoutTime = DateTime.UtcNow;
                        await _userSessionRepository.Update(userSession);
                        await _unitOfWork.SaveChangeAsync();

                        // Revoke the refresh token
                        await _tokenService.RevokeRefreshTokenAsync(userSession.RefreshTokenId);
                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });
        }
        public async Task<bool> ChangePassword(PasswordChangeRequestDto model, string email)
        {
            var user = await _accountRepository.Find(x => x.Email == model.Email);
            if (user == null || !HelperMethod.VerifyPassword(model.Password, user.Password))
            {
                throw new AppException("Tài khoản hoặc mật khẩu không hợp lệ", 404);
            }

            user.Password = model.Password.HashPassword();
            user.UpdatedBy = email;

            await _accountRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();

            return true;

        }

        public async Task<bool> ChangePassword(ChangePasswordDto model, string email)
        {
            var user = await _accountRepository.Find(x => x.Email == email);
            if (user == null || !HelperMethod.VerifyPassword(model.OldPassword, user.Password))
            {
                throw new AppException(Message.Error.PasswordDontMatch, 400);
            }

            user.Password = model.NewPassword.HashPassword();
            user.UpdatedBy = email;

            await _accountRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
        public async Task<bool> LockAccount(string emailUser, string email)
        {
            var account = await _accountRepository.Find(x => x.Email == emailUser);
            if (account == null)
            {
                throw new AppException("Tài khoản không tồn tại trong hệ thống", 404);
            }

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        account.IsLocked = true;
                        account.UpdatedBy = email;
                        await _accountRepository.Update(account);

                        // Revoke all refresh tokens for this user
                        await _tokenService.RevokeAllRefreshTokensForUserAsync(account.Id, email);

                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }
            });

        }
        public async Task<bool> LockAccountById(string id, string email)
        {
            var account = await _accountRepository.FindById(id);
            if (account == null)
            {
                throw new AppException("Tài khoản không tồn tại trong hệ thống", 404);
            }

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        account.IsLocked = true;
                        account.UpdatedBy = email;
                        await _accountRepository.Update(account);

                        // Revoke all refresh tokens for this user
                        await _tokenService.RevokeAllRefreshTokensForUserAsync(id, email);

                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }
            });
        }
        public async Task<bool> UnlockAccount(string userId, string email)
        {
            var account = await _accountRepository.FindById(userId);
            if (account == null)
            {
                throw new AppException("Tài khoản không tồn tại trong hệ thống", 404);
            }

            account.IsLocked = false;
            account.UpdatedBy = email;
            await _accountRepository.Update(account);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }
        public async Task<AccountDto> Find(Expression<Func<Account, bool>> match)
        {
            Account entity = await _accountRepository.Find(match);
            return _mapper.Map<AccountDto>(entity);
        }

        public async Task<bool> UpdateInfoAndPassword(UpdateAccountInfoDto model, string email)
        {
            var executionStrategy = _unitOfWork.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {


                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var user = await _accountRepository.Find(x => x.Email == email);
                        if (user == null)
                        {
                            throw new AppException("Tài khoản người dùng không hợp lệ", StatusCodes.Status400BadRequest);

                        }
                        // nếu người dùng có cập nhật thêm Mk
                        if (!string.IsNullOrEmpty(model.NewPassword))
                        {
                            //if (!HelperMethod.VerifyPassword(model.NewPassword, user.Password))
                            //    throw new AppException("Tài khoản người dùng không hợp lệ", StatusCodes.Status400BadRequest);

                            user.Password = model.NewPassword.HashPassword();

                        }

                        user.UpdatedBy = email;
                        user.FullName = model.FullName;
                        user.PhoneNumber = model.PhoneNumber;

                        await _accountRepository.Update(user);
                        await _unitOfWork.SaveChangeAsync();
                        await transaction.CommitAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }

                }
            });
        }
        public async Task<IEnumerable<SelectListItem>> GetSelectListUser()
        {
            var users = await _accountRepository.GetAll();
            var selectList = users.Where(x => !string.IsNullOrEmpty(x.FullName)).Select(x =>
                new SelectListItem
                {
                    Value = x.FullName,
                    Text = x.FullName
                }
            );
            return selectList;
        }
        public async Task<bool> VerifyPassword(string email, string password)
        {
            var user = await _accountRepository.Find(x => x.Email == email);
            if (user == null)
                throw new AppException("Tài khoản hoặc mật khẩu không hợp lệ", StatusCodes.Status404NotFound);


            return HelperMethod.VerifyPassword(password, user.Password);
        }

        public async Task<bool> VerifyCode(string email, string code)
        {
            var user = await _accountRepository.Find(x => x.Email == email);
            if (user == null) throw new AppException(string.Format(Message.Error.NotFound, "Account"), StatusCodes.Status404NotFound);

            if (user.VerificationCode != code || user.CodeExpiryTime < DateTime.UtcNow)
            {
                throw new AppException("Invalid or expired verification code", 400);
            }

            user.IsLocked = false; // unlock account
            user.VerificationCode = null;
            user.CodeExpiryTime = null;
            user.IsVerified = true; // Kích hoạt tài khoản

            await _accountRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }

        public async Task<bool> ResendVerificationCode(string email)
        {
            var user = await _accountRepository.Find(x => x.Email == email);
            if (user == null) throw new AppException("User not found", 404);

            user.VerificationCode = new Random().Next(100000, 999999).ToString();
            user.CodeExpiryTime = DateTime.UtcNow.AddMinutes(10);

            await _accountRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();

            // Send email logic here (use any email service)
            await SendVerificationEmail(user.Email, user.VerificationCode, user.FullName);

            return true;
        }

        public async Task<bool> ForgotPassword(string email)
        {
            var user = await _accountRepository.Find(x => x.Email == email);
            if (user == null) throw new AppException("User not found", 404);

            user.ResetPasswordCode = new Random().Next(100000, 999999).ToString();
            user.ResetPasswordCodeExpiryTime = DateTime.UtcNow.AddMinutes(10);

            await _accountRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();

            await SendResetPasswordEmail(user.Email, user.ResetPasswordCode, user.FullName);

            return true;
        }

        public async Task<bool> VerifyResetCode(string email, string code)
        {
            var user = await _accountRepository.Find(x => x.Email == email);
            if (user == null) throw new AppException("User not found", 404);

            if (user.ResetPasswordCode != code || user.ResetPasswordCodeExpiryTime < DateTime.UtcNow)
            {
                throw new AppException("Invalid or expired reset code", 400);
            }

            return true;
        }

        public async Task<bool> ResetPassword(string email, string newPassword)
        {
            var user = await _accountRepository.Find(x => x.Email == email);
            if (user == null) throw new AppException("User not found", 404);

            user.Password = newPassword.HashPassword();
            user.ResetPasswordCode = null;
            user.ResetPasswordCodeExpiryTime = null;

            await _accountRepository.Update(user);
            await _unitOfWork.SaveChangeAsync();

            return true;
        }

        private async Task SendResetPasswordEmail(string email, string code, string fullName)
        {
            string subject = "PingMeChat - Reset Your Password";
            string body = $@"
    <html>
    <body style='font-family: Arial, sans-serif; color: #333;'>
        <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 5px;'>
            <h1 style='color: #4a4a4a; text-align: center;'>PingMeChat Password Reset</h1>
            <p>Dear {fullName},</p>
            <p>We received a request to reset your password. Please use the following code to reset your password:</p>
            <h2 style='text-align: center; color: #007bff; font-size: 24px; padding: 10px; background-color: #f8f9fa; border-radius: 4px;'>{code}</h2>
            <p>This code will expire in 10 minutes.</p>
            <p>If you didn't request this code, please ignore this email.</p>
            <p>Best regards,<br>The PingMeChat Team</p>
        </div>
    </body>
    </html>";

            await _emailService.SendEmailAsync(email, subject, body);
        }
        //public async Task<AccountDto> GetByEmail(string email)
        //{
        //    var account = await _accountRepository.Find(x=> x.Email == email);
        //    if (account == null) throw new AppException($"Không tìm thấy tài khoản với email {email}", 404);
        //    var accountDto = _mapper.Map<AccountDto>(account);
        //    return accountDto;
        //}







        //}




        //public async Task<bool> AssignRoleToUser(string userId, string roleId)
        //{
        //    var user = await _accountRepository.FindById(userId);
        //    var role = await _roleRepository.FindById(roleId);
        //    if (user != null && role != null)
        //    {
        //        var result = await _accountRepository.AddToRole(user, role);
        //        return result.Succeeded;
        //    }
        //    return false;
        //}

        //public async Task<bool> RemoveRoleFromUser(string userId, string roleId)
        //{
        //    var user = await _accountRepository.FindById(userId);
        //    var role = await _roleRepository.FindById(roleId);

        //    if (user != null && role != null)
        //    {
        //        var result = await _accountRepository.RemoveFromRole(user, role);
        //        return result.Succeeded;
        //    }
        //    return false;
        //}

        //public async Task<bool> AssignMenuToUser(string userId, string menuId)
        //{
        //    var userMenu = new UsersMenus { AccountId = userId, MenusId = menuId };
        //    await _usersMenuRepository.Add(userMenu);
        //    await _unitOfWork.SaveChangeAsync();
        //    return true;
        //}

        //public async Task<bool> RemoveMenuFromUser(string userId, string menuId)
        //{
        //    var userMenu = await _usersMenuRepository.Find(um => um.AccountId == userId && um.MenusId == menuId);
        //    if (userMenu != null)
        //    {
        //        await _usersMenuRepository.Delete(userMenu);
        //        await _unitOfWork.SaveChangeAsync();
        //        return true;
        //    }
        //    return false;
        //}

        //public async Task<IEnumerable<MenuDto>> GetUserMenus(string userId)
        //{
        //    var userMenus = await _usersMenuRepository.FindAll(match: um => um.AccountId == userId, include: inc => inc.Include(x => x.Menu));
        //    var menus = userMenus.Select(um => um.Menu);
        //    var menuDtos = _mapper.Map<IEnumerable<MenuDto>>(menus);

        //    return menuDtos;
        //}

        //public async Task<bool> ChangeStatus(string id, string email)
        //{
        //    var user = await _accountRepository.FindById(id);
        //    if (user == null) return false;

        //    user.UpdatedBy = email;
        //    user.IsLocked = !user.IsLocked;

        //    await _accountRepository.Update(user);
        //    await _unitOfWork.SaveChangeAsync();
        //    return true;
        //}

    }
}
