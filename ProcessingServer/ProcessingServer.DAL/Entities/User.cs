using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Entities
{
    public class User
    {
        public string UserId { get; set; }
        public string Name { get; set; } // First name + Last name OR Company name
        public string Type { get; set; } // Possible values: Person, Company
        public string PhoneNumber { get; set; }
    }
}
