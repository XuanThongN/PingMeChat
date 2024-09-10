using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Exceptions;
using PingMeChat.CMS.Application.Feature.Service.BidaTables.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;
using NPOI.SS.Formula.Functions;

namespace PingMeChat.CMS.Application.Feature.Service
{
    public interface IBidaTableTypeService : IServiceBase<BidaTableType, BidaTableTypeCreateDto, BidaTableTypeUpdateDto, BidaTableTypeDto, IBidaTableTypeRepository>
    {
    }
    public class BidaTableTypeService : ServiceBase<BidaTableType, BidaTableTypeCreateDto, BidaTableTypeUpdateDto, BidaTableTypeDto, IBidaTableTypeRepository>, IBidaTableTypeService
    {
        private readonly IBidaTableRepository _bidaTableRepository;
        public BidaTableTypeService(IBidaTableTypeRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IUriService uriService, IBidaTableRepository bidaTableRepository) : base(repository, unitOfWork, mapper, uriService)
        {
            _bidaTableRepository = bidaTableRepository;
        }

        public override async Task<BidaTableTypeDto> Delete(string id)
        {
            //Kiểm tra xem có tồn tại các bảng bida liên quan không
            if (await HasRelatedEntities(id))
            {
                throw new AppException("Không thể xóa Loại bàn bida này vì có Bàn bida liên quan");
            }
            return await base.Delete(id);
        }

        private async Task<bool> HasRelatedEntities(string id)
        {
            // Check if there are any related child entities
            var hasRelatedEntities = await _bidaTableRepository.AnyAsync(x => x.BidaTableTypeId == id && x.IsDeleted == false);
            return hasRelatedEntities;
        }
    }
}
