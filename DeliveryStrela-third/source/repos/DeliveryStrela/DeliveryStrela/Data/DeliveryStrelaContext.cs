using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DeliveryStrela.Data
{
    public class DeliveryStrelaContext:DbContext
    {
        //Constructors
        public DeliveryStrelaContext(DbContextOptions<DeliveryStrelaContext> options) : base(options)
        {

        }

        //Tables
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<OrdersDetails> OrdersDetails { get; set; }

    }
}
