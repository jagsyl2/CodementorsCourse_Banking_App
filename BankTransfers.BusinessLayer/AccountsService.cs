using BankTransfers.DataLayer;
using BankTransfers.DataLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace BankTransfers.BusinessLayer
{
    public class AccountsService
    {
        private ClientAccounts _clientAccounts = new ClientAccounts();

        public void AddAccount(Account account)
        {
            _clientAccounts.BankAccounts.Add(account);
        }
        public List<Account> GetAllAccounts()
        {
            return _clientAccounts.BankAccounts.ToList();

        }
    }
}
