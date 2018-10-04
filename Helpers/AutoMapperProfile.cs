using AutoMapper;
using Evaluation.API.Dtos;
using Evaluation.API.Entities;

namespace Evaluation.API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}