using BookManagementSystem.Infrastructure;
using BookManagementSystem.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookManagementSystem.Service.Services
{
	public class TokenService : ITokenService
	{
		private readonly IConfiguration _configuration;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public TokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		{
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
		}

		public string UserName => _httpContextAccessor.HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Name);

		public string Email => _httpContextAccessor.HttpContext.User.FindFirstValue(JwtRegisteredClaimNames.Email);
		public string Role => _httpContextAccessor.HttpContext.User.FindFirstValue("Role");

		public string TokenGenerate(User user, string role)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			var claims = new[] {
				new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Name, user.UserName),
				new Claim("Role", role),
				new Claim("DateOfJoining", DateTime.UtcNow.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};
			var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
			  _configuration["Jwt:Issuer"],
			  claims,
			  expires: DateTime.Now.AddMinutes(60),
			  signingCredentials: credentials);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
