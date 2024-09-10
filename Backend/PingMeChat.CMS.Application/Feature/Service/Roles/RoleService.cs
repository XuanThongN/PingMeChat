using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.Roles.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities;

namespace PingMeChat.CMS.Application.Feature.Service.Roles
{
    public interface IRoleService : IServiceBase<Role, RoleCreateDto, RoleUpdateDto, RoleDto, IRoleRepository>
    {
        Task<List<RoleDto>> GetAllRoles();
        Task<List<RoleDto>> GetAllActive();
        Task<bool> CreateRole(string roleName);
        Task<bool> UpdateRole(RoleUpdateDto role);
        Task<bool> AssignMenuToRole(string roleId, string menuId);
        Task<bool> RemoveMenuFromRole(string roleId, string menuId);
        Task<bool> CheckExitsName(string name);
        Task<bool> CheckExitsNameForUpdate(string email, string name);
    }
    public class RoleService : ServiceBase<Role, RoleCreateDto, RoleUpdateDto, RoleDto, IRoleRepository>, IRoleService
    {
        private readonly IRolesMenuRepository _rolesMenuRepository;
        private readonly IAccountRepository _acountRepository;

        public RoleService(IRoleRepository repository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUriService uriService,
            IRolesMenuRepository rolesMenuRepository,
            IAccountRepository acountRepository) : base(repository, unitOfWork, mapper, uriService)
        {
            _rolesMenuRepository = rolesMenuRepository;
            _acountRepository = acountRepository;
        }

        public async Task<List<RoleDto>> GetAllRoles()
        {
            //var roles = await _repository.GetAllAsync();
            //var roleDtos = _mapper.Map<List<RoleDto>>(roles);
            //return roleDtos;

            return new List<RoleDto>();
        }
         public async Task<List<RoleDto>> GetAllActive()
        {
            var roles = await _repository.FindAll(x => x.Status == true);
            var roleDtos = _mapper.Map<List<RoleDto>>(roles);
            return roleDtos;
        }

        public async Task<bool> CreateRole(string roleName)
        {
            //await _repository.CreateRoleByNameAsync(roleName);
            //await _unitOfWork.SaveChangeAsync();

            return true;
        }

        public async Task<bool> UpdateRole(RoleUpdateDto role)
        {
            //await _repository.UpdateNameRole(role.Id, role.Name);
            //await _unitOfWork.SaveChangeAsync();

            return true;
        }

        public async Task<bool> AssignMenuToRole(string roleId, string menuId)
        {
            var roleMenu = new RoleMenu { RoleId = roleId, MenuId = menuId };
            await _rolesMenuRepository.Add(roleMenu);
            await _unitOfWork.SaveChangeAsync();
            return true;
        }

        public async Task<bool> RemoveMenuFromRole(string roleId, string menuId)
        {
            var roleMenu = await _rolesMenuRepository.Find(rm => rm.RoleId == roleId && rm.MenuId == menuId);
            if (roleMenu != null)
            {
                await _rolesMenuRepository.Delete(roleMenu);
                await _unitOfWork.SaveChangeAsync();
            }
            return true;
        }

        public async Task<bool> CheckExitsName(string name)
        {
            return await _repository.AnyAsync(x => x.Name.Trim().ToUpper().Equals(name.Trim().ToUpper()));
        }
        
        public async Task<bool> CheckExitsNameForUpdate(string email, string name)
        {
            var user = await _acountRepository.Find(x => x.Email == email);
            if (user == null)
            {
                throw new AppException("Không tìm thấy người dùng trong hệ thống", 404);
            }
            return await _repository.AnyAsync(x => x.Id != user.Id &&  x.Name.Trim().ToUpper().Equals(name.Trim().ToUpper()));
        }

    }
}
