using BookManagementSystem.Domain.Entities;

namespace BookManagementSystem.Service.Interfaces
{
    public interface IEmailManagerService
    {
        Task SendEmail(MailRequest mailRequest);
    }
}
