using BankTransfers.DataLayer;
using BankTransfers.DataLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace BankTransfers.BusinessLayer
{
    public class CustomersService
    {
        public void AddCustomer(Customer customer)
        {
            using(var context = new BankDbContex())
            {
                context.Customers.Add(customer);
                context.SaveChanges();
            }
        }

        public List<Customer> GetAllCustomers()
        {
            using (var context = new BankDbContex())
            {
                return context.Customers.ToList();
            }
        }

        public bool CheckingIfANewCustomerIsRegistering(string eMail)
        {
            using (var context = new BankDbContex())
            {
                if (context.Customers.Any(customer => customer.EMail == eMail))
                { 
                    return true;
                }
                return false;
            }
        }

        public bool CheckingCustomerSignIn(string eMail, string password)
        {
            using (var context = new BankDbContex())
            {
                if (context.Customers.Any(customer => (customer.EMail == eMail && customer.Password == password)))
                {
                    return true;
                }
                return false;
            }
        }
    }
}
