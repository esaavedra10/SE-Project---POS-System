using MongoExample.Models;

namespace MongoExample.Models.ViewModels;

public class RefundView
{
    public string? OriginalTransactionNumber { get; set; }
    public Transactions? OriginalTransaction { get; set; }

    public bool AlreadyRefunded { get; set; }
    public bool ShowApprovalPrompt { get; set; }
    public bool ApprovalGranted { get; set; }

    public string? RefundApprovalEmployeeId { get; set; }
    public string? RefundApprovalPassword { get; set; }

    public string? RefundApprovedByEmployeeId { get; set; }
    public DateTime? RefundApprovedAt { get; set; }

    public string? Message { get; set; }
}
