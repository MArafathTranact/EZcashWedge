using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZCashWedge
{
    public class EzCashAPIRequest
    {
        public string payment_nbr { get; set; }
        public decimal amount { get; set; }
        public string date { get; set; }
        public string yard_id { get; set; }
        public string cashier_id { get; set; }
        public string device_id { get; set; }
        public string payee { get; set; }

    }
}
