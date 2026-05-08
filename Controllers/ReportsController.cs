using Microsoft.AspNetCore.Mvc;
using MongoExample.Models.ViewModels;
using MongoExample.Services;

namespace SE_Project___POS_System.Controllers
{
    public class ReportsController : Controller
    {
        private readonly TransactionServices _transactionServices;

        public ReportsController(TransactionServices transactionServices)
        {
            _transactionServices = transactionServices;
        }

        public IActionResult Index()
        {
            return View(new DailyReportView());
        }

        [HttpPost]
        public async Task<IActionResult> Index(DailyReportView model)
        {
            var viewModel = new DailyReportView
            {
                SelectedDate = model.SelectedDate,
                HasRunReport = true
            };

            if (!model.SelectedDate.HasValue)
            {
                return View(viewModel);
            }

            var start = model.SelectedDate.Value.Date;
            var end = start.AddDays(1);

            var transactions = await _transactionServices.GetByDateRangeAsync(start, end);

            viewModel.Transactions = transactions;
            viewModel.TotalTransactions = transactions.Count;
            viewModel.TotalSales = transactions.Sum(x => x.total);
            viewModel.TotalRefunds = transactions
                .Where(x => x.refundedAt.HasValue)
                .Sum(x => x.refundAmount ?? 0m);
            viewModel.RefundCount = transactions.Count(x => x.refundedAt.HasValue);

            return View(viewModel);
        }
    }
}
