using ClientWebPortal.Models;
using Data.Models;

namespace ClientWebPortal.Service
{
    public interface IFaultReportService
    {
        IEnumerable<FaultReportViewModel> GetAllReports();
        FaultReportViewModel? GetReportById(Guid id);
        Task AddFaultReport(FaultReportViewModel faultReportViewModel);
    }
}
