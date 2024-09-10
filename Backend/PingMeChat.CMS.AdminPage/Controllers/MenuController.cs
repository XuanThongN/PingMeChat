using AutoWrapper.Models;
using PingMeChat.CMS.AdminPage.Common.Filter;
using PingMeChat.CMS.Application.Feature.Service.Menus;
using PingMeChat.CMS.Application.Feature.Service.Menus.Dto;
using PingMeChat.CMS.Application.Feature.Service.MvcControllerDiscovery;
using PingMeChat.CMS.Entities.Users;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using NToastNotify.Helpers;
using System.Security.Claims;

namespace PingMeChat.CMS.AdminPage.Controllers
{
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
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        [TypeFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> GetMenusByCurrentId()
        {
            var userId = GetUserId();
            var userName = GetUserName();

            var response = await _menuService.GetMenusByUser(userName, userId);

            return Ok(new ApiResponse("Danh sách menu của người dùng", response, StatusCodes.Status200OK));
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTree()
        {
            var response = await _menuService.GetAllTree();
            return Ok(new ApiResponse(response));
        }
        [HttpGet]
        public async Task<IActionResult> GetControllerByCondition()
        {
            var response = await _mvcControllerDiscoveryService.GetControllers();
            return Ok(new ApiResponse("Danh sách quyền - menu", response, 200));
        }
        [HttpPost]
        [ValidateUserAndModel]
        public async Task<IActionResult> Add([FromBody] MenuCreateDto model)
        {
            var email = GetEmail();
          
            model.CreatedBy = email;
            model.UpdatedBy = email;
            var itemNew = await _menuService.Add(model);

            return Ok(new ApiResponse("Đã tạo mới thành công menu - quyền", itemNew, 200));
        }
        [HttpPost]
        public async Task<IActionResult> GetAll([FromBody] MenusOptionSearchDto model)
        {
            var response = await _menuService.GetAll(model);
            return Ok(new ApiResponse("Danh sách menu - quyền", response, 200));
        }
    }
}
