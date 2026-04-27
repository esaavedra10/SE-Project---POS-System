using Microsoft.AspNetCore.Mvc;

namespace MongoExample.Controllers;

public class SaleController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("MakeSale", "Transactions");
    }
}
