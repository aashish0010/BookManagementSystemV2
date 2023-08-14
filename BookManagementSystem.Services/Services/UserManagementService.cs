using AutoMapper;
using BookManagementSystem.Domain.DTO;
using BookManagementSystem.Domain.Entities;
using BookManagementSystem.Infrastructure;
using BookManagementSystem.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace BookManagementSystem.Service.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _token;
        public UserManagementService(UserManager<User> userManager, IMapper mapper, ITokenService token)
        {
            _userManager = userManager;
            _mapper = mapper;
            _token = token;
        }
        public async Task<LoginResponse> Login(LoginRequest login)
        {
            LoginResponse response = new LoginResponse();
            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user == null)
            {
                response.Code = StatusCodes.Status404NotFound;
                response.Status = Level.Failed;
                response.Message = "Unable to Find User";
            }
            else if (await _userManager.CheckPasswordAsync(user, login.Password))
            {
                response.Code = StatusCodes.Status200OK;
                response.Status = Level.Success;
                response.Message = "Login SuccessFully";
                response.UserName = login.UserName;
                response.Token = _token.TokenGenerate(user);
            }
            else
            {
                response.Code = StatusCodes.Status401Unauthorized;
                response.Status = Level.Failed;
                response.Message = "Password Not Match";
            }
            return response;
        }

        public async Task<Common> Register(RegisterRequest register)
        {
            var ifuserexits = await _userManager.FindByNameAsync(register.UserName);
            if (ifuserexits != null)
            {
                return new Common()
                {
                    Code = StatusCodes.Status201Created,
                    Status = Level.Failed,
                    Message = "User Already Exists"
                };
            }
            User user = new User();
            user = _mapper.Map(register, user);
            var isregister = await _userManager.CreateAsync(user, register.Password);
            if (isregister.Succeeded)
            {
                return new Common()
                {
                    Code = StatusCodes.Status100Continue,
                    Status = Level.Success,
                    Message = "Register Succesfully"
                };
            }
            else
            {
                return new Common()
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = Level.Failed,
                    Message = isregister.Errors.FirstOrDefault().Description.ToString()
                };
            }
        }
    }
}
