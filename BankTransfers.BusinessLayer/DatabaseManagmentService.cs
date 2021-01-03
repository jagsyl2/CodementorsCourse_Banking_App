using BankTransfers.DataLayer;

namespace BankTransfers.BusinessLayer
{
    public interface IDatabaseManagmentService
    {
        public void EnsureDatabaseCreation();
    }

    public class DatabaseManagmentService : IDatabaseManagmentService
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