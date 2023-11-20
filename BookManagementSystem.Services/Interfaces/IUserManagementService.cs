using BookManagementSystem.Domain.DTO;
using BookManagementSystem.Domain.Entities;

namespace BookManagementSystem.Service.Interfaces
{
	public interface IUserManagementService
	{
		Task<Common> Register(RegisterRequest register);
		Task<LoginResponse> Login(LoginRequest login);
		Task<OptResponse> OtpManager(string email);
		Task<Common> UpdatePassword(OptValidateRequest validateRequest);
		Task<Common> ResitPassword(ChangePassword changePassword);
		Task<LoginResponse> ThirdPartyUserManager(ThirdPartyAuth thirdParty);
	}
}
