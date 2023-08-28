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


		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<User>().Property(x => x.PhoneNumber)
			   .HasMaxLength(20);
			builder.Entity<User>().Property(x => x.CreatedBy)
			   .HasMaxLength(100);
			builder.Entity<User>().Property(x => x.UpdatedBy)
			   .HasMaxLength(100);
			builder.Entity<User>().Property(x => x.IsActive)
			   .HasMaxLength(1);

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
		public DateTime Created { get; set; }
		public string CreatedBy { get; set; } = DateTime.UtcNow.ToString();
		public DateTime Updated { get; set; }
		public string UpdatedBy { get; set; }
		public string IsActive { get; set; }
		public DateTime LastLogin { get; set; }
	}
}
