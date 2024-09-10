using AutoWrapper.Models;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Application.Feature.Service.SessionServicess;
using Microsoft.AspNetCore.Mvc;

namespace PingMeChat.CMS.AdminPage.Controllers
{
    public class BidaTableSessionController : BaseController
    {
        private readonly IBidaTableSessionServices _bidaTableSessionServices;
        public BidaTableSessionController(IBidaTableSessionServices bidaTableSessionServices)
        {
            _bidaTableSessionServices = bidaTableSessionServices;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ApiResponse> GetAll()
        {
            var result = await _bidaTableSessionServices.GetAll();

            return new ApiResponse("oki", result, StatusCodes.Status200OK);
        }

        [HttpPost]
        public async Task<ApiResponse> GetAllByTableId([FromBody] BidaTableIdDto model)
        {
            var result = await _bidaTableSessionServices.GetAllByTableId(model.BidaTableId);

            return new ApiResponse("oki", result, StatusCodes.Status200OK);
        }
    }
}
