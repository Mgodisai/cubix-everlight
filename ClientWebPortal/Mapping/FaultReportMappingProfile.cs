using AutoMapper;
using ClientWebPortal.Models;
using Data.Models;

namespace ClientWebPortal.Mapping
{
    public class FaultReportMappingProfile : Profile
    {
        public FaultReportMappingProfile()
        {
            CreateMap<AddressViewModel, Address>()
                .ReverseMap();
            CreateMap<FaultReportViewModel, FaultReport>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ReverseMap();
        }
    }
}
