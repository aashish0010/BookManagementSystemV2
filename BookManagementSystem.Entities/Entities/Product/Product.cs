using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BookManagementSystem.Domain.Entities.Company;

namespace BookManagementSystem.Domain.Entities.Product
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string SKU { get; set; }
        public string ImageUrl { get; set; }
        public string StockStatus { get; set; } = "in_stock";
        public int Quantity { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; }
        public int CategoryId { get; set; }
        [JsonIgnore]
        public Category Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CompanyInfoId { get; set; }
        [JsonIgnore]
        public CompanyDetail CompanyInfo { get; set; }
    }
}
