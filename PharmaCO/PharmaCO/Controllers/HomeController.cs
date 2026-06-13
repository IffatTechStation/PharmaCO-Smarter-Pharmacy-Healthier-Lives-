using PharmaCO.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PharmaCO.Services;
using Newtonsoft.Json;

namespace PharmaCO.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IMedicineService _medicineService;
        private readonly IOrderService _orderService;

        public HomeController()
        {
            _customerService = new CustomerService();
            _medicineService = new MedicineService();
            _orderService = new OrderService();
        }

        // ---------------- Home Page ----------------
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult Index()
        {
            ViewBag.CustomerCount = _customerService.GetAllCustomers().Count();
            ViewBag.MedicineCount = _medicineService.GetAllMedicines().Count();
            ViewBag.OrderCount = _orderService.GetAllOrders().Count();

            var recentOrders = _orderService.GetAllOrders()
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    CustomerName = o.Customer?.Name ?? string.Empty,
                    OrderDate = o.OrderDate,
                    Items = o.OrderItems.Select(i => new OrderItemViewModel
                    {
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.TotalPrice
                    }).ToList()
                }).ToList();

            ViewBag.RecentOrders = recentOrders;

            return View();
        }

        // ---------------- Owner Dashboard ----------------
        [Authorize(Roles = "Owner")]
        public ActionResult OwnerDashboard()
        {
            // Summary counts
            ViewBag.CustomerCount = _customerService.GetAllCustomers().Count();
            ViewBag.MedicineCount = _medicineService.GetAllMedicines().Count();
            ViewBag.OrderCount = _orderService.GetAllOrders().Count();

            // Low Stock Medicines (sorted ascending by stock)
            var lowStock = _medicineService.GetAllMedicines()
                .Where(m => m.Stock <= 10)
                .OrderBy(m => m.Stock)
                .Select(m => new MedicineViewModel
                {
                    MedicineId = m.MedicineId,
                    Name = m.Name,
                    Stock = m.Stock,
                }).ToList();

            ViewBag.LowStockMedicines = lowStock;
            ViewBag.LowStockCount = lowStock.Count; // extra card count

            // Sales Chart Data (last 6 months)
            var last6Months = Enumerable.Range(0, 6)
                .Select(i => DateTime.Today.AddMonths(-i))
                .OrderBy(d => d)
                .ToList();

            var salesMonths = last6Months.Select(d => d.ToString("MMM yyyy")).ToList();
            var salesData = last6Months
                .Select(d => _orderService.GetAllOrders()
                    .Count(o => o.OrderDate.Month == d.Month && o.OrderDate.Year == d.Year))
                .ToList();

            ViewBag.SalesMonthsJson = JsonConvert.SerializeObject(salesMonths);
            ViewBag.SalesDataJson = JsonConvert.SerializeObject(salesData);

            return View();
        }
        // ---------------- SalesMan Dashboard ----------------
        [Authorize(Roles = "SalesMan")]
        public ActionResult SalesManDashboard()
        {
            // মোট অর্ডার সংখ্যা
            ViewBag.OrderCount = _orderService.GetAllOrders().Count();

            // আজকের অর্ডার সংখ্যা
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            ViewBag.TodayOrders = _orderService.GetAllOrders()
                .Count(o => o.OrderDate >= today && o.OrderDate < tomorrow);


            // Recent Orders
            var recentOrders = _orderService.GetAllOrders()
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    CustomerName = o.Customer?.Name ?? string.Empty,
                    OrderDate = o.OrderDate,
                    Items = o.OrderItems.Select(i => new OrderItemViewModel
                    {
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.TotalPrice
                    }).ToList()
                }).ToList();

            ViewBag.RecentOrders = recentOrders;

            // Low Stock Medicines
            var lowStock = _medicineService.GetAllMedicines()
                .Where(m => m.Stock <= 10)
                .Select(m => new MedicineViewModel
                {
                    MedicineId = m.MedicineId,
                    Name = m.Name,
                    Stock = m.Stock,
                    ReorderLevel = 10
                }).ToList();

            ViewBag.LowStockMedicines = lowStock;

            // Sales Trend (Last 7 Days)
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-i))
                .OrderBy(d => d)
                .ToList();

            var trendDates = last7Days.Select(d => d.ToString("yyyy-MM-dd")).ToList();
            var trendCounts = last7Days
                .Select(d => _orderService.GetAllOrders()
                    .Count(o => o.OrderDate.Date == d))
                .ToList();

            ViewBag.SalesTrendDates = trendDates;
            ViewBag.SalesTrendCounts = trendCounts;

            return View();
        }

        // ---------------- About Page ----------------
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        // ---------------- Contact Page ----------------
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        // ---------------- Redirect after login ----------------
        public ActionResult RedirectToDashboard()
        {
            if (User.IsInRole("Owner"))
                return RedirectToAction("OwnerDashboard");
            else if (User.IsInRole("SalesMan"))
                return RedirectToAction("SalesManDashboard");

            return RedirectToAction("Index");
        }
    }
}
