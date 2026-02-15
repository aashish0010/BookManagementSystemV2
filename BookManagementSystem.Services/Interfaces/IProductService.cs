using BookManagementSystem.Domain.DTO;

namespace BookManagementSystem.Service.Interfaces
{
    public interface IProductService
    {
        Task<ProductResponseDto> GetProducts(int companyInfoId, int? categoryId, string search, int page, int pageSize);
        Task<SingleProductResponseDto> GetProductBySlug(string slug);
        Task<CategoryResponseDto> GetCategories(int companyInfoId);
        Task<SingleProductResponseDto> CreateProduct(CreateProductRequest request);
        Task<CategoryResponseDto> SeedCategories(int companyInfoId);
    }
}
