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
            List<Transfer> history = _transfersHistory.GetAllTransfers();
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
            PrintAccounts();
            Console.WriteLine();

            string c = GetTextFromUser("The account from which the funds will be withdrawn - enter your account name");
            while (string.IsNullOrWhiteSpace(c))
            {
                Console.WriteLine("Incorrect name - try again...");
                c = GetTextFromUser("The account from which the funds will be withdrawn - enter your account name");
            }

            Guid targetAccount = GetGuidFromUser("The number of GUID");
            List<Account> guidNumber = _finances.GetAllAccounts();
            
            double amount = GetDoubleFromUser("Transfer amount");
            for (int i = 0; i < guidNumber.Count; i++)
            {
                if (guidNumber[i].AccountName == c)
                {
                    while (amount < 0 || amount > guidNumber[i].AccountBalance)
                    {
                        Console.WriteLine("Wrong amount - try again...");
                        return;
                    }
                }
            }

            
            Guid sourceAccount = default(Guid);
            
            for (int i = 0; i < guidNumber.Count; i++)
            {
                if (guidNumber[i].AccountName == c)
                {
                    sourceAccount = guidNumber[i].AccountNumber;
                }
            }

            //if (sourceAccount==default(Guid))
            //{
            //    Console.WriteLine("No field found");
            //    return;
            //}
            string title = GetTextFromUser("Transfer title");
            while (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Incorrect title - try again...");
                title = GetTextFromUser("Transfer title");
            }

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

            MakeAnOutgoingTransfer(c, amount);

            _transfersHistory.AddTransfer(newTransferOut);
        }

        private void MakeAnOutgoingTransfer(string account, double amount)
        {
            List<Account> outgoingTransfer = _finances.GetAllAccounts();
            for (int i = 0; i < outgoingTransfer.Count; i++)
            {
                Account k = outgoingTransfer[i];
                if (k.AccountName == account)
                {
                    k.AccountBalance = k.AccountBalance - amount;
                    Console.WriteLine($"Account: \"{k.AccountName}\" - Balance: {k.AccountBalance}$");
                }
            }
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
            List<Account> guidNumber = _finances.GetAllAccounts();
            if (guidNumber.Count < 2)
            {
                Console.WriteLine("It is not possible to make an enternal transfer - minimum 2 accounts needed");
                return;
            }

            PrintAccounts();
            Console.WriteLine();

            string a = GetTextFromUser("The account from which the funds will be withdrawn - enter your account name");

                //for (int i = 0; i < guidNumber.Count; i++)
                //{
                //    do
                //    {
                //        a = GetTextFromUser("The account from which the funds will be withdrawn - enter your account name");

                //    } while (guidNumber[i].AccountName == a);


                //}

                //for (int i = 0; i < guidNumber.Count; i++)
                //{
                //    while (a != guidNumber[i].AccountName)
                //    {
                //        Console.WriteLine("There is no such account");
                //        return;
                //    }
                //}
            
            string b = GetTextFromUser("The account to which the funds will be transferred - enter your account name");

            //for (int i = 0; i < guidNumber.Count; i++)
            //{
            //    if (b != guidNumber[i].AccountName)
            //    {
            //        Console.WriteLine("There is no such account");
            //        return;
            //    }
            //}

            double amount = GetDoubleFromUser("Transfer amount");
            for (int i = 0; i < guidNumber.Count; i++)
            {
                if (guidNumber[i].AccountName == a)
                {
                    while (amount < 0 || amount > guidNumber[i].AccountBalance)
                    {
                        Console.WriteLine("Wrong amount - try again...");
                        return;
                    }
                }
            }

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
                if (sourceAccount==targetAccount)
                {
                    Console.WriteLine("It is no possible to make an internal transfer-wrong account selected.");
                    return;
                }
            }

            //if (sourceAccount == default(Guid))
            //{
            //    Console.WriteLine("No field found");
            //    return;
            //}

            //if (targetAccount == default(Guid))
            //{
            //    Console.WriteLine("No field found");
            //    return;
            //}
            string title = GetTextFromUser("Transfer title");
            while (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Incorrect title - try again...");
                title = GetTextFromUser("Transfer title");
            }

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

            MakeADomesticTransfer(a, b, amount);

            _transfersHistory.AddTransfer(newTransfer);
        }

        //Account debitAccount = GetAccountByName(a);
        //Account creditAccount = GetAccountByName(b);

        //bool correctAmount;

        //do
        //{
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
               while (accountName == sprawdzenie[i].AccountName )
               {
                   Console.WriteLine("The name exist - try again...");
                   return;
               }
            }

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
