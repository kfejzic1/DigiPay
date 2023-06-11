using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Other
{
    public class VendorTransfer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string CompanyDetails { get; set; }
        public List<UserTransfer> AssignedUsers { get; set; }

    }
}
