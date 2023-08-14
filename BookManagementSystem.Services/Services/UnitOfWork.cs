using AutoMapper;
using BookManagementSystem.Infrastructure;
using BookManagementSystem.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace BookManagementSystem.Service.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UnitOfWork(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, IConfiguration configuration)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public TokenService tokenService => new TokenService(_configuration, _httpContextAccessor);

        public UserManagementService userManagementService => new UserManagementService(_userManager, _mapper, tokenService);

    }
}
