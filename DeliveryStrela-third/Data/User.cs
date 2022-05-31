using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryStrela.Data
{
    public class User:IdentityUser
    {
        public string FullName{ get; set; }

        public RoleType Role { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
