using PharmaCO.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PharmaCO.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly PharmaCODb _context;

        public MedicineService()
        {
            _context = new PharmaCODb();
        }

        public IEnumerable<Medicine> GetAllMedicines()
        {
            return _context.Medicines.ToList();
        }

        public Medicine GetMedicineById(int id)
        {
            return _context.Medicines.Find(id);
        }

        public void AddMedicine(Medicine medicine)
        {
            _context.Medicines.Add(medicine);
            _context.SaveChanges();
        }

        public void UpdateMedicine(Medicine medicine)
        {
            _context.Entry(medicine).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteMedicine(int id)
        {
            var medicine = _context.Medicines.Find(id);
            if (medicine != null)
            {
                _context.Medicines.Remove(medicine);
                _context.SaveChanges();
            }
        }
    }
}
