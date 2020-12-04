using BankTransfers.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank
{
    class Menu
    {
        private Dictionary<int, MenuItem> _options = new Dictionary<int, MenuItem>();

        public void AddOption(MenuItem item)
        {
            if (_options.ContainsKey(item.Keys))
            {
                Console.WriteLine("Option number exists. Try again...");
                return;
            }
            _options.Add(item.Keys, item);
        }

        public void ExecuteOption(int optionKey) 
        {
            if (!_options.ContainsKey(optionKey))
            {
                Console.WriteLine("Option number does not exists. Try again...");
                return;
            }
            var item = _options[optionKey];
            item.Action();                                                           /////skumać to!!!!!!!!!!!!!!!!!!!!!!! 
        }


        public void PrintAvailableOptions()
        {
            foreach( var option in _options)
            {
                Console.WriteLine($"{option.Key} {option.Value.Description}");
            }
        }
    }
}
