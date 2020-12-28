using BankTransfers.BusinessLayer.Serializers;
using BankTransfers.DataLayer;
using BankTransfers.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BankTransfers.BusinessLayer
{
    public class ServiceOfStatementsOfOperations
    {
        private JsonSerializer _serializer = new JsonSerializer(); 

        public bool SerializeStatementOfOperations(string targetDirectoryPath, List<StatementOfOperations> list)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return false;
            }
            var filePath = Path.Combine(targetDirectoryPath, "StatementOfOperations.json");

            //List<Transfer> statementOfOperations;

            //using (var context = new BankDbContex())
            //{
            //    statementOfOperations = context.Transfers
            //        .Include(x => x.Customer)
            //        .ToList();
            //}

            _serializer.Serialize(filePath, list);
            return true;
        }


    }
}
