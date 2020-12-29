using BankTransfers.DataLayer;
using BankTransfers.DataLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace BankTransfers.BusinessLayer
{
    public class TransfersService
    {
        private AccountsService _accountsService = new AccountsService();
        public void AddTransfer(Transfer transfer)
        {
            using (var context = new BankDbContex())
            {
                context.Transfers.Add(transfer);
                context.SaveChanges();
            }
        }

        public List<Transfer> GetAllTransfers()
        {
            using (var context = new BankDbContex())
            {
                return context.Transfers.ToList();
            }
        }

        public List<Transfer> GetTransfers(int customerId)
        {
            using (var context = new BankDbContex())
            {
                return context.Transfers
                    .Where(transfer => transfer.CustomerId == customerId)
                    .ToList();
            }
        }

        public List<Transfer> GetAllTransfersForCustomer(int customerId)
        {
            List<Transfer> listOfIncominAccounts = new List<Transfer>();

            using (var context = new BankDbContex())
            {
                var listOfAccounts = _accountsService.GetCustomerAccounts(customerId);

                listOfIncominAccounts.AddRange(GetTransfers(customerId));

                foreach (var account in listOfAccounts)
                {
                    var list2 = context.Transfers
                        .Where(transfer => transfer.CustomerId != customerId && transfer.TargetAccount == account.Number)
                        .ToList();
                    listOfIncominAccounts.AddRange(list2);
                }
            }
            listOfIncominAccounts.OrderBy(x => x.DateOfTheTransfer);

            return listOfIncominAccounts;
        }

        public List<Transfer> GetTransfersOutgoingFromTheAccount(Account account)
        {
            using (var context = new BankDbContex())
            {
                return context.Transfers
                    .Where(transfer => transfer.SourceAccount == account.Number)
                    .ToList();
            }
        }

        public List<Transfer> GetIncomingTransfersToTheAccount(Account account)
        {
            using (var context = new BankDbContex())
            {
                return context.Transfers
                    .Where(transfer => transfer.TargetAccount == account.Number)
                    .ToList();
            }
        }

        public double ReductionOfSourceAccountBalance(int accoundId, double amount)
        {
            using (var context = new BankDbContex())
            {
                var sourceAccount = context.Accounts
                    .FirstOrDefault(account => account.Id == accoundId);

                sourceAccount.Balance -= amount;

                context.SaveChanges();
                return sourceAccount.Balance;
            }
        }

        public Dictionary<int, double> BalanceChangeOfAccounts(int sourceAccountId, int targetAccountId, double amount)
        {
            using (var context = new BankDbContex())
            {
                var sourceAccount = context.Accounts
                    .Where(account => account.Id == sourceAccountId)
                    .FirstOrDefault();

                var targetAccount = context.Accounts
                    .Where(account => account.Id == targetAccountId)
                    .FirstOrDefault();

                sourceAccount.Balance -= amount;
                targetAccount.Balance += amount;

                context.SaveChanges();

                Dictionary<int, double> newAccountsBalance = new Dictionary<int, double>();
                newAccountsBalance[sourceAccount.Id] = sourceAccount.Balance;
                newAccountsBalance[targetAccount.Id] = targetAccount.Balance;
                
                return newAccountsBalance;
            }
        }
    }
}
