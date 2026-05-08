using Microsoft.AspNetCore.Mvc;
using MongoExample.Models;
using MongoExample.Models.ViewModels;
using MongoExample.Services;
using System.Text.Json;

namespace MongoExample.Controllers;

public class RefundsController : Controller
{
    private const string RefundApprovalSessionKey = "RefundApproval";

    private readonly TransactionServices _transactionServices;
    private readonly EmployeeServices _employeeServices;

    private sealed class RefundApprovalState
    {
        public string TransactionNumber { get; set; } = null!;
        public string ApprovedByEmployeeId { get; set; } = null!;
        public DateTime ApprovedAt { get; set; }
    }

    public RefundsController(
        TransactionServices transactionServices,
        EmployeeServices employeeServices)
    {
        _transactionServices = transactionServices;
        _employeeServices = employeeServices;
    }

    // -----------------------------
    // Lookup
    // -----------------------------
    [HttpGet]
    public IActionResult Lookup()
    {
        return View(new RefundView());
    }

    [HttpPost]
    public IActionResult Lookup(RefundView model)
    {
        var transactionNumber = model.OriginalTransactionNumber?.Trim();
        if (string.IsNullOrWhiteSpace(transactionNumber))
        {
            model.Message = "Please enter a transaction number.";
            return View(model);
        }

        return RedirectToAction(nameof(Issue), new { transactionNumber });
    }

    // -----------------------------
    // Issue (show refund screen)
    // -----------------------------
    [HttpGet]
    public async Task<IActionResult> Issue(string transactionNumber)
    {
        var vm = await BuildIssueViewModelAsync(transactionNumber);
        return View(vm);
    }

    // -----------------------------
    // Verify manager approval
    // -----------------------------
    [HttpPost]
    public async Task<IActionResult> VerifyRefundApproval(RefundView model)
    {
        var transactionNumber = model.OriginalTransactionNumber?.Trim();
        if (string.IsNullOrWhiteSpace(transactionNumber))
        {
            return RedirectToAction(nameof(Lookup));
        }

        var vm = await BuildIssueViewModelAsync(transactionNumber);

        if (vm.OriginalTransaction == null || vm.AlreadyRefunded)
        {
            return View("Issue", vm);
        }

        if (string.IsNullOrWhiteSpace(model.RefundApprovalEmployeeId) ||
            string.IsNullOrWhiteSpace(model.RefundApprovalPassword))
        {
            vm.Message = "Please enter manager Employee ID and Password.";
            vm.ShowApprovalPrompt = true;
            vm.ApprovalGranted = false;
            return View("Issue", vm);
        }

        var manager = await _employeeServices.ValidateManagerCredentialsAsync(
            model.RefundApprovalEmployeeId,
            model.RefundApprovalPassword);

        if (manager == null)
        {
            vm.Message = "Invalid manager credentials.";
            vm.ShowApprovalPrompt = true;
            vm.ApprovalGranted = false;
            return View("Issue", vm);
        }

        var approval = new RefundApprovalState
        {
            TransactionNumber = transactionNumber,
            ApprovedByEmployeeId = manager.EID,
            ApprovedAt = DateTime.UtcNow
        };
        SaveApproval(approval);

        vm.ApprovalGranted = true;
        vm.ShowApprovalPrompt = false;
        vm.RefundApprovedByEmployeeId = approval.ApprovedByEmployeeId;
        vm.RefundApprovedAt = approval.ApprovedAt;
        vm.Message = "Manager approval accepted. Click Confirm Refund to issue the refund.";
        return View("Issue", vm);
    }

