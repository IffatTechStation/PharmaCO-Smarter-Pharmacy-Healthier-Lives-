using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PharmaCO.Models
{
    public class PharmaCODb : IdentityDbContext<ApplicationUser>
    {
        public PharmaCODb() : base("DefaultConnection") { }

        // Static Create method for OWIN
        public static PharmaCODb Create()
        {
            return new PharmaCODb();
        }

        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }
    }
}