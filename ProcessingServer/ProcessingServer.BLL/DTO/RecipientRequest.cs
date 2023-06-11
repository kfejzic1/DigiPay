using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.DTO
{
    public class RecipientRequest
    {
        public string Name { get; set; } // First name + Last name OR Company name
        public string AccountNumber { get; set; }
     
    }
}
