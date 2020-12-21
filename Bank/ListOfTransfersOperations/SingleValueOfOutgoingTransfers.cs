using BankTransfers.BusinessLayer;
using BankTransfers.DataLayer.Models;
using System;
using System.Collections.Generic;

namespace Bank.ListOfTransfersOperations
{
    public abstract class SingleValueOfOutgoingTransfers
    {
        private TransfersService _transfersService = new TransfersService();

        public void ValueOfOutgoingTransfers(int customerId)
        {
            var listOfCustomerTransfers = GetAndCheckIfUserHasOutgoingTransfers(customerId);
            if (listOfCustomerTransfers == null)
            {
                return;
            }

            SelectedAndPrintValueOfTransfersSent(listOfCustomerTransfers);
        }

        public List<Transfer> GetAndCheckIfUserHasOutgoingTransfers(int customerId)
        {
            var listOfCustomerTransfers = _transfersService.GetTransfers(customerId);
            if (listOfCustomerTransfers == null)
            {
                Console.WriteLine("You don't have outgoing transfers.");
            }

            return listOfCustomerTransfers;
        }

        public abstract void SelectedAndPrintValueOfTransfersSent(List<Transfer> listOfCustomerTransfers);
    }
}
