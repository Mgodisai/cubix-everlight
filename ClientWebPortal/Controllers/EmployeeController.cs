using ClientWebPortal.Models;
using ClientWebPortal.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClientWebPortal.Controllers
{
    [Authorize] 
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public IActionResult Index()
        {
            var employees = _employeeService.GetAllEmployees();
            return View(employees);
        }

        public IActionResult Create()
        {
            var model = new EmployeeViewModel
            {
                Positions = _employeeService.GetAllPositions().Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                _employeeService.AddEmployee(model);

                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Delete(Guid id)
        {
            _employeeService.DeleteById(id);
            return RedirectToAction("Index");
        }
    }
}
