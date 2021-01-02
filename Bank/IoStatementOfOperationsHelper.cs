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
        private StatementsOfOperationsService _statementsOfOperationsService = new StatementsOfOperationsService();

        public StatementOfOperations GetStatementOfOperationForAccount(Account account)
        {
            var listOfOutgoingTransfers = _transfersService.GetTransfersOutgoingFromTheAccount(account);
            var listOfIncomingTransfers = _transfersService.GetIncomingTransfersToTheAccount(account);

            StatementOfOperations statementOfOperationForAccount = new StatementOfOperations
            {
                ForAccount = account.Number,
                TotalValueOutgoingTransfersToSpecificAccounts = _statementsOfOperationsService.CountSumOfTransfersSentToSpecificAccounts(listOfOutgoingTransfers, account.Number),
                TotalValueIncomingTransfersFromSpecificAccounts = _statementsOfOperationsService.CountSumOfTransfersIncomingFromSpecificAccounts(listOfIncomingTransfers, account.Number),
                TotalValueOutgoingTransfers = _statementsOfOperationsService.CountSumOfTransfers(listOfOutgoingTransfers),
                TotalValueIncomingTransfers = _statementsOfOperationsService.CountSumOfTransfers(listOfIncomingTransfers),
                HigestValueOutgoingTransfers = _statementsOfOperationsService.GetMaxValoueOfTransfers(listOfOutgoingTransfers),
                LowestValueOutgoingTransfers = _statementsOfOperationsService.GetMinValoueOfTransfers(listOfOutgoingTransfers),
                AverageValueOutgoingTransfers = _statementsOfOperationsService.GetAverageValoueOfTransfers(listOfOutgoingTransfers),
            };

            return statementOfOperationForAccount;
        }

        public void PrintStatementOfOperations(Account account, StatementOfOperations statement)
        {
            Console.WriteLine();
            Console.WriteLine($"For the account {account.Name} number {account.Number}:");

            Console.WriteLine();
            Console.WriteLine($"\tTotal value of transfers sent to specific bank accounts:");
            foreach (var sum in statement.TotalValueOutgoingTransfersToSpecificAccounts)
            {
                Console.WriteLine($"\t- in total {sum.SumOfTransfers}$ was sent to the account: {sum.TargetAccount}");
            }

            Console.WriteLine();
            Console.WriteLine("\tTotal value of transfers received from specific bank accounts:");
            foreach (var sum in statement.TotalValueIncomingTransfersFromSpecificAccounts)
            {
                Console.WriteLine($"\t- a total of {sum.SumOfTransfers}$ was received from the account: {sum.SourceAccount}");
            }

            Console.WriteLine();
            Console.WriteLine($"\tTotal value of outgoing transfers is {statement.TotalValueOutgoingTransfers}.");
            Console.WriteLine($"\tTotal value of incoming transfers is {statement.TotalValueIncomingTransfers}.");

            Console.WriteLine($"\tThe highest value of an outgoing transfer: {statement.HigestValueOutgoingTransfers}$");
            Console.WriteLine($"\tThe lowest value of an outgoing transfer: {statement.LowestValueOutgoingTransfers}$");

            Console.WriteLine($"\tThe average value of an outgoing transfer: {statement.AverageValueOutgoingTransfers}$");
        }


        //public IEnumerable<SumItem> CountAndPrintSumOfTransfersSentToSpecificAccounts(List<Transfer> listOfOutgoingTransfers, Guid number)
        //{
        //    var sumOfTransfers = listOfOutgoingTransfers
        //        .GroupBy(x => new { x.TargetAccount })
        //        .Select(g => new SumItem
        //            {
        //                SourceAccount = number,
        //                TargetAccount = g.Key.TargetAccount,
        //                SumOfTransfers = g.Sum(x => x.Amount)
        //            });

        //    Console.WriteLine();
        //    Console.WriteLine("\tTotal value of transfers sent to specific bank accounts:");

        //    foreach (var sum in sumOfTransfers)
        //    {
        //        Console.WriteLine($"\t- in total {sum.SumOfTransfers}$ was sent to the account: {sum.TargetAccount}");
        //    }

        //    return sumOfTransfers;
        //}

        //public IEnumerable<SumItem> CountAndPrintSumOfTransfersIncomingFromSpecificAccounts(List<Transfer> listOfIncomingTransfers, Guid number)
        //{
        //    var sumOfTransfers = listOfIncomingTransfers
        //        .GroupBy(y => new { y.SourceAccount })
        //        .Select(g =>
        //            new SumItem
        //            {
        //                SourceAccount = g.Key.SourceAccount,
        //                TargetAccount = number,
        //                SumOfTransfers = g.Sum(x => x.Amount)
        //            });

        //    Console.WriteLine();
        //    Console.WriteLine("\tTotal value of transfers received from specific bank accounts:");

        //    foreach (var sum in sumOfTransfers)
        //    {
        //        Console.WriteLine($"\t- a total of {sum.SumOfTransfers}$ was received from the account: {sum.SourceAccount}");
        //    }

        //    return sumOfTransfers;
        //}

        //public double CountAndPrintSumOfTransfers(List<Transfer> list, string typeOfTransfers)
        //{
        //    var sumOfTransfers = list
        //        .DefaultIfEmpty()
        //        .Sum(x => x == null ? 0 : x.Amount);

        //    Console.WriteLine($"\tTotal value of {typeOfTransfers} transfers is {sumOfTransfers}.");

        //    return sumOfTransfers;
        //}

        //public double GetAndPrintMaxValoueOfOutgoingTransfers(List<Transfer> list)
        //{
        //    var maxValoue = list
        //        .DefaultIfEmpty()
        //        .Max (x => x == null ? 0 : x.Amount);

        //    Console.WriteLine($"\tThe highest value of an outgoing transfer: {maxValoue}$");

        //    return maxValoue;
        //}

        //public double GetAndPrintMinValoueOfOutgoingTransfers(List<Transfer> list)
        //{
        //    var minValoue = list
        //        .DefaultIfEmpty()
        //        .Min(x => x == null ? 0 : x.Amount);

        //    Console.WriteLine($"\tThe lowest value of an outgoing transfer: {minValoue}$");

        //    return minValoue;
        //}

        //public double GetAndPrintAverageValoueOfOutgoingTransfers(List<Transfer> list)
        //{
        //    var averageValoue = Math.Round(list
        //        .DefaultIfEmpty()
        //        .Average(x => x == null ? 0 : x.Amount), 2);

        //    Console.WriteLine($"\tThe average value of an outgoing transfer: {averageValoue}$");

        //    return averageValoue;
        //}
    }
}
