using AutoMapper;
using BookManagementSystem.Domain.DTO;
using BookManagementSystem.Infrastructure;

namespace BookManagementSystem
{
	public class AutoMapperManager : Profile
	{
		public AutoMapperManager()
		{
			CreateMap<RegisterRequest, User>();
		}
	}
}
