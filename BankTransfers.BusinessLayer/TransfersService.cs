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

        //public void MakeADomesticTransfer(int sourceAccountId, int targetAccountId, double amount)
        //{
        //    using (var context = new BankDbContex())
        //    {
        //        var sourceAccount = context.Accounts
        //            .Where(account => account.Id == sourceAccountId)
        //            .FirstOrDefault();

        //        sourceAccount.Balance -= amount;

        //        var targetAccount = context.Accounts
        //            .Where(account => account.Id == targetAccountId)
        //            .FirstOrDefault();

        //        targetAccount.Balance += amount;

        //        context.SaveChanges();
        //    }
        //}

        public double BalanceChangeOfSourceAccount(int sourceAccountId, double amount)
        {
            using (var context = new BankDbContex())
            {
                var sourceAccount = context.Accounts
                    .Where(account => account.Id == sourceAccountId)
                    .FirstOrDefault();

                sourceAccount.Balance -= amount;

                context.SaveChanges();
                return sourceAccount.Balance;
            }
        }

        public double BalanceChangeOfTargetAccount(int targetAccountId, double amount)
        {
            using (var context = new BankDbContex())
            {
                var targetAccount = context.Accounts
                    .Where(account => account.Id == targetAccountId)
                    .FirstOrDefault();

                targetAccount.Balance += amount;

                context.SaveChanges();
                return targetAccount.Balance;
            }
        }
    }
}
