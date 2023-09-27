using BookManagementSystem.Infrastructure;
using BookManagementSystem.Service.Services;

namespace BookManagementSystem.Service.Interfaces
{
	public interface IUnitOfWork
	{
		UserManagementService userManagementService { get; }
		TokenService tokenService { get; }
		EmailManagerService mailService { get; }
        DbHelperService dbHelperService { get; }
    }
}
