using System;
using System.Collections.Generic;
using System.Text;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using NUnit.Framework;

namespace MoneyBox.App.Tests
{
    class TransferMoneyTests
    {
        [Test]
        public void TransfersMoneyWhenFromAccountHaveSufficientFundsAndToAccountDidntReachPayInLimit()
        {
            // Arrange
            var fromAccount = new Account(Guid.NewGuid(), null, 1000m, 0m, 0m);
            var toAccount = new Account(Guid.NewGuid(), null, 100m, 0m, 0m);

            var accountRepositoryMock = new Mock<IAccountRepository>();
            accountRepositoryMock
                .Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);
            accountRepositoryMock
                .Setup(x => x.GetAccountById(toAccount.Id))
                .Returns(toAccount);

            var notificationServiceMock = new Mock<INotificationService>();

            var transferMoneyFeature = new TransferMoney(accountRepositoryMock.Object, notificationServiceMock.Object);

            // Act
            transferMoneyFeature.Execute(fromAccount.Id, toAccount.Id, 250m);

            // Assert
            Assert.AreEqual(750m, fromAccount.Balance);
            Assert.AreEqual(350m, toAccount.Balance);
        }

        [Test]
        public void ThrowsExceptionWhenFromAccountHasInsufficientFunds()
        {
            // Arrange
            var fromAccount = new Account(Guid.NewGuid(), null, 100m, 0m, 0m);
            var toAccount = new Account(Guid.NewGuid(), null, 100m, 0m, 0m);

            var accountRepositoryMock = new Mock<IAccountRepository>();
            accountRepositoryMock
                .Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);
            accountRepositoryMock
                .Setup(x => x.GetAccountById(toAccount.Id))
                .Returns(toAccount);

            var notificationServiceMock = new Mock<INotificationService>();

            var transferMoneyFeature = new TransferMoney(accountRepositoryMock.Object, notificationServiceMock.Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                transferMoneyFeature.Execute(fromAccount.Id, toAccount.Id, 250m));
        }

        [Test]
        public void ThrowsExceptionWhenToAccountHasReachedPayInLimit()
        {
            // Arrange
            var fromAccount = new Account(Guid.NewGuid(), null, 1000m, 0m, 0m);
            var toAccount = new Account(Guid.NewGuid(), null, 100m, 0m, 4000m);

            var accountRepositoryMock = new Mock<IAccountRepository>();
            accountRepositoryMock
                .Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);
            accountRepositoryMock
                .Setup(x => x.GetAccountById(toAccount.Id))
                .Returns(toAccount);

            var notificationServiceMock = new Mock<INotificationService>();

            var transferMoneyFeature = new TransferMoney(accountRepositoryMock.Object, notificationServiceMock.Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                transferMoneyFeature.Execute(fromAccount.Id, toAccount.Id, 250m));
        }

        [Test]
        public void ShouldSendNotificationAboutApproachingWhenLessThan500LeftToPayInLimit()
        {
            // Arrange
            var user = new User(Guid.NewGuid(), "Name", "email");
            var fromAccount = new Account(Guid.NewGuid(), null, 1000m, 0m, 0m);
            var toAccount = new Account(Guid.NewGuid(), user, 100m, 0m, 3450m);

            var accountRepositoryMock = new Mock<IAccountRepository>();
            accountRepositoryMock
                .Setup(x => x.GetAccountById(fromAccount.Id))
                .Returns(fromAccount);
            accountRepositoryMock
                .Setup(x => x.GetAccountById(toAccount.Id))
                .Returns(toAccount);

            var notificationServiceMock = new Mock<INotificationService>();

            var transferMoneyFeature = new TransferMoney(accountRepositoryMock.Object, notificationServiceMock.Object);

            // Act
            transferMoneyFeature.Execute(fromAccount.Id, toAccount.Id, 250m);

            // Assert
            notificationServiceMock.Verify(x => x.NotifyApproachingPayInLimit(user.Email));
        }
    }
}
