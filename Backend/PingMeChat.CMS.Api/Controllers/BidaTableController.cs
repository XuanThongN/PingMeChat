using AutoWrapper.Models;
using PingMeChat.CMS.API.Routes;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Common.Filters;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Feature.Service;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.Shared;
using PingMeChat.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PingMeChat.CMS.Api.Controllers
{
    [Authorize]
    public class BidaTableController : BaseController
    {
        private readonly IBidaTableService _bidaTableService;

        public BidaTableController(IBidaTableService bidaTableService)
        {
            _bidaTableService = bidaTableService;
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.BidaTable.PaginationRoute)]
        [ProducesResponseType(typeof(ResultDataTable<BidaTableDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Pagination([FromBody] PaginationFilter<BidaTablePaginationFilterDto> model)
        {
            var pageRequest = new PaginationFilter(model.PageNumber, model.PageSize);
            var data = await _bidaTableService.Pagination(
                pageRequest.PageNumber,
                pageRequest.PageSize,
                predicate: x =>
                (string.IsNullOrEmpty(model.Keyword) || x.Code.Contains(model.Keyword))
                && (model.Data.BidaTableStatus == null || x.BidaTableStatus == model.Data.BidaTableStatus)
                && (string.IsNullOrEmpty(model.Data.BidaTableTypeId) || x.BidaTableTypeId == model.Data.BidaTableTypeId),
                include: inc => inc.Include(x => x.BidaTableType),
                orderBy: ord => ord.OrderByDescending(x => x.CreatedDate));

            var result = new ResultDataTable<BidaTableDto>(pageRequest.PageNumber, data.TotalRecords, data.TotalRecords, data.Data);
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedAllCompleted, "bàn bida"), result, StatusCodes.Status200OK));
        }

        [HttpGet]
        [Route(RootPath.Feature.BidaTable.GetByIdRoute)]
        [ProducesResponseType(typeof(BidaTableDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new AppException(Message.Warning.InvalidInfo, 400);
            }

            var data = await _bidaTableService.Find(match: t => t.Id == id, include: inc => inc.Include(x => x.BidaTableType));
            if (data == null)
            {
                return BadRequest(new ApiResponse(Message.Error.NotExisted, null, StatusCodes.Status404NotFound));
            }
            return Ok(new ApiResponse(string.Format(Message.Success.ReadedCompleted, "bàn bida"), data, 200));
        }

        [HttpPost]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.BidaTable.AddRoute)]
        [ProducesResponseType(typeof(BidaTableDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] BidaTableCreateDto dto)
        {
            var email = GetEmail();
            dto.CreatedBy = email;
            dto.UpdatedBy = email;
            var result = await _bidaTableService.Add(dto);

            return Ok(new ApiResponse(string.Format(Message.Success.CreateCompleted, "bàn bida"), result, 201));
        }

        [HttpPut]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.BidaTable.UpdateRoute)]
        [ProducesResponseType(typeof(BidaTableDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] BidaTableUpdateDto dto)
        {
            var email = GetEmail();
            dto.UpdatedBy = email;
            var result = await _bidaTableService.Update(dto);

            return Ok(new ApiResponse(string.Format(Message.Success.UpdatedCompleted, "bàn bida"), result, 200));
        }

        [HttpDelete]
        [ValidateUserAndModel]
        [Route(RootPath.Feature.BidaTable.DeleteRoute)]
        [ProducesResponseType(typeof(BidaTableDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new AppException(Message.Warning.InvalidInfo, 400);
            }

            var email = GetEmail();
            var result = await _bidaTableService.Delete(id, email);
            if (!result)
                return BadRequest(new ApiResponse(string.Format(Message.Error.DeletedError, "bàn bida"), null, 500));
            return Ok(new ApiResponse(string.Format(Message.Success.DeletedCompleted, "bàn bida"), result, 200));
        }

        [HttpGet]
        [Route(RootPath.Feature.BidaTable.GetEmptiesRoute)]
        [ProducesResponseType(typeof(BidaTableDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmpties()
        {
            var result = await _bidaTableService.GetEmptyTables();
            return Ok(new ApiResponse(Message.Success.BidaTable.GetEmpties, result, StatusCodes.Status200OK));
        }


        [HttpGet]
        [Route(RootPath.Feature.BidaTable.GetTotalWithStatusRoute)]
        [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalWithStatus()
        {
            var result = await _bidaTableService.GetTotalWithStatus();
            return Ok(new ApiResponse(Message.Success.BidaTable.GetTotalWithStatus, result, StatusCodes.Status200OK));
        }


    }
}
