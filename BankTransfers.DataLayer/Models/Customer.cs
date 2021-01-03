using System.Collections.Generic;

namespace BankTransfers.DataLayer.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string EMail { get; set; }
        public int PhoneNumber { get; set; }
        public string Password { get; set; }
        public List<Account> Accounts { get; set; }
        public List<Transfer> Transfers { get; set; }
    }
}