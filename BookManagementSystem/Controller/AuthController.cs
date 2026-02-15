using BookManagementSystem.Domain.DTO;
using BookManagementSystem.Domain.Entities;
using BookManagementSystem.Service.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Profiling;
using System.Security.Claims;

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
            using (MiniProfiler.Current.Step("Test2"))
            {
                var result = await _unitOfWork.userManagementService.Register(register);
                if (result.Status == Level.Success)
                    return Ok(result);

                return BadRequest(result);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            var result = await _unitOfWork.userManagementService.Login(login);
            if (result.Status == Level.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("getecho")]
        public IActionResult Test()
        {
            return Ok(new
            {
                Code = 200,
                Message = "I am alive"
            });
        }

        [HttpGet("sendopt")]
        public async Task<IActionResult> SendOpt(string email)
        {
            var result = await _unitOfWork.userManagementService.OtpManager(email);
            if (result.Status == Level.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("verifyopt")]
        public async Task<IActionResult> VerifyOpt(OptValidateRequest request)
        {
            var result = await _unitOfWork.userManagementService.UpdatePassword(request);
            if (result.Status == Level.Success)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePassword password)
        {
            var result = await _unitOfWork.userManagementService.ResitPassword(password);
            if (result.Status == Level.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("/thirdparty-login")]
        public IActionResult ThirdPartyLogin(string provider, string companyCode)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = provider == "Facebook"
                    ? Url.Action(nameof(FacebookCallback))
                    : Url.Action(nameof(GoogleCallback))
            };

            properties.Items.Add("CompanyCode", companyCode);

            string scheme = provider == "Facebook"
                ? FacebookDefaults.AuthenticationScheme
                : GoogleDefaults.AuthenticationScheme;

            return Challenge(properties, scheme);
        }
        [HttpGet("/google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {

            ThirdPartyAuth auth = new ThirdPartyAuth();
            var result = await HttpContext.AuthenticateAsync("Google");
            if (!result.Succeeded)
            {
                return Unauthorized();
            }
            var companyCode = result.Properties.Items.ContainsKey("CompanyCode")
                ? result.Properties.Items["CompanyCode"]
                : null;

            var companyDetail = await _unitOfWork.commonService.FindCompanyByCompanyCode(companyCode);

            auth.CompanyInfo = companyDetail.CompanyDetailRes.CompanyDetail;
            auth.Username = result.Principal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            auth.CreateDate = DateTime.UtcNow;
            auth.UserId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            auth.UserEmail = result.Principal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            auth.Provider = "Google";
            return Ok(await _unitOfWork.userManagementService.ThirdPartyUserManager(auth));
        }

        [HttpGet("/facebook-callback")]
        public async Task<IActionResult> FacebookCallback()
        {
            ThirdPartyAuth auth = new ThirdPartyAuth();

            var result = await HttpContext.AuthenticateAsync("Facebook");
            if (!result.Succeeded)
            {
                return Unauthorized();
            }
            var companyCode = result.Properties.Items.ContainsKey("CompanyCode")
               ? result.Properties.Items["CompanyCode"]
               : null;

            var companyDetail = await _unitOfWork.commonService.FindCompanyByCompanyCode(companyCode);

            auth.CompanyInfo = companyDetail.CompanyDetailRes.CompanyDetail;

            auth.Username = result.Principal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            auth.CreateDate = DateTime.UtcNow;
            auth.UserId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            auth.Provider = "Facebook";
            auth.UserEmail = auth.Username;
            return Ok(await _unitOfWork.userManagementService.ThirdPartyUserManager(auth));
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit()
        {
            return Ok();
        }

        [HttpGet("companydetails")]
        public async Task<IActionResult> GetCompanyInfo(string companyCode)
        {
            return Ok(await _unitOfWork.commonService.FindCompanyByCompanyCode(companyCode));
        }
    }
}