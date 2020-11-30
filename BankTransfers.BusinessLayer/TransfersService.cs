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
    }
}
