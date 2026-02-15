using BookManagementSystem.Domain.DTO;
using BookManagementSystem.Domain.Entities;
using BookManagementSystem.Domain.Entities.Company;
using BookManagementSystem.Domain.Entities.Product;
using BookManagementSystem.Infrastructure;
using BookManagementSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookManagementSystem.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        private static ProductDetailDto MapToDto(Product p)
        {
            var thumbnail = AttachmentDto.FromUrl(p.ImageUrl, p.Id, p.Slug ?? "product");
            return new ProductDetailDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                ShortDescription = p.ShortDescription,
                Description = p.Description,
                Price = p.Price,
                SalePrice = p.SalePrice,
                SKU = p.SKU,
                StockStatus = p.StockStatus,
                Quantity = p.Quantity,
                IsActive = p.IsActive,
                IsFeatured = p.IsFeatured,
                ProductThumbnailId = thumbnail != null ? p.Id : null,
                ProductThumbnail = thumbnail,
                ProductGalleries = thumbnail != null ? new List<AttachmentDto> { thumbnail } : new List<AttachmentDto>(),
                Categories = p.Category != null
                    ? new List<ProductCategoryDto> { new ProductCategoryDto { Id = p.Category.Id, Name = p.Category.Name, Slug = p.Category.Slug } }
                    : new List<ProductCategoryDto>(),
                CreatedAt = p.CreatedAt.ToString("o"),
                UpdatedAt = p.UpdatedAt.ToString("o")
            };
        }

        public async Task<ProductResponseDto> GetProducts(int companyInfoId, int? categoryId, string search, int page, int pageSize)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.CompanyInfoId == companyInfoId);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
            }

            var total = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);

            var rawProducts = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var products = rawProducts.Select(MapToDto).ToList();

            return new ProductResponseDto
            {
                Status = Level.Success,
                Code = 200,
                Message = "Products retrieved successfully",
                Products = products,
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };
        }

        public async Task<SingleProductResponseDto> GetProductBySlug(string slug)
        {
            var raw = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Slug == slug && p.IsActive)
                .FirstOrDefaultAsync();

            if (raw == null)
            {
                return new SingleProductResponseDto
                {
                    Status = Level.Failed,
                    Code = 404,
                    Message = "Product not found"
                };
            }

            return new SingleProductResponseDto
            {
                Status = Level.Success,
                Code = 200,
                Message = "Product retrieved successfully",
                Product = MapToDto(raw)
            };
        }

        public async Task<CategoryResponseDto> GetCategories(int companyInfoId)
        {
            var categories = await _context.Categories
                .Where(c => c.IsActive && c.CompanyInfoId == companyInfoId && c.ParentId == null)
                .Include(c => c.SubCategories)
                .Include(c => c.Products)
                .Select(c => new CategoryDetailDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    Description = c.Description,
                    ImageUrl = c.ImageUrl,
                    ParentId = c.ParentId,
                    IsActive = c.IsActive,
                    ProductsCount = c.Products.Count,
                    SubCategories = c.SubCategories
                        .Where(sc => sc.IsActive)
                        .Select(sc => new CategoryDetailDto
                        {
                            Id = sc.Id,
                            Name = sc.Name,
                            Slug = sc.Slug,
                            Description = sc.Description,
                            ImageUrl = sc.ImageUrl,
                            ParentId = sc.ParentId,
                            IsActive = sc.IsActive,
                            ProductsCount = sc.Products.Count
                        }).ToList()
                })
                .ToListAsync();

            return new CategoryResponseDto
            {
                Status = Level.Success,
                Code = 200,
                Message = "Categories retrieved successfully",
                Categories = categories
            };
        }

        public async Task<SingleProductResponseDto> CreateProduct(CreateProductRequest request)
        {
            var slug = request.Name.ToLower().Replace(" ", "-").Replace("'", "").Replace("\"", "");

            var product = new Product
            {
                Name = request.Name,
                Slug = slug,
                ShortDescription = request.ShortDescription,
                Description = request.Description,
                Price = request.Price,
                SalePrice = request.SalePrice,
                SKU = request.SKU,
                ImageUrl = request.ImageUrl,
                StockStatus = request.StockStatus ?? "in_stock",
                Quantity = request.Quantity,
                IsFeatured = request.IsFeatured,
                CategoryId = request.CategoryId,
                CompanyInfoId = request.CompanyInfoId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Reload with category for MapToDto
            product.Category = await _context.Categories.FindAsync(product.CategoryId);

            return new SingleProductResponseDto
            {
                Status = Level.Success,
                Code = 201,
                Message = "Product created successfully",
                Product = MapToDto(product)
            };
        }

        public async Task<CategoryResponseDto> SeedCategories(int companyInfoId)
        {
            try
            {
                // Ensure the company exists; create a default one if not
                var company = await _context.CompanyDetails.FindAsync(companyInfoId);
                if (company == null)
                {
                    company = new CompanyDetail
                    {
                        CompanyName = "WOW Commerce",
                        CompanyDescription = "Default e-commerce store",
                        CompanyPhoneNumber = "000-000-0000",
                        CompanyEmail = "admin@wowcommerce.com",
                        OperationsDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                        CompanyCode = "Wo100"
                    };
                    _context.CompanyDetails.Add(company);
                    await _context.SaveChangesAsync();
                    companyInfoId = company.Id;
                }

                var existingCategories = await _context.Categories
               .Where(c => c.CompanyInfoId == companyInfoId)
               .AnyAsync();

                if (existingCategories)
                {
                    return new CategoryResponseDto
                    {
                        Status = Level.Failed,
                        Code = 400,
                        Message = "Categories already exist for this company"
                    };
                }

                var now = DateTime.UtcNow;

                // ── Top-level categories ──
                var fashion = new Category { Name = "Fashion", Slug = "fashion", Description = "Trendy clothing, accessories, and seasonal styles", IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now };
                var shoes = new Category { Name = "Shoes", Slug = "shoes", Description = "Footwear for every occasion", IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now };
                var watch = new Category { Name = "Watch", Slug = "watch", Description = "Watches, smartwatches, and accessories", IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now };
                var men = new Category { Name = "Men", Slug = "men", Description = "Men's clothing and accessories", IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now };
                var female = new Category { Name = "Female", Slug = "female", Description = "Women's clothing and accessories", IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now };

                _context.Categories.AddRange(fashion, shoes, watch, men, female);
                await _context.SaveChangesAsync();

                // ── Subcategories ──
                var subCategories = new List<Category>
            {
                new Category { Name = "T-Shirts",        Slug = "t-shirts",        Description = "Casual and graphic tees",         ParentId = fashion.Id, IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Category { Name = "Jackets",         Slug = "jackets",         Description = "Outerwear and jackets",            ParentId = fashion.Id, IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Category { Name = "Denim",           Slug = "denim",           Description = "Jeans and denim wear",             ParentId = fashion.Id, IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Category { Name = "Running",         Slug = "running-shoes",   Description = "Performance running shoes",        ParentId = shoes.Id,   IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Category { Name = "Sneakers",        Slug = "sneakers",        Description = "Casual and lifestyle sneakers",    ParentId = shoes.Id,   IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Category { Name = "Formal Shoes",    Slug = "formal-shoes",    Description = "Dress shoes and oxfords",          ParentId = shoes.Id,   IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Category { Name = "Smart Watches",   Slug = "smart-watches",   Description = "Digital and smart watches",        ParentId = watch.Id,   IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Category { Name = "Analog Watches",  Slug = "analog-watches",  Description = "Classic analog timepieces",        ParentId = watch.Id,   IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Category { Name = "Men's Shirts",    Slug = "mens-shirts",     Description = "Formal and casual shirts for men", ParentId = men.Id,     IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Category { Name = "Men's Trousers",  Slug = "mens-trousers",   Description = "Pants and trousers for men",       ParentId = men.Id,     IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Category { Name = "Dresses",         Slug = "dresses",         Description = "Casual and formal dresses",        ParentId = female.Id,  IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Category { Name = "Handbags",        Slug = "handbags",        Description = "Bags, purses, and clutches",       ParentId = female.Id,  IsActive = true, CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
            };
                _context.Categories.AddRange(subCategories);
                await _context.SaveChangesAsync();

                // ── Products (36 items across all categories) ──
                var products = new List<Product>
            {
                // ─── Fashion ───
                new Product { Name = "Classic White T-Shirt",       Slug = "classic-white-t-shirt",       ShortDescription = "Essential cotton crew-neck tee",           Description = "Made from 100% organic cotton, this classic white t-shirt features a relaxed fit and ribbed crew neck. Pre-shrunk fabric holds its shape wash after wash.",                            Price = 24.99m,  SalePrice = 19.99m,  SKU = "FSH-001", StockStatus = "in_stock",     Quantity = 200, IsFeatured = true,  CategoryId = fashion.Id,  CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Graphic Print Tee",           Slug = "graphic-print-tee",           ShortDescription = "Bold urban graphic t-shirt",               Description = "Stand out with this eye-catching graphic tee. Features a vintage-inspired print on soft ringspun cotton. Unisex regular fit.",                                                         Price = 34.99m,  SalePrice = 29.99m,  SKU = "FSH-002", StockStatus = "in_stock",     Quantity = 150, IsFeatured = false, CategoryId = fashion.Id,  CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Leather Biker Jacket",        Slug = "leather-biker-jacket",        ShortDescription = "Genuine leather moto jacket",              Description = "Timeless biker style in premium full-grain leather. Asymmetric zip closure, snap lapel collar, and quilted shoulder panels. Satin lined for comfort.",                               Price = 249.99m, SalePrice = 199.99m, SKU = "FSH-003", StockStatus = "in_stock",     Quantity = 30,  IsFeatured = true,  CategoryId = fashion.Id,  CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Puffer Down Jacket",          Slug = "puffer-down-jacket",          ShortDescription = "Warm lightweight puffer",                  Description = "700-fill duck down puffer jacket with water-resistant shell. Packs into its own pocket for travel. Elastic cuffs and adjustable hem keep warmth in.",                                  Price = 179.99m, SalePrice = 149.99m, SKU = "FSH-004", StockStatus = "in_stock",     Quantity = 45,  IsFeatured = true,  CategoryId = fashion.Id,  CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Slim Fit Denim Jeans",        Slug = "slim-fit-denim-jeans",        ShortDescription = "Modern slim stretch jeans",                Description = "These slim-fit jeans blend style and comfort with 2% elastane stretch denim. Medium wash with subtle whiskering. Five-pocket construction, zip fly.",                                  Price = 69.99m,  SalePrice = 59.99m,  SKU = "FSH-005", StockStatus = "in_stock",     Quantity = 120, IsFeatured = false, CategoryId = fashion.Id,  CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Oversized Hoodie",            Slug = "oversized-hoodie",            ShortDescription = "Cozy drop-shoulder hoodie",                Description = "Ultra-soft fleece-lined hoodie with an oversized drop-shoulder silhouette. Features a kangaroo pocket and drawstring hood. Perfect for layering.",                                     Price = 54.99m,  SalePrice = 44.99m,  SKU = "FSH-006", StockStatus = "in_stock",     Quantity = 90,  IsFeatured = true,  CategoryId = fashion.Id,  CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Linen Summer Shirt",          Slug = "linen-summer-shirt",          ShortDescription = "Breathable 100% linen shirt",              Description = "Stay cool in this lightweight linen shirt. Relaxed fit with a camp collar and coconut shell buttons. Garment-dyed for a lived-in feel.",                                              Price = 59.99m,  SKU = "FSH-007",                      StockStatus = "in_stock",     Quantity = 65,  IsFeatured = false, CategoryId = fashion.Id,  CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },

                // ─── Shoes ───
                new Product { Name = "UltraBoost Running Shoe",     Slug = "ultraboost-running-shoe",     ShortDescription = "Energy-returning performance runner",       Description = "Engineered mesh upper with responsive foam midsole returns energy with every stride. Continental rubber outsole grips on wet and dry surfaces. Lace closure with heel counter.",        Price = 139.99m, SalePrice = 119.99m, SKU = "SHO-001", StockStatus = "in_stock",     Quantity = 70,  IsFeatured = true,  CategoryId = shoes.Id,    CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Trail Runner Pro",            Slug = "trail-runner-pro",            ShortDescription = "All-terrain trail running shoe",            Description = "Aggressive lug pattern for grip on mud, rock, and gravel. Reinforced toe cap and gusseted tongue keep debris out. 8mm drop with rock plate protection.",                              Price = 129.99m, SalePrice = 109.99m, SKU = "SHO-002", StockStatus = "in_stock",     Quantity = 40,  IsFeatured = false, CategoryId = shoes.Id,    CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Classic Canvas Sneaker",      Slug = "classic-canvas-sneaker",      ShortDescription = "Timeless low-top canvas shoe",             Description = "The sneaker that never goes out of style. Durable canvas upper, vulcanized rubber sole, and padded collar. Available in 12 colors.",                                                  Price = 49.99m,  SalePrice = 39.99m,  SKU = "SHO-003", StockStatus = "in_stock",     Quantity = 200, IsFeatured = true,  CategoryId = shoes.Id,    CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "High-Top Leather Sneaker",    Slug = "high-top-leather-sneaker",    ShortDescription = "Premium leather high-top",                 Description = "Full-grain leather upper with perforated side panels. Cushioned footbed and rubber cupsole. Padded collar and tongue for ankle support.",                                              Price = 119.99m, SKU = "SHO-004",                      StockStatus = "in_stock",     Quantity = 55,  IsFeatured = false, CategoryId = shoes.Id,    CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Oxford Dress Shoe",           Slug = "oxford-dress-shoe",           ShortDescription = "Classic leather oxford",                   Description = "Hand-polished calfskin leather with closed lacing. Blake-stitched leather sole and full leather lining. A wardrobe staple for formal occasions.",                                      Price = 189.99m, SalePrice = 159.99m, SKU = "SHO-005", StockStatus = "in_stock",     Quantity = 35,  IsFeatured = true,  CategoryId = shoes.Id,    CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Suede Chelsea Boot",          Slug = "suede-chelsea-boot",          ShortDescription = "Versatile suede ankle boot",               Description = "Italian suede Chelsea boot with elastic side panels and pull tab. Leather lined with a stacked leather heel. Goodyear welted for re-soleability.",                                    Price = 169.99m, SKU = "SHO-006",                      StockStatus = "in_stock",     Quantity = 25,  IsFeatured = false, CategoryId = shoes.Id,    CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Sport Slide Sandal",          Slug = "sport-slide-sandal",          ShortDescription = "Contoured footbed slide",                  Description = "Lightweight EVA slide with a contoured footbed that molds to your foot. Textured strap, great for post-workout recovery or poolside.",                                                Price = 29.99m,  SalePrice = 22.99m,  SKU = "SHO-007", StockStatus = "in_stock",     Quantity = 150, IsFeatured = false, CategoryId = shoes.Id,    CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },

                // ─── Watch ───
                new Product { Name = "Smart Watch Pro X",           Slug = "smart-watch-pro-x",           ShortDescription = "Advanced health and fitness smartwatch",    Description = "AMOLED always-on display, GPS, heart rate, SpO2, sleep tracking, and 5 ATM water resistance. 7-day battery life. Compatible with iOS and Android.",                                  Price = 299.99m, SalePrice = 249.99m, SKU = "WAT-001", StockStatus = "in_stock",     Quantity = 60,  IsFeatured = true,  CategoryId = watch.Id,    CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Fitness Band Lite",           Slug = "fitness-band-lite",           ShortDescription = "Slim fitness tracker",                     Description = "Track steps, distance, calories, and sleep with this lightweight band. OLED display shows notifications. USB direct charge, 14-day battery.",                                         Price = 49.99m,  SalePrice = 39.99m,  SKU = "WAT-002", StockStatus = "in_stock",     Quantity = 120, IsFeatured = false, CategoryId = watch.Id,    CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Chronograph Dive Watch",      Slug = "chronograph-dive-watch",      ShortDescription = "200m water-resistant diver",               Description = "Japanese quartz movement with date display. Unidirectional rotating bezel, luminous hands and markers. Stainless steel case with screw-down crown. 200m water resistance.",            Price = 219.99m, SalePrice = 189.99m, SKU = "WAT-003", StockStatus = "in_stock",     Quantity = 20,  IsFeatured = true,  CategoryId = watch.Id,    CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Minimalist Analog Watch",     Slug = "minimalist-analog-watch",     ShortDescription = "Clean dial dress watch",                   Description = "Swiss quartz movement in a slim 40mm case. Scratch-resistant sapphire crystal, genuine leather strap. Understated elegance for everyday wear.",                                       Price = 159.99m, SKU = "WAT-004",                      StockStatus = "in_stock",     Quantity = 45,  IsFeatured = false, CategoryId = watch.Id,    CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Digital Sports Watch",        Slug = "digital-sports-watch",        ShortDescription = "Rugged multi-function digital",            Description = "Shock-resistant with world time, stopwatch, countdown timer, and alarm. 100m water resistance, LED backlight, and resin band. Built for adventure.",                                  Price = 59.99m,  SalePrice = 49.99m,  SKU = "WAT-005", StockStatus = "in_stock",     Quantity = 85,  IsFeatured = false, CategoryId = watch.Id,    CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Rose Gold Bracelet Watch",    Slug = "rose-gold-bracelet-watch",    ShortDescription = "Elegant rose gold ladies watch",           Description = "Delicate rose gold-tone stainless steel bracelet with a mother-of-pearl dial. Swarovski crystal hour markers. Fold-over clasp with safety catch.",                                    Price = 179.99m, SalePrice = 149.99m, SKU = "WAT-006", StockStatus = "in_stock",     Quantity = 30,  IsFeatured = true,  CategoryId = watch.Id,    CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },

                // ─── Men ───
                new Product { Name = "Slim Fit Formal Shirt",       Slug = "slim-fit-formal-shirt",       ShortDescription = "Wrinkle-free dress shirt",                 Description = "Non-iron cotton-blend shirt with a spread collar and adjustable cuffs. Slim fit through chest and waist. Perfect under a blazer or on its own.",                                     Price = 59.99m,  SalePrice = 49.99m,  SKU = "MEN-001", StockStatus = "in_stock",     Quantity = 100, IsFeatured = true,  CategoryId = men.Id,      CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Chino Trousers",              Slug = "chino-trousers",              ShortDescription = "Versatile stretch chinos",                 Description = "Cotton twill chinos with 2% stretch for all-day comfort. Flat front, slash pockets, and welt back pockets. Works from office to weekend.",                                           Price = 49.99m,  SalePrice = 39.99m,  SKU = "MEN-002", StockStatus = "in_stock",     Quantity = 90,  IsFeatured = false, CategoryId = men.Id,      CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Wool Blend Blazer",           Slug = "wool-blend-blazer",           ShortDescription = "Two-button tailored blazer",               Description = "Italian wool-blend fabric with half canvas construction. Notch lapel, two-button closure, and four interior pockets. Tailored regular fit.",                                          Price = 199.99m, SalePrice = 169.99m, SKU = "MEN-003", StockStatus = "in_stock",     Quantity = 25,  IsFeatured = true,  CategoryId = men.Id,      CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Merino Wool Polo",            Slug = "merino-wool-polo",            ShortDescription = "Premium merino knit polo",                 Description = "Extra-fine merino wool polo shirt. Naturally temperature-regulating, moisture-wicking, and odor-resistant. Ribbed collar and cuffs.",                                                 Price = 79.99m,  SKU = "MEN-004",                      StockStatus = "in_stock",     Quantity = 55,  IsFeatured = false, CategoryId = men.Id,      CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Leather Belt",                Slug = "leather-belt",                ShortDescription = "Full-grain leather dress belt",            Description = "35mm full-grain Italian leather belt with a brushed nickel buckle. Feathered edge and five-hole adjustment. Comes in a branded gift box.",                                            Price = 44.99m,  SalePrice = 34.99m,  SKU = "MEN-005", StockStatus = "in_stock",     Quantity = 130, IsFeatured = false, CategoryId = men.Id,      CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Performance Joggers",         Slug = "performance-joggers",         ShortDescription = "Technical athletic joggers",               Description = "Four-way stretch woven joggers with zip pockets and reflective accents. Tapered leg with elastic cuff. DWR coating repels light rain.",                                               Price = 64.99m,  SalePrice = 54.99m,  SKU = "MEN-006", StockStatus = "in_stock",     Quantity = 75,  IsFeatured = false, CategoryId = men.Id,      CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Cashmere V-Neck Sweater",     Slug = "cashmere-v-neck-sweater",     ShortDescription = "Luxury cashmere knit",                     Description = "100% Grade-A Mongolian cashmere. Lightweight yet warm, with a classic V-neck silhouette. Ribbed hem and cuffs. Dry clean recommended.",                                               Price = 149.99m, SKU = "MEN-007",                      StockStatus = "in_stock",     Quantity = 20,  IsFeatured = true,  CategoryId = men.Id,      CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },

                // ─── Female ───
                new Product { Name = "Floral Midi Dress",           Slug = "floral-midi-dress",           ShortDescription = "Breezy floral print midi",                 Description = "Lightweight viscose midi dress with an all-over floral print. V-neckline, flutter sleeves, and self-tie waist. Fully lined. Perfect from brunch to evening.",                        Price = 79.99m,  SalePrice = 64.99m,  SKU = "FEM-001", StockStatus = "in_stock",     Quantity = 70,  IsFeatured = true,  CategoryId = female.Id,   CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Satin Wrap Blouse",           Slug = "satin-wrap-blouse",           ShortDescription = "Elegant satin wrap top",                   Description = "Luxe satin-finish wrap blouse with long sleeves and a self-tie closure. Draped front creates a flattering silhouette. Pairs beautifully with tailored trousers.",                     Price = 54.99m,  SalePrice = 44.99m,  SKU = "FEM-002", StockStatus = "in_stock",     Quantity = 60,  IsFeatured = false, CategoryId = female.Id,   CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "High-Waist Wide Leg Pants",   Slug = "high-waist-wide-leg-pants",   ShortDescription = "Flowing wide-leg trousers",                Description = "High-rise, wide-leg silhouette in crepe fabric. Front pleats, invisible side zip, and pressed crease give a polished look. Sits at the natural waist.",                               Price = 69.99m,  SalePrice = 59.99m,  SKU = "FEM-003", StockStatus = "in_stock",     Quantity = 50,  IsFeatured = false, CategoryId = female.Id,   CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Quilted Crossbody Bag",       Slug = "quilted-crossbody-bag",       ShortDescription = "Compact quilted leather bag",              Description = "Soft lambskin leather with diamond quilting. Adjustable chain strap converts from crossbody to shoulder. Interior zip pocket and card slot. Gold-tone hardware.",                      Price = 129.99m, SalePrice = 109.99m, SKU = "FEM-004", StockStatus = "in_stock",     Quantity = 35,  IsFeatured = true,  CategoryId = female.Id,   CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Knit Cardigan",               Slug = "knit-cardigan",               ShortDescription = "Soft open-front cardigan",                 Description = "Relaxed-fit cardigan in a soft cotton-wool blend. Open front with dropped shoulders, ribbed trim, and patch pockets. Great layering piece year-round.",                               Price = 64.99m,  SKU = "FEM-005",                      StockStatus = "in_stock",     Quantity = 80,  IsFeatured = false, CategoryId = female.Id,   CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Silk Scarf",                  Slug = "silk-scarf",                  ShortDescription = "Hand-rolled pure silk scarf",              Description = "100% mulberry silk, 90x90cm. Digitally printed abstract design with hand-rolled edges. Wear as a headscarf, neck tie, or bag accessory.",                                            Price = 89.99m,  SalePrice = 74.99m,  SKU = "FEM-006", StockStatus = "in_stock",     Quantity = 40,  IsFeatured = false, CategoryId = female.Id,   CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Structured Tote Bag",         Slug = "structured-tote-bag",         ShortDescription = "Premium leather work tote",                Description = "Full-grain leather tote with reinforced base and magnetic snap closure. Interior padded laptop sleeve fits up to 14\". Two interior pockets and one exterior zip.",                    Price = 189.99m, SalePrice = 159.99m, SKU = "FEM-007", StockStatus = "in_stock",     Quantity = 25,  IsFeatured = true,  CategoryId = female.Id,   CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
                new Product { Name = "Yoga Leggings",               Slug = "yoga-leggings",               ShortDescription = "High-waist compression leggings",          Description = "Squat-proof, four-way stretch fabric with moisture-wicking finish. High waistband with hidden pocket. Flat-lock seams prevent chafing. Great for yoga, gym, or errands.",              Price = 44.99m,  SalePrice = 34.99m,  SKU = "FEM-008", StockStatus = "in_stock",     Quantity = 110, IsFeatured = false, CategoryId = female.Id,   CompanyInfoId = companyInfoId, CreatedAt = now, UpdatedAt = now },
            };

                _context.Products.AddRange(products);
                await _context.SaveChangesAsync();

                return await GetCategories(companyInfoId);
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }
    }
}
