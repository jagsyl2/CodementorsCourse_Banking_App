using BankTransfers.BusinessLayer.Serializers;
using System.Collections.Generic;
using System.IO;

namespace BankTransfers.BusinessLayer
{
    public class StatementsOfOperationsService
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
    }
}
