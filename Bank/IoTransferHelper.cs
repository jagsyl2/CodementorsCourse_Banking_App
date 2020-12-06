using BankTransfers.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank
{
    class IoTransferHelper
    {
        private IoHelper _ioHelper = new IoHelper();

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
            bool existAccounId;
            int sourceAccountId;

            do
            {
                sourceAccountId = _ioHelper.GetIntFromUser("Provide the source account number");
                existAccounId = true;

                if (!customerAccounts.Any(account => account.Id == sourceAccountId))
                {
                    _ioHelper.WriteString("Incorrect account Id!");
                    existAccounId = false;
                    continue;
                }
                if (!customerAccounts
                    .Where(account => account.Id == sourceAccountId)
                    .Any(account => (account.Balance > 0)))
                {
                    _ioHelper.WriteString("There are no funds in this account");
                    existAccounId = false;
                }
            }
            while (existAccounId == false);

            var sourceAccount = customerAccounts.First(account => sourceAccountId == account.Id);
            return sourceAccount;
        }

        public Account GetAccountFromUser(List<Account> customerAccounts, int sourceAccountId)
        {
            bool existAccounId;
            int targetAccountId;

            do
            {
                targetAccountId = _ioHelper.GetIntFromUser("Provide the target account number");
                existAccounId = true;

                if (!customerAccounts.Any(account => account.Id == targetAccountId))
                {
                    _ioHelper.WriteString("Incorrect account Id!");
                    existAccounId = false;
                }

                if (sourceAccountId == targetAccountId)
                {
                    _ioHelper.WriteString("Same account selected - try again...");
                    existAccounId = false;
                }
            }
            while (existAccounId == false);

            var targetAccount = customerAccounts.First(account => targetAccountId == account.Id);
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
    }
}
