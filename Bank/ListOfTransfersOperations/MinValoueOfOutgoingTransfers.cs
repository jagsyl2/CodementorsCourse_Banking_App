using BankTransfers.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bank.ListOfTransfersOperations
{
    public class MinValueOfOutgoingTransfers : SingleValueOfOutgoingTransfers
    {
        public override void SelectedAndPrintValueOfTransfersSent(List<Transfer> listOfCustomerTransfers)
        {
            var minValoue = listOfCustomerTransfers.Min(x => x.Amount);

            Console.WriteLine($"The lowest value of an outgoing transfer: {minValoue}$");
        }
    }
   
}
