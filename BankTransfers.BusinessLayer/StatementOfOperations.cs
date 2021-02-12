using System;
using System.Collections.Generic;

namespace BankTransfers.BusinessLayer
{
    public class StatementOfOperations
    {
        public Guid ForAccount;
        public IEnumerable<SumItem> TotalValueOutgoingTransfersToSpecificAccounts;
        public IEnumerable<SumItem> TotalValueIncomingTransfersFromSpecificAccounts;
        public double TotalValueOutgoingTransfers;
        public double TotalValueIncomingTransfers;
        public double HigestValueOutgoingTransfers;
        public double LowestValueOutgoingTransfers;
        public double AverageValueOutgoingTransfers;
    }
}