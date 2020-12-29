using BankTransfers.BusinessLayer;
using BankTransfers.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank
{
    public class IoStatementOfOperationsHelper
    {
        private TransfersService _transfersService = new TransfersService();

        public StatementOfOperations GetStatementOfOperationForAccount(Account account)
        {
            var listOfOutgoingTransfers = _transfersService.GetTransfersOutgoingFromTheAccount(account);
            var listOfIncomingTransfers = _transfersService.GetIncomingTransfersToTheAccount(account);

            StatementOfOperations statementOfOperationForAccount = new StatementOfOperations
            {
                ForAccount = account.Number,
                TotalValueOutgoingTransfersToSpecificAccounts = CountAndPrintSumOfTransfersSentToSpecificAccounts(listOfOutgoingTransfers, account.Number),
                TotalValueIncomingTransfersFromSpecificAccounts = CountAndPrintSumOfTransfersIncomingFromSpecificAccounts(listOfIncomingTransfers, account.Number),
                TotalValueOutgoingTransfers = CountAndPrintSumOfTransfers(listOfOutgoingTransfers, "outgoing"),
                TotalValueIncomingTransfers = CountAndPrintSumOfTransfers(listOfIncomingTransfers, "incoming"),
                HigestValueOutgoingTransfers = GetAndPrintMaxValoueOfOutgoingTransfers(listOfOutgoingTransfers),
                LowestValueOutgoingTransfers = GetAndPrintMinValoueOfOutgoingTransfers(listOfOutgoingTransfers),
                AverageValueOutgoingTransfers = GetAndPrintAverageValoueOfOutgoingTransfers(listOfOutgoingTransfers),
            };

            return statementOfOperationForAccount;
        }

        public IEnumerable<SumItem> CountAndPrintSumOfTransfersSentToSpecificAccounts(List<Transfer> listOfOutgoingTransfers, Guid number)
        {
            var sumOfTransfers = listOfOutgoingTransfers
                .GroupBy(x => new { x.TargetAccount })
                .Select(g => new SumItem
                    {
                        SourceAccount = number,
                        TargetAccount = g.Key.TargetAccount,
                        SumOfTransfers = g.Sum(x => x.Amount)
                    });

            Console.WriteLine();
            Console.WriteLine("\tTotal value of transfers sent to specific bank accounts:");

            foreach (var sum in sumOfTransfers)
            {
                Console.WriteLine($"\t- in total {sum.SumOfTransfers}$ was sent to the account: {sum.TargetAccount}");
            }

            return sumOfTransfers;
        }

        public IEnumerable<SumItem> CountAndPrintSumOfTransfersIncomingFromSpecificAccounts(List<Transfer> listOfIncomingTransfers, Guid number)
        {
            var sumOfTransfers = listOfIncomingTransfers
                .GroupBy(y => new { y.SourceAccount })
                .Select(g =>
                    new SumItem
                    {
                        SourceAccount = g.Key.SourceAccount,
                        TargetAccount = number,
                        SumOfTransfers = g.Sum(x => x.Amount)
                    });

            Console.WriteLine();
            Console.WriteLine("\tTotal value of transfers received from specific bank accounts:");

            foreach (var sum in sumOfTransfers)
            {
                Console.WriteLine($"\t- a total of {sum.SumOfTransfers}$ was received from the account: {sum.SourceAccount}");
            }

            return sumOfTransfers;
        }

        public double CountAndPrintSumOfTransfers(List<Transfer> list, string typeOfTransfers)
        {
            var sumOfTransfers = list
                .DefaultIfEmpty()
                .Sum(x => x == null ? 0 : x.Amount);

            Console.WriteLine($"\tTotal value of {typeOfTransfers} transfers is {sumOfTransfers}.");

            return sumOfTransfers;
        }

        public double GetAndPrintMaxValoueOfOutgoingTransfers(List<Transfer> list)
        {
            var maxValoue = list
                .DefaultIfEmpty()
                .Max (x => x == null ? 0 : x.Amount);

            Console.WriteLine($"\tThe highest value of an outgoing transfer: {maxValoue}$");

            return maxValoue;
        }

        public double GetAndPrintMinValoueOfOutgoingTransfers(List<Transfer> list)
        {
            var minValoue = list
                .DefaultIfEmpty()
                .Min(x => x == null ? 0 : x.Amount);

            Console.WriteLine($"\tThe lowest value of an outgoing transfer: {minValoue}$");

            return minValoue;
        }

        public double GetAndPrintAverageValoueOfOutgoingTransfers(List<Transfer> list)
        {
            var averageValoue = Math.Round(list
                .DefaultIfEmpty()
                .Average(x => x == null ? 0 : x.Amount), 2);

            Console.WriteLine($"\tThe average value of an outgoing transfer: {averageValoue}$");

            return averageValoue;
        }
    }
}
