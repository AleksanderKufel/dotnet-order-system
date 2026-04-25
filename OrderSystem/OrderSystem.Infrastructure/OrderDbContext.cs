using Microsoft.EntityFrameworkCore;
using OrderSystem.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderSystem.Infrastructure
{
    public class OrderDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options) { }
    }
}
