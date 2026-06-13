using Newtonsoft.Json;
using PagedList;
using PharmaCO.Models;
using PharmaCO.Services;
using PharmaCO.ViewModels;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PharmaCO.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IMedicineService _medicineService;

        public OrderController()
        {
            _orderService = new OrderService();
            _customerService = new CustomerService();
            _medicineService = new MedicineService();
        }

        // INDEX: Owner + SalesMan
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult Index(string search, string sortOrder, string customerFilter, int page = 1, int pageSize = 10)
        {
            var orders = _orderService.GetAllOrders();

            if (!string.IsNullOrEmpty(search))
            {
                orders = orders.Where(o =>
                    (o.Customer != null && o.Customer.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    o.OrderId.ToString().IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrEmpty(customerFilter))
            {
                orders = orders.Where(o => o.Customer != null && o.Customer.Name == customerFilter);
            }

            ViewBag.SortOrder = sortOrder;
            switch (sortOrder)
            {
                case "date_desc":
                    orders = orders.OrderByDescending(o => o.OrderDate);
                    break;
                case "customer":
                    orders = orders.OrderBy(o => o.Customer.Name);
                    break;
                case "customer_desc":
                    orders = orders.OrderByDescending(o => o.Customer.Name);
                    break;
                default:
                    orders = orders.OrderBy(o => o.OrderDate);
                    break;
            }

            var vmList = orders.Select(o => new OrderViewModel
            {
                OrderId = o.OrderId,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.Name ?? string.Empty,
                OrderDate = o.OrderDate,
                IsPaid = o.IsPaid,   
                Items = o.OrderItems?.Select(i => new OrderItemViewModel
                {
                    OrderItemId = i.OrderItemId,
                    MedicineId = i.MedicineId,
                    MedicineName = i.Medicine?.Name ?? string.Empty,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice,
                    ImagePath = i.Medicine?.ImagePath ?? string.Empty
                }).ToList() ?? new List<OrderItemViewModel>()
            });

            var pagedOrders = vmList.ToPagedList(page, pageSize);
            return View(pagedOrders);
        }


        // DETAILS: Owner + SalesMan
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult Details(int id)
        {
            var o = _orderService.GetOrderById(id);
            if (o == null) return HttpNotFound();

            var vm = new OrderViewModel
            {
                OrderId = o.OrderId,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.Name ?? string.Empty,
                OrderDate = o.OrderDate,
                IsPaid = o.IsPaid,
                Items = o.OrderItems?.Select(i => new OrderItemViewModel
                {
                    OrderItemId = i.OrderItemId,
                    MedicineId = i.MedicineId,
                    MedicineName = i.Medicine?.Name ?? string.Empty,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice,
                    ImagePath = i.Medicine?.ImagePath ?? string.Empty
                }).ToList() ?? new List<OrderItemViewModel>()
            };

            return View(vm);
        }

        // CREATE GET: Owner + SalesMan
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult Create()
        {
            PopulateDropdowns();
            var vm = new OrderViewModel
            {
                OrderDate = DateTime.Today,
                Items = new List<OrderItemViewModel> { new OrderItemViewModel { Quantity = 1 } }
            };
            return View(vm);
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult Create(OrderViewModel vm)
        {
            if (vm.Items == null || !vm.Items.Any(i => i != null && i.MedicineId > 0))
            {
                ModelState.AddModelError("", "Please add at least one medicine to the order.");
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(vm);
            }

            var order = new Order
            {
                CustomerId = vm.CustomerId,
                OrderDate = vm.OrderDate,
                IsPaid = vm.IsPaid,
                OrderItems = new List<OrderItem>()
            };

            foreach (var itemVm in vm.Items.Where(i => i != null && i.MedicineId > 0))
            {
                var medicine = _medicineService.GetMedicineById(itemVm.MedicineId);
                if (medicine == null)
                {
                    ModelState.AddModelError("", $"Medicine with id {itemVm.MedicineId} not found.");
                    continue;
                }

                var qty = itemVm.Quantity <= 0 ? 1 : itemVm.Quantity;
                var unitPrice = medicine.Price;
                var totalPrice = unitPrice * qty;

                order.OrderItems.Add(new OrderItem
                {
                    MedicineId = itemVm.MedicineId,
                    Quantity = qty,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice
                });
            }

            if (!order.OrderItems.Any())
            {
                ModelState.AddModelError("", "No valid order items to save.");
                PopulateDropdowns();
                return View(vm);
            }

            _orderService.AddOrder(order);
            return RedirectToAction("Index");
        }

        // EDIT GET: only Owner
        [Authorize(Roles = "Owner")]
        public ActionResult Edit(int id)
        {
            var o = _orderService.GetOrderById(id);
            if (o == null) return HttpNotFound();

            var vm = new OrderViewModel
            {
                OrderId = o.OrderId,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.Name ?? string.Empty,
                OrderDate = o.OrderDate,
                IsPaid = o.IsPaid,
                Items = o.OrderItems?.Select(i => new OrderItemViewModel
                {
                    OrderItemId = i.OrderItemId,
                    MedicineId = i.MedicineId,
                    MedicineName = i.Medicine?.Name ?? string.Empty,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice,
                    ImagePath = string.IsNullOrEmpty(i.Medicine?.ImagePath)
    ? string.Empty
    : Url.Content(i.Medicine.ImagePath)
                }).ToList() ?? new List<OrderItemViewModel>()
            };

            PopulateDropdowns();
            return View(vm);
        }

        // EDIT POST: only Owner
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public ActionResult Edit(OrderViewModel vm)
        {
            if (vm.Items == null || !vm.Items.Any(i => i != null && i.MedicineId > 0))
            {
                ModelState.AddModelError("", "Please add at least one medicine to the order.");
            }

            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(vm);
            }

            // Build a new Order DTO (do NOT modify tracked entities here)
            var orderDto = new Order
            {
                OrderId = vm.OrderId,
                CustomerId = vm.CustomerId,
                OrderDate = vm.OrderDate,
                IsPaid = vm.IsPaid,
                OrderItems = new List<OrderItem>()
            };

            foreach (var itemVm in vm.Items.Where(i => i != null && i.MedicineId > 0))
            {
                var medicine = _medicineService.GetMedicineById(itemVm.MedicineId);
                if (medicine == null) continue;

                var qty = itemVm.Quantity <= 0 ? 1 : itemVm.Quantity;
                var unitPrice = medicine.Price;
                var totalPrice = unitPrice * qty;

                // Create NEW OrderItem instances (do not reuse incoming VM objects)
                orderDto.OrderItems.Add(new OrderItem
                {
                    // Note: do not set OrderItemId here for new items (leave default 0)
                    MedicineId = itemVm.MedicineId,
                    Quantity = qty,
                    UnitPrice = unitPrice,
                    TotalPrice = totalPrice
                });
            }

            if (!orderDto.OrderItems.Any())
            {
                ModelState.AddModelError("", "No valid order items to save.");
                PopulateDropdowns();
                return View(vm);
            }

            // Let the service load the existing Order, delete old items and add these new ones
            _orderService.UpdateOrder(orderDto);

            return RedirectToAction("Index");
        }
        // DELETE GET: only Owner
        [Authorize(Roles = "Owner")]
        public ActionResult Delete(int id)
        {
            var o = _orderService.GetOrderById(id);
            if (o == null) return HttpNotFound();

            var vm = new OrderViewModel
            {
                OrderId = o.OrderId,
                CustomerId = o.CustomerId,
                CustomerName = o.Customer?.Name ?? string.Empty,
                OrderDate = o.OrderDate,
                Items = o.OrderItems?.Select(i => new OrderItemViewModel
                {
                    OrderItemId = i.OrderItemId,
                    MedicineId = i.MedicineId,
                    MedicineName = i.Medicine?.Name ?? string.Empty,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice,
                    ImagePath = i.Medicine?.ImagePath ?? string.Empty
                }).ToList() ?? new List<OrderItemViewModel>()
            };

            return View(vm);
        }

        // DELETE POST: only Owner
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public ActionResult DeleteConfirmed(int id)
        {
            _orderService.DeleteOrder(id);
            return RedirectToAction("Index");
        }

        // AJAX: Autocomplete customer search
        [Authorize(Roles = "Owner,SalesMan")]
        public JsonResult SearchCustomers(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }

            var customers = _customerService.GetAllCustomers()
                .Where(c => c.Name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(c => new
                {
                    id = c.CustomerId,
                    label = c.Name,
                    value = c.Name
                })
                .ToList();

            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        // AJAX: Autocomplete medicine search with price + image
        [Authorize(Roles = "Owner,SalesMan")]

        public JsonResult SearchMedicines(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);

            var medicines = _medicineService.GetAllMedicines()
                .Where(m => m.Name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                .Select(m => new
                {
                    id = m.MedicineId,
                    label = m.Name,
                    value = m.Name,
                    price = m.Price,
                    imagePath = string.IsNullOrEmpty(m.ImagePath)
                        ? Url.Content("~/images/no-image.png")
                        : (m.ImagePath.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                            ? m.ImagePath
                            : Url.Content(m.ImagePath.StartsWith("~") ? m.ImagePath : ("~/images/" + m.ImagePath)))
                })
                .ToList();

            return Json(medicines, JsonRequestBehavior.AllowGet);
        }


        // Returns a partial row HTML for adding a new item row dynamically
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult GetItemRow(int index)
        {
            PopulateDropdowns();
            var vm = new OrderItemViewModel { Quantity = 1 };
            ViewData["Index"] = index;
            return PartialView("_OrderItemRow", vm);
        }

        // Invoice PDF using Rotativa
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult Invoice(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null) return HttpNotFound();

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var vm = new OrderViewModel
            {
                OrderId = order.OrderId,
                CustomerName = order.Customer?.Name ?? string.Empty,
                OrderDate = order.OrderDate,
                Items = order.OrderItems.Select(i => new OrderItemViewModel
                {
                    MedicineName = i.Medicine?.Name ?? string.Empty,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice,
                    ImagePath = string.IsNullOrEmpty(i.Medicine?.ImagePath) ? null : baseUrl + i.Medicine.ImagePath
                }).ToList()
            };

            return new ViewAsPdf("Invoice", vm)
            {
                FileName = $"Invoice_{order.OrderId}.pdf"
            };
        }

        // Helper: populate dropdowns and medInfo JSON
        private void PopulateDropdowns()
        {
            var customers = _customerService.GetAllCustomers()
                .Select(c => new { c.CustomerId, c.Name })
                .ToList();

            var medicines = _medicineService.GetAllMedicines(); // replace with _medicineService.GetAllMedicines()

            ViewBag.CustomerList = new SelectList(customers, "CustomerId", "Name");
            ViewBag.MedicineList = new SelectList(medicines.Select(m => new { m.MedicineId, m.Name }), "MedicineId", "Name");

            var medInfo = medicines.ToDictionary(
                m => m.MedicineId.ToString(),
                m => new { price = m.Price, imagePath = m.ImagePath ?? string.Empty, name = m.Name }
            );

            ViewBag.MedicineInfo = JsonConvert.SerializeObject(medInfo);
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
