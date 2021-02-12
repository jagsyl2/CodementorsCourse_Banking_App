using BankTransfers.BusinessLayer.Serializers;
using BankTransfers.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BankTransfers.BusinessLayer
{
    public interface IStatementsOfOperationsService
    {
        public bool SerializeStatementOfOperations(string targetDirectoryPath, List<StatementOfOperations> list);
        public IEnumerable<SumItem> CountSumOfTransfersSentToSpecificAccounts(List<Transfer> listOfOutgoingTransfers, Guid number);
        public IEnumerable<SumItem> CountSumOfTransfersIncomingFromSpecificAccounts(List<Transfer> listOfIncomingTransfers, Guid number);
        public double CountSumOfTransfers(List<Transfer> list);
        public double GetMaxValoueOfTransfers(List<Transfer> list);
        public double GetMinValoueOfTransfers(List<Transfer> list);
        public double GetAverageValoueOfTransfers(List<Transfer> list);
    }

    public class StatementsOfOperationsService : IStatementsOfOperationsService
    {
        private JsonSerializer _serializer = new JsonSerializer();

        public bool SerializeStatementOfOperations(string targetDirectoryPath, List<StatementOfOperations> list)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return false;
            }
            
            var filePath = Path.Combine(targetDirectoryPath, "StatementOfOperations.json");

            _serializer.Serialize(filePath, list);
            return true;
        }

        public IEnumerable<SumItem> CountSumOfTransfersSentToSpecificAccounts(List<Transfer> listOfOutgoingTransfers, Guid number)
        {
            var sumOfTransfers = listOfOutgoingTransfers
                .GroupBy(x => new { x.TargetAccount })
                .Select(g => new SumItem
                {
                    SourceAccount = number,
                    TargetAccount = g.Key.TargetAccount,
                    SumOfTransfers = g.Sum(x => x.Amount)
                });

            return sumOfTransfers;
        }

        public IEnumerable<SumItem> CountSumOfTransfersIncomingFromSpecificAccounts(List<Transfer> listOfIncomingTransfers, Guid number)
        {
            var sumOfTransfers = listOfIncomingTransfers
                .GroupBy(y => new { y.SourceAccount })
                .Select(g =>
                    new SumItem
                    {
                        SourceAccount = g.Key.SourceAccount,
                        TargetAccount = number,
                        SumOfTransfers = g.Sum(x => x.Amount)
                    });

            return sumOfTransfers;
        }

        public double CountSumOfTransfers(List<Transfer> list)
        {
            var sumOfTransfers = list
                .DefaultIfEmpty()
                .Sum(x => x == null ? 0 : x.Amount);

            return sumOfTransfers;
        }

        public double GetMaxValoueOfTransfers(List<Transfer> list)
        {
            var maxValoue = list
                .DefaultIfEmpty()
                .Max(x => x == null ? 0 : x.Amount);

            return maxValoue;
        }

        public double GetMinValoueOfTransfers(List<Transfer> list)
        {
            var minValoue = list
                .DefaultIfEmpty()
                .Min(x => x == null ? 0 : x.Amount);

            return minValoue;
        }

        public double GetAverageValoueOfTransfers(List<Transfer> list)
        {
            var averageValoue = Math.Round(list
                .DefaultIfEmpty()
                .Average(x => x == null ? 0 : x.Amount), 2);

            return averageValoue;
        }
    }
}