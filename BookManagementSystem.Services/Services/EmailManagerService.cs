using BookManagementSystem.Domain.Entities;
using BookManagementSystem.Service.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace BookManagementSystem.Service.Services
{
    public class EmailManagerService : IEmailManagerService
    {
        private readonly MailSettings _mailsetting;

        public EmailManagerService(IOptions<MailSettings> mail)
        {
            _mailsetting = mail.Value;
        }
        public async Task SendEmail(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailsetting.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailsetting.Host, _mailsetting.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailsetting.Mail, _mailsetting.Password);
            var emil = await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
