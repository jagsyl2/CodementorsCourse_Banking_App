using BankTransfers.DataLayer;
using BankTransfers.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BankTransfers.BusinessLayer
{
    public class AccountsService
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
    }
}
