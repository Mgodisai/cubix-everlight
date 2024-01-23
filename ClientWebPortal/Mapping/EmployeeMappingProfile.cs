using AutoMapper;
using ClientWebPortal.Models;
using DataContextLib.Models;

namespace ClientWebPortal.Mapping
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            CreateMap<Employee, EmployeeViewModel>()
                .ForMember(dest => dest.PositionName, opt => opt.MapFrom(src => src.Position.Name))
                .ReverseMap();
        }
    }
}
