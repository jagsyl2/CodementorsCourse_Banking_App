using System;
using System.Collections.Generic;
using BankTransfers.BusinessLayer;
using BankTransfers.DataLayer.Models;


namespace Bank
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private AccountsService _accountsService = new AccountsService();
        private TransfersService _transfersService = new TransfersService();
            
        private void Run()
        {
            bool exit = false;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Choose action:");
                Console.WriteLine("Press 1 to Create account");
                Console.WriteLine("Press 2 to Make a domestic transfer");
                Console.WriteLine("Press 3 to Make an outgoing transfer");
                Console.WriteLine("Press 4 to Check yours accounts balance");
                Console.WriteLine("Press 5 to Check the history of transfers");
                Console.WriteLine("Press 6 to exit");

                int userChoice = GetIntFromUser("Select number");
                
                Console.WriteLine();

                switch (userChoice)
                {
                    case 1:
                        CreateAccount();
                        break;
                    case 2:
                        DomesticTransfer();
                        break;
                    case 3:
                        OutgoingTransfer();
                        break;
                    case 4:
                        PrintAccounts();
                        break;
                    case 5:
                        CheckTheHistoryOfTransfers();
                        break;
                    case 6:
                        exit=true;
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }            
            } while (!exit);
        }

        private void CheckTheHistoryOfTransfers()
        {
            List<Transfer> history = _transfersService.GetAllTransfers();
            if (history.Count == 0)
            {
                Console.WriteLine("No transfers has been sent");
            }
            
            Console.WriteLine("The history of transfers:");
            
            foreach (Transfer transfers in history)
            {                                                                           
                string text = @$"Date of the transfer:  {transfers.DateOfTheTransfer}
                                 GUID source account:   {transfers.SourceAccount}
                                 GUID target account:   {transfers.TargetAccount}
                                 Transfer Title:        {transfers.Title}  
                                 Transfers Amount:      {transfers.Amount}
                                 Typ of transfer:       {transfers.TypOfTransfer}";
                Console.WriteLine(text);
                Console.WriteLine();
            }
        }

        private void OutgoingTransfer()
        {
            List<Account> outgoingTransfers = _accountsService.GetAllAccounts();
            if (outgoingTransfers.Count < 1)
            {
                Console.WriteLine("It is not possible to make an outgoing transfer - minimum 1 accounts needed");
                return;
            }

            PrintAccounts();
            Console.WriteLine();
            Console.WriteLine("Make an outgoing transfer:");

            string debitMessage = "The account from which the funds will be withdrawn - enter your debit account name";
            string accountNameFrom = EnterYourAccountName(outgoingTransfers, debitMessage, null);

            Guid targetAccount = GetGuidFromUser("The number of GUID");

            double amount;
            GetAmountFromUser(outgoingTransfers, accountNameFrom, out amount);

            Guid sourceAccount = default(Guid);
            sourceAccount = AssignmentGuidToAccount(outgoingTransfers, accountNameFrom, sourceAccount);

            string title;
            GetTheTitleOfTransfer(out title);

            Transfer newTransferOut = new Transfer()
            {
                Title = title,
                Amount = amount,
                DateOfTheTransfer = DateTime.Now,
                TypOfTransfer = "External transfer",
                SourceAccount = sourceAccount,
                TargetAccount = targetAccount,
            };
            Console.WriteLine($"Date of the transfer: {newTransferOut.DateOfTheTransfer}");

            MakeAnOutgoingTransfer(accountNameFrom, amount);

            _transfersService.AddTransfer(newTransferOut);
        }

        private void MakeAnOutgoingTransfer(string account, double amount)
        {
            List<Account> outgoingTransfer = _accountsService.GetAllAccounts();
            for (int i = 0; i < outgoingTransfer.Count; i++)
            {
                if (outgoingTransfer[i].Name == account)
                {
                    outgoingTransfer[i].Balance = outgoingTransfer[i].Balance - amount;
                    Console.WriteLine($"Account: \"{outgoingTransfer[i].Name}\" - Balance: {outgoingTransfer[i].Balance}$");
                }
            }
        }

        private Guid GetGuidFromUser(string message)
        {
            Guid guid;
            while (!Guid.TryParse(GetTextFromUser(message), out guid))
            {
                Console.WriteLine("Incorrect number - try again...");
                Console.WriteLine();
            }
            return guid;
        }

        private void DomesticTransfer()
        {
            List<Account> numberOfAccounts = _accountsService.GetAllAccounts();
            if (numberOfAccounts.Count < 2)
            {
                Console.WriteLine("It is not possible to make a domestic transfer - minimum 2 accounts needed");
                return;
            }

            PrintAccounts();
            Console.WriteLine();
            Console.WriteLine("Make a domestic transfer:");

            string debitMessage = "The account from which the funds will be withdrawn - enter your debit account name";
            string accountNameFrom = EnterYourAccountName(numberOfAccounts, debitMessage, null);

            string creditMessage = "The account to which the funds will be transferred - enter your credit account name";
            string accountNameTo = EnterYourAccountName(numberOfAccounts, creditMessage, accountNameFrom, true);

            double amount;
            GetAmountFromUser(numberOfAccounts, accountNameFrom, out amount);

            Guid sourceAccount = default(Guid);
            sourceAccount = AssignmentGuidToAccount(numberOfAccounts, accountNameFrom, sourceAccount);

            Guid targetAccount = default(Guid);
            targetAccount = AssignmentGuidToAccount(numberOfAccounts, accountNameFrom, targetAccount);

            string title;
            GetTheTitleOfTransfer(out title);

            Transfer newTransfer = new Transfer()
            {
                Title = title,
                Amount = amount,
                DateOfTheTransfer = DateTime.Now,
                TypOfTransfer = "Internal transfer",
                SourceAccount = sourceAccount,
                TargetAccount = targetAccount,
            };
            Console.WriteLine($"Date of the transfer: {newTransfer.DateOfTheTransfer}");

            MakeADomesticTransfer(accountNameFrom, accountNameTo, amount);

            _transfersService.AddTransfer(newTransfer);
        }

        private string EnterYourAccountName(List<Account> list, string instruction, string accountCredit, bool debitEqualCredit = false)
        {
            string accountName;
            bool accountTo = false;
            do
            {
                accountName = GetTextFromUser(instruction);

                string message = "There is no such account - try again...";
                accountTo = CheckingIfTheAccountNameIsOnTheList(list, accountName, accountTo, message);

                if ((debitEqualCredit == true) && (accountCredit == accountName))
                {
                    Console.WriteLine("Same account selected - try again...");
                    Console.WriteLine();
                    accountTo = false;
                }
            }
            while (accountTo == false);
            return accountName;
        }

        private void GetTheTitleOfTransfer(out string title)
        {
            bool correctTitle;
            do
            {
                title = GetTextFromUser("Transfer title");
                correctTitle = true;
                string message = "Incorrect title - try again...";
                correctTitle = CheckingIfIsNullOrWhiteSpace(title, correctTitle, message);
            }
            while (correctTitle == false);
        }

        private static Guid AssignmentGuidToAccount(List<Account> numberOfAccounts, string accountNameFrom, Guid sourceAccount)
        {
            for (int i = 0; i < numberOfAccounts.Count; i++)
            {
                if (accountNameFrom == numberOfAccounts[i].Name)
                {
                    sourceAccount = numberOfAccounts[i].Number;
                }
            }
            return sourceAccount;
        }

        private void GetAmountFromUser(List<Account> numberOfAccounts, string accountNameFrom, out double amount)
        {
            bool goodAmount;
            do
            {
                amount = GetDoubleFromUser("Transfer amount");
                goodAmount = true;
                for (int i = 0; i < numberOfAccounts.Count; i++)
                {
                    if (numberOfAccounts[i].Name == accountNameFrom && (amount <= 0 || amount > numberOfAccounts[i].Balance))
                    {
                        Console.WriteLine("Wrong amount - try again...");
                        Console.WriteLine();
                        goodAmount = false;
                    }
                }
            }
            while (goodAmount == false);
        }

        private bool CheckingIfTheAccountNameIsOnTheList(List<Account> list, string accountName, bool accountTrue, string message)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (accountName == list[i].Name)
                {
                    accountTrue = true;
                }
            }

            if (accountTrue == false)
            {
                Console.WriteLine(message);
                Console.WriteLine();
            }
            return accountTrue;
        }

        private void MakeADomesticTransfer(string accountNameFrom, string accountNameTo, double amount)
        {
            List <Account> domesticTransfer = _accountsService.GetAllAccounts();
            for (int i= 0; i< domesticTransfer.Count; i++) 
            {
                if (accountNameFrom == domesticTransfer[i].Name)
                {
                    domesticTransfer[i].Balance = domesticTransfer[i].Balance - amount;
                    Console.WriteLine($"Account: \"{domesticTransfer[i].Name}\" - Balance: {domesticTransfer[i].Balance}$");
                }
                
                if (accountNameTo == domesticTransfer[i].Name)
                {
                    domesticTransfer[i].Balance = domesticTransfer[i].Balance + amount;
                    Console.WriteLine($"Account: \"{domesticTransfer[i].Name}\" - Balance: {domesticTransfer[i].Balance}$");
                    Console.WriteLine();
                }
            }
        }

        private double GetDoubleFromUser(string message)
        {
            double amount;
            while (!double.TryParse(GetTextFromUser(message), out amount))
            {
                Console.WriteLine("Negative value - try again...");
                Console.WriteLine();
            }
            return amount; 
        }

        private void PrintAccounts()
        {
            List<Account> yourAccounts = _accountsService.GetAllAccounts();
            if (yourAccounts.Count == 0)
            {
                Console.WriteLine("No accounts has been created");
                Console.WriteLine();
            }

            Console.WriteLine("Your accounts balance:");

            for (int i = 0; i < yourAccounts.Count; i++)
            {
                string text = $"Account: \"{yourAccounts[i].Name}\" - Balance: {yourAccounts[i].Balance}$ - Account Number: {yourAccounts[i].Number}";
                Console.WriteLine(text);
            }
        }

        private void CreateAccount()
        {
            Console.WriteLine("Create Account:");

            string accountName;
            bool createAccount;
            List<Account> sprawdzenie = _accountsService.GetAllAccounts();
            do
            {
                accountName = GetTextFromUser("Provide account name");
                createAccount = true;
                string message = "Incorrect name - try again...";
                createAccount = CheckingIfIsNullOrWhiteSpace(accountName, createAccount, message);

                for (int i = 0; i < sprawdzenie.Count; i++)
                {
                    if (accountName == sprawdzenie[i].Name)
                    {
                        Console.WriteLine("The name exist - try again...");
                        Console.WriteLine();
                        createAccount = false;
                    }
                }
            } 
            while (createAccount==false);
            
            Account newAccount = new Account()
            {
                Name = accountName,
                Number = GenerateGuidToUser(),
                Balance = 1000,
            };
            
            Console.WriteLine($"The opening balance of the account: {newAccount.Balance}$");

            _accountsService.AddAccount(newAccount);
            Console.WriteLine($"\nAccount \"{newAccount.Name}\" added successfully");
        }

        private bool CheckingIfIsNullOrWhiteSpace(string accountName, bool createAccount, string message)
        {
            if (string.IsNullOrWhiteSpace(accountName))
            {
                Console.WriteLine(message);
                Console.WriteLine();
                createAccount = false;
            }
            return createAccount;
        }

        private Guid GenerateGuidToUser()
        {
            Guid g = Guid.NewGuid();
            Console.WriteLine($"Your account number: {g}");
            return g;
        }

        private int GetIntFromUser(string message)
        {
            int number;
            while (!int.TryParse(GetTextFromUser(message), out number))
            {
                Console.WriteLine("Incorrect option - try again...");
                Console.WriteLine();
            }
            return number;
        }

        private string GetTextFromUser(string message)
        {
            Console.Write($"{message}: ");
            return Console.ReadLine();
        }
    }
}