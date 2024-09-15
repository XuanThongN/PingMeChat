using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.Groups.Dto;
using PingMeChat.CMS.Application.Feature.Service.Roles.Dto;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities;
using PingMeChat.Shared.Utils;
using Microsoft.EntityFrameworkCore;

namespace PingMeChat.CMS.Application.Feature.Service.Groups
{
    public interface IGroupService : IServiceBase<Group, GroupCreateDto, GroupUpdateDto, GroupDto, IGroupsRepository>
    {
        Task<bool> CheckExitsName(string name);
        Task<List<GroupDto>> GetAllActive();
        Task<bool> CheckExitsNameForUpdate(string id, string name);
        Task<GroupDto> ChangeStatus(string id, string email);
        Task<GroupDto> Delete(string id, string email);


    }
    public class GroupService : ServiceBase<Group, GroupCreateDto, GroupUpdateDto, GroupDto, IGroupsRepository>, IGroupService
    {
        private readonly IGroupRoleRepository _groupRoleRepository;
        private readonly IGroupUserRepository _groupUserRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IRoleRepository _roleRepository;
        public GroupService(IGroupsRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IGroupRoleRepository groupRoleRepository,
            IGroupUserRepository groupUserRepository,
            IAccountRepository accountRepository,
            IRoleRepository roleRepository
            ) : base(repository, unitOfWork, mapper, uriService)
        {
            _groupRoleRepository = groupRoleRepository;
            _groupUserRepository = groupUserRepository;
            _roleRepository = roleRepository;
            _accountRepository = accountRepository;
        }
        public override async Task<GroupDto> Add(GroupCreateDto dto)
        {
            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        // Tạo mới Group từ GroupCreateDto
                        var group = _mapper.Map<Group>(dto);

                        // Thêm Group vào repository
                        await _repository.Add(group);
                        await _unitOfWork.SaveChangeAsync();

                        // Thêm GroupRole nếu có RoleIds
                        if (dto.RoleIds != null && dto.RoleIds.Any())
                        {
                            var groupRoles = dto.RoleIds.Select(roleId => new GroupRole
                            {
                                GroupId = group.Id,
                                RoleId = roleId
                            });
                            await _groupRoleRepository.AddRange(groupRoles);
                        }

                        // Thêm GroupUser nếu có UserIds
                        if (dto.UserIds != null && dto.UserIds.Any())
                        {
                            var groupUsers = dto.UserIds.Select(userId => new GroupUser
                            {
                                GroupId = group.Id,
                                AccountId = userId
                            });
                            await _groupUserRepository.AddRange(groupUsers);
                        }

                        // Lưu các thay đổi
                        await _unitOfWork.SaveChangeAsync();

                        // Commit transaction
                        await transaction.CommitAsync();

                        // Map và trả về GroupDto
                        var groupDto = _mapper.Map<GroupDto>(group);
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

        public async Task<bool> CheckExitsName(string name)
        {
            return await _repository.AnyAsync(x => x.GroupName.Trim().ToUpper().Equals(name.Trim().ToUpper()));
        }

        public async Task<List<GroupDto>> GetAllActive()
        {
            var users = await _repository.FindAll(x => x.Status == true);
            var result = _mapper.Map<List<GroupDto>>(users);

            return result;
        }

        public async Task<bool> CheckExitsNameForUpdate(string id, string name)
        {

            return await _repository.AnyAsync(x => x.Id != id && x.GroupName.Trim().ToUpper().Equals(name.Trim().ToUpper()));
        }

        public override async Task<GroupDto> FindById(string id)
        {
            var group = await _repository.Find(x => x.Id == id, include: incl => incl.Include(x => x.GroupRoles).Include(x => x.GroupUsers));
            if (group == null)
            {
                throw new AppException(Message.Error.NotFound, null, 404);
            }
            var groupDto = _mapper.Map<GroupDto>(group);

            if (group.GroupRoles != null && group.GroupRoles.Any())
            {
                var roles = await _roleRepository.GetAll();

                if (roles != null && roles.Any())
                {
                    groupDto.Roles = roles.Where(x => group.GroupRoles.Select(x => x.RoleId).Contains(x.Id)).Select(x => new RoleDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Status = x.Status
                    }).ToList();
                }
            }

            if (group.GroupUsers != null && group.GroupUsers.Any())
            {
                var users = await _accountRepository.FindAll(x => x.UserName != "superadmin");

                if (users != null && users.Any())
                {
                    groupDto.Users = users.Where(x => group.GroupUsers.Select(x => x.AccountId).Contains(x.Id)).Select(x => new UserDto
                    {
                        Id = x.Id,
                        Email = x.Email,
                        FullName = x.FullName,
                        PhoneNumber = x.PhoneNumber,
                        IsLocked = x.IsLocked,
                        UserName = x.UserName
                    }).ToList();
                }
            }
            return groupDto;
        }

