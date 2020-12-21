using BankTransfers.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank.ListOfTransfersOperations
{
    public class TotalValueOfOutgoingTransfers : ValueOfOutgoingTransfers
    {
        public override void PrintListOfOperation(List<SelectItem> sumOfTransfersSent)
        {
            Console.WriteLine("Total value of outgoing transfers:");
            foreach (var sum in sumOfTransfersSent)
            {
                Console.WriteLine($"From the account: {sum.SourceAccount} total value of outgoing transfers is {sum.SumOfTransfers}$");
            }
        }

        public override List<SelectItem> SumOfTransfersSent(List<Transfer> listOfCustomerTransfers)
        {
            var sumOfTransfersSent = listOfCustomerTransfers
                .GroupBy(x => new { x.SourceAccount })
                .Select(g => new SelectItem
                        {
                            SourceAccount = g.Key.SourceAccount,
                            SumOfTransfers = g.Sum(x => x.Amount)
                        })
                //.OrderBy(x => x.SourceAccount)
                .ToList();

            return sumOfTransfersSent;
        }
    }
}