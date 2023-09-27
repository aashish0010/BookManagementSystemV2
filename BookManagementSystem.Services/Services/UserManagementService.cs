using AutoMapper;
using BookManagementSystem.Domain.DTO;
using BookManagementSystem.Domain.Entities;
using BookManagementSystem.Infrastructure;
using BookManagementSystem.Service.Function;
using BookManagementSystem.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Data;

namespace BookManagementSystem.Service.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _token;
        private readonly IEmailManagerService _emailManagerService;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _identityRole;
        private readonly ILogger<User> _logger;
        public UserManagementService(UserManager<User> userManager,
            IMapper mapper, ITokenService token,
            IEmailManagerService emailManagerService,
            ApplicationDbContext context,
            RoleManager<IdentityRole> identityRole,
            ILogger<User> logger
            )
        {
            _userManager = userManager;
            _mapper = mapper;
            _token = token;
            _emailManagerService = emailManagerService;
            _context = context;
            _identityRole = identityRole;
            _logger = logger;
        }
        public async Task<LoginResponse> Login(LoginRequest login)
        {
            LoginResponse response = new LoginResponse();
            User user = await _userManager.FindByNameAsync(login.UserName);
            if (user == null)
            {
                _logger.LogInformation("Unable to Find User");
                response.Code = StatusCodes.Status404NotFound;
                response.Status = Level.Failed;
                response.Message = "Unable to Find User";
            }
            else if (await _userManager.CheckPasswordAsync(user, login.Password))
            {
                var role = await _userManager.GetRolesAsync(user);
                response.Code = StatusCodes.Status200OK;
                response.Status = Level.Success;
                response.Message = "Login SuccessFully";
                response.UserName = login.UserName;
                response.Token = _token.TokenGenerate(user, role.FirstOrDefault());

            }
            else
            {
                response.Code = StatusCodes.Status401Unauthorized;
                response.Status = Level.Failed;
                response.Message = "Password Not Match";
            }
            return response;
        }

        public async Task<OptResponse> OtpManager(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new OptResponse()
                {

                    Message = email + " doesnot exist..!!",
                    Code = StatusCodes.Status404NotFound

                };
            }
            MailRequest mail = new MailRequest();
            Random random = new Random(10);
            mail.Subject = "Forget Password";
            string otp = Convert.ToString(random.Next());
            mail.Body = Helper.EmailHelper("Book Management System",
                otp, user.UserName);

            Random ran = new Random();

            mail.ToEmail = email;
            string processid = ran.Next(100000, 10000000).ToString();
            OtpHandler handler = new OtpHandler()
            {
                ProcessId = processid,
                Email = email,
                Otp = otp,
                CreateDate = DateTime.UtcNow,
                IsVerify = "p"
            };

            await _context.OtpManager.AddAsync(handler);
            int issave = await _context.SaveChangesAsync();
            if (issave > 0)
            {
                await _emailManagerService.SendEmail(mail);
                return new OptResponse()
                {
                    ProcessId = processid,
                    Email = email,
                    Message = "Opt Obtained Successfully",
                    Code = StatusCodes.Status200OK

                };
            }
            else
            {
                return new OptResponse()
                {
                    Message = "Opt Obtained Fail",
                    Code = StatusCodes.Status400BadRequest

                };
            }


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
                var userrole = await _identityRole.FindByNameAsync("user");
                if (userrole == null)
                {

                    await _identityRole.CreateAsync(new IdentityRole
                    {
                        Name = "user",
                        ConcurrencyStamp = DateTime.Now.Millisecond.ToString(),
                        NormalizedName = "user".ToUpper(),
                    });

                }
                await _userManager.AddToRoleAsync(user, "user");
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

        public async Task<Common> ResitPassword(ChangePassword changePassword)
        {
            var isemailexits = await _userManager.FindByEmailAsync(changePassword.Email);
            if (isemailexits != null)
            {
                var isvalid = _context.OtpManager.Where(x => x.Email.Equals(isemailexits.Email)
                && x.IsVerify.Equals("P") && x.CreateDate.AddMinutes(20) > DateTime.UtcNow
                && x.ProcessId.Equals(changePassword.ProcessId)
                );
                if (isvalid != null)
                {

                    var ispasswordchange = await _userManager.ChangePasswordAsync(isemailexits, changePassword.NewPassword, changePassword.OldPassword);
                    if (ispasswordchange.Succeeded)
                    {
                        return new Common
                        {
                            Code = StatusCodes.Status200OK,
                            Status = Level.Success,
                            Message = "Password Reset Successfully"
                        };
                    }
                    else
                    {
                        return new Common
                        {
                            Code = StatusCodes.Status400BadRequest,
                            Status = Level.Failed,
                            Message = "Password Not Matched"
                        };
                    }


                }
                else
                {
                    return new Common
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = Level.Failed,
                        Message = "Opt Doesnot Match"

                    };
                }
            }
            else
            {
                return new Common
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = Level.Failed,
                    Message = "Unable to find email"
                };
            }
        }


        public async Task<Common> UpdatePassword(OptValidateRequest validateRequest)
        {
            var isemailexits = await _userManager.FindByEmailAsync(validateRequest.Email);
            if (isemailexits != null)
            {
                var isvalid = _context.OtpManager.Where(x => x.Email.Equals(isemailexits.Email)
                && x.IsVerify.Equals("P") && x.CreateDate.AddMinutes(20) > DateTime.UtcNow
                && x.ProcessId.Equals(validateRequest.ProcessId)
                );
                if (isvalid != null)
                {
                    var code = await _userManager.GeneratePasswordResetTokenAsync(isemailexits);
                    var ispasswordchange = await _userManager.ResetPasswordAsync(isemailexits, code, validateRequest.NewPassword);
                    if (ispasswordchange.Succeeded)
                    {
                        return new Common
                        {
                            Code = StatusCodes.Status200OK,
                            Status = Level.Success,
                            Message = "Password Reset Successfully"
                        };
                    }
                    else
                    {
                        return new Common
                        {
                            Code = StatusCodes.Status400BadRequest,
                            Status = Level.Failed,
                            Message = "Password Reset Failed"
                        };
                    }


                }
                else
                {
                    return new Common
                    {
                        Code = StatusCodes.Status404NotFound,
                        Status = Level.Failed,
                        Message = "Opt Doesnot Match"

                    };
                }
            }
            else
            {
                return new Common
                {
                    Code = StatusCodes.Status400BadRequest,
                    Status = Level.Failed,
                    Message = "Unable to find email"
                };
            }
        }

    }
}
