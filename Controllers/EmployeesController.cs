using Microsoft.AspNetCore.Mvc;
using MongoExample.Models;
using MongoExample.Services;

namespace MongoExample.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeesServices _employeeServices;

        public EmployeesController(EmployeesServices employeeServices)
        {
            _employeeServices = employeeServices;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeServices.GetAsync();
            return View(employees);
        }

        public async Task<IActionResult> Details(string id)
        {
            var employee = await _employeeServices.GetAsync(id);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employees employee)
        {
            if (!ModelState.IsValid)
                return View(employee);

            await _employeeServices.CreateAsync(employee);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var employee = await _employeeServices.GetAsync(id);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, Employees employee)
        {
            if (!ModelState.IsValid)
                return View(employee);

            await _employeeServices.UpdateAsync(id, employee);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var employee = await _employeeServices.GetAsync(id);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _employeeServices.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}