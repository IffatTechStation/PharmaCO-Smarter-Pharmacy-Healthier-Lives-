using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PharmaCO.ViewModels
{
    public class SalesReportViewModel
    {
        public DateTime Date { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalAmount { get; set; }
    }
}