using System;

namespace Bank
{
    public interface IIoHelper
    {
        public Guid GetGuidFromUser(string message);
        public double GetDoubleFromUser(string message);
        public bool CheckingIfIsNullOrWhiteSpace(string accountName);

        public SerializationDesire GetSerializationDesireFromUser(string message);
        public Guid GenerateGuidToUser();
        public int GetIntFromUser(string message);

        public string GetTextFromUser(string message);

        public void WriteString(string message);

    }

    public class IoHelper : IIoHelper
    {
        public Guid GetGuidFromUser(string message)
        {
            Guid guid;
            while (!Guid.TryParse(GetTextFromUser(message), out guid))
            {
                WriteString("Incorrect number - try again...");
            }
            return guid;
        }

        public double GetDoubleFromUser(string message)
        {
            double amount;
            while (!double.TryParse(GetTextFromUser(message), out amount))
            {
                WriteString("Negative value - try again...");
            }
            return amount;
        }

        public bool CheckingIfIsNullOrWhiteSpace(string accountName)
        {
            if (string.IsNullOrWhiteSpace(accountName))
            {
                WriteString("Incorrect name - try again...");
                return true;
            }
            return false;
        }

        public SerializationDesire GetSerializationDesireFromUser(string message)
        {
            var correctValues = "";

            foreach (var item in (SerializationDesire[])Enum.GetValues(typeof(SerializationDesire)))
            {
                correctValues += $"{item},";
            }

            object result;
            while (!Enum.TryParse(typeof(SerializationDesire), GetTextFromUser($"{message} ({correctValues})"), out result))
            {
                Console.WriteLine("Not correct value - use Yes or No. Try again...");
            }

            return (SerializationDesire)result;
        }

        public Guid GenerateGuidToUser()
        {
            Guid g = Guid.NewGuid();
            Console.WriteLine($"Your account number: {g}");
            return g;
        }

        public int GetIntFromUser(string message)
        {
            int number;
            while (!int.TryParse(GetTextFromUser(message), out number))
            {
                WriteString("Incorrect option - try again...");
            }
            return number;
        }

        public string GetTextFromUser(string message)
        {
            Console.Write($"{message}: ");
            return Console.ReadLine();
        }

        public void WriteString (string message)
        {
            Console.WriteLine(message);
            Console.WriteLine();
        }
    }
}
