using Microsoft.AspNetCore.Mvc;
using MongoExample.Models;
using MongoExample.Services;
using SE_Project___POS_System.Extensions;

namespace MongoExample.Controllers;

public class ProductsController : Controller
{
    private readonly ProductsServices _productsServices;

    public ProductsController(ProductsServices productsServices)
    {
        _productsServices = productsServices;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productsServices.GetAsync();
        return View(products);
    }

    public async Task<IActionResult> Details(string id)
    {
        var product = await _productsServices.GetAsync(id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Products product)
    {
        if (!ModelState.IsValid)
            return View(product);

        await _productsServices.CreateAsync(product);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var product = await _productsServices.GetAsync(id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(string id, Products product)
    {
        if (!ModelState.IsValid)
            return View(product);

        await _productsServices.UpdateAsync(id, product);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string id)
    {
        var product = await _productsServices.GetAsync(id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await _productsServices.RemoveAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> VoidProduct(string id, string? voidReason)
    {
        var accessCheck = this.RequireManagerFromSession();
        if (accessCheck != null)
            return accessCheck;

        var product = await _productsServices.GetAsync(id);
        if (product == null)
            return NotFound();

        if (product.isVoided)
        {
            TempData["Message"] = "Product is already voided.";
            return RedirectToAction("Lookup", "Items");
        }

        product.isVoided = true;
        product.voidReason = string.IsNullOrWhiteSpace(voidReason) ? "Voided by manager." : voidReason.Trim();
        product.voidedBy = HttpContext.Session.GetString("EmployeeId");
        product.voidedAt = DateTime.UtcNow;

        await _productsServices.UpdateAsync(id, product);

        TempData["Message"] = "Product was voided successfully.";
        return RedirectToAction("Lookup", "Items");
    }
}