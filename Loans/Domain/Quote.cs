using System;
using Loans.Configuration;

namespace Loans.Domain
{
    public class Quote : IQuote
    {
        private readonly IAppSettings _appSettings;

        public Quote(
            IAppSettings appSettings, int requestedAmount, decimal rate, 
            decimal monthlyRepayment, decimal totalRepayment)
        {
            _appSettings = appSettings;
            RequestedAmount = requestedAmount;
            Rate = rate;
            MonthlyRepayment = monthlyRepayment;
            TotalRepayment = totalRepayment;
        }

        public int RequestedAmount { get; }
        public decimal Rate { get; }
        public decimal MonthlyRepayment { get; }
        public decimal TotalRepayment { get; }

        public override string ToString()
        {
            return
                AddNewLine($"Requested amount: £{RequestedAmount}") +
                AddNewLine($"Rate: {Math.Round(Rate*100, _appSettings.RateRoundngDecimalPlaces)}%") + 
                AddNewLine($"Monthly repayment: £{Math.Round(MonthlyRepayment, _appSettings.RepaymentsRoundngDecimalPlaces)}") +
                AddNewLine($"Total repayment: £{Math.Round(TotalRepayment, _appSettings.RepaymentsRoundngDecimalPlaces)}");
        }

        private static string AddNewLine(string line)
        {
            return line + Environment.NewLine;
        }
    }
}