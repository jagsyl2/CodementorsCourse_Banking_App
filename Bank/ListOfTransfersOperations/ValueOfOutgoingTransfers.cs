using BankTransfers.BusinessLayer;
using BankTransfers.DataLayer.Models;
using System;
using System.Collections.Generic;

namespace Bank.ListOfTransfersOperations
{
    public abstract class ValueOfOutgoingTransfers
    {
        private TransfersService _transfersService = new TransfersService();

        public void PrintTotalValueOfOutgoingTransfers(int customerId)
        {
            var listOfCustomerTransfers = GetAndCheckIfUserHasOutgoingTransfers(customerId);
            if (listOfCustomerTransfers == null)
            {
                return;
            }

            PrintListOfOperation(SumOfTransfersSent(listOfCustomerTransfers));
        }

        //public List<SelectItem> GetTotalValueOfOutgoingTransfers(int customerId)
        //{
        //    var listOfCustomerTransfers = GetAndCheckIfUserHasOutgoingTransfers(customerId);
        //    if (listOfCustomerTransfers == null)
        //    {
        //        return null;
        //    }

        //    return SumOfTransfersSent(listOfCustomerTransfers);
        //}

        public List<Transfer> GetAndCheckIfUserHasOutgoingTransfers(int customerId)
        {
            var listOfCustomerTransfers = _transfersService.GetTransfers(customerId);
            if (listOfCustomerTransfers == null)
            {
                Console.WriteLine("You don't have outgoing transfers.");
            }
            
            return listOfCustomerTransfers;
        }

        public abstract List<SelectItem> SumOfTransfersSent(List<Transfer> listOfCustomerTransfers);

        public abstract void PrintListOfOperation(List<SelectItem> sumOfTransfersSent);


    }
}
