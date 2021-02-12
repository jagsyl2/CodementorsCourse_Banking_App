using System;
using System.Collections.Generic;

namespace Bank
{
    public interface IMenu
    {
        public void AddOption(MenuItem item);
        public void ExecuteOption(int optionKey);
        public void PrintAvailableOptions();
    }

    class Menu : IMenu
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
                Console.WriteLine();
                return;
            }

            var item = _options[optionKey];
            item.Action(); 
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
