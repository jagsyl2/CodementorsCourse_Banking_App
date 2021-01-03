using BankTransfers.BusinessLayer;
using BankTransfers.DataLayer.Models;
using System;

namespace Bank
{
    public interface IIoStatementOfOperationsHelper
    {
        public StatementOfOperations GetStatementOfOperationForAccount(Account account);
        public void PrintStatementOfOperations(Account account, StatementOfOperations statement);
    }

    public class IoStatementOfOperationsHelper : IIoStatementOfOperationsHelper
    {
        private ITransfersService _transfersService = new TransfersService();
        private IStatementsOfOperationsService _statementsOfOperationsService = new StatementsOfOperationsService();

        public IoStatementOfOperationsHelper(
            ITransfersService transfersService,
            IStatementsOfOperationsService statementsOfOperationsService)
        {
                    _transfersService = transfersService;
                    _statementsOfOperationsService = statementsOfOperationsService;
        }

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
    }
}