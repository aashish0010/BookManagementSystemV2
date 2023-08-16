using BookManagementSystem.Domain.DTO;
using BookManagementSystem.Domain.Entities;
using BookManagementSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookManagementSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest register)
        {
            var result = await _unitOfWork.userManagementService.Register(register);
            if (result.Status == Level.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            var result = await _unitOfWork.userManagementService.Login(login);
            if (result.Status == Level.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("getecho")]
        public IActionResult Test()
        {

            return Ok("I am alive");
        }
        [HttpGet("sendopt")]
        public async Task<IActionResult> SendOpt(string email)
        {
            var result = await _unitOfWork.userManagementService.OtpManager(email);
            if (result.Status == Level.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("verifyopt")]
        public async Task<IActionResult> VerifyOpt(OtpVerifyRequest request)
        {

            return Ok();
        }
    }
}
