using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
                        Console.WriteLine("Make an outgoing transfer");           //funkcja
                        break;
                    case 4:
                        PrintAccounts();
                        break;
                    case 5:
                        CheckTheHistoryOfTransfers();
                                                                         //funkcja
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
            if (userChoice == 6)                                        //ew. funkcja
            {
                exit = true;
            }
        }

        private void CheckTheHistoryOfTransfers()
        {
            List<Transfer> history = _transfersHistory.GetAllTransfers();
            foreach (Transfer transfers in history)
            {                                                                           
                string text = @$"   Date of the transfer:  {transfers.DateOfTheTransfer}
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
            PrintAccounts();
            Console.WriteLine();

            string c = GetTextFromUser("The account from which the funds will be withdrawn - enter your account name");
            // IfIsNullOrWhiteSpace(a);
            Guid targetAccount = GetGuidFromUser("The number of GUID");

            double amount;
            amount = GetDoubleFromUser("Transfer amount");

            List<Account> guidNumber = _finances.GetAllAccounts();
            Guid sourceAccount = default(Guid);
            
            for (int i = 0; i < guidNumber.Count; i++)
            {
                if (guidNumber[i].AccountName == c)
                {
                    sourceAccount = guidNumber[i].AccountNumber;
                }
            }
            
            if (sourceAccount==default(Guid))
            {
                Console.WriteLine("No field found");
                return;
            }
            
            Transfer newTransferOut = new Transfer()
            {
                TransferTitle = GetTextFromUser("Transfer title"),
                TransferAmount = amount,
                DateOfTheTransfer = DateTime.Now,
                TypOfTransfer = "External transfer",
                SourceAccount = sourceAccount,
                TargetAccount = targetAccount,
            };
            Console.WriteLine($"Date of the transfer: {newTransferOut.DateOfTheTransfer}");

            //MakeADomesticTransfer(a, b, amount);

            _transfersHistory.AddTransfer(newTransferOut);


        }

        private Guid GetGuidFromUser(string message)
        {
            Guid guid;
            while (!Guid.TryParse(GetTextFromUser(message), out guid))
            {
                Console.WriteLine("Incorrect number - try again...");
            }
            return guid;
        }

        private void DomesticTransfer()
        {
            PrintAccounts();
            Console.WriteLine();

            
            string a = GetTextFromUser("The account from which the funds will be withdrawn - enter your account name");
            // IfIsNullOrWhiteSpace(a);
            
            string b = GetTextFromUser("The account to which the funds will be transferred - enter your account name");
            // IfIsNullOrWhiteSpace(b);

            //Account debitAccount = GetAccountByName(a);
            //Account creditAccount = GetAccountByName(b);
            double amount;
            //bool correctAmount;

            //do
            //{
               amount = GetDoubleFromUser("Transfer amount");



            //                                //for (int i = 0; i < accounts.Count; i++)
            //                                //{
            //                                //    if (accounts[i].AccountName == a)
            //                                //    {
            //                                //        debitAccount = accounts[i];
            //                                //    };
            //                                //    if (accounts[i].AccountName == b)
            //                                //    {
            //                                //        creditAccount = accounts[i];
            //                                //    };                    
            //                                //}

            //    correctAmount = (amount > 0 && amount <= debitAccount.AccountBalance);
            //    if (!correctAmount)
            //    {
            //        Console.WriteLine("zła kwota");
            //    }
            //} while (!correctAmount);





            //    while (amount < 0 || amount > newAccount.AccountBalance)
            //    {
            //        Console.WriteLine("Wrong amount - try again...");
            //        amount = GetDoubleFromUser("Transfer amount");
            //    }
            //}
            List<Account> guidNumber = _finances.GetAllAccounts();
            Guid sourceAccount = default(Guid);
            Guid targetAccount = default(Guid);

            for (int i = 0; i < guidNumber.Count; i++)
            {
                if (guidNumber[i].AccountName == a)
                {
                    sourceAccount = guidNumber[i].AccountNumber;

                }
                if (guidNumber[i].AccountName == b)
                {
                    targetAccount = guidNumber[i].AccountNumber;
                }
            }

            if (sourceAccount == default(Guid))
            {
                Console.WriteLine("No field found");
                return;
            }

            if (targetAccount == default(Guid))
            {
                Console.WriteLine("No field found");
                return;
            }

            Transfer newTransfer = new Transfer()
            {
                TransferTitle = GetTextFromUser("Transfer title"),
                TransferAmount = amount,
                DateOfTheTransfer = DateTime.Now,
                TypOfTransfer = "Internal transfer",
                SourceAccount = sourceAccount,
                TargetAccount = targetAccount,

            };
            Console.WriteLine($"Date of the transfer: {newTransfer.DateOfTheTransfer}");

            MakeADomesticTransfer(a, b, amount);

            _transfersHistory.AddTransfer(newTransfer);

        }

                        //private Account GetAccountByName(string accountName)
                        //{
                        //    List<Account> accounts = _finances.GetAllAccounts();
                        //    Account accountToReturn = null;
                        //    foreach (Account account in accounts)
                        //    {
                        //        if (account.AccountName == accountName)
                        //        {
                        //            accountToReturn = account;
                        //        }               
                        //    };
                        //    return accountToReturn;
                        //}

        //private void IfIsNullOrWhiteSpace(string instruction)
        //{
        //    while (string.IsNullOrWhiteSpace(instruction))
        //    {
        //        Console.WriteLine("Incorrect name - try again...");
        //        instruction = GetTextFromUser(instruction);
        //    }

        //}
        
        private void MakeADomesticTransfer(string account1, string account2, double amount)
        {
            List <Account> domesticTransfer = _finances.GetAllAccounts();
            for (int i= 0; i< domesticTransfer.Count; i++) 
                {
                    Account k = domesticTransfer[i];
                    if (k.AccountName == account1)
                    {
                        k.AccountBalance = k.AccountBalance - amount;
                        Console.WriteLine($"Account: \"{k.AccountName}\" - Balance: {k.AccountBalance}$");
                    }
                    if (k.AccountName == account2)
                    {
                        k.AccountBalance = k.AccountBalance + amount;
                        Console.WriteLine($"Account: \"{k.AccountName}\" - Balance: {k.AccountBalance}$");
                    }
            }
            
            
        }

        private double GetDoubleFromUser(string message)
        {
            double amount;
            while (!double.TryParse(GetTextFromUser(message), out amount))
                {
                    Console.WriteLine("Negative value - try again...");
                }
            return amount; 
        }

        private void PrintAccounts()
        {
            List<Account> yourAccounts = _finances.GetAllAccounts();
            for (int i = 0; i < yourAccounts.Count; i++)
            {
                Account account = yourAccounts[i];
                string text = $"Account: \"{account.AccountName}\" - Balance: {account.AccountBalance}$ - Account Number: {account.AccountNumber}";
                Console.WriteLine(text);
            }
            if (yourAccounts.Count == 0)
            {
                Console.WriteLine("No accounts has been created");
            } 
            

            
        }

        private int GetIntFromUser(string message)
        {
           int number;
           while (!int.TryParse(GetTextFromUser(message), out number))
           {
               Console.WriteLine("Incorrect option - try again...");
           }
           return number;
           
        }

        private void CreateAccount()
        {
            Console.WriteLine("Create Account:");

            string accountName = GetTextFromUser("Provide account name");
            while (string.IsNullOrWhiteSpace(accountName))
            {
                Console.WriteLine("Incorrect name - try again...");
                accountName = GetTextFromUser("Provide account name");
            }
            List<Account> sprawdzenie = _finances.GetAllAccounts();
            for (int i = 0; i < sprawdzenie.Count; i++)
            {
               while (accountName == sprawdzenie[i].AccountName )                                                                     //IfIsNullOrWhiteSpace(accountName);
                {
                    Console.WriteLine("The name exist - try again...");
                }
            }

            Account newAccount = new Account()
            {
                AccountName = accountName,      // nazwa konta: 1. nie pusta; 2. nie taka sama jak juz jest!! 
                AccountNumber = GenerateGuidToUser(),
                AccountBalance = 1000,                                      // wartosc konta poczatkowa!!!!!!!!!!!!!
            };
            
            Console.WriteLine($"The opening balance of the account: {newAccount.AccountBalance}$");

            _finances.AddAccounts(newAccount);
            Console.WriteLine($"\nAccount \"{newAccount.AccountName}\" added successfully");
        }

        private Guid GenerateGuidToUser()
        {
            Guid g = Guid.NewGuid();
            Console.WriteLine($"Your account number: {g}");
            return g;
        }

        private string GetTextFromUser(string message)
        {
            Console.Write($"{message}: ");
            return Console.ReadLine();

        }
    }
}
