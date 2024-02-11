using AutoMapper;
using ClientWebPortal.Models;
using ClientWebPortal.Models.Dtos;
using DataContextLib.Models;

namespace ClientWebPortal.Mapping;

public class FaultReportMappingProfile : Profile
{
    public FaultReportMappingProfile()
    {
        CreateMap<AddressViewModel, Address>()
            .ReverseMap();
        CreateMap<FaultReportViewModel, FaultReport>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ReverseMap();

        CreateMap<FaultReportDto, FaultReport>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Address, opt => opt.Ignore());
        CreateMap<FaultReport, FaultReportDto>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.ReportedAt, opt => opt.MapFrom(src => src.ReportedAt))

            .ForMember(dest => dest.AddressDto, opt => opt.MapFrom(src => src.Address))
            ;

        CreateMap<AddressDto, Address>()
            .ReverseMap();
    }
}