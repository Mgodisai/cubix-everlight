using AutoMapper;
using ClientWebPortal.Controllers;
using ClientWebPortal.Models;
using ClientWebPortal.Service.Specifications;
using Data.Models;
using Data.Repository;
using Microsoft.Extensions.Localization;

namespace ClientWebPortal.Service
{
    public class FaultReportService : IFaultReportService
    {
        private readonly IRepository<FaultReport> _faultReportRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<FaultReportController> _localizer;
        private readonly IEmailService _emailService;
        private readonly ILogger<FaultReportController> _logger;

        public FaultReportService(IRepository<FaultReport> faultReportRepository, IMapper mapper, IStringLocalizer<FaultReportController> localizer, IEmailService emailService, ILogger<FaultReportController> logger)
        {
            _faultReportRepository = faultReportRepository;
            _mapper = mapper;
            _localizer = localizer;
            _emailService = emailService;
            _logger = logger;
        }

        public IEnumerable<FaultReportViewModel> GetAllReports()
        {
            List<FaultReportViewModel> faultReportViewModels = [];
            var result = _faultReportRepository.GetAll();
            return result.Select(fr=>_mapper.Map<FaultReportViewModel>(fr)).ToList();
        }

        public FaultReportViewModel GetReportById(Guid id)
        {
            var spec = new FaultReportWithAddressSpecification(id);
            var result = _faultReportRepository.FindWithSpecification(spec).FirstOrDefault();
            return _mapper.Map<FaultReportViewModel>(result);
        }

        public async Task AddFaultReport(FaultReportViewModel reportViewModel)
        {
            var faultReport = _mapper.Map<FaultReport>(reportViewModel);

            _faultReportRepository.Add(faultReport);
            await SendEmail(reportViewModel);
        }

        private async Task SendEmail(FaultReportViewModel faultReportViewModel)
        {
            string subject = _localizer["EmailSubject"];
            string body = $@"
                    <p>{string.Format(_localizer["EmailGreetingLine"], faultReportViewModel.Email)}<p>
                    <p>{_localizer["EmailFirstLine"]}</p>
                    <p>{_localizer["EmailDetails"]}:</p>
                    <ul>
                        <li><strong>{_localizer["EmailDescription"]}:</strong> {faultReportViewModel.Description}</li>
                        <li><strong>{_localizer["EmailAddress"]}:</strong>{faultReportViewModel.Address?.PostalCode} {faultReportViewModel.Address?.City}, {faultReportViewModel.Address?.Street}, {faultReportViewModel.Address?.HouseNumber}</li>
                        <li><strong>{_localizer["EmailEmail"]}:</strong> {faultReportViewModel.Email}</li>
                    </ul>
                    <p>{_localizer["EmailFurtherInfoLine"]}</p>
                    <p>{_localizer["EmailFarewellLine"]}</p>
                    <p>{_localizer["EmailSignature"]}</p>";

            try
            {
                await _emailService.SendEmailAsync(faultReportViewModel.Email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email sending: ");
            }
        }

    }
}
