using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmaCO.ViewModels
{
    public class CustomerReportViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
