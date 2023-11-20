using System.ComponentModel.DataAnnotations;

namespace BookManagementSystem.Domain.Entities
{
	public class ThirdPartyAuth
	{
		[Key]
		public Guid Id { get; set; }
		public string UserId { get; set; }
		public string Username { get; set; }
		public string UserEmail { get; set; }
		public string Provider { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime UpdateDate { get; set; }
	}
}
