using ClientWebPortal.Models;
using ClientWebPortal.Models.Dtos;

namespace ClientWebPortal.Service
{
    public interface IFaultReportService
    {
        Task<IEnumerable<FaultReportViewModel>> GetAllReportsAsync();
        Task<FaultReportViewModel> GetReportByIdAsync(Guid id);
        Task<FaultReportDto> GetReportDtoByIdAsync(Guid id);
        Task AddFaultReportAsync(FaultReportViewModel reportViewModel);

        Task<FaultReportDto> AddFaultReportAsync(FaultReportDto faultReportDto);
        Task<IEnumerable<FaultReportDto>> GetReportBySpecialQueryAsync(string specialQuery);
    }
}
