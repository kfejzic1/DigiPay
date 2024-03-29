﻿using ProcessingServer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.BLL.DTO
{
    public class NonPersonalBankAccountResponse
    {
        public string AccountNumber { get; set; }
        public string Currency { get; set; }
        public string BankName { get; set; }
        public User Owner { get; set; }
    }
}
