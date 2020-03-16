using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("MoneyBox.App.Tests")]
namespace Moneybox.App.Domain
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;

        internal Account(Guid id, User user, decimal balance, decimal withdrawn, decimal paidIn)
        {
            Id = id;
            User = user;
            Balance = balance;
            Withdrawn = withdrawn;
            PaidIn = paidIn;
        }
       
        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        internal AccountResponse Withdraw(decimal amount)
        {
            var response = new AccountResponse();

            decimal balanceAfterWithdrawal = Balance - amount;

            if (balanceAfterWithdrawal < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to withdraw funds");
            }

            if (balanceAfterWithdrawal < 500m)
            {
                response.AreFundsLow = true;
            }

            Balance = Balance - amount;
            Withdrawn = Withdrawn - amount;

            return response;
        }

        internal AccountResponse PayIn(decimal amount)
        {
            var response = new AccountResponse();

            var paidIn = PaidIn + amount;

            if (paidIn > PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            if (PayInLimit - paidIn < 500m)
            {
                response.IsApproachingPayInLimit = true;
            }

            Balance = Balance + amount;
            PaidIn = PaidIn + amount;

            return response;
        }
    }
}
