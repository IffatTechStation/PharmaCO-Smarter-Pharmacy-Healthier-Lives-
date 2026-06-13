using PagedList;
using PharmaCO.Services;
using PharmaCO.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PharmaCO.Controllers
{
    public class ReportController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IMedicineService _medicineService;

        public ReportController()
        {
            _orderService = new OrderService();
            _customerService = new CustomerService();
            _medicineService = new MedicineService();
        }

        // Dashboard: Owner + SalesMan
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult Index()
        {
            return View();
        }

        // Export Sales Report to PDF: শুধু Owner
        [Authorize(Roles = "Owner")]
        public ActionResult ExportSalesReportPdf(DateTime? startDate, DateTime? endDate)
        {
            var report = GetSalesReportData(startDate, endDate);
            return new Rotativa.ViewAsPdf("SalesReport", report)
            {
                FileName = "SalesReport.pdf"
            };
        }

        // Export Sales Report to Excel: শুধু Owner
        [Authorize(Roles = "Owner")]
        public FileResult ExportSalesReportExcel(DateTime? startDate, DateTime? endDate)
        {
            var report = GetSalesReportData(startDate, endDate);

            var csv = "Date,TotalOrders,TotalAmount\n";
            foreach (var item in report)
            {
                csv += $"{item.Date:yyyy-MM-dd},{item.TotalOrders},{item.TotalAmount}\n";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return File(bytes, "text/csv", "SalesReport.csv");
        }

        // Helper method: Sales report data (List)
        private List<SalesReportViewModel> GetSalesReportData(DateTime? startDate, DateTime? endDate)
        {
            var orders = _orderService.GetAllOrders();

            if (startDate.HasValue)
                orders = orders.Where(o => o.OrderDate >= startDate.Value);

            if (endDate.HasValue)
                orders = orders.Where(o => o.OrderDate <= endDate.Value);

            var report = orders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new SalesReportViewModel
                {
                    Date = g.Key,
                    TotalOrders = g.Count(),
                    TotalAmount = g.Sum(o => o.OrderItems.Sum(i => i.TotalPrice))
                })
                .OrderBy(r => r.Date)
                .ToList();

            return report;
        }

        // Sales report: Owner + SalesMan (Paged)
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult SalesReport(DateTime? startDate, DateTime? endDate, string search, string sortOrder, int? page)
        {
            var report = GetSalesReportData(startDate, endDate).AsQueryable();

            // Searching
            if (!string.IsNullOrEmpty(search))
                report = report.Where(r => r.Date.ToString("yyyy-MM-dd").Contains(search));

            // Sorting
            ViewBag.SortOrder = sortOrder;
            ViewBag.Search = search;
            switch (sortOrder)
            {
                case "orders_desc": report = report.OrderByDescending(r => r.TotalOrders); break;
                case "amount": report = report.OrderBy(r => r.TotalAmount); break;
                case "amount_desc": report = report.OrderByDescending(r => r.TotalAmount); break;
                default: report = report.OrderBy(r => r.Date); break;
            }

            // Pagination
            int pageSize = 10;
            int pageNumber = page ?? 1;
            return View(report.ToPagedList(pageNumber, pageSize));
        }

        // Customer-wise report: Owner + SalesMan
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult CustomerReport(string search, string sortOrder, int? page)
        {
            var customers = _customerService.GetAllCustomers();
            var orders = _orderService.GetAllOrders();

            var report = customers.Select(c => new CustomerReportViewModel
            {
                CustomerId = c.CustomerId,
                CustomerName = c.Name,
                TotalOrders = orders.Count(o => o.CustomerId == c.CustomerId),
                TotalSpent = orders.Where(o => o.CustomerId == c.CustomerId)
                                   .Sum(o => o.OrderItems.Sum(i => i.TotalPrice))
            }).AsQueryable();

            // Searching
            if (!string.IsNullOrEmpty(search))
                report = report.Where(r => r.CustomerName.Contains(search));

            // Sorting
            ViewBag.SortOrder = sortOrder;
            ViewBag.Search = search;
            switch (sortOrder)
            {
                case "orders_desc": report = report.OrderByDescending(r => r.TotalOrders); break;
                case "spent": report = report.OrderBy(r => r.TotalSpent); break;
                case "spent_desc": report = report.OrderByDescending(r => r.TotalSpent); break;
                default: report = report.OrderBy(r => r.CustomerName); break;
            }

            // Pagination
            int pageSize = 10;
            int pageNumber = page ?? 1;
            return View(report.ToPagedList(pageNumber, pageSize));
        }

        // Medicine stock report: Owner + SalesMan
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult MedicineStockReport(string search, string sortOrder, int? page)
        {
            var medicines = _medicineService.GetAllMedicines();

            var report = medicines.Select(m => new MedicineStockReportViewModel
            {
                MedicineId = m.MedicineId,
                MedicineName = m.Name,
                Price = m.Price,
                Stock = m.Stock,
                ImagePath = m.ImagePath
            }).AsQueryable();

            // Searching
            if (!string.IsNullOrEmpty(search))
                report = report.Where(r => r.MedicineName.Contains(search));

            // Sorting
            ViewBag.SortOrder = sortOrder;
            ViewBag.Search = search;
            switch (sortOrder)
            {
                case "price": report = report.OrderBy(r => r.Price); break;
                case "price_desc": report = report.OrderByDescending(r => r.Price); break;
                case "stock": report = report.OrderBy(r => r.Stock); break;
                case "stock_desc": report = report.OrderByDescending(r => r.Stock); break;
                default: report = report.OrderBy(r => r.MedicineName); break;
            }

            // Pagination
            int pageSize = 10;
            int pageNumber = page ?? 1;
            return View(report.ToPagedList(pageNumber, pageSize));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            (_orderService as IDisposable)?.Dispose();
            (_customerService as IDisposable)?.Dispose();
            (_medicineService as IDisposable)?.Dispose();
        }
    }
}
