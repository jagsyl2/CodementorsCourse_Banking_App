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

        [TestCase(10, 20, 30, 40, 40)]
        [TestCase(600, 720, 30, 80, 720)]
        [TestCase(5, 1000, 100, 999, 1000)]
        [TestCase(0, 10, 60, 50, 60)]
        public void GetMaxValoueOfTransfers_FourDifferentAmounts_ReturnsHigestValoue(double amount1, double amount2, double amount3, double amount4, double max)
        {
            var listOfTransfers = new List<Transfer>();
            listOfTransfers.Add(new Transfer(1, amount1));
            listOfTransfers.Add(new Transfer(2, amount2));
            listOfTransfers.Add(new Transfer(3, amount3));
            listOfTransfers.Add(new Transfer(4, amount4));

            var service = new StatementsOfOperationsService();

            double maxValoue = service.GetMaxValoueOfTransfers(listOfTransfers);

            maxValoue.Should().Be(max);
        }

        [Test]
        public void GeneratingAListOfOperations_ListWith3Accounts_ReturnsStatementOfOperations3Times()
        {
            var listOfAccounts = new List<Account>();
            listOfAccounts.Add(new Account(1, "pierwsze"));
            listOfAccounts.Add(new Account(2, "drugie"));
            listOfAccounts.Add(new Account(3, "trzecie"));

            var customer = new Customer
            {
                Id = 1,
            };

            var accountsServiceMock = new Mock<IAccountsService>();
            accountsServiceMock
                .Setup(repo => repo.GetCustomerAccounts(customer.Id))
                .Returns(listOfAccounts);

            var ioTransferHelperMock = new Mock<IIoTransferHelper>();
            ioTransferHelperMock
                .Setup(x => x.ChceckIfListIsEmpty(listOfAccounts))
                .Returns(false);

            StatementOfOperations statementOfOperationt = new StatementOfOperations();

            var ioStatementOfOperationsHelperMock = new Mock<IIoStatementOfOperationsHelper>();
            ioStatementOfOperationsHelperMock
                .Setup(x => x.GetStatementOfOperationForAccount(It.IsAny<Account>()))
                .Returns(statementOfOperationt);

            ioStatementOfOperationsHelperMock
                .Setup(x => x.PrintStatementOfOperations(It.IsAny<Account>(), statementOfOperationt));

            var registerMenu = new Mock<IMenu>();
            var customerMenu = new Mock<IMenu>();
            var ioHelper = new Mock<IIoHelper>();
            var ioRegisterHelper = new Mock<IIoRegisterHelper>();
            var customersService = new Mock<ICustomersService>();
            var transfersService = new Mock<ITransfersService>();
            var databaseManagmentService = new Mock<IDatabaseManagmentService>();
            var serializationDesireHelper = new Mock<IIoSerializationDesireHelper>();

            var service = new Program(
                registerMenu.Object,
                customerMenu.Object,
                ioHelper.Object,
                ioRegisterHelper.Object,
                ioTransferHelperMock.Object,
                accountsServiceMock.Object,
                customersService.Object,
                transfersService.Object,
                databaseManagmentService.Object,
                serializationDesireHelper.Object,
                ioStatementOfOperationsHelperMock.Object);

            service.Customer = customer;
            service.GeneratingAListOfOperations();

            ioStatementOfOperationsHelperMock.Verify(getStatement => getStatement.GetStatementOfOperationForAccount(It.IsAny<Account>()), Times.Exactly(3));
        }

        [Test]
        public void GeneratingAListOfOperations_EmptyList_DoesNoReturnsSerialization()
        {
            var listOfAccounts = new List<Account>();

            var customer = new Customer
            {
                Id = 1,
            };

            var accountsServiceMock = new Mock<IAccountsService>();
            accountsServiceMock
                .Setup(repo => repo.GetCustomerAccounts(customer.Id))
                .Returns(listOfAccounts);

            var ioTransferHelperMock = new Mock<IIoTransferHelper>();
            ioTransferHelperMock
                .Setup(x => x.ChceckIfListIsEmpty(listOfAccounts))
                .Returns(true);

            var ioStatementOfOperationsHelperMock = new Mock<IIoStatementOfOperationsHelper>();
            var registerMenu = new Mock<IMenu>();
            var customerMenu = new Mock<IMenu>();
            var ioHelper = new Mock<IIoHelper>();
            var ioRegisterHelper = new Mock<IIoRegisterHelper>();
            var customersService = new Mock<ICustomersService>();
            var transfersService = new Mock<ITransfersService>();
            var databaseManagmentService = new Mock<IDatabaseManagmentService>();
            var serializationDesireHelper = new Mock<IIoSerializationDesireHelper>();

            var service = new Program(
                registerMenu.Object,
                customerMenu.Object,
                ioHelper.Object,
                ioRegisterHelper.Object,
                ioTransferHelperMock.Object,
                accountsServiceMock.Object,
                customersService.Object,
                transfersService.Object,
                databaseManagmentService.Object,
                serializationDesireHelper.Object,
                ioStatementOfOperationsHelperMock.Object);

            service.Customer = customer;
            service.GeneratingAListOfOperations();

            serializationDesireHelper.Verify(serialization => serialization.ImplementationOfSerializationDecision(It.IsAny<SerializationDesire>(), It.IsAny<List<StatementOfOperations>>()), Times.Never);
        }

        [Test]
        public void GetStatementOfOperationForAccount_DefinesStatementOfOperations_StatementOfOperationsShouldBeEquivalentToStatementFromMethod()
        {
            var account = new Account();

            IEnumerable<SumItem> sumItems = null;

            var statementOfOperations = new StatementOfOperations()
            {
                ForAccount = account.Number,
                TotalValueOutgoingTransfersToSpecificAccounts = null,
                TotalValueIncomingTransfersFromSpecificAccounts = null,
                TotalValueOutgoingTransfers = 120,
                TotalValueIncomingTransfers = 120,
                HigestValueOutgoingTransfers = 100,
                LowestValueOutgoingTransfers = 20,
                AverageValueOutgoingTransfers = 60,
            };

            var transferServiceMock = new Mock<ITransfersService>();

            var statementOfOpertionsServiceMock = new Mock<IStatementsOfOperationsService>();
            statementOfOpertionsServiceMock
                .Setup(x => x.CountSumOfTransfersSentToSpecificAccounts(It.IsAny<List<Transfer>>(), account.Number))
                .Returns(sumItems);
            statementOfOpertionsServiceMock
                .Setup(x => x.CountSumOfTransfersIncomingFromSpecificAccounts(It.IsAny<List<Transfer>>(), account.Number))
                .Returns(sumItems);
            statementOfOpertionsServiceMock
                .Setup(x => x.CountSumOfTransfers(It.IsAny<List<Transfer>>()))
                .Returns(120);
            statementOfOpertionsServiceMock
                .Setup(x => x.GetMaxValoueOfTransfers(It.IsAny<List<Transfer>>()))
                .Returns(100);
            statementOfOpertionsServiceMock
                .Setup(x => x.GetMinValoueOfTransfers(It.IsAny<List<Transfer>>()))
                .Returns(20);
            statementOfOpertionsServiceMock
                .Setup(x => x.GetAverageValoueOfTransfers(It.IsAny<List<Transfer>>()))
                .Returns(60);

            var service = new IoStatementOfOperationsHelper(transferServiceMock.Object, statementOfOpertionsServiceMock.Object);
            var statement =  service.GetStatementOfOperationForAccount(account);

            statement.Should().BeEquivalentTo(statementOfOperations);
        }
    }
}