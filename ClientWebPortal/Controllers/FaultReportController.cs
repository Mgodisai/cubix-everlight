using ClientWebPortal.Models;
using ClientWebPortal.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ClientWebPortal.Controllers
{
    public class FaultReportController(IFaultReportService faultReportService, IStringLocalizer<FaultReportController> localizer, ILogger<FaultReportController> logger) : Controller
    {
        private readonly IFaultReportService _faultReportService = faultReportService;
        private readonly IStringLocalizer<FaultReportController> _localizer = localizer;
        private readonly ILogger<FaultReportController> _logger = logger;

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var reports = await _faultReportService.GetAllReportsAsync();
            return View(reports.OrderBy(r=>r.ReportedAt));
        }

        [Authorize]
        public async Task<IActionResult> Details(Guid id)
        {
            var report = await _faultReportService.GetReportByIdAsync(id);
            return View(report);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FaultReportViewModel faultReportViewModel)
        {
            if (ModelState.IsValid)
            {
                faultReportViewModel.ReportedAt = DateTime.UtcNow;
                await _faultReportService.AddFaultReportAsync(faultReportViewModel);
                return RedirectToAction("Index");
            }
            else
            {
                return View(faultReportViewModel);
            }
        }
    }
}
