using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryStrela.Models
{
    public class OrderVM
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int Count { get; set; }

        public DateTime OrderOn { get; set; }

    }
}
