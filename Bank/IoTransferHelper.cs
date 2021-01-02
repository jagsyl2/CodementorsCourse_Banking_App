using BankTransfers.BusinessLayer;
using BankTransfers.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank
{
    public class IoTransferHelper
    {
        private IIoHelper _ioHelper;
        private IAccountService _accountsService;

        public IoTransferHelper(
            IIoHelper ioHelper,
            IAccountService accountsService)
        {
            _ioHelper = ioHelper;
            _accountsService = accountsService;
        }

        public double GetAmountFromUser(Account account, List<Transfer> scheduledTransfers)
        {
            double sumOfScheduledTransfers = CountSumOfScheduledTransfers(account, scheduledTransfers);

            bool correctAmount;
            double amount;
            do
            {
                amount = _ioHelper.GetDoubleFromUser("Transfer amount");
                correctAmount = true;

                if (amount <= 0 || amount > account.Balance - sumOfScheduledTransfers)
                {
                    _ioHelper.WriteString("Wrong amount (below 0$ or insufficient funds on the account) - try again...");
                    correctAmount = false;
                }
            }
            while (correctAmount == false);

            return amount;
        }

        public double CountSumOfScheduledTransfers(Account account, List<Transfer> scheduledTransfers)
        {
            double sum = 0;

            foreach (var transfer in scheduledTransfers)
            {
                if (transfer.SourceAccount == account.Number)
                {
                    sum += transfer.Amount;
                }
            }

            return sum;
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

        internal bool CheckingIfTargetGuidIsCustomerAccount(int customerId, Guid targetAccount)
        {
            var customerAccounts = _accountsService.GetCustomerAccounts(customerId);

            if (customerAccounts.Any(x => x.Number == targetAccount))
            {
                Console.WriteLine("Check the account number - this is your account!");
                return true;
            }
            return false;
        }

        public bool CheckingIfTargetGuidIsAccountInOurBank(int CustomerId, Guid accountNumber)
        {
            var listOfAllAccounts = _accountsService.GetAllAccounts();

            if (!listOfAllAccounts
                .Where(x => x.CustomerId != CustomerId)
                .Any(x => x.Number == accountNumber))
            {
                return false;
            }
            return true;
        }

        public bool ChceckIfListIsEmpty(List<Account> accounts)
        {
            if (accounts.Count == 0)
            {
                Console.WriteLine();
                _ioHelper.WriteString("You don't have accounts in our bank.");
                return true;
            }

            return false;
        }

        public void PrintCustomerAccounts(List<Account> customerAccounts, List<Transfer> scheduledTransfers)
        {
            Console.WriteLine();
            Console.WriteLine("Your accounts balance:");
            foreach (var account in customerAccounts)
            {
                double sumOfScheduledTransfers = CountSumOfScheduledTransfers(account, scheduledTransfers);
                PrintAccount(account, sumOfScheduledTransfers);
            }
        }

        public void PrintAccount(Account account, double sumOfScheduledTransfers)
        {
            Console.WriteLine($"Number: {account.Id} - Account: \"{account.Name}\" - Balance: {account.Balance}$ - Account Number: {account.Number} - Available funds: {account.Balance - sumOfScheduledTransfers}");
        }
    }
}
