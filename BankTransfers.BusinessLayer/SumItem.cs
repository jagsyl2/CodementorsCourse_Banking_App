using System;

namespace BankTransfers.BusinessLayer
{ 
    public class SumItem
    {
        public Guid SourceAccount;
        public Guid TargetAccount;
        public double SumOfTransfers;
    }
}
