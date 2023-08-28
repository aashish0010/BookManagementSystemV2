using BookManagementSystem.Controller;
using BookManagementSystem.Domain.DTO;
using BookManagementSystem.Domain.Entities;
using BookManagementSystem.Service.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BookManagementSystem.Test
{
	public class AuthTests
	{
		private readonly AuthController _authController;
		private readonly IUnitOfWork _unitOfWork;
		public AuthTests(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
			_authController = new AuthController(_unitOfWork);
		}
		[Fact]
		public async Task Register_ReturnsCommonSuccess()
		{
			//Arrage

			RegisterRequest reg = new RegisterRequest();
			reg.UserName = "test";
			reg.Email = "test@gmail.com";
			reg.PhoneNumber = "12345";
			reg.Password = "12132414aa";
			Common common = new Common();

			var userManagementServiceMock = new Mock<IUserManagementService>();
			userManagementServiceMock.Setup(u => u.Register(It.IsAny<RegisterRequest>())).Returns(Task.FromResult(new Common()
			{
				Code = StatusCodes.Status100Continue,
				Status = Level.Success,
				Message = "Register Succesfully"
			}));

			//Act

			var regdata = await _authController.Register(reg);

			//Assert
			regdata.Should().NotBeNull();
			regdata.Should().BeOfType<IActionResult>();
			regdata.Should().Be(200);

		}
	}
}
