using BankTransfers.DataLayer;
using BankTransfers.DataLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace BankTransfers.BusinessLayer
{
    public class TransfersService
    {
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

        public double ReductionOfSourceAccountBalance(int accoundId, double amount)
        {
            using (var context = new BankDbContex())
            {
                var sourceAccount = context.Accounts
                    .Where(account => account.Id == accoundId)
                    .FirstOrDefault();

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
