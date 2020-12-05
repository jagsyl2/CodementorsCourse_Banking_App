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

                var userChoice = _ioHelper.GetIntFromUser("Select number");

                _registerMenu.ExecuteOption(userChoice);
            } 
            while (!_exit);
        }

        private void RegisterMenuOptions()
        {
            Console.WriteLine("Choose option:");

            _registerMenu.AddOption(new MenuItem { Keys = 1, Action = SignIn,   Description = "Sign In" });
            _registerMenu.AddOption(new MenuItem { Keys = 2, Action = Register, Description = "Register" });
        }

        private void SignIn()
        {
            Console.WriteLine();
            var eMail = _ioHelper.GetTextFromUser("Enter your e-mail");
            var password = _ioHelper.GetTextFromUser("Enter password");
            
            var customer = _customersService.GetCustomer(eMail, password);
            if (customer == null)
            {
                _ioHelper.WriteString("Login failed. Try again...");
                return;
            }

            _ioHelper.WriteString("You are logged in!");
            
            _customer = customer;
            Menu();
        }

        private void Register()
        {
            Console.WriteLine();
            var eMail = GetEMailFromUser("Enter your e-mail");
            var phoneNumber = _ioHelper.GetPhoneNumberFromUser("Enter phone number");
            var password = _ioHelper.GetTextFromUser("Enter password");

            var newCustomer = new Customer
            {
                EMail = eMail,
                PhoneNumber = phoneNumber,
                Password = password,
            };

            _customersService.AddCustomer(newCustomer);
            _ioHelper.WriteString("Registration is correct!");
        }

        public string GetEMailFromUser(string message)
        {
            string eMail;
            bool validation;

            do
            {
                eMail = _ioHelper.GetTextFromUser(message);
                validation = true;

                if (!(eMail).Contains("@"))
                {
                    _ioHelper.WriteString("Incorrect adress e-mail (must contain the @ sign). Try again...");

                    validation = false;
                    continue;
                }
                
                if (_customersService.CheckingIfANewCustomerIsRegistering(eMail) == true)
                {
                    _ioHelper.WriteString("The given email address exists. Try again...");
                    validation = false;
                }
            }
            while (validation == false);

            return eMail;
        }

        private void Menu()
        {
            CustomerMenuOption();

            do
            {
                _customerMenu.PrintAvailableOptions();
                
                var userChoice = _ioHelper.GetIntFromUser("Select number");

                _customerMenu.ExecuteOption(userChoice);
            } 
            while (!_exit);
        }

        private void CustomerMenuOption()
        {
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
            var history = _transfersService.GetTransfers(_customer.Id);
            if (history.Count == 0)
            {
                Console.WriteLine();
                _ioHelper.WriteString("No transfers has been sent");
                return;
            }

            Console.WriteLine();
            _ioHelper.WriteString("The history of transfers:");
            
            foreach (Transfer transfers in history)
            {                                                                           
                string text = @$"Date of the transfer:  {transfers.DateOfTheTransfer}
                                 GUID source account:   {transfers.SourceAccount}
                                 GUID target account:   {transfers.TargetAccount}
                                 Transfer Title:        {transfers.Title}  
                                 Transfers Amount:      {transfers.Amount}
                                 Typ of transfer:       {transfers.TypOfTransfer}";
                _ioHelper.WriteString(text);
            }
        }

        private void PrintAccounts()
        {
            var customerAccounts = _accountsService.GetCustomerAccounts(_customer.Id);

            if (customerAccounts.Count == 0)
            {
                Console.WriteLine();
                _ioHelper.WriteString("No accounts has been created");
                return;
            }

            PrintCustomerAccounts(customerAccounts);
            Console.WriteLine();
        }

        private void OutgoingTransfer()
        {
            var customerAccounts = _accountsService.GetCustomerAccounts(_customer.Id);
            if (customerAccounts.Count < 1)
            {
                Console.WriteLine();
                _ioHelper.WriteString("It is not possible to make an outgoing transfer - minimum 1 accounts needed");
                return;
            }

            PrintCustomerAccounts(customerAccounts);
            
            var sourceAccount = _ioHelper.GetAccountFromUser("Make an outgoing transfer:", customerAccounts);
            var amount = _ioHelper.GetAmountFromUser(sourceAccount);
            var targetGuid = _ioHelper.GetGuidFromUser("Provide the target account number (the number of GUID)");
            var title = CheckingIfItIsCorrect("Transfer title");

            Transfer newTransfer = new Transfer()
            {
                CustomerId = _customer.Id,
                Title = title,
                Amount = amount,
                DateOfTheTransfer = DateTime.Now,
                TypOfTransfer = "External transfer",
                SourceAccount = sourceAccount.Number,
                TargetAccount = targetGuid,
            };
            _ioHelper.WriteString($"Date of the transfer: {newTransfer.DateOfTheTransfer}");

            var currentBalance = _transfersService.BalanceChangeOfSourceAccount(sourceAccount.Id, amount);
            _ioHelper.WriteString($"Account: \"{sourceAccount.Name}\" - Balance: {currentBalance}$");

            _transfersService.AddTransfer(newTransfer);
        }

        private void DomesticTransfer()
        {
            var customerAccounts = _accountsService.GetCustomerAccounts(_customer.Id);
            if (customerAccounts.Count < 2)
            {
                Console.WriteLine();
                _ioHelper.WriteString("It is not possible to make a domestic transfer - minimum 2 accounts needed");
                return;
            }

            PrintCustomerAccounts(customerAccounts);

            var sourceAccount = _ioHelper.GetAccountFromUser("Make a domestic transfer:", customerAccounts);
            var targetAccount = _ioHelper.GetAccountFromUser(customerAccounts, sourceAccount.Id);
            var amount = _ioHelper.GetAmountFromUser(sourceAccount);
            var title = CheckingIfItIsCorrect("Transfer title");

            Transfer newTransfer = new Transfer()
            {
                CustomerId = _customer.Id,
                Title = title,
                Amount = amount,
                DateOfTheTransfer = DateTime.Now,
                TypOfTransfer = "Internal transfer",
                SourceAccount = sourceAccount.Number,
                TargetAccount = targetAccount.Number,
            };
            _ioHelper.WriteString($"Date of the transfer: {newTransfer.DateOfTheTransfer}");

            var currentBalanceSource = _transfersService.BalanceChangeOfSourceAccount(sourceAccount.Id, amount);
            Console.WriteLine($"Account: \"{sourceAccount.Name}\" - Balance: {currentBalanceSource}$");

            var currentBalanceTarget = _transfersService.BalanceChangeOfTargetAccount(targetAccount.Id, amount);
            _ioHelper.WriteString($"Account: \"{targetAccount.Name}\" - Balance: {currentBalanceTarget}$");

            _transfersService.AddTransfer(newTransfer);
        }

        private void PrintCustomerAccounts(List<Account> customerAccounts)
        {
            Console.WriteLine();
            Console.WriteLine("Your accounts balance:");
            foreach (var account in customerAccounts)
            {
                _ioHelper.PrintAccount(account);
            }
        }

        private void CreateAccount()
        {
            Console.WriteLine();
            Console.WriteLine("Create Account:");
            var accountName = CheckingIfItIsCorrect("Provide account name");

            Account newAccount = new Account()
            {
                CustomerId = _customer.Id,
                Name = accountName,
                Number = _ioHelper.GenerateGuidToUser(),
                Balance = 1000,
            };

            Console.WriteLine($"The opening balance of the account: {newAccount.Balance}$");

            _accountsService.AddAccount(newAccount);
            _ioHelper.WriteString($"\nAccount \"{newAccount.Name}\" added successfully");
        }

        private string CheckingIfItIsCorrect(string message)
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