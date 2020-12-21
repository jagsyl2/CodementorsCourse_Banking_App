using BankTransfers.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank.ListOfTransfersOperations
{
    public class TotalValueOfTransfersSentToSpecificBankAccounts : ValueOfOutgoingTransfers
    {
        public override void PrintListOfOperation(List<SelectItem> sumOfTransfersSent)
        {
            Console.WriteLine("Total value of transfers sent to specific bank accounts:");
            foreach (var sum in sumOfTransfersSent)
            {
                Console.WriteLine($"From: {sum.SourceAccount} in total {sum.SumOfTransfers}$ was sent to account: {sum.TargetAccount}");
            }
        }

        public override List<SelectItem> SumOfTransfersSent(List<Transfer> listOfCustomerTransfers)
        {
            var sumOfTransfersSent = listOfCustomerTransfers
                .GroupBy(x => new { x.SourceAccount, x.TargetAccount })
                .Select(g => new SelectItem
                        {
                            SourceAccount = g.Key.SourceAccount,
                            TargetAccount = g.Key.TargetAccount,
                            SumOfTransfers = g.Sum(x => x.Amount)
                        })
                .OrderBy(x => x.SourceAccount)
                .ToList();

            return sumOfTransfersSent;
        }
    }
}
