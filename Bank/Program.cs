using System;
using System.Collections.Generic;
using System.Timers;
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

        private Menu                          _registerMenu                  = new Menu();
        private Menu                          _customerMenu                  = new Menu();
        private IoHelper                      _ioHelper                      = new IoHelper();
        private IoRegisterHelper              _ioRegisterHelper              = new IoRegisterHelper();
        private IoTransferHelper              _ioTransferHelper              = new IoTransferHelper(new IoHelper(), new AccountsService());
        private AccountsService               _accountsService               = new AccountsService();
        private CustomersService              _customersService              = new CustomersService();
        private TransfersService              _transfersService              = new TransfersService();
        private DatabaseManagmentService      _databaseManagmentService      = new DatabaseManagmentService();
        private IoSerializationDesireHelper   _serializationDesireFactory    = new IoSerializationDesireHelper();
        private IoStatementOfOperationsHelper _ioStatementOfOperationsHelper = new IoStatementOfOperationsHelper();
        private List<Transfer>                _listOfTransfers               = new List<Transfer>();

        private Customer _customer = null;
        private bool _exit = false;
        private static Timer aTimer = new Timer(300000);


        private void Run()
        {
            _databaseManagmentService.EnsureDatabaseCreation();
            aTimer.Start();
            SetTimer();
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
            var eMail = _ioRegisterHelper.GetEMailFromUser("Enter your e-mail");
            var phoneNumber = _ioRegisterHelper.GetPhoneNumberFromUser("Enter phone number");
            var password = _ioTransferHelper.GetNotNullTextFromUser("Enter password");

            var newCustomer = new Customer
            {
                EMail = eMail,
                PhoneNumber = phoneNumber,
                Password = password,
            };

            _customersService.AddCustomer(newCustomer);
            _ioHelper.WriteString("Registration is correct!");
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
            _customerMenu.AddOption(new MenuItem { Keys = 6, Action = GeneratingAListOfOperations,Description = "Generating a list of operations" });
            _customerMenu.AddOption(new MenuItem { Keys = 7, Action = () => { _exit = true; },    Description = "Exit" });
        }

        private void GeneratingAListOfOperations()
        {
            var customerAccounts = _accountsService.GetCustomerAccounts(_customer.Id);
            if (_ioTransferHelper.ChceckIfListIsEmpty(customerAccounts) == true)
            {
                return;
            }

            var listOfStatementOfOperations = new List<StatementOfOperations>();

            foreach (var account in customerAccounts)
            {
                var statementOfOperationForAccount = _ioStatementOfOperationsHelper.GetStatementOfOperationForAccount(account);

                _ioStatementOfOperationsHelper.PrintStatementOfOperations(account, statementOfOperationForAccount);

                listOfStatementOfOperations.Add(statementOfOperationForAccount);
            }

            Console.WriteLine();
            var desire = _ioHelper.GetSerializationDesireFromUser("Do you want to export the operation statement to a file?");
            Console.WriteLine();
            
            _serializationDesireFactory.ImplementationOfSerializationDecision(desire, listOfStatementOfOperations);
        }

        private void CheckTheHistoryOfTransfers()
        {
            var history = _transfersService.GetAllTransfersForCustomer(_customer.Id);
            if (history.Count == 0)
            {
                Console.WriteLine();
                _ioHelper.WriteString("No transfers has been sent");
                return;
            }

            Console.WriteLine();
            _ioHelper.WriteString("The history of transfers :");
            
            foreach (Transfer transfers in history)
            {                                                                           
                string text = @$"Date of the transfer:  {transfers.DateOfTheTransfer}
                                 GUID source account:   {transfers.SourceAccount}
                                 GUID target account:   {transfers.TargetAccount}
                                 Transfer Title:        {transfers.Title}  
                                 Transfers Amount:      {transfers.Amount}
                                 Type of transfer:      {transfers.TypOfTransfer}";
                _ioHelper.WriteString(text);
            }
        }

        private void PrintAccounts()
        {
            var customerAccounts = _accountsService.GetCustomerAccounts(_customer.Id);
            if (_ioTransferHelper.ChceckIfListIsEmpty(customerAccounts) == true)
            {
                return;
            }

            _ioTransferHelper.PrintCustomerAccounts(customerAccounts, _listOfTransfers);
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

            if (_ioTransferHelper.CheckingIfThereIsAccountWithPositiveBalance(customerAccounts)==false)
            {
                return;
            }

            _ioTransferHelper.PrintCustomerAccounts(customerAccounts, _listOfTransfers);
            
            var sourceAccount = _ioTransferHelper.GetAccountFromUser("Make an outgoing transfer:", customerAccounts);
            var amount = _ioTransferHelper.GetAmountFromUser(sourceAccount, _listOfTransfers);

            Guid targetGuid;
            do
            {
                targetGuid = _ioHelper.GetGuidFromUser("Provide the target account number (the number of GUID)");
            }
            while (_ioTransferHelper.CheckingIfTargetGuidIsCustomerAccount(_customer.Id, targetGuid));
            
            var title = _ioTransferHelper.GetNotNullTextFromUser("Transfer title");

            Transfer newTransfer = new Transfer()
            {
                CustomerId = _customer.Id,
                Title = title,
                Amount = amount,
                TypOfTransfer = "External transfer",
                SourceAccount = sourceAccount.Number,
                TargetAccount = targetGuid,
            };
            

            if (!_ioTransferHelper.CheckingIfTargetGuidIsAccountInOurBank(_customer.Id, targetGuid))
            {
                _listOfTransfers.Add(newTransfer);
                _ioHelper.WriteString("Your transfer has been accepted for processing.");

            }
            else
            {
                newTransfer.DateOfTheTransfer = DateTime.Now;
                _ioHelper.WriteString($"Date of the transfer: {newTransfer.DateOfTheTransfer}");

                _transfersService.BalanceChangeOfAccounts(sourceAccount.Number, targetGuid, amount);
                var currentBalanceSource = _accountsService.GetCurrentBalanceOfAccount(sourceAccount.Id);

                _ioHelper.WriteString($"Account: \"{sourceAccount.Name}\" - Balance: {currentBalanceSource}$");
                
                _transfersService.AddTransfer(newTransfer);
            }
        }

        public void SetTimer()
        {
            aTimer.Elapsed += SendTransfers;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void SendTransfers(object source, ElapsedEventArgs e)
        {
            foreach (var transfer in _listOfTransfers)
            {
                _transfersService.ReductionOfSourceAccountBalance(transfer.SourceAccount, transfer.Amount);
                
                transfer.DateOfTheTransfer = e.SignalTime;
                _transfersService.SendTransferOut(transfer);

                _transfersService.AddTransfer(transfer);
            }
            _listOfTransfers.Clear();
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

            if (_ioTransferHelper.CheckingIfThereIsAccountWithPositiveBalance(customerAccounts) == false)
            {
                return;
            }

            _ioTransferHelper.PrintCustomerAccounts(customerAccounts, _listOfTransfers);

            var sourceAccount = _ioTransferHelper.GetAccountFromUser("Make a domestic transfer:", customerAccounts);
            var targetAccount = _ioTransferHelper.GetAndCheckIfTheAccountIsNonSourceAccount(customerAccounts, sourceAccount.Id);
            var amount = _ioTransferHelper.GetAmountFromUser(sourceAccount, _listOfTransfers);
            var title = _ioTransferHelper.GetNotNullTextFromUser("Transfer title");

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

            _transfersService.BalanceChangeOfAccounts(sourceAccount.Number, targetAccount.Number, amount);
            var currentBalanceSource = _accountsService.GetCurrentBalanceOfAccount(sourceAccount.Id);
            var currentBalanceTarget = _accountsService.GetCurrentBalanceOfAccount(targetAccount.Id);

            Console.WriteLine($"Account: \"{sourceAccount.Name}\" - Balance: {currentBalanceSource}$");
            _ioHelper.WriteString($"Account: \"{targetAccount.Name}\" - Balance: {currentBalanceTarget}$");

            _transfersService.AddTransfer(newTransfer);
        }

        private void CreateAccount()
        {
            Console.WriteLine();
            Console.WriteLine("Create Account:");
            var accountName = _ioTransferHelper.GetNotNullTextFromUser("Provide account name");

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
    }
}