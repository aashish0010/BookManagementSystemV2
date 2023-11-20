using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagementSystem.Infrastructure
{
	public static class InfrastructureCollectionServices
	{
		public static IServiceCollection InfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
					options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
			services.AddIdentityCore<User>().AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);

			return services;
		}
	}
}
