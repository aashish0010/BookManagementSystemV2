using AutoMapper;
using BookManagementSystem.Domain.Entities;
using BookManagementSystem.Infrastructure;
using BookManagementSystem.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BookManagementSystem.Service.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<MailSettings> _mailSettings;
        private readonly ApplicationDbContext _context;
        public UnitOfWork(UserManager<User> userManager, IOptions<MailSettings> mailSettings,
            IHttpContextAccessor httpContextAccessor, IMapper mapper
            , IConfiguration configuration, ApplicationDbContext context)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _mailSettings = mailSettings;
            _context = context;
        }
        public TokenService tokenService => new TokenService(_configuration, _httpContextAccessor);

        public UserManagementService userManagementService => new UserManagementService(_userManager, _mapper, tokenService, mailService, _context);

        public EmailManagerService mailService => new EmailManagerService(_mailSettings);
    }
}
