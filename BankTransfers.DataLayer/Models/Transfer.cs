using System;

namespace BankTransfers.DataLayer.Models
{
    public class Transfer
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string Title { get; set; }
        public double Amount { get; set; }
        public DateTime DateOfTheTransfer { get; set; }
        public string TypOfTransfer { get; set; }
        public Guid SourceAccount { get; set; }
        public Guid TargetAccount { get; set; }
    }
}
