using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities;
using PingMeChat.CMS.Entities.Users;
using PingMeChat.Shared.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PingMeChat.CMS.Entities.Module;
using PingMeChat.CMS.Application.App.Repositories;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;

namespace PingMeChat.CMS.Application.Feature.Service.Users
{
    public interface IUserService : IServiceBase<Account, UserCreateDto, UserUpdateDto, UserDto, IAccountRepository>
    {
        Task<List<UserDto>> GetAllActive();

    }
    public class UserService : ServiceBase<Account, UserCreateDto, UserUpdateDto, UserDto, IAccountRepository>, IUserService
    {
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly ILogErrorRepository _logErrorRepository;
        public UserService(IAccountRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IGroupUserRepository groupUserRepository,
             ILogErrorRepository logErrorRepository) : base(repository, unitOfWork, mapper, uriService)
        {
            _groupUserRepository = groupUserRepository;
            _logErrorRepository = logErrorRepository;
        }

        public override async Task<UserDto> Add(UserCreateDto dto)
        {


            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        // Tạo mới Group từ GroupCreateDto
                        var account = _mapper.Map<Account>(dto);
                        account.Password = account.Password.HashPassword();
                        // Thêm Group vào repository
                        await _repository.Add(account);
                        await _unitOfWork.SaveChangeAsync();

                        // Thêm GroupRole nếu có RoleIds
                        if (dto.GroupIds != null && dto.GroupIds.Any())
                        {
                            var groupUsers = dto.GroupIds.Select(group => new GroupUser
                            {
                                GroupId = group,
                                AccountId = account.Id
                            });
                            await _groupUserRepository.AddRange(groupUsers);
                        }
                        // Lưu các thay đổi
                        await _unitOfWork.SaveChangeAsync();

                        // Commit transaction
                        await transaction.CommitAsync();

                        // Map và trả về GroupDto
                        var groupDto = _mapper.Map<UserDto>(account);
                        return groupDto;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw ex;
                    }
                }
            });
        }
        public async Task<List<UserDto>> GetAllActive()
        {
            var users = await _repository.FindAll(x => x.IsLocked == false);
            var result = _mapper.Map<List<UserDto>>(users);

            return result;
        }
        public override async Task<UserDto> FindById(string id)
        {
            var user = await _repository.Find(match: x => x.Id == id, include: inc => inc.Include(x => x.GroupUsers));
            if (user == null)
            {
                throw new AppException("Không tìm thấy người dùng theo yêu cầu", 400);
            }
            var userDto = _mapper.Map<UserDto>(user);
            // Gán GroupIds
            userDto.GroupIds = user.GroupUsers.Select(gu => gu.GroupId).ToList();
            return userDto;
        }

        public override async Task<UserDto> Update(UserUpdateDto dto)
        {
            var user = await _repository.Find(x=> x.Id == dto.Id, include: x=> x.Include(x=> x.GroupUsers));
            if (user == null)
            {
                throw new AppException("Không tìm thấy người dùng theo yêu cầu", 400);
            }
            // Kiểm tra email có tồn tại không
            if (await IsExitEmailOtherCurrent(dto.Email, dto.Id))
            {
                throw new AppException("Email đã tồn tại", 400);
            }
            // Kiểm tra số điện thoại có tồn tại không
            if (await IsExitPhoneNumberOtherCurrent(dto.PhoneNumber, dto.Id))
            {
                throw new AppException("Số điện thoại đã tồn tại", 400);
            }

            // Cập nhật thông tin người dùng
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.IsLocked = dto.IsLocked;
            user.PhoneNumber = dto.PhoneNumber;

            // Cập nhật nhóm người dùng
            if (dto.GroupIds != null)
            {
                var currentGroupIds = user.GroupUsers.Select(gu => gu.GroupId).ToList();
                var newGroupIds = dto.GroupIds;

                // Xóa các nhóm không còn trong danh sách mới
                user.GroupUsers = user.GroupUsers.Where(gu => newGroupIds.Contains(gu.GroupId)).ToList();

                // Thêm các nhóm mới (chỉ những nhóm chưa tồn tại)
                foreach (var groupId in newGroupIds.Where(id => !currentGroupIds.Contains(id)))
                {
                    user.GroupUsers.Add(new GroupUser { GroupId = groupId, AccountId = user.Id });
                }
            }
            else
            {
                // Nếu GroupIds là null, xóa tất cả các nhóm
                user.GroupUsers.Clear();
            }
            // Cập nhật thông tin người dùng
            await _repository.Update(user);
            await _unitOfWork.SaveChangeAsync();
            // Map và trả về UserDto
            return _mapper.Map<UserDto>(user);
        }

        // kiểm tra xem có tồn tại email nào khác với email hiện tại không
        private async Task<bool> IsExitEmailOtherCurrent(string email, string id)
        {
            var user = await _repository.Find(match: x => x.Email == email && x.Id != id);
            return user != null;
        }

        // kiểm tra xem có số điện thoại nào khác với số điện thoại hiện tại không
        private async Task<bool> IsExitPhoneNumberOtherCurrent(string phoneNumber, string id)
        {
            var user = await _repository.Find(match: x => x.PhoneNumber == phoneNumber && x.Id != id);
            return user != null;
        }
    }
}
