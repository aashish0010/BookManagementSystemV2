using BookManagementSystem.Domain.Entities;
using BookManagementSystem.Domain.Entities.Company;
using BookManagementSystem.Domain.Entities.Product;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Product = BookManagementSystem.Domain.Entities.Product.Product;
using Category = BookManagementSystem.Domain.Entities.Product.Category;

namespace BookManagementSystem.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<OtpHandler> OtpManagers { get; set; }
        public DbSet<ThirdPartyAuth> ThirdPartyLoginHandlers { get; set; }

        public DbSet<CompanyDetail> CompanyDetails { get; set; }
        public DbSet<CompanyService> CompanyServices { get; set; }
        public DbSet<CompanySocialInfo> CompanySocialInfos { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Users

            builder.Entity<User>().Property(x => x.PhoneNumber)
               .HasMaxLength(20);
            //builder.Entity<User>().Property(x => x.CreatedBy)
            //   .HasMaxLength(100);
            builder.Entity<User>().Property(x => x.UpdatedBy)
               .HasMaxLength(100);
            builder.Entity<User>().Property(x => x.ThirdPartyId)
               .HasMaxLength(100);
            builder.Entity<User>().Property(x => x.FirstName)
               .HasMaxLength(100);
            builder.Entity<User>().Property(x => x.MiddleName)
               .HasMaxLength(100);
            builder.Entity<User>().Property(x => x.LastName)
               .HasMaxLength(100);
            builder.Entity<User>().Property(x => x.IsActive)
               .HasMaxLength(1);
            builder.Entity<User>().HasOne(x => x.CompanyInfo).WithMany()
             .HasForeignKey(x => x.CompanyInfoId)
             .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region

            builder.Entity<ThirdPartyAuth>().Property(x => x.Username)
               .HasMaxLength(20);
            builder.Entity<ThirdPartyAuth>().Property(x => x.UserId)
               .HasMaxLength(100);
            builder.Entity<ThirdPartyAuth>().Property(x => x.Provider)
               .HasMaxLength(100);
            builder.Entity<ThirdPartyAuth>().Property(x => x.UserEmail)
               .HasMaxLength(100);
            builder.Entity<ThirdPartyAuth>().HasOne(x => x.CompanyInfo).WithMany()
            .HasForeignKey(x => x.CompanyInfoId)
            .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region OTP Handler


            builder.Entity<OtpHandler>().Property(x => x.IsVerify)
               .HasMaxLength(1);
            builder.Entity<OtpHandler>().Property(x => x.Email)
               .HasMaxLength(50);
            builder.Entity<OtpHandler>().Property(x => x.Otp)
                .HasMaxLength(50);
            builder.Entity<OtpHandler>().HasKey(x => x.Id);

            #endregion

            #region Company Info

            builder.Entity<CompanyDetail>().HasKey(x => x.Id);
            builder.Entity<CompanyDetail>().Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<CompanyDetail>().Property(x => x.CompanyName)
                .HasMaxLength(200).IsRequired();
            builder.Entity<CompanyDetail>().Property(x => x.CompanyDescription)
                .HasMaxLength(500);

            builder.Entity<CompanyDetail>().Property(x => x.CompanyEmail)
                .HasMaxLength(200).IsRequired();

            builder.Entity<CompanyDetail>().Property(x => x.OperationsDate)
                .HasMaxLength(200);

            builder.Entity<CompanyDetail>().Property(x => x.CompanyPhoneNumber)
               .HasMaxLength(200).IsRequired();

            builder.Entity<CompanyDetail>().Property(x => x.CompanyCode)
                .HasMaxLength(50).IsRequired();

            #endregion

            #region Company Service
            builder.Entity<CompanyService>().HasKey(x => x.Id);
            builder.Entity<CompanyService>().Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Entity<CompanyService>().HasOne(x => x.CompanyInfo).WithMany()
                .HasForeignKey(x => x.CompanyInfoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CompanyService>().Property(x => x.Name).HasMaxLength(200).IsRequired();

            #endregion

            #region Company Social Info
            builder.Entity<CompanySocialInfo>().HasKey(x => x.Id);
            builder.Entity<CompanySocialInfo>().Property(x => x.SocialMediaName).HasMaxLength(200).IsRequired();
            builder.Entity<CompanySocialInfo>().Property(x => x.SocialMediaDesc).HasMaxLength(200);
            builder.Entity<CompanySocialInfo>().HasOne(x => x.CompanyInfo).WithMany()
                .HasForeignKey(x => x.CompanyInfoId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Category

            builder.Entity<Category>().HasKey(x => x.Id);
            builder.Entity<Category>().Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Entity<Category>().Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Entity<Category>().Property(x => x.Slug).HasMaxLength(200).IsRequired();
            builder.Entity<Category>().Property(x => x.Description).HasMaxLength(500);
            builder.Entity<Category>().Property(x => x.ImageUrl).HasMaxLength(500);
            builder.Entity<Category>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.SubCategories)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
            builder.Entity<Category>()
                .HasOne(x => x.CompanyInfo)
                .WithMany()
                .HasForeignKey(x => x.CompanyInfoId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Product

            builder.Entity<Product>().HasKey(x => x.Id);
            builder.Entity<Product>().Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Entity<Product>().Property(x => x.Name).HasMaxLength(300).IsRequired();
            builder.Entity<Product>().Property(x => x.Slug).HasMaxLength(300).IsRequired();
            builder.Entity<Product>().Property(x => x.ShortDescription).HasMaxLength(500);
            builder.Entity<Product>().Property(x => x.Description).HasMaxLength(5000);
            builder.Entity<Product>().Property(x => x.Price).HasColumnType("decimal(18,2)");
            builder.Entity<Product>().Property(x => x.SalePrice).HasColumnType("decimal(18,2)");
            builder.Entity<Product>().Property(x => x.SKU).HasMaxLength(100);
            builder.Entity<Product>().Property(x => x.ImageUrl).HasMaxLength(500);
            builder.Entity<Product>().Property(x => x.StockStatus).HasMaxLength(50);
            builder.Entity<Product>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Product>()
                .HasOne(x => x.CompanyInfo)
                .WithMany()
                .HasForeignKey(x => x.CompanyInfoId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion


        }
    }
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime Created { get; set; }
        public string ThirdPartyId { get; set; }
        //	public string CreatedBy { get; set; } = DateTime.UtcNow.ToString();
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public string IsActive { get; set; }
        public DateTime LastLogin { get; set; }

        public int CompanyInfoId { get; set; }
        public CompanyDetail CompanyInfo { get; set; }
    }
}
