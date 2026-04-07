using Microsoft.AspNetCore.Mvc;

namespace SE_Project___POS_System.Controllers
{
    public class ItemsController : Controller
    {
        public IActionResult Lookup()
        {
            return View();
        }
    }
}
