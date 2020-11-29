using BankTransfers.DataLayer;
using BankTransfers.DataLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace BankTransfers.BusinessLayer
{
    public class TransfersService
    {
        private TransfersHistory _transfersHistory = new TransfersHistory();
        public void AddTransfer(Transfer transfer)
        {
            _transfersHistory.Transfers.Add(transfer);
        }

        public List<Transfer> GetAllTransfers()
        {
            return _transfersHistory.Transfers.ToList();
        }
    }
}
