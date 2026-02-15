using BookManagementSystem.Domain.Entities;
using BookManagementSystem.Domain.Entities.Company;

namespace BookManagementSystem.Domain.DTO
{
    public class CompanyDetailResponseDto : Common
    {
        public CompanyDetailResponseDto()
        {
            CompanyDetailRes = new CompanyDetailRes();
        }
        public CompanyDetailRes CompanyDetailRes { get; set; }
    }
    public class CompanyDetailRes
    {
        public CompanyDetailRes()
        {
            CompanyDetail =new CompanyDetail();
            CompanyServices = new List<CompanyService>();
            CompanySocialInfos = new List<CompanySocialInfo>();
        }
        public CompanyDetail CompanyDetail { get; set; }
        public List<CompanyService> CompanyServices { get; set; }
        public List<CompanySocialInfo> CompanySocialInfos { get; set; }
    }
}
