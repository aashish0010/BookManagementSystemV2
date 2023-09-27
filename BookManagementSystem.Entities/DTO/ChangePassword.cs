using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookManagementSystem.Domain.DTO
{
    public class ChangePassword
    {
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string Otp { get; set; }
        public string ProcessId { get; set; }
    }
}
