using Microsoft.AspNetCore.Mvc;
using MongoExample.Services;
using MongoExample.Models;
using MongoExample.Models.ViewModels;
namespace MongoExample.Controllers;

public class ItemsController : Controller
{
    private readonly ProductsServices _productsServices;

    public ItemsController(ProductsServices productsServices)
    {
        _productsServices = productsServices;
    }

    [HttpGet]
    public IActionResult Lookup()
    {
        return View(new ItemLookupView());
    }

    [HttpPost]
    public async Task<IActionResult> Lookup(ItemLookupView model)
    {
        if (!string.IsNullOrWhiteSpace(model.SearchTerm))
        {
            model.SelectedProduct = await _productsServices.GetBySkuAsync(model.SearchTerm);
        }

        return View(model);
    }
}