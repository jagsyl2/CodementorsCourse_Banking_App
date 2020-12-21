using BankTransfers.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank.ListOfTransfersOperations
{
    public class AverageValouOfOutgoingTransfers : SingleValueOfOutgoingTransfers
    {
        public override void SelectedAndPrintValueOfTransfersSent(List<Transfer> listOfCustomerTransfers)
        {
            var averageValoue = listOfCustomerTransfers.Average(x => x.Amount);

            Console.WriteLine($"The average value of an outgoing transfer: {averageValoue}$");
        }
    }
}
