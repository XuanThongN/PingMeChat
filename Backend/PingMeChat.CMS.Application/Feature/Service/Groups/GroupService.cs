using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Feature.Service.Groups.Dto;
using PingMeChat.CMS.Application.Feature.Service.Users.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities;
using Microsoft.EntityFrameworkCore;

namespace PingMeChat.CMS.Application.Feature.Service.Groups
{
    public interface IGroupService : IServiceBase<Group, GroupCreateDto, GroupUpdateDto, GroupDto, IGroupsRepository>
    {
        Task<bool> CheckExitsName(string name);
        Task<List<GroupDto>> GetAllActive();

    }
    public class GroupService : ServiceBase<Group, GroupCreateDto, GroupUpdateDto, GroupDto, IGroupsRepository>, IGroupService
    {
        private readonly IGroupRoleRepository _groupRoleRepository;
        private readonly IGroupUserRepository _groupUserRepository;
        public GroupService(IGroupsRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IGroupRoleRepository groupRoleRepository,
            IGroupUserRepository groupUserRepository
            ) : base(repository, unitOfWork, mapper, uriService)
        {
            _groupRoleRepository = groupRoleRepository;
            _groupUserRepository = groupUserRepository;
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
    }
}
