using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace BankTransfers.BusinessLayer.Serializers
{
    class JsonSerializer
    {
        public void Serialize (string filePath, List<StatementOfOperations> dataSet)
        {
            var jsonData = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }
    }
}
