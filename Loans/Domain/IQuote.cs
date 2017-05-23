namespace Loans.Domain
{
    public interface IQuote
    {
        decimal MonthlyRepayment { get; }
        decimal Rate { get; }
        int RequestedAmount { get; }
        decimal TotalRepayment { get; }
    }
}