using System.Text.Json.Serialization;
using BookManagementSystem.Domain.Entities;

namespace BookManagementSystem.Domain.DTO
{
    public class AttachmentDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("collection_name")]
        public string CollectionName { get; set; } = "product";

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        [JsonPropertyName("mime_type")]
        public string MimeType { get; set; } = "image/jpeg";

        [JsonPropertyName("disk")]
        public string Disk { get; set; } = "public";

        [JsonPropertyName("conversions_disk")]
        public string ConversionsDisk { get; set; } = "public";

        [JsonPropertyName("size")]
        public string Size { get; set; } = "0";

        [JsonPropertyName("original_url")]
        public string OriginalUrl { get; set; }

        [JsonPropertyName("created_by_id")]
        public int CreatedById { get; set; } = 1;

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }

        public static AttachmentDto FromUrl(string url, int id = 0, string name = "product-image")
        {
            if (string.IsNullOrEmpty(url)) return null;
            var now = DateTime.UtcNow.ToString("o");
            return new AttachmentDto
            {
                Id = id,
                Name = name,
                FileName = name + ".jpg",
                OriginalUrl = url,
                CreatedAt = now,
                UpdatedAt = now
            };
        }
    }

    public class ProductResponseDto : Common
    {
        [JsonPropertyName("data")]
        public List<ProductDetailDto> Products { get; set; } = new List<ProductDetailDto>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class ProductDetailDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("short_description")]
        public string ShortDescription { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("sale_price")]
        public decimal? SalePrice { get; set; }

        [JsonPropertyName("sku")]
        public string SKU { get; set; }

        [JsonPropertyName("stock_status")]
        public string StockStatus { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("status")]
        public bool IsActive { get; set; }

        [JsonPropertyName("is_featured")]
        public bool IsFeatured { get; set; }

        [JsonPropertyName("product_thumbnail_id")]
        public int? ProductThumbnailId { get; set; }

        [JsonPropertyName("product_thumbnail")]
        public AttachmentDto ProductThumbnail { get; set; }

        [JsonPropertyName("product_galleries")]
        public List<AttachmentDto> ProductGalleries { get; set; } = new List<AttachmentDto>();

        [JsonPropertyName("categories")]
        public List<ProductCategoryDto> Categories { get; set; } = new List<ProductCategoryDto>();

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }
    }

    public class ProductCategoryDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }
    }

    public class SingleProductResponseDto : Common
    {
        public ProductDetailDto Product { get; set; }
    }

    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string SKU { get; set; }
        public string ImageUrl { get; set; }
        public string StockStatus { get; set; } = "in_stock";
        public int Quantity { get; set; }
        public bool IsFeatured { get; set; }
        public int CategoryId { get; set; }
        public int CompanyInfoId { get; set; }
    }

    public class UpdateProductRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string SKU { get; set; }
        public string ImageUrl { get; set; }
        public string StockStatus { get; set; }
        public int Quantity { get; set; }
        public bool IsFeatured { get; set; }
        public int CategoryId { get; set; }
    }

    public class CategoryResponseDto : Common
    {
        [JsonPropertyName("data")]
        public List<CategoryDetailDto> Categories { get; set; } = new List<CategoryDetailDto>();

        [JsonPropertyName("total")]
        public int Total => Categories?.Count ?? 0;
    }

    public class CategoryDetailDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("parent_id")]
        public int? ParentId { get; set; }

        [JsonPropertyName("status")]
        public bool IsActive { get; set; }

        [JsonPropertyName("subcategories")]
        public List<CategoryDetailDto> SubCategories { get; set; } = new List<CategoryDetailDto>();

        [JsonPropertyName("products_count")]
        public int ProductsCount { get; set; }
    }
}
