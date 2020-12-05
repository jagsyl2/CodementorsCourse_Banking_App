using BankTransfers.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bank
{
    public class IoHelper
    {
        public double GetAmountFromUser(Account account)
        {
            bool correctAmount;
            double amount;
            do
            {
                amount = GetDoubleFromUser("Transfer amount");
                correctAmount = true;

                if (amount <= 0 || amount > account.Balance)
                {
                    WriteString("Wrong amount (below 0$ or insufficient funds on the account) - try again...");
                    correctAmount = false;
                }
            }
            while (correctAmount == false);

            return amount;
        }

        public Account GetAccountFromUser(string message, List<Account> customerAccounts)
        {
            WriteString(message);
            bool existAccounId;
            int sourceAccountId;

            do
            {
                sourceAccountId = GetIntFromUser("Provide the source account number");
                existAccounId = true;

                if (!customerAccounts.Any(account => account.Id == sourceAccountId))
                {
                    WriteString("Incorrect account Id!");
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
                targetAccountId = GetIntFromUser("Provide the target account number");
                existAccounId = true;

                if (!customerAccounts.Any(account => account.Id == targetAccountId))
                {
                    WriteString("Incorrect account Id!");
                    existAccounId = false;
                }

                if (sourceAccountId == targetAccountId)
                {
                    WriteString("Same account selected - try again...");
                    existAccounId = false;
                }
            }
            while (existAccounId == false);

            var targetAccount = customerAccounts.First(account => targetAccountId == account.Id);
            return targetAccount;
        }

        public void PrintAccount(Account account)
        {
            Console.WriteLine($"Number: {account.Id} - Account: \"{account.Name}\" - Balance: {account.Balance}$ - Account Number: {account.Number}");
        }

        public Guid GetGuidFromUser(string message)
        {
            Guid guid;
            while (!Guid.TryParse(GetTextFromUser(message), out guid))
            {
                WriteString("Incorrect number - try again...");
            }
            return guid;
        }

        public double GetDoubleFromUser(string message)
        {
            double amount;
            while (!double.TryParse(GetTextFromUser(message), out amount))
            {
                WriteString("Negative value - try again...");
            }
            return amount;
        }

        public bool CheckingIfIsNullOrWhiteSpace(string accountName)
        {
            if (string.IsNullOrWhiteSpace(accountName))
            {
                WriteString("Incorrect name - try again...");
                return true;
            }
            return false;
        }

        public Guid GenerateGuidToUser()
        {
            Guid g = Guid.NewGuid();
            Console.WriteLine($"Your account number: {g}");
            return g;
        }

        public int GetIntFromUser(string message)
        {
            int number;
            while (!int.TryParse(GetTextFromUser(message), out number))
            {
                WriteString("Incorrect option - try again...");
            }
            return number;
        }

        public string GetTextFromUser(string message)
        {
            Console.Write($"{message}: ");
            return Console.ReadLine();
        }

        public int GetPhoneNumberFromUser(string message)
        {
            int phoneNumber;
            bool validation;

            do
            {
                phoneNumber = GetIntFromUser(message);
                validation = phoneNumber.ToString().Length == 9;
                Console.WriteLine(validation ? "" : "Incorrect phone number (must contain 9 digits). Try again...");
            }
            while (validation == false);

            return phoneNumber;
        }

        public void WriteString (string message)
        {
            Console.WriteLine(message);
            Console.WriteLine();
        }
    }
}
