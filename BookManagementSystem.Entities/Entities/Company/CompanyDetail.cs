using System.ComponentModel.DataAnnotations;

namespace BookManagementSystem.Domain.Entities.Company
{
    public class CompanyDetail
    {
        [Key]
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string CompanyDescription { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string CompanyEmail { get; set; }
        public string OperationsDate { get; set; }
        public string CompanyCode { get; set; }
    }
}
