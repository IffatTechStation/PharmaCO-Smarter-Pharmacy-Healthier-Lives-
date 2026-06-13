using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PharmaCO.Models
{
    public class Medicine
    {
        public int MedicineId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; }

        [Required, Range(1, 10000)]
        public decimal Price { get; set; }

        [Required]
        public int Stock { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        public string ImagePath { get; set; }

        // ✅ নতুন property: Company Name
        [Required, StringLength(100)]
        public string CompanyName { get; set; }

        // Relation: One Medicine can be in many Orders
        public ICollection<Order> Orders { get; set; }
    }
}
