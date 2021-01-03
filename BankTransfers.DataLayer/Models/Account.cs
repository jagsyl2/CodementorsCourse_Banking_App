using System;

namespace BankTransfers.DataLayer.Models
{
    public class Account
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer customer { get; set; }
        public string Name { get; set; }
        public Guid Number { get; set; }
        public double Balance { get; set; }

        public Account()
        {
        }

        public Account(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}

