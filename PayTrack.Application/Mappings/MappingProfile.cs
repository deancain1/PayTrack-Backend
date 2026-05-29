using AutoMapper;
using PayTrack.Application.DTOs;
using PayTrack.Application.Features.Department.Commands;
using PayTrack.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserDTO>()
           .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Roles, opt => opt.Ignore()).ReverseMap();

            CreateMap<CreateDepartmentCommand, Departments>()
           .ForMember(dest => dest.ApplicationUserId, opt => opt.Ignore());

            CreateMap<Departments, DepartmentDTO>();

        }
    }
}
