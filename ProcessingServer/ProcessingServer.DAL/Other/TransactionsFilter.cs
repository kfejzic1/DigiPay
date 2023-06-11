using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.DAL.Other
{
    public class TransactionsFilter
    {
        public string? AmountStartFilter { get; set; }
        public string? AmountEndFilter { get; set; }
        public string? CurrencyFilter { get; set; }
        public string? TransactionTypeFilter { get; set; }
        public string? RecipientNameFilter { get; set; }
        public string? RecipientAccountNumberFilter { get; set; }
        public string? SenderNameFilter { get; set; }
        public string? CreatedAtStartFilter { get; set; }
        public string? CreatedAtEndFilter { get; set; }
        public string? CategoryFilter { get; set; }
    }
}