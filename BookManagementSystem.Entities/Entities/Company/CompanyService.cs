using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookManagementSystem.Domain.Entities.Company
{
    public class CompanyService
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int CompanyInfoId { get; set; }
        [JsonIgnore]
        public CompanyDetail CompanyInfo { get; set; }
    }
}
