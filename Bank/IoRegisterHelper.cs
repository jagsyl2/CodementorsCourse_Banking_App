using BankTransfers.BusinessLayer;

namespace Bank
{
    class IoRegisterHelper
    {
        private IoHelper _ioHelper = new IoHelper();
        private CustomersService _customersService = new CustomersService();

        public int GetPhoneNumberFromUser(string message)
        {
            int phoneNumber;
            bool validation;

            do
            {
                phoneNumber = _ioHelper.GetIntFromUser(message);
                if (phoneNumber.ToString().Length != 9)
                {
                    _ioHelper.WriteString("Incorrect phone number (must contain 9 digits). Try again...");
                    validation = false;
                }
                else
                {
                    validation = true;
                }
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
                eMail = _ioHelper.GetTextFromUser(message);
                validation = true;

                if (!(eMail).Contains("@"))
                {
                    _ioHelper.WriteString("Incorrect adress e-mail (must contain the @ sign). Try again...");

                    validation = false;
                    continue;
                }

                if (_customersService.CheckingIfANewCustomerIsRegistering(eMail) == true)
                {
                    _ioHelper.WriteString("The given email address exists. Try again...");
                    validation = false;
                }
            }
            while (validation == false);

            return eMail;
        }
    }
}
