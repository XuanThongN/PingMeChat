using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.App.Repositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.Menus.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities;
using PingMeChat.Shared.Utils;
using LinqKit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Octokit;
using System.Linq.Expressions;

namespace PingMeChat.CMS.Application.Feature.Service.Menus
{
    public interface IMenuService : IServiceBase<Menu, MenuCreateDto, MenuUpdateDto, MenuDto, IMenuRepository>
    {
        Task<IEnumerable<MenuDto>> GetRolesByUserId(string userId);
        Task<IEnumerable<MenuDto>> GetMenusByUser(string userName, string userId);
        Task<IEnumerable<MenuDto>> GetMenusByUserForTree(string userName, string userId);
        Task<IEnumerable<MenuDto>> GetMenusByUserId(string userId);
        Task<IEnumerable<MenuDto>> GetAll(MenusOptionSearchDto model);
        Task<IEnumerable<MenuDto>> GetAllTree();
        Task<IEnumerable<MenuDto>> GetRolesByUserIdAndUrl(string userId);
        Task<MenuDto> ChangeStatus(string id, string email);
        Task<MenuDto> Delete(string id, string email);

    }
    public class MenuService : ServiceBase<Menu, MenuCreateDto, MenuUpdateDto, MenuDto, IMenuRepository>, IMenuService
    {
        private readonly IGroupsRepository _groupsRepository;
        private readonly IAccountRepository _accountRepository;
        public MenuService(IMenuRepository repository,
                            IUnitOfWork unitOfWork,
                            IMapper mapper,
                            IUriService uriService,
                            IGroupsRepository groupsRepository,
                            IAccountRepository accountRepository) : base(repository, unitOfWork, mapper, uriService)
        {
            _groupsRepository = groupsRepository;
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<MenuDto>> GetRolesByUserId(string userId)
        {
            var menus = await _repository.FindAll(x =>
                                            (x.UserMenus.Any(u => u.Account.Id == userId) ||
                                            x.RoleMenus.Any(r => r.Role.GroupRoles.Any(gr => gr.Group.GroupUsers.Any(grUr => grUr.AccountId == userId))))
                                            && x.IsActive);

            return _mapper.Map<IEnumerable<MenuDto>>(menus);
        }
        public async Task<IEnumerable<MenuDto>> GetMenusByUser(string userName, string userId)
        {
            // Normalize userName for comparison
            var normalizedUserName = userName.Trim().ToLowerInvariant();

            // Get user's groups
            var userGroups = await _groupsRepository.FindAll(x => x.GroupUsers.Any(u => u.AccountId == userId));

            IEnumerable<Menu> menus;

            if (normalizedUserName == "superadmin")
            {
                // Superadmin gets all active non-MenuType menus
                menus = await _repository.FindAll(x => x.IsActive, orde => orde.OrderBy(o => o.SortOrder));

            }
            else
            {
                // Get menus directly assigned to the user
                var userMenus = await _repository.FindAll(x => x.UserMenus.Any(u => u.AccountId == userId)
                                                           && x.IsActive, orde => orde.OrderBy(o => o.SortOrder));

                if (userGroups.Any())
                {
                    // Get all role IDs associated with the user's groups
                    var groupRoleIds = userGroups.SelectMany(g => g.GroupRoles.Select(gr => gr.RoleId))
                                                 .Distinct()
                                                 .ToList();

                    // Get menus associated with the user's group roles
                    var groupMenus = await _repository.FindAll(x => x.RoleMenus.Any(rm => groupRoleIds.Contains(rm.RoleId))
                                                                 && x.IsActive, orde => orde.OrderBy(o => o.SortOrder));

                    // Combine and deduplicate user menus and group menus
                    menus = userMenus.Union(groupMenus).Distinct().ToList();
                }
                else
                {
                    // If user has no groups, just use the direct user menus
                    menus = userMenus;
                }
            }

            // Filter out menus that have parents in the result set
            // var response = menus.Where(m => !menus.Any(x => x.Id == m.ParentId)).ToList();

            // Map to DTOs and return
            return _mapper.Map<IEnumerable<MenuDto>>(menus);
        }
        public async Task<IEnumerable<MenuDto>> GetRolesByUserIdAndUrl(string userId)
        {
            // Tìm menu hiện tại dựa trên userId
            var currentMenu = await _repository.Find(x =>
                (x.UserMenus.Any(u => u.AccountId == userId) ||
                 x.RoleMenus.Any(r => r.Role.GroupRoles.Any(gr => gr.Group.GroupUsers.Any(grUr => grUr.AccountId == userId)))) &&
                x.IsActive &&
                !x.MenuType
            );

            // Nếu không tìm thấy menu, trả về danh sách rỗng
            if (currentMenu == null) return Enumerable.Empty<MenuDto>();

            // Tìm tất cả các menu con của menu hiện tại
            var childMenus = await _repository.FindAll(x =>
                (x.UserMenus.Any(u => u.AccountId == userId) ||
                 x.RoleMenus.Any(r => r.Role.GroupRoles.Any(gr => gr.Group.GroupUsers.Any(grUr => grUr.AccountId == userId)))) &&
                x.MenuType &&
                x.IsActive &&
                x.ParentId == currentMenu.Id
            );

            // Chuyển đổi các menu con thành MenuDto và trả về
            return _mapper.Map<IEnumerable<MenuDto>>(childMenus);
        }
        public async Task<IEnumerable<MenuDto>> GetMenusByUserForTree(string userName, string userId)
        {
            var normalizedUserName = userName.Trim().ToLowerInvariant();
            var userGroups = await _groupsRepository.FindAll(x => x.GroupUsers.Any(u => u.AccountId == userId));
            IEnumerable<Menu> menus;

            if (normalizedUserName == "superadmin")
            {
                menus = await _repository.FindAll(x => x.IsActive && !x.MenuType, order => order.OrderBy(o => o.SortOrder));
            }
            else
            {
                var userMenus = await _repository.FindAll(x => x.UserMenus.Any(u => u.AccountId == userId)
                                                           && !x.MenuType && x.IsActive, order => order.OrderBy(o => o.SortOrder));
                if (userGroups.Any())
                {
                    var groupRoleIds = userGroups.SelectMany(g => g.GroupRoles.Select(gr => gr.RoleId))
                                                 .Distinct()
                                                 .ToList();
                    var groupMenus = await _repository.FindAll(x => x.RoleMenus.Any(rm => groupRoleIds.Contains(rm.RoleId))
                                                                && !x.MenuType && x.IsActive, order => order.OrderBy(o => o.SortOrder));
                    menus = userMenus.Union(groupMenus).Distinct().ToList();
                }
                else
                {
                    menus = userMenus;
                }
            }

            // Tạo cấu trúc cây menu
            var menuDtos = _mapper.Map<IEnumerable<MenuDto>>(menus);
            var rootMenus = BuildMenuTree(menuDtos);

            return rootMenus;
        }
        public async Task<IEnumerable<MenuDto>> GetMenusByUserId(string userId)
        {
            var user = await _accountRepository.FindById(userId);
            if (user == null) throw new AppException("Tài khoản không tồn tại trong hệ thống", 404);

            IEnumerable<Menu> menus = null;
            var menuByGroups = await _groupsRepository.FindAll(x => x.GroupUsers.Any(u => u.AccountId == userId));

            if (user.UserName.ToLower().Trim().Equals("supperadmin".ToLower().Trim()))
            {
                menus = await _repository.FindAll(x => !x.MenuType && x.IsActive,
                                                    orderBy: x => x.OrderBy(x => x.SortOrder));
            }
            else
            {
                if (menuByGroups != null)
                {
                    // get menu by user
                    menus = await _repository.FindAll(x => (x.UserMenus.Any(u => u.AccountId == userId) ||
                                                                menuByGroups.Any(grp => x.RoleMenus.Any(rm => grp.GroupRoles.Any(grpr => grpr.RoleId == rm.RoleId))))
                                                                && !x.MenuType && x.IsActive,
                                                                orderBy: x => x.OrderBy(x => x.SortOrder));
                }
                else
                {
                    menus = await _repository.FindAll(x => x.UserMenus.Any(u => u.AccountId == userId)
                                                            && !x.MenuType && x.IsActive,
                                                            orderBy: x => x.OrderBy(x => x.SortOrder));
                }
            }

            var response = new List<Menu>();
            foreach (var itemMenu in menus)
            {
                if (!CheckExistItemByParrentId(menus, itemMenu.ParentId))
                {
                    response.Add(itemMenu);
                }
            }

            return _mapper.Map<IEnumerable<MenuDto>>(response);
        }
        public async Task<IEnumerable<MenuDto>> GetAll(MenusOptionSearchDto model)
        {

            Expression<Func<Menu, bool>> predicate = BuildPredicate(model);

            if (predicate == null)
            {
                var menusWithoutSearch = await _repository.FindAll(match: x => x.ParentId == null,
                                                        orderBy: ord => ord.OrderBy(x => x.SortOrder),
                                                        include: inc => inc.Include(x => x.Children));

                return _mapper.Map<List<MenuDto>>(menusWithoutSearch);
            }

            var menus = await _repository.FindAll(predicate, orderBy: ord => ord.OrderBy(x => x.SortOrder), include: inc => inc.Include(x => x.Children));
            var response = new List<Menu>();
            foreach (var itemMenu in menus)
            {
                if (!CheckExistItemByParrentId(menus, itemMenu.ParentId))
                {
                    response.Add(itemMenu);
                }
            }

            return _mapper.Map<List<MenuDto>>(response);
            //var menus = await _repository.FindAll(predicate,
            //                                            orderBy: ord => ord.OrderBy(x => x.SortOrder),
            //                                            include: inc => inc.Include(x => x.Children));
            //return _mapper.Map<List<MenuDto>>(menus);

        }
        public async Task<IEnumerable<MenuDto>> GetAllTree()
        {
            var menus = await _repository.GetAll();
            var treeTable = new List<Menu>();
            GetTreeTable(null, menus.ToList(), treeTable, string.Empty);

            return _mapper.Map<IEnumerable<MenuDto>>(treeTable);
        }
        public List<Menu> CateChil(string? parentId, List<Menu> objcate)
        {
            return objcate.Where(info => info.ParentId == parentId).ToList();
        }
        public async Task<MenuDto> ChangeStatus(string id, string email)
        {
            var menu = await _repository.FindById(id);
            if (menu == null)
            {
               throw new AppException(string.Format(Message.Error.NotFound, "menu - quyền"), null, StatusCodes.Status400BadRequest);
            }
            
            menu.IsActive = menu.IsActive ? false : true;
            menu.UpdatedBy = email;

            await _repository.Update(menu);
            await _unitOfWork.SaveChangeAsync();

            return _mapper.Map<MenuDto>(menu);
        }
        public async Task<MenuDto> Delete(string id, string email)
        {
            var menu = await _repository.FindById(id);
            if(menu == null)
            {
                throw new AppException(Message.Error.NotValid, 400);
            }

            menu.IsDeleted = true;
            menu.DeletedBy = email;
            menu.UpdatedBy = email;

            await _repository.Update(menu);
            await _unitOfWork.SaveChangeAsync();

            return _mapper.Map<MenuDto>(menu);

        }

        private List<Menu> GetTreeTable(string? parentId, List<Menu> objOriginal, List<Menu> objResults, string space)
        {
            var objbind = CateChil(parentId, objOriginal);
            if (objbind != null && objbind.Count > 0)
            {
                foreach (var t in objbind)
                {
                    var objtab = t;
                    objtab.Title = space + objtab.Title;
                    objResults.Add(objtab);
                    GetTreeTable(objtab.Id, objOriginal, objResults, space + "--- ");
                }
            }
            return objResults;
        }
        private Expression<Func<Menu, bool>> BuildPredicate(MenusOptionSearchDto model)
        {
            if (model == null || (string.IsNullOrWhiteSpace(model.Title) && model.Status == null))
            {
                return null;
            }

            var predicate = PredicateBuilder.New<Menu>();

            if (!string.IsNullOrWhiteSpace(model.Title))
            {
                predicate = predicate.And(x => x.Title.ToLower().Contains(model.Title.ToLower().Trim()));
            }

            if (model.Status.HasValue)
            {
                predicate = predicate.And(x => x.IsActive == model.Status.Value);
            }

            return predicate;
        }
        private bool CheckExistItemByParrentId(IEnumerable<Menu> sources, string? parentId)
        {
            bool result = false;
            if (parentId == null) return result;


            foreach (var menu in sources)
            {
                if (menu.Id == parentId)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
        private IEnumerable<MenuDto> BuildMenuTree(IEnumerable<MenuDto> allMenus)
        {
            var menuDict = allMenus.ToDictionary(m => m.Id);
            var rootMenus = new List<MenuDto>();

            foreach (var menu in allMenus)
            {
                if (string.IsNullOrEmpty(menu.ParentId))
                {
                    rootMenus.Add(menu);
                }
                else if (menuDict.TryGetValue(menu.ParentId, out var parentMenu))
                {
                    parentMenu.Children ??= new List<MenuDto>();
                    parentMenu.Children.Add(menu);
                }
            }

            return rootMenus;
        }
    }
}
