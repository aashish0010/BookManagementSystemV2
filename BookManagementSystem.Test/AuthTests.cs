using AutoMapper;
using BookManagementSystem.Controller;
using BookManagementSystem.Domain.DTO;
using BookManagementSystem.Domain.Entities;
using BookManagementSystem.Infrastructure;
using BookManagementSystem.Service.Interfaces;
using BookManagementSystem.Service.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace BookManagementSystem.Test
{
	public class AuthTests
	{



		[Fact]
		public async Task Register_ReturnsCommonSuccess()
		{
			// var userManagerMock = new Mock<UserManager<User>>();

			var serviceProvider = new ServiceCollection()
	   .AddLogging()
	   .BuildServiceProvider();

			var userManagerMock = new Mock<UserManager<User>>(
		new Mock<IUserStore<User>>().Object,
		null,
			null,
			null,
			null,
		null,
		null,
		null,
		serviceProvider.GetService<ILogger<UserManager<User>>>());

			//var mapperMock = new Mock<IMapper>();

			var configuration = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<RegisterRequest, User>()
					.ForAllMembers(opt => opt.Ignore()); // Ignore unmapped properties
			});

			var _mapper = new Mapper(configuration);

			var tokenServiceMock = new Mock<ITokenService>();
			var emailManagerServiceMock = new Mock<IEmailManagerService>();


			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
						.UseInMemoryDatabase(databaseName: "InMemoryDatabase")
						.Options;


			var contextMock = new Mock<ApplicationDbContext>(options);


			//var identityRoleMock = new Mock<RoleManager<IdentityRole>>();


			var storeMock = new Mock<IRoleStore<IdentityRole>>();
			var logger = new Mock<ILogger<RoleManager<IdentityRole>>>().Object;

			var roleManager = new RoleManager<IdentityRole>(storeMock.Object, null, null, null, logger);
			// Arrange
			RegisterRequest reg = new RegisterRequest();
			reg.UserName = "test";
			reg.Email = "test@gmail.com";
			reg.PhoneNumber = "12345";
			reg.Password = "12132414aa";
			Common common = new Common();


			// Act
			var unitofwork = new Mock<IUnitOfWork>();
			var registerrepo = new Mock<IUserManagementService>();

			//var registerrepov2 = new UserManagementService(userManagerMock.Object, _mapper, tokenServiceMock.Object, emailManagerServiceMock.Object, contextMock.Object, roleManager);

			//unitofwork.Setup(u => u.userManagementService).Returns(registerrepov2);

			var controller = new AuthController(unitofwork.Object);

			common = new Common
			{
				Code = StatusCodes.Status100Continue,
				Status = Level.Success,
				Message = "Register Succesfully"
			};
			registerrepo.Setup(r => r.Register(reg)).ReturnsAsync(common);


			var result = await controller.Register(reg);
			// Assert

			result.Should().NotBeNull();


		}
	}
}