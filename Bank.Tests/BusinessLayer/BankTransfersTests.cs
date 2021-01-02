using BankTransfers.BusinessLayer;
using BankTransfers.DataLayer.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Bank.Tests.BusinessLayer
{
    public class BankTransfersTests
    {
        [Test]
        public void CountSumOfTransfers_ThreeDifferentTransfers_ReturnsCorrectSum105()
        {
            var listOfTransfers = new List<Transfer>();
            listOfTransfers.Add(new Transfer(1, 50));
            listOfTransfers.Add(new Transfer(2, 45));
            listOfTransfers.Add(new Transfer(3, 10));

            var service = new StatementsOfOperationsService();

            double sum = service.CountSumOfTransfers(listOfTransfers);
            sum.Should().Be(105);
        }

        [Test]
        public void GeneratingAListOfOperations_EmptyList_ReturnsTrue()
        {
            var accountServiceMock = new Mock<IAccountService>();
            accountServiceMock
                .Setup(repo => repo.GetCustomerAccounts(1))
                .Returns(new List<Account>(0));

            var ioHelper = new Mock<IIoHelper>();
            var service = new IoTransferHelper(ioHelper.Object, accountServiceMock.Object);

            var customerAccounts = service.ChceckIfListIsEmpty();

            customerAccounts.Should().BeTrue();
        }


        var customerAccounts = _accountsService.GetCustomerAccounts(_customer.Id);
            if (_ioTransferHelper.ChceckIfListIsEmpty(customerAccounts) == true)
            {
                return;
            }
    }
}