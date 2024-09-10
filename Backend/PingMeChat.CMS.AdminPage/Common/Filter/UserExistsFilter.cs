using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.App.Repositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Octokit;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Common.Filter
{
    public class UserExistsFilter : IAsyncActionFilter
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogErrorRepository _logErrorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private static readonly AsyncLocal<bool> _isExecuted = new AsyncLocal<bool>();

        public UserExistsFilter(IAccountRepository accountRepository, ILogErrorRepository logErrorRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _logErrorRepository = logErrorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Kiểm tra người dùng hiện tại
            await CheckCurrentUser(context);

            // Kiểm tra thông tin mới nếu là action tạo mới
            if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor &&
                actionDescriptor.ActionName == "Create" &&
                !_isExecuted.Value)
            {
                _isExecuted.Value = true;
                try
                {
                    if (context.ActionArguments.TryGetValue("dto", out var dtoObject) && dtoObject is UserCreateDto dto)
                    {
                        await IsInforExits(dto);
                    }
                }
                finally
                {
                    _isExecuted.Value = false;
                }
            }

            // Chỉ gọi next() một lần và trả về kết quả
            var resultContext = await next();
            context.Result = resultContext.Result;
        }

        private async Task CheckCurrentUser(ActionExecutingContext context)
        {
            string? email = context.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                throw new AppException("Không tìm thấy email tài khoản đang hoạt động", 404);

            var route = context.HttpContext.Request.Path.Value;
            if (string.IsNullOrEmpty(route))
                throw new AppException("Yêu cầu tài nguyên từ url không tìm thấy", 404);

            var userId = context.HttpContext.User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userId))
                throw new AppException("Không tìm thấy tài khoản trong hệ thống", 404);

            var userName = context.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(userName))
                throw new AppException("Không tìm thấy tài khoản trong hệ thống", 404);
        }

        private async Task IsInforExits(UserCreateDto dto)
        {
            bool userNameExists = await IsUserExist(dto.UserName);
            bool emailExists = await IsEmailExist(dto.Email);
            bool phoneExists = await IsPhoneExist(dto.PhoneNumber);

            var errorMessages = new List<string>();
            if (userNameExists) errorMessages.Add("Tài khoản người dùng đã tồn tại");
            if (emailExists) errorMessages.Add("Email đã tồn tại");
            if (phoneExists) errorMessages.Add("Số điện thoại đã tồn tại");

            if (errorMessages.Any())
            {
                var errorLog = new PingMeChat.CMS.Entities.Module.ErrorLog
                {
                    ControllerName = "User",
                    ActionName = "Create",
                    IsError = true,
                    ErrorMessage = string.Join("; ", errorMessages),
                    Exception = typeof(AppException).Name,
                };
                await _logErrorRepository.Add(errorLog);
                await _unitOfWork.SaveChangeAsync();
                throw new AppException(string.Join("; ", errorMessages), 400);
            }
        }

        private async Task<bool> IsUserExist(string username)
        {
            return await _accountRepository.AnyAsync(x => x.UserName == username);
        }

        private async Task<bool> IsEmailExist(string email)
        {
            return await _accountRepository.AnyAsync(x => x.Email == email);
        }

        private async Task<bool> IsPhoneExist(string phone)
        {
            return await _accountRepository.AnyAsync(x => x.PhoneNumber == phone);
        }
    }
}
