namespace BookManagementSystem.Domain.Entities
{

	public class OtpHandler
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Otp { get; set; }
		public string Email { get; set; }
		public string ProcessId { get; set; }
		public string IsVerify { get; set; }

		public DateTime ConfirmDate { get; set; }
		public DateTime CreateDate { get; set; }
	}

}
