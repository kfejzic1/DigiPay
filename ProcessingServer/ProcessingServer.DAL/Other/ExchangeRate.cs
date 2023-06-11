using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Other
{
    public class ExchangeRate
    {
        public string InputCurrency { get; set; }
        public string OutputCurrency { get; set; }
        public double Rate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
