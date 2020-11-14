using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bank
{
    class Finances
    {
        private List<Account> _bankAccounts = new List<Account>();

        public void AddAccounts(Account account)
        {
            _bankAccounts.Add(account);
        }

        public List<Account> GetAllAccounts() 
        {
            return _bankAccounts.ToList();
        }
    }
}
