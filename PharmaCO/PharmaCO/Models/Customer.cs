using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PharmaCO.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Range(1, 120)]
        public int Age { get; set; }

        [Required]
        public string Contact { get; set; }

        public string Address { get; set; }

        // Relation: One Customer can place many Orders
        public ICollection<Order> Orders { get; set; }
    }
}