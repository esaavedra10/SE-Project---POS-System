using Microsoft.AspNetCore.Mvc;
using MongoExample.Services;

namespace SE_Project___POS_System.Controllers
{
    public class AuthController : Controller
    {
        private readonly EmployeeServices _employeeServices;

        public AuthController(EmployeeServices employeeServices)
        {
            _employeeServices = employeeServices;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string employeeId, string password)
        {
            if (string.IsNullOrWhiteSpace(employeeId) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter both Employee ID and Password.";
                return View();
            }

            var employee = await _employeeServices.GetByEmployeeIdAsync(employeeId);

            if (employee == null)
            {
                ViewBag.Error = "Employee ID not found.";
                return View();
            }

            if (employee.password != password)
            {
                ViewBag.Error = "Incorrect password.";
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}