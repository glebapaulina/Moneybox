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
    class WithdrawMoneyTests
    {
        [Test]
        public void UpdatesBalanceWhenWithdrawnWithSufficientFunds()
        {
            // Arrange
            var account = new Account(Guid.NewGuid(), null, 1000m, 0m, 0m);

            var accountRepositoryMock = new Mock<IAccountRepository>();
            accountRepositoryMock
                .Setup(x => x.GetAccountById(account.Id))
                .Returns(account);

            var notificationServiceMock = new Mock<INotificationService>();

            var withdrawMoneyFeature = new WithdrawMoney(accountRepositoryMock.Object, notificationServiceMock.Object);

            // Act
            withdrawMoneyFeature.Execute(account.Id, 250m);

            // Assert
            Assert.AreEqual(750m, account.Balance);
        }

        [Test]
        public void ThrowsExceptionWhenWithdrawingWIthInsufficientFunds()
        {
            // Arrange
            var account = new Account(Guid.NewGuid(), null, 200m, 0m, 0m);

            var accountRepositoryMock = new Mock<IAccountRepository>();
            accountRepositoryMock
                .Setup(x => x.GetAccountById(account.Id))
                .Returns(account);

            var notificationServiceMock = new Mock<INotificationService>();

            var withdrawMoneyFeature = new WithdrawMoney(accountRepositoryMock.Object, notificationServiceMock.Object);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => withdrawMoneyFeature.Execute(account.Id, 250m));
        }

        [Test]
        public void ShouldSendNotificationAboutLowFundsWhenLessThan500Left()
        {
            // Arrange
            var user = new User(Guid.NewGuid(), "Name", "email");
            var account = new Account(Guid.NewGuid(), user, 500m, 0m, 0m);

            var accountRepositoryMock = new Mock<IAccountRepository>();
            accountRepositoryMock
                .Setup(x => x.GetAccountById(account.Id))
                .Returns(account);

            var notificationServiceMock = new Mock<INotificationService>();

            var withdrawMoneyFeature = new WithdrawMoney(accountRepositoryMock.Object, notificationServiceMock.Object);

            // Act
            withdrawMoneyFeature.Execute(account.Id, 250m);

            // Act & Assert
            notificationServiceMock.Verify(x => x.NotifyFundsLow(user.Email));
        }
    }
}
