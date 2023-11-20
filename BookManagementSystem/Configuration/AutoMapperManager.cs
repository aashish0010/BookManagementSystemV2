using AutoMapper;
using BookManagementSystem.Domain.DTO;
using BookManagementSystem.Infrastructure;

namespace BookManagementSystem.Configuration
{
	public class AutoMapperManager : Profile
	{
		public AutoMapperManager()
		{
			CreateMap<RegisterRequest, User>();
		}
	}
}
