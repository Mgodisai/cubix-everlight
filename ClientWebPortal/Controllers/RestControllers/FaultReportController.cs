using AutoMapper;
using ClientWebPortal.Exceptions;
using ClientWebPortal.Models.Dtos;
using ClientWebPortal.Service;
using Microsoft.AspNetCore.Mvc;

namespace ClientWebPortal.Controllers.RestControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaultReportController : ControllerBase
    {
        private readonly IFaultReportService _faultReportService;
        private readonly IMapper _mapper;

        public FaultReportController(IFaultReportService faultReportService, IMapper mapper)
        {
            _faultReportService = faultReportService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetFaultReports([FromQuery] string specialQuery = null)
        {
            var x = Request;
            if (!string.IsNullOrEmpty(specialQuery))
            {
                var faultReport = await _faultReportService.GetReportBySpecialQueryAsync(specialQuery);
                if (faultReport == null)
                {
                    return NotFound();
                }
                return Ok(faultReport);
            }


            try
            {
                var faultReports = await _faultReportService.GetAllReportsAsync();
                return Ok(faultReports);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }


        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFaultReportById(Guid id)
        {
            var faultReport = await _faultReportService.GetReportDtoByIdAsync(id);
            if (faultReport == null)
            {
                return NotFound();
            }
            return Ok(faultReport);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFaultReport([FromBody] FaultReportDto faultReportDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponseDto
                {
                    StatusCode = 400,
                    Message = "Validation failed. Please check your input."
                });
            }

            FaultReportDto storedFaultReportDto;
            try
            {
                storedFaultReportDto = await _faultReportService.AddFaultReportAsync(faultReportDto);
            }
            catch (ServiceException)
            {
                return BadRequest(new ErrorResponseDto
                {
                    StatusCode = 400,
                    Message = "Invalid request data. Please check your input."
                });
            }
            return CreatedAtAction(nameof(GetFaultReportById), new { id = storedFaultReportDto.Id }, storedFaultReportDto);
        }
    }
}
