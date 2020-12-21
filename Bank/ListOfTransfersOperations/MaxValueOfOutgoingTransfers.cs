using BankTransfers.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank.ListOfTransfersOperations
{
    public class MaxValueOfOutgoingTransfers : SingleValueOfOutgoingTransfers
    {
        public override void SelectedAndPrintValueOfTransfersSent(List<Transfer> listOfCustomerTransfers)
        {
            var maxValoue = listOfCustomerTransfers.Max(x => x.Amount);

            Console.WriteLine($"The highest value of an outgoing transfer: {maxValoue}$"); 
        }
    }
}
