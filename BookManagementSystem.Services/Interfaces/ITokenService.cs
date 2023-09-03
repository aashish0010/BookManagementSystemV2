using BookManagementSystem.Infrastructure;

namespace BookManagementSystem.Service.Interfaces
{
	public interface ITokenService
	{
		string TokenGenerate(User user, string role);

		string UserName { get; }
		string Email { get; }
		string Role { get; }

	}
}
