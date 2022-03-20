using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryStrela.Data
{
    public class OrdersDetails
    {
        public int Id { get; set; }

        public int IdOrder { get; set; }
        public Order Order { get; set; }

        public int IdProduct { get; set; }
        public Product Product { get; set; }
       
    }
}
