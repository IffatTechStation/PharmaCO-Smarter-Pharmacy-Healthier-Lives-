using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmaCO.ViewModels
{
    public class MedicineViewModel
    {
        public int MedicineId { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public int ReorderLevel { get; set; }
    }
}