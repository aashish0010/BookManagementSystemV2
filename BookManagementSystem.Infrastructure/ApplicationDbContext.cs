using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookManagementSystem.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


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
