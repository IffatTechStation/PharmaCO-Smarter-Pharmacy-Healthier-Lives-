using PharmaCO.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PharmaCO.Services
{
    public class OrderService : IOrderService
    {
        private readonly PharmaCODb _context;

        public OrderService()
        {
            _context = new PharmaCODb();
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders
                .Include("Customer")
                .Include("OrderItems.Medicine")
                .ToList();
        }

        public Order GetOrderById(int id)
        {
            return _context.Orders
                .Include("Customer")
                .Include("OrderItems.Medicine")
                .FirstOrDefault(o => o.OrderId == id);
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            // Load the tracked existing order with its items
            var existing = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.OrderId == order.OrderId);

            if (existing == null) return;

            // Update master fields
            existing.CustomerId = order.CustomerId;
            existing.OrderDate = order.OrderDate;
            existing.IsPaid = order.IsPaid;   // ✅ এখানে যোগ করো

            // Snapshot existing items to avoid collection-modified issues
            var oldItems = existing.OrderItems.ToList();

            // Explicitly delete old items so EF issues DELETE statements
            if (oldItems.Any())
            {
                _context.OrderItems.RemoveRange(oldItems);
            }

            // Add incoming items as brand-new entities
            var incoming = order.OrderItems ?? new List<OrderItem>();
            foreach (var inc in incoming)
            {
                if (inc == null || inc.MedicineId <= 0) continue;
                var qty = inc.Quantity <= 0 ? 1 : inc.Quantity;
                var unit = inc.UnitPrice;
                var total = inc.TotalPrice;

                var newItem = new OrderItem
                {
                    MedicineId = inc.MedicineId,
                    Quantity = qty,
                    UnitPrice = unit,
                    TotalPrice = total,
                    OrderId = existing.OrderId
                };
                _context.OrderItems.Add(newItem);
            }

            _context.SaveChanges();
        }









        public void DeleteOrder(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.OrderId == id);

            if (order != null)
            {
                // remove child items first if needed
                _context.OrderItems.RemoveRange(order.OrderItems);
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }
    }
}
