using PharmaCO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmaCO.Services
{
    public interface IMedicineService
    {
     
            IEnumerable<Medicine> GetAllMedicines();
            Medicine GetMedicineById(int id);
            void AddMedicine(Medicine medicine);
            void UpdateMedicine(Medicine medicine);
            void DeleteMedicine(int id);
        
    }
}
