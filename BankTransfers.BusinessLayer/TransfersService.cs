using BankingProject.OutgoingTransfers.Sender;
using BankTransfers.DataLayer;
using BankTransfers.DataLayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankTransfers.BusinessLayer
{
    public class TransfersService
    {
        private AccountsService _accountsService = new AccountsService();
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

        public List<Transfer> GetTransfers(int customerId)
        {
            using (var context = new BankDbContex())
            {
                return context.Transfers
                    .Where(transfer => transfer.CustomerId == customerId)
                    .ToList();
            }
        }

        public List<Transfer> GetAllTransfersForCustomer(int customerId)
        {
            List<Transfer> listOfAllTransfers = new List<Transfer>();

            using (var context = new BankDbContex())
            {
                listOfAllTransfers.AddRange(GetTransfers(customerId));

                var listOfAccounts = _accountsService.GetCustomerAccounts(customerId);
                foreach (var account in listOfAccounts)
                {
                    var listOfIncomingTransfers = context.Transfers
                        .Where(transfer => transfer.CustomerId != customerId && transfer.TargetAccount == account.Number)
                        .ToList();
                    
                    listOfAllTransfers.AddRange(listOfIncomingTransfers);
                }
            }

            listOfAllTransfers.OrderBy(x => x.DateOfTheTransfer);

            return listOfAllTransfers;
        }

        public List<Transfer> GetTransfersOutgoingFromTheAccount(Account account)
        {
            using (var context = new BankDbContex())
            {
                return context.Transfers
                    .Where(transfer => transfer.SourceAccount == account.Number)
                    .ToList();
            }
        }

        public List<Transfer> GetIncomingTransfersToTheAccount(Account account)
        {
            using (var context = new BankDbContex())
            {
                return context.Transfers
                    .Where(transfer => transfer.TargetAccount == account.Number)
                    .ToList();
            }
        }

        public void ReductionOfSourceAccountBalance(Guid accountNumber, double amount)
        {
            using (var context = new BankDbContex())
            {
                var sourceAccount = context.Accounts
                    .FirstOrDefault(account => account.Number == accountNumber);

                sourceAccount.Balance -= amount;

                context.SaveChanges();
            }
        }

        public void BalanceChangeOfAccounts(Guid sourceNumber, Guid targetNumber, double amount)
        {
            using (var context = new BankDbContex())
            {
                var sourceAccount = context.Accounts
                    .Where(account => account.Number == sourceNumber)
                    .FirstOrDefault();

                var targetAccount = context.Accounts
                    .Where(account => account.Number == targetNumber)
                    .FirstOrDefault();

                sourceAccount.Balance -= amount;
                targetAccount.Balance += amount;

                context.SaveChanges();
            }
        }

        public void SendTransferOut(Transfer transfer)
        {
            var sender = new GlobalOutgoingTransfersSender();
            var jsonData = JsonConvert.SerializeObject(transfer);
            sender.Send(jsonData);
        }
    }
}
