using MongoExample.Models;

namespace MongoExample.Models.ViewModels;

public class DailyReportView
{
    public DateTime? SelectedDate { get; set; }
    public bool HasRunReport { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalRefunds { get; set; } = 0m;
    public decimal TotalDiscounts { get; set; } = 0m;
    public List<Transactions> Transactions { get; set; } = new();
}
