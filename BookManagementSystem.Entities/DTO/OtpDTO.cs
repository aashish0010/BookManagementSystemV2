using BookManagementSystem.Domain.Entities;

namespace BookManagementSystem.Domain.DTO
{
    public class OptResponse : Common
    {
        public string Email { get; set; }
    }
    public class OtpVerifyRequest
    {
        public string Email { get; set; }

        public string Otp { get; set; }

    }
}
