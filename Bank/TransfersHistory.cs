using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bank
{
    class TransfersHistory
    {
        private List<Transfer> _transfers = new List<Transfer>();

        public void AddTransfer(Transfer transfer)
        {
            _transfers.Add(transfer);
        }

        public List<Transfer> GetAllTransfers()
        {
            return _transfers.ToList();
        }
    }
}
