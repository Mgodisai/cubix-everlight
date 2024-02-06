using AutoMapper;
using ClientWebPortal.Controllers;
using ClientWebPortal.Exceptions;
using ClientWebPortal.Models;
using ClientWebPortal.Models.Dtos;
using ClientWebPortal.Service.Specifications;
using DataContextLib;
using DataContextLib.Models;
using DataContextLib.UnitOfWorks;
using Microsoft.Extensions.Localization;
using ServiceConsole.Specifications;
using System.Text.RegularExpressions;

namespace ClientWebPortal.Service;

public class FaultReportService(IUnitOfWork<DataDbContext> unitOfWork, IMapper mapper, IStringLocalizer<FaultReportController> localizer, IEmailService emailService, ILogger<FaultReportService> logger) : IFaultReportService
{
    private readonly IUnitOfWork<DataDbContext> _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly IStringLocalizer<FaultReportController> _localizer = localizer;
    private readonly IEmailService _emailService = emailService;
    private readonly ILogger<FaultReportService> _logger = logger;

    private const string regexPatternBudapestDistrict = @"^(1(0\d|1\d|2[0-3]))$";
    private const string regexPatternHungarianPostalCode = @"^[1-9]\d{3}$";
    private const string regexPatternForDays = @"^\d{1,2}$";

    private static readonly Regex BudapestDistrictRegex = new Regex(regexPatternBudapestDistrict, RegexOptions.Compiled);
    private static readonly Regex HungarianPostalCodeRegex = new Regex(regexPatternHungarianPostalCode, RegexOptions.Compiled);
    private static readonly Regex DaysPatternRegex = new Regex(regexPatternForDays, RegexOptions.Compiled);


    public async Task<IEnumerable<FaultReportViewModel>> GetAllReportsAsync()
    {
        List<FaultReportViewModel> faultReportViewModels = [];
        var result = await _unitOfWork.GetRepository<FaultReport>().GetAllAsync();
        return result.Select(fr => _mapper.Map<FaultReportViewModel>(fr)).ToList();
    }

    public async Task<FaultReportViewModel> GetReportByIdAsync(Guid id)
    {
        var spec = new FaultReportWithAddressSpecification(id);
        var result = await _unitOfWork.GetRepository<FaultReport>().FindWithSpecificationAsync(spec);
        return _mapper.Map<FaultReportViewModel>(result.FirstOrDefault());
    }

    public async Task<FaultReportDto> GetReportDtoByIdAsync(Guid id)
    {
        var spec = new FaultReportWithAddressSpecification(id);
        var result = await _unitOfWork.GetRepository<FaultReport>().FindWithSpecificationAsync(spec);
        return _mapper.Map<FaultReportDto>(result.FirstOrDefault());
    }

    public async Task AddFaultReportAsync(FaultReportViewModel reportViewModel)
    {
        var address = _mapper.Map<Address>(reportViewModel.Address);
        var faultReport = _mapper.Map<FaultReport>(reportViewModel);
        var result = await StoreFaultReportAsync(faultReport, address);
        if (result.FirstOrDefault() != null)
        {
            await SendEmail(reportViewModel);
        }
    }

    public async Task<FaultReportDto> AddFaultReportAsync(FaultReportDto faultReportDto)
    {
        var address = _mapper.Map<Address>(faultReportDto.AddressDto);
        var faultReport = _mapper.Map<FaultReport>(faultReportDto);
        var result = await StoreFaultReportAsync(faultReport, address);
        return _mapper.Map<FaultReportDto>(result.FirstOrDefault());
    }

    public async Task<IEnumerable<FaultReportDto>> GetReportBySpecialQueryAsync(string specialQuery)
    {
        var result = await ExecuteSpecialQueryAsync(specialQuery);
        var r = result.Select(fr => _mapper.Map<FaultReportDto>(fr)).ToList();
        return r;
    }

    private async Task<IEnumerable<FaultReport>> StoreFaultReportAsync(FaultReport faultReport, Address address)
    {
        var addressRepository = _unitOfWork.GetRepository<Address>();
        var faultReportRepository = _unitOfWork.GetRepository<FaultReport>();
        try
        {
            var existingAddress = await addressRepository.FindWithSpecificationAsync(new AddressSpecification(address));

            await _unitOfWork.CreateTransactionAsync();
            faultReport.Address = existingAddress.FirstOrDefault() ?? address;
            await faultReportRepository.InsertAsync(faultReport);
            await _unitOfWork.SaveAsync();
            var x = faultReport.Id;
            await _unitOfWork.CommitAsync();

            return await faultReportRepository.FindWithSpecificationAsync(new FaultReportWithAddressSpecification(x));
        }
        catch (AutoMapperMappingException ex)
        {
            _logger.LogError(ex, "Mapping failure due to invalid input");
            throw new ServiceException("Mapping failure", ex);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            _logger.LogError(ex, "Error during storing");
            throw new ServiceException("Db failure", ex);
        }
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

    private async Task<IEnumerable<FaultReport>> ExecuteSpecialQueryAsync(string input)
    {
        var faultReportRepository = _unitOfWork.GetRepository<FaultReport>();

        if (BudapestDistrictRegex.IsMatch(input))
        {
            return await faultReportRepository.FindWithSpecificationAsync(new FaultReportByDistrictSpecification(input));
        }

        if (HungarianPostalCodeRegex.IsMatch(input))
        {
            return await faultReportRepository.FindWithSpecificationAsync(new FaultReportByNormalPostalCodeSpecification(input));
        }

        if (DaysPatternRegex.IsMatch(input))
        {
            var result = int.TryParse(input, out var days);
            if (!result)
            {
                return Enumerable.Empty<FaultReport>();
            }
            return await faultReportRepository.FindWithSpecificationAsync(new FaultReportByDaysBeforeTodaySpecification(days));
        }
        return Enumerable.Empty<FaultReport>();
    }
}