    // -----------------------------
    // Issue refund (final commit)
    // -----------------------------
    [HttpPost]
    public async Task<IActionResult> IssueRefund(RefundView model)
    {
        var transactionNumber = model.OriginalTransactionNumber?.Trim();
        if (string.IsNullOrWhiteSpace(transactionNumber))
        {
            return RedirectToAction(nameof(Lookup));
        }

        var loggedInEmployeeId = HttpContext.Session.GetString("EmployeeId");
        if (string.IsNullOrWhiteSpace(loggedInEmployeeId))
        {
            return RedirectToAction("Login", "Auth");
        }

        var original = await _transactionServices.GetByTransactionNumberAsync(transactionNumber);
        if (original == null || string.IsNullOrWhiteSpace(original.Id))
        {
            var notFoundVm = await BuildIssueViewModelAsync(transactionNumber);
            notFoundVm.Message = $"Transaction {transactionNumber} not found.";
            return View("Issue", notFoundVm);
        }

        if (original.refundedAt.HasValue)
        {
            var alreadyVm = await BuildIssueViewModelAsync(transactionNumber);
            alreadyVm.Message = "Transaction has already been refunded.";
            return View("Issue", alreadyVm);
        }

        var approval = GetApproval();
        if (approval == null ||
            !string.Equals(approval.TransactionNumber, transactionNumber, StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(approval.ApprovedByEmployeeId))
        {
            var needsApprovalVm = await BuildIssueViewModelAsync(transactionNumber);
            needsApprovalVm.Message = "Manager approval is required before issuing a refund.";
            needsApprovalVm.ShowApprovalPrompt = true;
            needsApprovalVm.ApprovalGranted = false;
            return View("Issue", needsApprovalVm);
        }

        original.refundAmount = original.total;
        original.refundedAt = DateTime.UtcNow;
        original.refundedByEmployeeId = loggedInEmployeeId;
        original.refundApprovedByEmployeeId = approval.ApprovedByEmployeeId;
        original.refundApprovedAt = approval.ApprovedAt;

        await _transactionServices.UpdateAsync(original.Id, original);

        ClearApproval();

        return RedirectToAction(nameof(RefundComplete), new { transactionNumber = original.transactionNumber });
    }

    // -----------------------------
    // Refund complete
    // -----------------------------
    [HttpGet]
    public async Task<IActionResult> RefundComplete(string transactionNumber)
    {
        var transaction = await _transactionServices.GetByTransactionNumberAsync(transactionNumber);
        if (transaction == null)
            return NotFound();

        return View(transaction);
    }

    // -----------------------------
    // Helpers
    // -----------------------------
    private async Task<RefundView> BuildIssueViewModelAsync(string? transactionNumber)
    {
        var trimmed = transactionNumber?.Trim() ?? string.Empty;
        var vm = new RefundView
        {
            OriginalTransactionNumber = trimmed
        };

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            vm.Message = "Please enter a transaction number.";
            return vm;
        }

        var transaction = await _transactionServices.GetByTransactionNumberAsync(trimmed);
        if (transaction == null)
        {
            vm.Message = $"Transaction {trimmed} not found.";
            return vm;
        }

        vm.OriginalTransaction = transaction;

        if (transaction.refundedAt.HasValue)
        {
            vm.AlreadyRefunded = true;
            vm.RefundApprovedByEmployeeId = transaction.refundApprovedByEmployeeId;
            vm.RefundApprovedAt = transaction.refundApprovedAt;
            vm.Message = "This transaction has already been refunded.";
            return vm;
        }

        var approval = GetApproval();
        if (approval != null && string.Equals(approval.TransactionNumber, trimmed, StringComparison.Ordinal))
        {
            vm.ApprovalGranted = true;
            vm.ShowApprovalPrompt = false;
            vm.RefundApprovedByEmployeeId = approval.ApprovedByEmployeeId;
            vm.RefundApprovedAt = approval.ApprovedAt;
        }
        else
        {
            vm.ShowApprovalPrompt = true;
            vm.ApprovalGranted = false;
        }

        return vm;
    }

    private RefundApprovalState? GetApproval()
    {
        var json = HttpContext.Session.GetString(RefundApprovalSessionKey);
        if (string.IsNullOrWhiteSpace(json))
            return null;

        return JsonSerializer.Deserialize<RefundApprovalState>(json);
    }

    private void SaveApproval(RefundApprovalState state)
    {
        HttpContext.Session.SetString(RefundApprovalSessionKey, JsonSerializer.Serialize(state));
    }

    private void ClearApproval()
    {
        HttpContext.Session.Remove(RefundApprovalSessionKey);
    }
}
