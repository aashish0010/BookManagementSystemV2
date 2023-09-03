using BookManagementSystem.Domain.Entities;

namespace BookManagementSystem.Domain.DTO
{
	public class LoginRequest
	{
		public string UserName { get; set; }
		public string Password { get; set; }

	}
	public class RegisterRequest
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
	}
	public class LoginResponse : Common
	{
		public string UserName { get; set; }
		public string Token { get; set; }
	}
}
