using AutoMapper;
using BookManagementSystem.Domain.Entities;
using BookManagementSystem.Infrastructure;
using BookManagementSystem.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ILogger<User> _logger;
		public UnitOfWork(UserManager<User> userManager, IOptions<MailSettings> mailSettings,
			IHttpContextAccessor httpContextAccessor, IMapper mapper
			, IConfiguration configuration,
			ApplicationDbContext context,
			RoleManager<IdentityRole> roleManager,
			ILogger<User> logger)
		{
			_userManager = userManager;
			_mapper = mapper;
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
			_mailSettings = mailSettings;
			_context = context;
			_roleManager = roleManager;
			_logger = logger;
		}
		public TokenService tokenService => new TokenService(_configuration, _httpContextAccessor);

		public UserManagementService userManagementService =>
			new UserManagementService(_userManager, _mapper,
				tokenService, mailService, _context, _roleManager, _logger);

		public EmailManagerService mailService => new EmailManagerService(_mailSettings);

        public DbHelperService dbHelperService => new DbHelperService(_configuration);
    }
}
