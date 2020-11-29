using System;

namespace BankTransfers.DataLayer.Models
{
    public class Transfer
    {
        public string Title;
        public double Amount;
        public DateTime DateOfTheTransfer;
        public string TypOfTransfer;
        public Guid SourceAccount;
        public Guid TargetAccount;
    }
}
