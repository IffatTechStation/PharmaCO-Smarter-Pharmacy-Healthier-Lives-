using System.Linq;
using System.Web.Mvc;
using PharmaCO.Models;
using PharmaCO.Services;
using PagedList;

namespace PharmaCO.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _service;

        public CustomerController()
        {
            _service = new CustomerService();
        }

        // Index: Owner + SalesMan দুজনেই দেখতে পারবে
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult Index(string search, string sortOrder, int page = 1, int pageSize = 10)
        {
            var customers = _service.GetAllCustomers();

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                customers = customers.Where(c => c.Name.Contains(search));
            }

            // Sort
            ViewBag.SortOrder = sortOrder;
            switch (sortOrder)
            {
                case "name_desc":
                    customers = customers.OrderByDescending(c => c.Name);
                    break;
                default:
                    customers = customers.OrderBy(c => c.Name);
                    break;
            }

            // Pagination
            return View(customers.ToPagedList(page, pageSize));
        }

        // Details: Owner + SalesMan দুজনেই দেখতে পারবে
        [Authorize(Roles = "Owner,SalesMan")]
        public ActionResult Details(int id)
        {
            var customer = _service.GetCustomerById(id);
            if (customer == null) return HttpNotFound();
            return View(customer);
        }

        // Create: শুধু Owner পারবে
        [Authorize(Roles = "Owner")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public ActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _service.AddCustomer(customer);
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // Edit: শুধু Owner পারবে
        [Authorize(Roles = "Owner")]
        public ActionResult Edit(int id)
        {
            var customer = _service.GetCustomerById(id);
            if (customer == null) return HttpNotFound();
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public ActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _service.UpdateCustomer(customer);
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // Delete: শুধু Owner পারবে
        [Authorize(Roles = "Owner")]
        public ActionResult Delete(int id)
        {
            var customer = _service.GetCustomerById(id);
            if (customer == null) return HttpNotFound();
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public ActionResult DeleteConfirmed(int id)
        {
            _service.DeleteCustomer(id);
            return RedirectToAction("Index");
        }
    }
}
