using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZCashWedge
{
    public class EzCashResponse
    {
        public EzCashResponse()
        {
            Success = false;
            FailureString = string.Empty;
            Barcode = null;
            Reprint = false;
            CardStatus = string.Empty;
            InitialAmount = 0m;
            AvailableAmount = 0m;
            PartialPayPaidAmount = 0m;
            PartialPayTotal = 0m;
            PaymentNumber = string.Empty;
        }

        public bool Success { get; set; }

        public bool Rejected { get; set; }

        public bool Failed { get; set; }

        public bool PartialPay { get; set; }

        public string Barcode { get; set; }

        public string FailureString { get; set; }

        public decimal PartialPayPaidAmount { get; set; }

        public decimal PartialPayTotal { get; set; }

        public bool Reprint { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal AvailableAmount { get; set; }
        public string CardStatus { get; set; }
        public string PaymentNumber { get; set; }
        public long TranID { get; set; }
        public string amount { get; set; }

    }
}
