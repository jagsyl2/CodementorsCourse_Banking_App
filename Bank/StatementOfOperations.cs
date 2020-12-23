using Bank.ListOfTransfersOperations;
using System;
using System.Collections.Generic;

namespace Bank
{
    public class StatementOfOperations
    {
        public List<dupa> TotalValueOutgoingTransfersToSpecificAccounts;
        public List<dupa> TotalValueIncomingTransfersFromSpecificAccounts;
        public List<dupa2> TotalValueOutgoingTransfers;
        public List<dupa2> TotalValueIncomingTransfers;
        public List<dupa2> HigestValueOutgoingTransfers;
        public List<dupa2> LowestValueOutgoingTransfers;
        public List<dupa2> AverageValueOutgoingTransfers;



        //public StatementOfOperations(int customerId)
        //{
        //    TotalValueOutgoingTransfersToSpecificAccounts = _totalValueOfTransfers.PrintTotalValueOfOutgoingTransfers();
        //}




    }

    public class dupa
    {
        public Guid SourceAccount;
        public Guid TargetAccount;
        public double SumOfTransfers;
    }

    public class dupa2
    {
        public Guid Account;
        public double Value;
    }






}
