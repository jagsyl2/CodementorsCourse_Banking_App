using BankTransfers.DataLayer;
using BankTransfers.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
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



        //public List<Transfer> GetIncomingTransfers(Customer customer)
        //{
        //    using (var context = new BankDbContex())
        //    {
        //        var customerAccounts = context.Accounts
        //            .Where(x => x.CustomerId == customer.Id)
        //            ;

        //        return context.Transfers
        //            .Where(transfer => transfer.CustomerId != customer.Id)

        //            .Select(transfer => transfer.TargetAccount == customerAccounts)
        //            .ToList();
        //    }
        //}



        //!listOfAllAccounts
        //        .Where(x => x.CustomerId != CustomerId)
        //        .Any(x => x.Number == accountNumber))




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
