using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillTracker
{
    public class Payment
    {
        public string PayeeName { get; set; }
        public decimal AmountDue { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime DateDue { get; set; }
        public string ConfirmationNumber { get; set; }
    }
}
