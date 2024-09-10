using AutoWrapper.Models;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Feature.Service.Menus;
using PingMeChat.CMS.Application.Feature.Service.Menus.Dto;
using PingMeChat.CMS.Application.Feature.Service.MvcControllerDiscovery;
using PingMeChat.CMS.Application.Feature.Service.Orders.Dto;
using PingMeChat.CMS.Entities;
using PingMeChat.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PingMeChat.CMS.Api.Controllers
{
    [Authorize]
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;
        private readonly IMvcControllerDiscoveryService _mvcControllerDiscoveryService;
        public MenuController(IMenuService menuService,
            IMvcControllerDiscoveryService mvcControllerDiscoveryService)
        {
            _menuService = menuService;
            _mvcControllerDiscoveryService = mvcControllerDiscoveryService;
        }

        [HttpGet]
        [TypeFilter(typeof(UserExistsFilter))]
        [Route(RootPath.Feature.Menu.GetAllByCurrentUserRoute)]
        [ProducesResponseType(typeof(IEnumerable<MenuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllByCurrentUser()
        {
            var userId = GetUserId();
            var userName = GetUserName();

            var response = await _menuService.GetMenusByUserForTree(userName, userId);
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "menu"), response, StatusCodes.Status200OK));
        }

        [HttpGet]
        [Route(RootPath.Feature.Menu.GetAllTreeRoute)]
        [ProducesResponseType(typeof(IEnumerable<MenuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTree()
        {
            var response = await _menuService.GetAllTree();
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "menu theo dạng cây"), response, StatusCodes.Status200OK));
        }

        [HttpGet]
        [Route(RootPath.Feature.Menu.GetControllerByConditionRoute)]
        [ProducesResponseType(typeof(IEnumerable<MvcControllerInfo>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetControllerByCondition()
        {
            var response = await _mvcControllerDiscoveryService.GetControllers();
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "enpoint - controller"), response, StatusCodes.Status200OK));
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.Menu.AddRoute)]
        [ProducesResponseType(typeof(MenuDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Add([FromBody] MenuCreateDto model)
        {
            var email = GetEmail();

            model.CreatedBy = email;
            model.UpdatedBy = email;
            var itemNew = await _menuService.Add(model);

            return Ok(new ApiResponse(string.Format(Message.Success.CreateCompleted, "menu -quyền"), itemNew, StatusCodes.Status201Created));
        }

        [HttpPost]
        [ServiceFilter(typeof(ModelStateFilter))]
        [Route(RootPath.Feature.Menu.GetAllRoute)]
        [ProducesResponseType(typeof(IEnumerable<MenuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromBody] MenusOptionSearchDto model)
        {
            var response = await _menuService.GetAll(model);
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "menu - quyền"), response, 200));
        }

        [HttpGet]
        [Route(RootPath.Feature.Menu.GetRoleByCurrentUserModuleRoute)]
        [ServiceFilter(typeof(UserExistsFilter))]
        [ProducesResponseType(typeof(IEnumerable<MenuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRoleByCurrentUserModule()
        {
            var currentId = GetUserId();

            var menus = await _menuService.GetRolesByUserIdAndUrl(currentId);
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "menu - quyền"), menus, StatusCodes.Status200OK));
        }

        [HttpGet]
        [Route(RootPath.Feature.Menu.GetByIdRoute)]
        [ProducesResponseType(typeof(IEnumerable<MenuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse(string.Format(Message.Error.NotFound, "menu - quyền"), null, StatusCodes.Status400BadRequest));
            }
            var data = await _menuService.FindById(id);
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedCompleted, "menu - quyền"), data, StatusCodes.Status200OK));
        }

        [HttpPut]
        [ServiceFilter(typeof(UserExistsFilter))]
        [Route(RootPath.Feature.Menu.ChangeStatusRoute)]
        [ProducesResponseType(typeof(IEnumerable<MenuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse(string.Format(Message.Error.NotFound, "menu - quyền"), null, StatusCodes.Status400BadRequest));
            }

            var email = GetEmail();
            var data = await _menuService.ChangeStatus(id, email);

            return Ok(new ApiResponse(string.Format(Message.Success.UpdatedCompleted, "trạng thái menu - quyền"), data, StatusCodes.Status200OK));
        }

        [HttpPut]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.Menu.UpdateRoute)]
        [ProducesResponseType(typeof(IEnumerable<MenuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] MenuUpdateDto model)
        {
            var email = GetEmail();

            model.UpdatedBy = email;
            var data = await _menuService.Update(model);

            return Ok(new ApiResponse(string.Format(Message.Success.UpdatedCompleted, "menu - quyền"), data, StatusCodes.Status200OK));
        }

        [HttpDelete]
        [ServiceFilter(typeof(UserExistsFilter))]
        [Route(RootPath.Feature.Menu.DeleteRoute)]
        [ProducesResponseType(typeof(IEnumerable<MenuDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiResponse(string.Format(Message.Error.NotFound, "menu - quyền"), null, StatusCodes.Status400BadRequest));
            }

            var email = GetEmail();
            var data = await _menuService.Delete(id, email);

            return Ok(new ApiResponse(string.Format(Message.Success.DeletedCompleted, "trạng thái menu - quyền"), data, StatusCodes.Status200OK));
        }

    }
}
