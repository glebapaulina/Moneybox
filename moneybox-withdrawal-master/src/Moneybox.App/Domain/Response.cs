namespace Moneybox.App.Domain
{
    public class AccountResponse
    {
        public bool IsApproachingPayInLimit { get; set; }
        public bool AreFundsLow { get; set; }
    }
}
