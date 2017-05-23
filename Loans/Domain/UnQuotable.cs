namespace Loans.Domain
{
    public class UnQuotable : IQuote
    {
        // I don't quite like this and the Adapter Pattern is not quite applicable
        public decimal MonthlyRepayment { get; }
        public decimal Rate { get; }
        public int RequestedAmount { get; }
        public decimal TotalRepayment { get; }

        public override string ToString()
        {
            return "We are very sorry, it has not been possible to provide a quote for that amount at this time.";
        }
    }
}