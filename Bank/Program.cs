using System;
using System.Collections.Generic;
using System.Linq;
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

        private Menu                        _registerMenu               = new Menu();
        private Menu                        _customerMenu               = new Menu();
        private IoHelper                    _ioHelper                   = new IoHelper();
        private AccountsService             _accountsService            = new AccountsService();
        private CustomersService            _customersService           = new CustomersService();
        private TransfersService            _transfersService           = new TransfersService();
        private DatabaseManagmentService    _databaseManagmentService   = new DatabaseManagmentService();

        private Customer _customer = null;
        private bool _exit = false;

        private void Run()
        {
            _databaseManagmentService.EnsureDatabaseCreation();
            RegisterMenuOptions();
            
            do
            {
                _registerMenu.PrintAvailableOptions();

                int userChoice = _ioHelper.GetIntFromUser("Select number");

                _registerMenu.ExecuteOption(userChoice);
            } 
            while (!_exit);
        }

        private void RegisterMenuOptions()
        {
            Console.WriteLine("Choose option:");

            //var addCustomerItem = new MenuItem                                        //do aktualizacji!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //{
            //    Keys = 1,
            //    Action = SignIn,
            //    Description = "Sign In"
            //};
            //_menu.AddOption(addCustomerItem);
            
            _registerMenu.AddOption(new MenuItem { Keys = 1, Action = SignIn,   Description = "Sign In" });
            _registerMenu.AddOption(new MenuItem { Keys = 2, Action = Register, Description = "Register" });
        }

        private void SignIn()
        {
            var eMail = _ioHelper.GetTextFromUser("Enter your e-mail");
            var password = _ioHelper.GetTextFromUser("Enter password");
            
            var customer = _customersService.GetCustomer(eMail, password);
            if (customer == null)
            {
                Console.WriteLine("Login failed. Try again...");
                return;
            }
            
            Console.WriteLine("You are logged in!");
            
            _customer = customer;    
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
            CustomerMenuOption();

            do
            {
                _customerMenu.PrintAvailableOptions();
                
                int userChoice = _ioHelper.GetIntFromUser("Select number");

                _customerMenu.ExecuteOption(userChoice);
            } 
            while (!_exit);
        }

        private void CustomerMenuOption()
        {
            Console.WriteLine();
            Console.WriteLine("Choose option:");               
            _customerMenu.AddOption(new MenuItem { Keys = 1, Action = CreateAccount,              Description = "Create account" });
            _customerMenu.AddOption(new MenuItem { Keys = 2, Action = DomesticTransfer,           Description = "Make a domestic transfer" });
            _customerMenu.AddOption(new MenuItem { Keys = 3, Action = OutgoingTransfer,           Description = "Make an outgoing transfer" });
            _customerMenu.AddOption(new MenuItem { Keys = 4, Action = PrintAccounts,              Description = "Check yours accounts balance" });
            _customerMenu.AddOption(new MenuItem { Keys = 5, Action = CheckTheHistoryOfTransfers, Description = "Check the history of transfers" });
            _customerMenu.AddOption(new MenuItem { Keys = 6, Action = () => { _exit = true; },    Description = "Exit" });
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
            var customerAccounts = _accountsService.GetCustomerAccounts(_customer.Id);
            if (customerAccounts.Count < 1)
            {
                Console.WriteLine("It is not possible to make an outgoing transfer - minimum 1 accounts needed");
                return;
            }

            Console.WriteLine("Your accounts balance:");
            foreach (var account in customerAccounts)
            {
                _ioHelper.PrintCustomerAccounts(account);
            }

            Console.WriteLine("Make an outgoing transfer:");
            var sourceAccountId = _ioHelper.GetIntFromUser("Provide the source account number");

            double amount = GetAmountFromUser(customerAccounts, sourceAccountId);

            Guid sourceAccount = customerAccounts[sourceAccountId].Number;
            Guid targetAccount = _ioHelper.GetGuidFromUser("Provide the target account number (the number of GUID)");

            string title = GetTheTitleOfTransfer();

            Transfer newTransfer = new Transfer()
            {
                CustomerId = _customer.Id,
                Title = title,
                Amount = amount,
                DateOfTheTransfer = DateTime.Now,
                TypOfTransfer = "External transfer",
                SourceAccount = sourceAccount,
                TargetAccount = targetAccount,
            };
            Console.WriteLine($"Date of the transfer: {newTransfer.DateOfTheTransfer}");

            double currentBalance = _transfersService.BalanceChangeOfTargetAccount(sourceAccountId, amount);
            Console.WriteLine($"Account: \"{customerAccounts[sourceAccountId].Name}\" - Balance: {currentBalance}$");

            _transfersService.AddTransfer(newTransfer);
        

            //_transfersService.AddTransfer(newTransferOut);
        }

        private void DomesticTransfer()
        {
            var customerAccounts = _accountsService.GetCustomerAccounts(_customer.Id);
            if (customerAccounts.Count < 2)
            {
                Console.WriteLine("It is not possible to make a domestic transfer - minimum 2 accounts needed");
                return;
            }

            Console.WriteLine("Your accounts balance:");
            foreach (var account in customerAccounts)
            {
                _ioHelper.PrintCustomerAccounts(account);
            }
            
            Console.WriteLine("Make a domestic transfer:");
            //var sourceAccountId = GetAccountIdFromUser(customerAccounts);
            var sourceAccountId = _ioHelper.GetIntFromUser("Provide the source account number");

            if (!customerAccounts.Any(account => account.Id == sourceAccountId))
            {
                Console.WriteLine("Incorrect author Id!");
                return;
            }



            var targetAccountId = _ioHelper.GetIntFromUser("Provide the target account number");

            while (sourceAccountId == targetAccountId)
            {
                Console.WriteLine("Same account selected - try again...");
                Console.ReadLine();
            }

            var amount = GetAmountFromUser(customerAccounts, sourceAccountId);

            Guid sourceAccount = customerAccounts[sourceAccountId].Number;
            Guid targetAccount = customerAccounts[targetAccountId].Number;

            var title = GetTheTitleOfTransfer();

            Transfer newTransfer = new Transfer()
            {
                CustomerId = _customer.Id,
                Title = title,
                Amount = amount,
                DateOfTheTransfer = DateTime.Now,
                TypOfTransfer = "Internal transfer",
                SourceAccount = sourceAccount,
                TargetAccount = targetAccount,
            };
            Console.WriteLine($"Date of the transfer: {newTransfer.DateOfTheTransfer}");

            var currentBalanceSource = _transfersService.BalanceChangeOfSourceAccount(sourceAccountId, amount);
            Console.WriteLine($"Account: \"{customerAccounts[sourceAccountId].Name}\" - Balance: {currentBalanceSource}$");

            var currentBalanceTarget = _transfersService.BalanceChangeOfTargetAccount(targetAccountId, amount);
            Console.WriteLine($"Account: \"{customerAccounts[targetAccountId].Name}\" - Balance: {currentBalanceTarget}$");



            _transfersService.AddTransfer(newTransfer);
        }

        //private int GetAccountIdFromUser(List<Account> accounts)
        //{
        //    var sourceAccountId = _ioHelper.GetIntFromUser("Provide the source account number");

        //    if (!accounts.Any(account => account.Id == sourceAccountId))
        //    {
        //        Console.WriteLine("Incorrect author Id!");
        //        return;
        //    }

        //}

        public double GetAmountFromUser(List<Account> customerAccounts, int AccountId)
        {
            var amount = _ioHelper.GetDoubleFromUser("Transfer amount");

            while (amount <= 0 || amount > customerAccounts[AccountId].Balance)
            {
                Console.WriteLine("Wrong amount - try again...");
                Console.WriteLine();
            }

            return amount;
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

        private string GetTheTitleOfTransfer()
        {
            string title;
            bool correctTitle;
            do
            {
                title = _ioHelper.GetTextFromUser("Transfer title");
                correctTitle = true;
                correctTitle = _ioHelper.CheckingIfIsNullOrWhiteSpace(title, correctTitle);
            }
            while (correctTitle == false);

            return title;
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
            bool isAccountNameCorrect;

            do
            {
                accountName = _ioHelper.GetTextFromUser("Provide account name");
                isAccountNameCorrect = true;
                isAccountNameCorrect = _ioHelper.CheckingIfIsNullOrWhiteSpace(accountName, isAccountNameCorrect);

                //for (int i = 0; i < AllAccounts.Count; i++)
                //{
                //    if (accountName == AllAccounts[i].Name)
                //    {
                //        Console.WriteLine("The name exist - try again...");
                //        Console.WriteLine();
                //        createAccount = false;
                //    }
                //}
            }
            while (isAccountNameCorrect == false);

            Account newAccount = new Account()
            {
                CustomerId = _customer.Id,
                Name = accountName,
                Number = _ioHelper.GenerateGuidToUser(),
                Balance = 1000,
            };
            
            Console.WriteLine($"The opening balance of the account: {newAccount.Balance}$");            //moge o to pytac czy siegam do bazy danych??

            _accountsService.AddAccount(newAccount);
            Console.WriteLine($"\nAccount \"{newAccount.Name}\" added successfully");
        }


    }
}