        public async Task<GroupDto> ChangeStatus(string id, string email)
        {
            var user = await _repository.FindById(id);
            if (user == null)
            {
                throw new AppException(Message.Error.NotValid, 400);
            }

            user.Status = !user.Status;
            user.UpdatedBy = email;

            await _repository.Update(user);
            await _unitOfWork.SaveChangeAsync();

            return _mapper.Map<GroupDto>(user);
        }

        public override async Task<GroupDto> Update(GroupUpdateDto dto)
        {
            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        // Lấy Group từ id
                        var group = await _repository.FindById(dto.Id);
                        if (group == null)
                        {
                            throw new AppException(Message.Error.NotFound, 404);
                        }

                        // Cập nhật thông tin Group từ GroupUpdateDto
                        _mapper.Map(dto, group);

                        // Cập nhật Group vào repository
                        await _repository.Update(group);
                        await _unitOfWork.SaveChangeAsync();

                        // Xóa GroupRole cũ
                        var groupRoles = await _groupRoleRepository.FindAll(x => x.GroupId == group.Id);
                        if (groupRoles != null && groupRoles.Any())
                        {
                            await _groupRoleRepository.RemoveRange(groupRoles);
                        }

                        // Thêm GroupRole nếu có RoleIds
                        if (dto.RoleIds != null && dto.RoleIds.Any())
                        {
                            groupRoles = dto.RoleIds.Select(roleId => new GroupRole
                            {
                                GroupId = group.Id,
                                RoleId = roleId
                            });
                            await _groupRoleRepository.AddRange(groupRoles);
                        }

                        // Xóa GroupUser cũ
                        var groupUsers = await _groupUserRepository.FindAll(x => x.GroupId == group.Id);
                        if (groupUsers != null && groupUsers.Any())
                        {
                            await _groupUserRepository.RemoveRange(groupUsers);
                        }

                        // Thêm GroupUser nếu có UserIds
                        if (dto.UserIds != null && dto.UserIds.Any())
                        {
                            groupUsers = dto.UserIds.Select(userId => new GroupUser
                            {
                                GroupId = group.Id,
                                AccountId = userId
                            });
                            await _groupUserRepository.AddRange(groupUsers);
                        }

                        // Lưu các thay đổi
                        await _unitOfWork.SaveChangeAsync();

                        // Commit transaction
                        await transaction.CommitAsync();

                        // Map và trả về GroupDto
                        var groupDto = _mapper.Map<GroupDto>(group);
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
        public async Task<GroupDto> Delete(string id, string email)
        {
            var group = await _repository.FindById(id);
            if (group == null)
            {
                throw new AppException(Message.Error.NotValid, 400);
            }

            group.IsDeleted = true;
            group.DeletedBy = email;
            group.UpdatedBy = email;

            await _repository.Update(group);
            await _unitOfWork.SaveChangeAsync();

            return _mapper.Map<GroupDto>(group);

        }
    }
}
