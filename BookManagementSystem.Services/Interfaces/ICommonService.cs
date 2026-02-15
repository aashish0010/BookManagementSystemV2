using BookManagementSystem.Domain.DTO;
using BookManagementSystem.Domain.Entities.Company;

namespace BookManagementSystem.Service.Interfaces
{
    public interface ICommonService
    {
        Task<CompanyDetailResponseDto> FindCompanyByCompanyCode(string companyCode);
    }
}
