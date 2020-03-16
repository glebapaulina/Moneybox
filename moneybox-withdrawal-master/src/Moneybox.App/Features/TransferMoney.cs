using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;
using Moneybox.App.Domain;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var from = _accountRepository.GetAccountById(fromAccountId);
            var to = _accountRepository.GetAccountById(toAccountId);

            AccountResponse withdrawResponse = from.Withdraw(amount);
            AccountResponse payInResponse = to.PayIn(amount);

            if (withdrawResponse.AreFundsLow)
            {
                _notificationService.NotifyFundsLow(from.User.Email);
            }

            if (payInResponse.IsApproachingPayInLimit)
            {
                _notificationService.NotifyApproachingPayInLimit(to.User.Email);
            }

            _accountRepository.Update(from);
            _accountRepository.Update(to);
        }
    }
}
