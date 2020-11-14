using System;
using System.Collections.Generic;

namespace Bank
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        
        }

        private Finances _finances = new Finances();
        private TransfersHistory _transfersHistory = new TransfersHistory();
            
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
                        QuitTheProgram(userChoice, exit=true);
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            
            } while (!exit);
        }

        private void QuitTheProgram(int userChoice, bool exit)
        {
            if (userChoice == 6)
            {
                exit = true;
            }
        }

        private void CheckTheHistoryOfTransfers()
        {
            Console.WriteLine("The history of transfers:");
            List <Transfer> history = _transfersHistory.GetAllTransfers();
            foreach (Transfer transfers in history)
            {                                                                           
                string text = @$"Date of the transfer:  {transfers.DateOfTheTransfer}
                                 GUID source account:   {transfers.SourceAccount}
                                 GUID target account:   {transfers.TargetAccount}
                                 Transfer Title:        {transfers.TransferTitle}  
                                 Transfers Amount:      {transfers.TransferAmount}
                                 Typ of transfer:       {transfers.TypOfTransfer}";
                Console.WriteLine(text);
                Console.WriteLine();
            }

            if (history.Count == 0)
            {
                Console.WriteLine("No transfers has been sent");
            }
        }

        private void OutgoingTransfer()
        {
            List<Account> outgoingTransfers = _finances.GetAllAccounts();
            if (outgoingTransfers.Count < 1)
            {
                Console.WriteLine("It is not possible to make an outgoing transfer - minimum 1 accounts needed");
                return;
            }

            PrintAccounts();
            Console.WriteLine();
            Console.WriteLine("Make an outgoing transfer:");
            
            string accountNameFrom;
            bool accountFrom = false;
            do
            {
                accountNameFrom = GetTextFromUser("The account from which the funds will be withdrawn - enter your account name");

                string message = "There is no such account - try again...";
                accountFrom = CheckingIfTheAccountNameIsOnTheList(outgoingTransfers, accountNameFrom, accountFrom, message);
            }
            while (accountFrom == false);

            Guid targetAccount = GetGuidFromUser("The number of GUID");

            double amount;
            bool goodAmount;
            GetAmountFromUser(outgoingTransfers, accountNameFrom, out amount, out goodAmount);

            Guid sourceAccount = default(Guid);
            sourceAccount = AssignmentGuidToAccount(outgoingTransfers, accountNameFrom, sourceAccount);
            if (sourceAccount == default(Guid))
            {
                Console.WriteLine("No field found");
                return;
            }

            string title;
            bool correctTitle;
            GetTheTitleOfTransfer(out title, out correctTitle);

            Transfer newTransferOut = new Transfer()
            {
                TransferTitle = title,
                TransferAmount = amount,
                DateOfTheTransfer = DateTime.Now,
                TypOfTransfer = "External transfer",
                SourceAccount = sourceAccount,
                TargetAccount = targetAccount,
            };
            Console.WriteLine($"Date of the transfer: {newTransferOut.DateOfTheTransfer}");

            MakeAnOutgoingTransfer(accountNameFrom, amount);

            _transfersHistory.AddTransfer(newTransferOut);
        }

        private void MakeAnOutgoingTransfer(string account, double amount)
        {
            List<Account> outgoingTransfer = _finances.GetAllAccounts();
            for (int i = 0; i < outgoingTransfer.Count; i++)
            {
                if (outgoingTransfer[i].AccountName == account)
                {
                    outgoingTransfer[i].AccountBalance = outgoingTransfer[i].AccountBalance - amount;
                    Console.WriteLine($"Account: \"{outgoingTransfer[i].AccountName}\" - Balance: {outgoingTransfer[i].AccountBalance}$");
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
            List<Account> numberOfAccounts = _finances.GetAllAccounts();
            if (numberOfAccounts.Count < 2)
            {
                Console.WriteLine("It is not possible to make a domestic transfer - minimum 2 accounts needed");
                return;
            }

            PrintAccounts();
            Console.WriteLine();
            Console.WriteLine("Make a domestic transfer:");

            string accountNameFrom;
            bool accountFrom = false;
            do
            {
                accountNameFrom = GetTextFromUser("The account from which the funds will be withdrawn - enter your account name");

                string message = "There is no such account - try again...";
                accountFrom = CheckingIfTheAccountNameIsOnTheList(numberOfAccounts, accountNameFrom, accountFrom, message);
            }
            while (accountFrom == false);
            
            string accountNameTo;
            bool accountTo = false;
            
            do
            {
                accountNameTo = GetTextFromUser("The account to which the funds will be transferred - enter your account name");

                string message = "There is no such account - try again...";
                accountTo = CheckingIfTheAccountNameIsOnTheList(numberOfAccounts, accountNameTo, accountTo, message);

                if (accountNameFrom == accountNameTo)
                {
                    Console.WriteLine("Same account selected - try again...");
                    accountTo = false;
                }
            }
            while (accountTo == false);
            
            double amount;
            bool goodAmount;
            GetAmountFromUser(numberOfAccounts, accountNameFrom, out amount, out goodAmount);

            Guid sourceAccount = default(Guid);
            sourceAccount = AssignmentGuidToAccount(numberOfAccounts, accountNameFrom, sourceAccount);
            //if (sourceAccount == default(Guid))
            //{
            //    Console.WriteLine("No field found");
            //    return;
            //}

            Guid targetAccount = default(Guid);
            targetAccount = AssignmentGuidToAccount(numberOfAccounts, accountNameFrom, targetAccount);
            //if (targetAccount == default(Guid))
            //{
            //    Console.WriteLine("No field found");
            //    return;
            //}

            string title;
            bool correctTitle;
            GetTheTitleOfTransfer(out title, out correctTitle);

            Transfer newTransfer = new Transfer()
            {
                TransferTitle = title,
                TransferAmount = amount,
                DateOfTheTransfer = DateTime.Now,
                TypOfTransfer = "Internal transfer",
                SourceAccount = sourceAccount,
                TargetAccount = targetAccount,
            };
            Console.WriteLine($"Date of the transfer: {newTransfer.DateOfTheTransfer}");

            MakeADomesticTransfer(accountNameFrom, accountNameTo, amount);

            _transfersHistory.AddTransfer(newTransfer);
        }

        private void GetTheTitleOfTransfer(out string title, out bool correctTitle)
        {
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
                if (accountNameFrom == numberOfAccounts[i].AccountName)
                {
                    sourceAccount = numberOfAccounts[i].AccountNumber;
                }
            }

            return sourceAccount;
        }

        private void GetAmountFromUser(List<Account> numberOfAccounts, string accountNameFrom, out double amount, out bool goodAmount)
        {
            do
            {
                amount = GetDoubleFromUser("Transfer amount");
                goodAmount = true;
                for (int i = 0; i < numberOfAccounts.Count; i++)
                {
                    if (numberOfAccounts[i].AccountName == accountNameFrom && (amount <= 0 || amount > numberOfAccounts[i].AccountBalance))
                    {
                        Console.WriteLine("Wrong amount - try again...");
                        goodAmount = false;
                    }
                }
            }
            while (goodAmount == false);
        }

        private static bool CheckingIfTheAccountNameIsOnTheList(List<Account> List, string accountName, bool accountTrue, string message)
        {
            for (int i = 0; i < List.Count; i++)
            {
                if (accountName == List[i].AccountName)
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
            List <Account> domesticTransfer = _finances.GetAllAccounts();
            for (int i= 0; i< domesticTransfer.Count; i++) 
            {
                if (accountNameFrom == domesticTransfer[i].AccountName)
                {
                    domesticTransfer[i].AccountBalance = domesticTransfer[i].AccountBalance - amount;
                    Console.WriteLine($"Account: \"{domesticTransfer[i].AccountName}\" - Balance: {domesticTransfer[i].AccountBalance}$");
                }
                
                if (accountNameTo == domesticTransfer[i].AccountName)
                {
                    domesticTransfer[i].AccountBalance = domesticTransfer[i].AccountBalance + amount;
                    Console.WriteLine($"Account: \"{domesticTransfer[i].AccountName}\" - Balance: {domesticTransfer[i].AccountBalance}$");
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
            Console.WriteLine("Your accounts balance:");
            List<Account> yourAccounts = _finances.GetAllAccounts();
            for (int i = 0; i < yourAccounts.Count; i++)
            {
                string text = $"Account: \"{yourAccounts[i].AccountName}\" - Balance: {yourAccounts[i].AccountBalance}$ - Account Number: {yourAccounts[i].AccountNumber}";
                Console.WriteLine(text);
            }
            
            if (yourAccounts.Count == 0)
            {
                Console.WriteLine("No accounts has been created");
                Console.WriteLine();
            } 
        }

        private void CreateAccount()
        {
            Console.WriteLine("Create Account:");

            string accountName;
            bool createAccount;
            List<Account> sprawdzenie = _finances.GetAllAccounts();
            do
            {
                accountName = GetTextFromUser("Provide account name");
                createAccount = true;
                string message = "Incorrect name - try again...";
                createAccount = CheckingIfIsNullOrWhiteSpace(accountName, createAccount, message);

                for (int i = 0; i < sprawdzenie.Count; i++)
                {
                    if (accountName == sprawdzenie[i].AccountName)
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
                AccountName = accountName,
                AccountNumber = GenerateGuidToUser(),
                AccountBalance = 1000,
            };
            
            Console.WriteLine($"The opening balance of the account: {newAccount.AccountBalance}$");

            _finances.AddAccounts(newAccount);
            Console.WriteLine($"\nAccount \"{newAccount.AccountName}\" added successfully");
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