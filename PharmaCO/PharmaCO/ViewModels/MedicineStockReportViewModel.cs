using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmaCO.ViewModels
{
    public class MedicineStockReportViewModel
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImagePath { get; set; }
    }
}