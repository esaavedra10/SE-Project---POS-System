using Microsoft.AspNetCore.Mvc;
using MongoExample.Models;
using MongoExample.Models.ViewModels;
using MongoExample.Services;
using System.Text.Json;

namespace MongoExample.Controllers;

public class TransactionsController : Controller
{
    private readonly TransactionServices _transactionServices;
    private readonly ProductsServices _productsServices;

    public TransactionsController(
        TransactionServices transactionServices,
        ProductsServices productsServices)
    {
        _transactionServices = transactionServices;
        _productsServices = productsServices;
    }

    // -----------------------------
    // Helpers
    // -----------------------------
    private List<SaleCartItem> GetCart()
    {
        var cartJson = HttpContext.Session.GetString("SaleCart");

        if (string.IsNullOrEmpty(cartJson))
            return new List<SaleCartItem>();

        return JsonSerializer.Deserialize<List<SaleCartItem>>(cartJson) ?? new List<SaleCartItem>();
    }

    private void SaveCart(List<SaleCartItem> cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);
        HttpContext.Session.SetString("SaleCart", cartJson);
    }

    private void ClearCart()
    {
        HttpContext.Session.Remove("SaleCart");
    }

    private MakeSaleView BuildViewModel(List<SaleCartItem> cart, string? message = null)
    {
        var subtotal = cart.Sum(x => x.lineTotal);
        var tax = Math.Round(subtotal * 0.0825m, 2);
        var total = subtotal + tax;

        return new MakeSaleView
        {
            CartItems = cart,
            Subtotal = subtotal,
            Tax = tax,
            Total = total,
            Message = message
        };
    }

    // -----------------------------
    // Make Sale screen
    // -----------------------------
    [HttpGet]
    public IActionResult MakeSale()
    {
        var cart = GetCart();
        var model = BuildViewModel(cart);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(MakeSaleView model)
    {
        var cart = GetCart();

        if (string.IsNullOrWhiteSpace(model.SkuInput) && string.IsNullOrWhiteSpace(model.NameInput))
        {
            return View("MakeSale", BuildViewModel(cart, "Please enter a SKU or product name."));
        }

        Products? product;

        if (!string.IsNullOrWhiteSpace(model.SkuInput))
        {
            product = await _productsServices.GetBySkuAsync(model.SkuInput);
        }
        else
        {
            product = await _productsServices.GetByNameAsync(model.NameInput!);
        }

        if (product == null)
        {
            return View("MakeSale", BuildViewModel(cart, "No product found."));
        }

        if (product.isVoided)
        {
            return View("MakeSale", BuildViewModel(cart, $"{product.name} is voided and cannot be sold."));
        }

        if (product.stock <= 0)
        {
            return View("MakeSale", BuildViewModel(cart, $"{product.name} is out of stock and cannot be sold."));
        }

        if (product.ageRestricted == true)
        {
            var vm = BuildViewModel(cart, $"{product.name} is age restricted. Manager approval required. Approve to add this item?");
            vm.PendingRestrictedSku = product.sku;
            return View("MakeSale", vm);
        }

        var existingItem = cart.FirstOrDefault(x => x.sku == product.sku);

        if (existingItem != null)
        {
            if (existingItem.quantity >= product.stock)
            {
                return View("MakeSale", BuildViewModel(cart, $"Cannot add more {product.name}. Only {product.stock} in stock."));
            }

            existingItem.quantity++;
            existingItem.lineTotal = existingItem.price * existingItem.quantity;
        }
        else
        {
            cart.Add(new SaleCartItem
            {
                ProductId = product.Id,
                sku = product.sku,
                name = product.name,
                category = product.category,
                price = product.price,
                quantity = 1,
                lineTotal = product.price
            });
        }

        SaveCart(cart);

        return View("MakeSale", BuildViewModel(cart, $"{product.name} added to cart."));
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmRestricted(string sku, bool isApproved)
    {
        var cart = GetCart();

        if (string.IsNullOrWhiteSpace(sku))
        {
            return View("MakeSale", BuildViewModel(cart, "No SKU provided for approval."));
        }

        if (!isApproved)
        {
            var notAddedVm = BuildViewModel(cart, "Item was not added.");
            notAddedVm.PendingRestrictedSku = null;
            return View("MakeSale", notAddedVm);
        }

        var approvalVm = BuildViewModel(cart);
        approvalVm.PendingRestrictedSku = sku;
        approvalVm.ShowApprovalLogin = true;
        return View("MakeSale", approvalVm);
    }

    [HttpPost]
    public async Task<IActionResult> VerifyApproval(MakeSaleView model)
    {
        var cart = GetCart();

        var sku = model.PendingRestrictedSku;
        if (string.IsNullOrWhiteSpace(sku))
        {
            return View("MakeSale", BuildViewModel(cart, "No SKU provided for approval."));
        }

        var employeeServices = (EmployeeServices?)HttpContext.RequestServices.GetService(typeof(EmployeeServices));
        if (employeeServices == null)
        {
            var unavailableVm = BuildViewModel(cart, "Approval service unavailable.");
            unavailableVm.PendingRestrictedSku = sku;
            unavailableVm.ShowApprovalLogin = true;
            return View("MakeSale", unavailableVm);
        }

        if (string.IsNullOrWhiteSpace(model.ApprovalEmployeeId) || string.IsNullOrWhiteSpace(model.ApprovalPassword))
        {
            var missingVm = BuildViewModel(cart, "Please enter both Employee ID and Password.");
            missingVm.PendingRestrictedSku = sku;
            missingVm.ShowApprovalLogin = true;
            return View("MakeSale", missingVm);
        }

        var employee = await employeeServices.GetByEmployeeIdAsync(model.ApprovalEmployeeId);
        if (employee == null || employee.password != model.ApprovalPassword)
        {
            var invalidVm = BuildViewModel(cart, "Invalid employee credentials.");
            invalidVm.PendingRestrictedSku = sku;
            invalidVm.ShowApprovalLogin = true;
            return View("MakeSale", invalidVm);
        }

        var product = await _productsServices.GetBySkuAsync(sku);

        if (product == null)
        {
            var notFoundVm = BuildViewModel(cart, "No product found.");
            notFoundVm.PendingRestrictedSku = sku;
            notFoundVm.ShowApprovalLogin = true;
            return View("MakeSale", notFoundVm);
        }

        if (product.isVoided)
        {
            var voidedVm = BuildViewModel(cart, $"{product.name} is voided and cannot be sold.");
            voidedVm.PendingRestrictedSku = sku;
            voidedVm.ShowApprovalLogin = true;
            return View("MakeSale", voidedVm);
        }

        if (product.stock <= 0)
        {
            var outOfStockVm = BuildViewModel(cart, $"{product.name} is out of stock and cannot be sold.");
            outOfStockVm.PendingRestrictedSku = sku;
            outOfStockVm.ShowApprovalLogin = true;
            return View("MakeSale", outOfStockVm);
        }

        var existingItem = cart.FirstOrDefault(x => x.sku == product.sku);

        if (existingItem != null)
        {
            if (existingItem.quantity >= product.stock)
            {
                var maxVm = BuildViewModel(cart, $"Cannot add more {product.name}. Only {product.stock} in stock.");
                maxVm.PendingRestrictedSku = sku;
                maxVm.ShowApprovalLogin = true;
                return View("MakeSale", maxVm);
            }

            existingItem.quantity++;
            existingItem.lineTotal = existingItem.price * existingItem.quantity;
        }
        else
        {
            cart.Add(new SaleCartItem
            {
                ProductId = product.Id,
                sku = product.sku,
                name = product.name,
                category = product.category,
                price = product.price,
                quantity = 1,
                lineTotal = product.price
            });
        }

        SaveCart(cart);

        var approvedVm = BuildViewModel(cart, $"{product.name} added to cart.");
        approvedVm.PendingRestrictedSku = null;
        approvedVm.ShowApprovalLogin = false;
        return View("MakeSale", approvedVm);
    }

    [HttpPost]
    public IActionResult RemoveItem(string sku)
    {
        var cart = GetCart();

        var item = cart.FirstOrDefault(x => x.sku == sku);
        if (item != null)
        {
            cart.Remove(item);
            SaveCart(cart);
        }

        return View("MakeSale", BuildViewModel(cart));
    }

    [HttpPost]
    public async Task<IActionResult> CompleteSale(MakeSaleView model)
    {
        var cart = GetCart();

        if (cart.Count == 0)
        {
            Console.WriteLine("CompleteSale stopped: cart is empty.");
            return View("MakeSale", BuildViewModel(cart, "Cart is empty."));
        }

        // Get the logged-in employee ID from session
        var loggedInEmployeeId = HttpContext.Session.GetString("EmployeeId");

        if (string.IsNullOrWhiteSpace(loggedInEmployeeId))
        {
            return RedirectToAction("Login", "Auth");
        }

        var subtotal = cart.Sum(x => x.lineTotal);
        var tax = Math.Round(subtotal * 0.0825m, 2);
        var total = subtotal + tax;

        var transaction = new Transactions
        {
            transactionNumber = $"TXN-{DateTime.Now:yyyyMMddHHmmss}",
            employeeId = loggedInEmployeeId,
            paymentMethod = string.IsNullOrWhiteSpace(model.PaymentMethod) ? "Cash" : model.PaymentMethod,
            subtotal = subtotal,
            tax = tax,
            total = total,
            createdAt = DateTime.UtcNow,
            items = cart.Select(x => new TransactionItem
            {
                productId = x.ProductId ?? "",
                sku = x.sku,
                name = x.name,
                price = x.price,
                quantity = x.quantity,
                lineTotal = x.lineTotal
            }).ToList()
        };

        await _transactionServices.CreateAsync(transaction);

        foreach (var item in cart)
        {
            var product = await _productsServices.GetBySkuAsync(item.sku);
            if (product == null)
                continue;

            var updatedStock = product.stock - item.quantity;
            product.stock = updatedStock < 0 ? 0 : updatedStock;

            await _productsServices.UpdateAsync(product.Id, product);
        }


        var allTransactions = await _transactionServices.GetAsync();

        ClearCart();

        return RedirectToAction(nameof(SaleComplete), new { transactionNumber = transaction.transactionNumber });
    }

    [HttpGet]
    public async Task<IActionResult> SaleComplete(string transactionNumber)
    {
        var transaction = await _transactionServices.GetByTransactionNumberAsync(transactionNumber);

        if (transaction == null)
            return NotFound();

        return View(transaction);
    }
}