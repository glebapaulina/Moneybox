using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;
using Moneybox.App.Domain;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private readonly IAccountRepository _accountRepository;
        private readonly INotificationService _notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            Account from = _accountRepository.GetAccountById(fromAccountId);

            AccountResponse response = from.Withdraw(amount);
            
            if (response.AreFundsLow)
            {
                _notificationService.NotifyFundsLow(from.User.Email);
            }

            _accountRepository.Update(from);
        }
    }
}
