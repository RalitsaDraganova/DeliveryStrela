using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryStrela.Models
{
    public class OrderDetailsVM
    {
        public int Id { get; set; }

        public int IdOrder { get; set; }

        public int IdProduct { get; set; }
    }
}
