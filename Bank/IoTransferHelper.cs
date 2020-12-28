using BankTransfers.BusinessLayer;
using BankTransfers.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank
{
    class IoTransferHelper
    {
        private IoHelper _ioHelper = new IoHelper();
        private AccountsService _accountsService = new AccountsService();
        private TransfersService _transfersService = new TransfersService();

        public double GetAmountFromUser(Account account)
        {
            bool correctAmount;
            double amount;
            do
            {
                amount = _ioHelper.GetDoubleFromUser("Transfer amount");
                correctAmount = true;

                if (amount <= 0 || amount > account.Balance)
                {
                    _ioHelper.WriteString("Wrong amount (below 0$ or insufficient funds on the account) - try again...");
                    correctAmount = false;
                }
            }
            while (correctAmount == false);

            return amount;
        }

        public Account GetAccountFromUser(string message, List<Account> customerAccounts)
        {
            Console.WriteLine();
            Console.WriteLine(message);

            Account sourceAccount = null;

            do
            {
                int sourceAccountId = _ioHelper.GetIntFromUser("Provide the source account number");

                sourceAccount = customerAccounts.FirstOrDefault(account => sourceAccountId == account.Id);

                if (sourceAccount == null)
                {
                    _ioHelper.WriteString("Incorrect account Id!");
                    continue;
                }
                
                if (sourceAccount.Balance <= 0)
                {
                    _ioHelper.WriteString("There are no funds in this account");
                    sourceAccount = null;
                }
            }
            while (sourceAccount == null);

            return sourceAccount;
        }

        public Account GetAndCheckIfTheAccountIsNonSourceAccount(List<Account> customerAccounts, int sourceAccountId)
        {
            Account targetAccount = null;

            do
            {
                int targetAccountId = _ioHelper.GetIntFromUser("Provide the target account number");

                targetAccount = customerAccounts.First(account => targetAccountId == account.Id);

                if (targetAccount == null)
                {
                    _ioHelper.WriteString("Incorrect account Id!");
                    continue;
                }

                if (sourceAccountId == targetAccount.Id)
                {
                    _ioHelper.WriteString("Same account selected - try again...");
                    targetAccount = null;
                }
            }
            while (targetAccount == null);

            return targetAccount;
        }

        public bool CheckingIfThereIsAccountWithPositiveBalance(List<Account> accounts)
        {
            if (!accounts.Any(account => account.Balance > 0))
            {
                Console.WriteLine();
                _ioHelper.WriteString("You don't have an account with a positive balance");
                return false;
            }
            return true;
        }

        public string GetNotNullTextFromUser(string message)
        {
            string title;
            do
            {
                title = _ioHelper.GetTextFromUser(message);
            }
            while (_ioHelper.CheckingIfIsNullOrWhiteSpace(title));

            return title;
        }

        public Account CheckingIfTargetGuidIsAccountInOurBank(int CustomerId, Guid accountNumber)
        {
            var listOfAllAccounts = _accountsService.GetAllAccounts();

            if (!listOfAllAccounts
                .Where(x => x.CustomerId != CustomerId)
                .Any(x => x.Number == accountNumber))
            {
                return null;
            }
            else
            {
               return listOfAllAccounts.FirstOrDefault(x => x.Number == accountNumber);
            }
        }

        //public void TotalValueOfTransfersSentToSpecificBankAccounts(int customerId)
        //{
        //    var listOfCustomerTransfers = _transfersService.GetTransfers(customerId);
        //    if (listOfCustomerTransfers == null)
        //    {
        //        Console.WriteLine("You don't have outgoing transfers.");
        //        return;
        //    }

        //    var sumOfTransfersSent = listOfCustomerTransfers
        //        .GroupBy(x => new { x.SourceAccount, x.TargetAccount })
        //        .Select(g =>
        //                new {
        //                    SourceAccount = g.Key.SourceAccount,
        //                    TargetAccount = g.Key.TargetAccount,
        //                    SumOfTransfers = g.Sum(x => x.Amount)
        //                    }
        //                )
        //        .OrderBy(x => x.SourceAccount);

        //    Console.WriteLine("Total value of transfers sent to specific bank accounts:");
        //    foreach (var sum in sumOfTransfersSent)
        //    {
        //        Console.WriteLine($"From: {sum.SourceAccount} in total {sum.SumOfTransfers}$ was sent to account: {sum.TargetAccount}");
        //    }
        //}

        //public void TotalValueOfTransfersReceivedFromSpecificBankAccounts(int customerId)
        //{
        //    var listOfAllTransfers = _transfersService.GetAllTransfers();
        //    var listOfCustomerAccounts = _accountsService.GetCustomerAccounts(customerId);
            
        //    if (listOfCustomerAccounts == null)
        //    {
        //        Console.WriteLine("You don't have accounts in our bank.");
        //        return;
        //    }

        //    List<Transfer> listOfIncomingTransfers = new List<Transfer>();
        //    foreach (var account in listOfCustomerAccounts)
        //    {
        //        listOfIncomingTransfers
        //            .AddRange(listOfAllTransfers.Where(x => x.TargetAccount == account.Number));
        //    }
            
        //    if (listOfIncomingTransfers == null)
        //    {
        //        Console.WriteLine("You don't have incoming transfers.");
        //        return;
        //    }

        //    var sumOfTransfersIncoming = listOfIncomingTransfers
        //        .GroupBy(y => new { y.TargetAccount, y.SourceAccount })
        //        .Select(g =>
        //            new
        //            {
        //                TargetAccount = g.Key.TargetAccount,
        //                SourceAccount = g.Key.SourceAccount,
        //                SumOfTransfers = g.Sum(x => x.Amount)
        //            }
        //        )
        //        .OrderBy(x => x.TargetAccount);

        //    Console.WriteLine("Total value of transfers received from specific bank accounts:");
        //    foreach (var sum in sumOfTransfersIncoming)
        //    {
        //        Console.WriteLine($"Account: {sum.TargetAccount} received a total of {sum.SumOfTransfers}$ from account: {sum.SourceAccount}");
        //    }
        //}
        //public void TotalValueOfOutgoingTransfers(int customerId)
        //{
        //    var listOfCustomerTransfers = _transfersService.GetTransfers(customerId);
        //    if (listOfCustomerTransfers == null)
        //    {
        //        Console.WriteLine("You don't have outgoing transfers.");
        //        return;
        //    }

        //    var sumOfTransfersSent = listOfCustomerTransfers
        //        .GroupBy(x => new { x.SourceAccount })
        //        .Select(g =>
        //                new
        //                {
        //                    SourceAccount = g.Key.SourceAccount,
        //                    SumOfTransfers = g.Sum(x => x.Amount)
        //                }
        //                );
        //        //.OrderBy(x => x.SourceAccount);

        //    Console.WriteLine("Total value of outgoing transfers:");
        //    foreach (var sum in sumOfTransfersSent)
        //    {
        //        Console.WriteLine($"From the account: {sum.SourceAccount} total value of outgoing transfers is {sum.SumOfTransfers}$");
        //    }
        //}

    }
}
