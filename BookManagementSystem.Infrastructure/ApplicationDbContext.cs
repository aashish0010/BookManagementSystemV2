using BookManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookManagementSystem.Infrastructure
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
		}

		public DbSet<OtpHandler> OtpManager { get; set; }
		public DbSet<ThirdPartyAuth> ThirdPartyLoginHandler { get; set; }


		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

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

			builder.Entity<ThirdPartyAuth>().Property(x => x.Username)
			   .HasMaxLength(20);
			builder.Entity<ThirdPartyAuth>().Property(x => x.UserId)
			   .HasMaxLength(100);
			builder.Entity<ThirdPartyAuth>().Property(x => x.Provider)
			   .HasMaxLength(100);
			builder.Entity<ThirdPartyAuth>().Property(x => x.UserEmail)
			   .HasMaxLength(100);


			builder.Entity<OtpHandler>().Property(x => x.IsVerify)
			   .HasMaxLength(1);
			builder.Entity<OtpHandler>().Property(x => x.Email)
			   .HasMaxLength(50);
			builder.Entity<OtpHandler>().Property(x => x.Otp)
				.HasMaxLength(50);
			builder.Entity<OtpHandler>().HasKey(x => x.Id);
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
	}
}
