using BankTransfers.DataLayer;

namespace BankTransfers.BusinessLayer
{
    public class DatabaseManagmentService
    {
        public void EnsureDatabaseCreation()
        {
            using (var context = new BankDbContex())
            {
                context.Database.EnsureCreated();
            }
        }
    }
}
