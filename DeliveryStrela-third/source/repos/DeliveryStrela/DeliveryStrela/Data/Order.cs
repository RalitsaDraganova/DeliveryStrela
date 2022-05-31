using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryStrela.Data
{
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int OrderDetailsId { get; set; }
        public virtual OrdersDetails OrdersDetails { get; set; }

        public ICollection<OrdersDetails> Details { get; set; }
        public int Count { get; set; }

        public DateTime OrderOn { get; set; }

    }
}

