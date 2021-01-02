using BankTransfers.DataLayer;
using BankTransfers.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BankTransfers.BusinessLayer
{
    public interface IAccountService
    {
        public void AddAccount(Account account);
        public List<Account> GetAllAccounts();
        public List<Account> GetCustomerAccounts(int customerId);
        public double GetCurrentBalanceOfAccount(int accountId);

    }

    public class AccountsService : IAccountService
    {
        public void AddAccount(Account account)
        {
            using (var context = new BankDbContex())
            {
                context.Accounts.Add(account);
                context.SaveChanges();
            }
        }

        public List<Account> GetAllAccounts()
        {
            using (var context = new BankDbContex())
            {
                return context.Accounts
                    .Include(x=> x.customer)
                    .ToList();
            }
        }

        public List<Account> GetCustomerAccounts(int customerId)
        {
            using (var context = new BankDbContex())
            {
                return context.Accounts
                    .Where(account => customerId == account.CustomerId)
                    .ToList();
            }
        }

        public double GetCurrentBalanceOfAccount(int accountId)
        {
            using (var context = new BankDbContex())
            {
                return context.Accounts
                    .Where(x => x.Id == accountId)
                    .Select(x => x.Balance)
                    .FirstOrDefault();
            }
        }
    }
}
