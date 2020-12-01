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
        private CustomersService _customersService = new CustomersService();
        private TransfersService _transfersService = new TransfersService();
        private DatabaseManagmentService _databaseManagmentService = new DatabaseManagmentService();
        private IoHelper _ioHelper = new IoHelper();
        

        private void Run()
        {
            _databaseManagmentService.EnsureDatabaseCreation();

            do
            {
                Console.WriteLine();
                Console.WriteLine("Choose action:");
                Console.WriteLine("Press 1 to Sign In");
                Console.WriteLine("Press 2 to Register");

                int userChoice = _ioHelper.GetIntFromUser("Select number");

                switch (userChoice)
                {
                    case 1:
                        SignIn();
                        break;
                    case 2:
                        Register();
                        break;
                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            } while (true);
        }


        private void SignIn()
        {
            var eMail = _ioHelper.GetTextFromUser("Enter your e-mail");
            var password = _ioHelper.GetTextFromUser("Enter password");
            
            bool success = _customersService.CheckingCustomerSignIn(eMail, password);
            if (success == false)
            {
                Console.WriteLine("Login failed. Try again...");
                return;
            }
                Console.WriteLine("You are logged in!");
                Menu();
            

        }



        private void Register()
        {
            var eMail = _ioHelper.GetEMailFromUser("Enter your e-mail");
            var phoneNumber = _ioHelper.GetPhoneNumberFromUser("Enter phone number");
            var password = _ioHelper.GetTextFromUser("Enter password");

            var newCustomer = new Customer
            {
                EMail = eMail,
                PhoneNumber = phoneNumber,
                Password = password,
            };

            bool success = _customersService.CheckingIfANewCustomerIsRegistering(eMail);
            if (success == true)
            {
                Console.WriteLine("The given email address exists. Try again...");
                return;
            }
            
            _customersService.AddCustomer(newCustomer);
            Console.WriteLine("Registration is correct!");
        }

        private void Menu()
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

                int userChoice = _ioHelper.GetIntFromUser("Select number");
                
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

            Guid targetAccount = _ioHelper.GetGuidFromUser("The number of GUID");

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
                accountName = _ioHelper.GetTextFromUser(instruction);

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
                title = _ioHelper.GetTextFromUser("Transfer title");
                correctTitle = true;
                string message = "Incorrect title - try again...";
                correctTitle = _ioHelper.CheckingIfIsNullOrWhiteSpace(title, correctTitle, message);
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
                amount = _ioHelper.GetDoubleFromUser("Transfer amount");
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
            List<Account> AllAccounts = _accountsService.GetAllAccounts();
            do
            {
                accountName = _ioHelper.GetTextFromUser("Provide account name");
                createAccount = true;
                string message = "Incorrect name - try again...";
                createAccount = _ioHelper.CheckingIfIsNullOrWhiteSpace(accountName, createAccount, message);

                for (int i = 0; i < AllAccounts.Count; i++)
                {
                    if (accountName == AllAccounts[i].Name)
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
                Number = _ioHelper.GenerateGuidToUser(),
                Balance = 1000,
            };
            
            Console.WriteLine($"The opening balance of the account: {newAccount.Balance}$");

            _accountsService.AddAccount(newAccount);
            Console.WriteLine($"\nAccount \"{newAccount.Name}\" added successfully");
        }


    }
}