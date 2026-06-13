using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PharmaCO.ViewModels
{
    public class OrderItemViewModel
    {
        public int OrderItemId { get; set; }

        [Required(ErrorMessage = "Medicine is required")]
        public int MedicineId { get; set; }

        public string MedicineName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;

        [Range(0, double.MaxValue, ErrorMessage = "Unit Price must be non-negative")]
        public decimal UnitPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total Price must be non-negative")]
        public decimal TotalPrice { get; set; }

        // Optional: image path for preview
        public string ImagePath { get; set; }
    }
}
