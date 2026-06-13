using PharmaCO.Models;
using PharmaCO.Services;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace PharmaCO.Controllers
{
    [Authorize(Roles = "Owner,SalesMan")] // ✅ Controller level authorize
    public class MedicineController : Controller
    {
        private readonly IMedicineService _service;

        public MedicineController()
        {
            _service = new MedicineService();
        }

        // INDEX: Owner + SalesMan দুজনেই দেখতে পারবে
        public ActionResult Index(string search, string categoryFilter, string companyFilter, string sortOrder, int page = 1, int pageSize = 10)
        {
            var medicines = _service.GetAllMedicines().AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                medicines = medicines.Where(m => m.Name.Contains(search) || m.CompanyName.Contains(search));
            }

            // Filter by Category
            if (!string.IsNullOrEmpty(categoryFilter))
            {
                medicines = medicines.Where(m => m.Category == categoryFilter);
            }

            // Filter by Company
            if (!string.IsNullOrEmpty(companyFilter))
            {
                medicines = medicines.Where(m => m.CompanyName == companyFilter);
            }

            // Sort
            ViewBag.SortOrder = sortOrder;
            switch (sortOrder)
            {
                case "name_desc":
                    medicines = medicines.OrderByDescending(m => m.Name);
                    break;
                case "price":
                    medicines = medicines.OrderBy(m => m.Price);
                    break;
                case "price_desc":
                    medicines = medicines.OrderByDescending(m => m.Price);
                    break;
                case "company":
                    medicines = medicines.OrderBy(m => m.CompanyName);
                    break;
                case "company_desc":
                    medicines = medicines.OrderByDescending(m => m.CompanyName);
                    break;
                default:
                    medicines = medicines.OrderBy(m => m.Name);
                    break;
            }

            return View(medicines.ToPagedList(page, pageSize));
        }

        // DETAILS
        public ActionResult Details(int id)
        {
            var medicine = _service.GetMedicineById(id);
            if (medicine == null) return HttpNotFound();
            return View(medicine);
        }

        // CREATE: শুধু Owner পারবে
        [Authorize(Roles = "Owner")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public ActionResult Create(Medicine medicine, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // image upload logic
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    imageFile.SaveAs(path);
                    medicine.ImagePath = "~/Images/" + fileName;
                }

                _service.AddMedicine(medicine);
                return RedirectToAction("Index");
            }
            return View(medicine);
        }

        // EDIT: শুধু Owner পারবে
        [Authorize(Roles = "Owner")]
        public ActionResult Edit(int id)
        {
            var medicine = _service.GetMedicineById(id);
            if (medicine == null) return HttpNotFound();
            return View(medicine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public ActionResult Edit(Medicine medicine, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                // image upload logic
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);
                    imageFile.SaveAs(path);
                    medicine.ImagePath = "~/Images/" + fileName;
                }

                _service.UpdateMedicine(medicine);
                return RedirectToAction("Index");
            }
            return View(medicine);
        }

        // DELETE: শুধু Owner পারবে
        [Authorize(Roles = "Owner")]
        public ActionResult Delete(int id)
        {
            var medicine = _service.GetMedicineById(id);
            if (medicine == null) return HttpNotFound();
            return View(medicine);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public ActionResult DeleteConfirmed(int id)
        {
            _service.DeleteMedicine(id);
            return RedirectToAction("Index");
        }
    }
}
