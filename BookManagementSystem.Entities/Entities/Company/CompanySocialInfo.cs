using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BookManagementSystem.Domain.Entities.Company
{
    public class CompanySocialInfo
    {
        [Key]
        public int Id { get; set; }
        public string SocialMediaName { get; set; }
        public string SocialMediaLink { get; set; }
        public string SocialMediaDesc { get; set; }
        public int CompanyInfoId { get; set; }
        [JsonIgnore]
        public CompanyDetail CompanyInfo { get; set; }
    }
}
