using BankTransfers.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace BankTransfers.DataLayer
{
    public class BankDbContex : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transfer> Transfers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.;Database=BankTransfers_Dev;Trusted_Connection=True;");
        }
    }
}
