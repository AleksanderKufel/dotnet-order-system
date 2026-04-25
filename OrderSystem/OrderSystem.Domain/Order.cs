using System;
using System.Collections.Generic;
using System.Text;

namespace OrderSystem.Domain
{
    public class Order
    {
        public Guid Id { get; private set; }
        public string CustomerEmail { get; private set; }
        public decimal Amount { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private Order() { }

        public Order(string customerEmail, decimal amount)
        {
            Id = Guid.NewGuid();
            CustomerEmail = customerEmail;
            Amount = amount;
            Status = OrderStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsProcessed()
        {
            Status = OrderStatus.Processed;
        }

        public void MarkAsFailed()
        {
            Status = OrderStatus.Failed;
        }
    }
}
