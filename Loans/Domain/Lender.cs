namespace Loans.Domain
{
    public class Lender
    {
        public Lender(string name, decimal rate, int available)
        {
            Name = name;
            Rate = rate;
            Available = available;
        }

        public string Name { get; }
        public decimal Rate { get; }
        public int Available { get; }
    }
}