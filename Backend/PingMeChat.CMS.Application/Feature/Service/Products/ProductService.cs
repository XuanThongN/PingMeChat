using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Feature.Service.Products.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using PingMeChat.CMS.Entities.Feature;

namespace PingMeChat.CMS.Application.Feature.Service.Products
{
    public interface IProductService : IServiceBase<Product, ProductCreateDto, ProductUpdateDto, ProductDto, IProductRepository>
    {
        Task<List<ProductDto>> GetAllNotEmpty();
        Task<string> GenerateUniqueCodeAsync();
    }
    public class ProductService : ServiceBase<Product, ProductCreateDto, ProductUpdateDto, ProductDto, IProductRepository>, IProductService
    {
        public ProductService(IProductRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IUriService uriService) : base(repository, unitOfWork, mapper, uriService)
        {
        }

        public override async Task<ProductDto> Add(ProductCreateDto dto)
        {
            var entity = await _repository.Find(x => x.Code == dto.Code);
            if (entity != null) throw new ApplicationException("Mã sản phẩm đã tồn tại");

            var Product = _mapper.Map<Product>(dto);
            //entity.ProductStatus = ProductStatus.Available;
            await _repository.Add(Product);
            await _unitOfWork.SaveChangeAsync();
            return _mapper.Map<ProductDto>(Product);
        }

        public async Task<List<ProductDto>> GetAllNotEmpty()
        {
            var entities = await _repository.FindAll(x => x.Quantity > 0);
            return _mapper.Map<List<ProductDto>>(entities);
        }

        #region tạo mã sản phẩm
        public async Task<string> GenerateUniqueCodeAsync()
        {
            string code;
            do
            {
                code = GenerateCode();
            } while (await CodeExistsAsync(code));

            return code;
        }

        private string GenerateCode()
        {
            // Format: PRD-YYMMDD-HHMMSS-XXXX
            // YY: năm, MM: tháng, DD: ngày, HH: giờ, MM: phút, SS: giây, XXXX: số ngẫu nhiên 4 chữ số
            var timestamp = DateTime.Now;
            var random = new Random();
            return $"PRD-{timestamp:yyMMdd-HHmmss}-{random.Next(1000, 9999):D4}";
        }

        private async Task<bool> CodeExistsAsync(string code)
        {
            return await _repository.AnyAsync(o => o.Code == code);
        }
        #endregion
    }
}
