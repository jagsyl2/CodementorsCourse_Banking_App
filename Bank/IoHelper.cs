﻿using System;

namespace Bank
{
    public class IoHelper
    {
        public Guid GetGuidFromUser(string message)
        {
            Guid guid;
            while (!Guid.TryParse(GetTextFromUser(message), out guid))
            {
                Console.WriteLine("Incorrect number - try again...");
                Console.WriteLine();
            }
            return guid;
        }
        public double GetDoubleFromUser(string message)
        {
            double amount;
            while (!double.TryParse(GetTextFromUser(message), out amount))
            {
                Console.WriteLine("Negative value - try again...");
                Console.WriteLine();
            }
            return amount;
        }
        public bool CheckingIfIsNullOrWhiteSpace(string accountName, bool createAccount, string message)
        {
            if (string.IsNullOrWhiteSpace(accountName))
            {
                Console.WriteLine(message);
                Console.WriteLine();
                createAccount = false;
            }
            return createAccount;
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
                Console.WriteLine("Incorrect option - try again...");
                Console.WriteLine();
            }
            return number;
        }

        public string GetTextFromUser(string message)
        {
            Console.Write($"{message}: ");
            return Console.ReadLine();
        }

        public int GetPhoneNumberFromUser(string message)
        {
            int phoneNumber;
            bool validation;

            do
            {
                phoneNumber = GetIntFromUser(message);
                validation = phoneNumber.ToString().Length == 9;
                Console.WriteLine(validation ? "" : "Incorrect phone number. Try again...");
            }
            while (validation == false);

            return phoneNumber;
        }

        public string GetEMailFromUser(string message)
        {
            string eMail;
            bool validation;

            do
            {
                eMail = GetTextFromUser(message);
                validation = (eMail).Contains("@");
                Console.WriteLine(validation ? "" : "Incorrect adress e-mail. Try again..."  ); 
            }
            while (validation == false);

            return eMail;
        }
    }
}