using BookManagementSystem.Infrastructure;

namespace BookManagementSystem.Service.Interfaces
{
    public interface ITokenService
    {
        string TokenGenerate(User user);

        string UserName { get; }
        string Email { get; }

    }
}
