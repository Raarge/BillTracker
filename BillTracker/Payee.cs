using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillTracker
{
    public class Payee
    {
        public string PayeeName { get; set; }
        public int DateDue { get; set; }
        public decimal Amountdue { get; set; }
        public string URL { get; set; }

    }
}
