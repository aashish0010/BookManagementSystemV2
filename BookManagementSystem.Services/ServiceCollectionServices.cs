using BookManagementSystem.Service.Interfaces;
using BookManagementSystem.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookManagementSystem.Service
{
	public static class ServiceCollectionServices
	{
		public static IServiceCollection ServiceServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IUnitOfWork, UnitOfWork>();
			services.AddHttpContextAccessor();
			return services;
		}
	}
}
