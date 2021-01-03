using BankTransfers.BusinessLayer;
using System;
using System.Collections.Generic;

namespace Bank
{
    public interface IIoSerializationDesireHelper
    {
        public void ImplementationOfSerializationDecision(SerializationDesire desire, List<StatementOfOperations> list);
        public void DoSerialization(List<StatementOfOperations> list);
    }

    public enum SerializationDesire
    {
        Yes,
        No
    }

    public class IoSerializationDesireHelper : IIoSerializationDesireHelper
    {
        private IoHelper _ioHelper = new IoHelper();
        private StatementsOfOperationsService _statementsOfOperationsService = new StatementsOfOperationsService();

        public void ImplementationOfSerializationDecision(SerializationDesire desire, List<StatementOfOperations> list)
        {
            switch (desire)
            {
                case SerializationDesire.Yes:
                    DoSerialization(list);
                    break;
                case SerializationDesire.No:
                    break;
                default:
                    Console.WriteLine("Unknow serializtion desire. Try again...");
                    break;
            }
        }

        public void DoSerialization(List<StatementOfOperations> list)
        {
            var targetPath = _ioHelper.GetTextFromUser("Enter target path");
            
            if (_statementsOfOperationsService.SerializeStatementOfOperations(targetPath, list))
            {
                _ioHelper.WriteString("The file with operation exported successfull");
            }
            else
            {
                _ioHelper.WriteString("Error during the file operations export");

            }
        }
    }
}
