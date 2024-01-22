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
        public IActionResult Index()
        {
            var reports = _faultReportService.GetAllReports();
            return View(reports);
        }

        [Authorize]
        public IActionResult Details(Guid id)
        {
            var report = _faultReportService.GetReportById(id);
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
                await _faultReportService.AddFaultReport(faultReportViewModel);
                return RedirectToAction("Index");
            }
            else
            {
                return View(faultReportViewModel);
            }
        }
    }
}
