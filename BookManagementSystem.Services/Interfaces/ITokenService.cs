using BookManagementSystem.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace BookManagementSystem.Service.Interfaces
{
	public interface ITokenService
	{
		string TokenGenerate(User user, IdentityRole role);

		string UserName { get; }
		string Email { get; }

	}
}
