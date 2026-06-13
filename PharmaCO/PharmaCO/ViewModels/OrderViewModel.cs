using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PharmaCO.ViewModels
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }  

        [Required(ErrorMessage = "Customer is required")]
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Order Date is required")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        public bool IsPaid { get; set; } 

        [Required(ErrorMessage = "Items are required")]
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    }
}

