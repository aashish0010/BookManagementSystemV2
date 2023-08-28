using BookManagementSystem.Domain.Entities;

namespace BookManagementSystem.Domain.DTO
{
	public class OptResponse : Common
	{
		public string Email { get; set; }
		public string ProcessId { get; set; }
	}
	public class OptValidateRequest
	{
		public string Email { get; set; }
		public string Otp { get; set; }
		public string ProcessId { get; set; }
		public string NewPassword { get; set; }
	}
}
