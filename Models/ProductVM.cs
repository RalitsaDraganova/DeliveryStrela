using DeliveryStrela.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryStrela.Models
{
    public class ProductVM
    {
        public int Id { get; set; }

        public int CatalogNumber { get; set; }

        public string Name { get; set; }

        public string Diet { get; set; }

        public double Grams { get; set; }

        public decimal Price { get; set; }

        public CategoryKind Category { get; set; }

        public string Description { get; set; } 

        public string IdUser { get; set; }

        public int Count { get; set; }
    }
}
