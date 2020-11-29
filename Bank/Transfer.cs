using System;
using System.Collections.Generic;
using System.Text;

namespace Bank
{
    class Transfer
    {
        public string TransferTitle;
        public double TransferAmount;
        public DateTime DateOfTheTransfer;
        public string TypOfTransfer;
        public Guid SourceAccount;
        public Guid TargetAccount;
    }
}
