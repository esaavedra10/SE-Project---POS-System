using Microsoft.AspNetCore.Mvc;

namespace SE_Project___POS_System.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string employeeId, string password)
        {
            if (!string.IsNullOrWhiteSpace(employeeId) && !string.IsNullOrWhiteSpace(password))
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Please enter both Employee ID and Password.";
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}
