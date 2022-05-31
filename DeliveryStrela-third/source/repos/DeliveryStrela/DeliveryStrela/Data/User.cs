using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryStrela.Data
{
    public class User
    {
        public int Id { get; set; }

        public string FullName{ get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public RoleType Role { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
