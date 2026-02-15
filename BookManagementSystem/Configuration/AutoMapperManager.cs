using AutoMapper;
using BookManagementSystem.Domain.DTO;
using BookManagementSystem.Domain.Entities.Product;
using BookManagementSystem.Infrastructure;

namespace BookManagementSystem.Configuration
{
	public class AutoMapperManager : Profile
	{
		public AutoMapperManager()
		{
			CreateMap<RegisterRequest, User>();
			CreateMap<CreateProductRequest, Product>()
				.ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Name.ToLower().Replace(" ", "-").Replace("'", "").Replace("\"", "")))
				.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));
			CreateMap<Product, ProductDetailDto>()
				.ForMember(dest => dest.Categories, opt => opt.MapFrom(src =>
					src.Category != null
						? new List<ProductCategoryDto> { new ProductCategoryDto { Id = src.Category.Id, Name = src.Category.Name, Slug = src.Category.Slug } }
						: new List<ProductCategoryDto>()));
		}
	}
}
