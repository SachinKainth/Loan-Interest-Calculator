using System;
using System.Collections.Generic;
using System.Linq;
using Loans.Configuration;
using Loans.Domain;

namespace Loans.BusinessLogic
{
    public class LoanCalculator
    {
        private readonly IList<Lender> _lenders;
        private readonly IAppSettings _appSettings;
        private readonly int _amount;

        public LoanCalculator(IList<Lender> lenders, IAppSettings appSettings, int amount)
        {
            _lenders = lenders;
            _appSettings = appSettings;
            _amount = amount;
        }

        public IQuote Calculate()
        {
            ValidateLoanAmount();

            var ableToLend = IsEnoughMoneyToLend();
            if (!ableToLend)
            {
                return new UnQuotable();
            }

            var orderedLenders = _lenders.OrderBy(l => l.Rate);
            var qualifyingLenders = FindQualifyingLenders(orderedLenders);
            var rate = CalculateRate(qualifyingLenders);
            var quote = CalculateQuoteForRate(rate);

            return quote;
        }

        private IQuote CalculateQuoteForRate(decimal rate)
        {
            var monthlyRate = Math.Pow((double) (1 + rate), (double)1 / 12) - 1;
            var pvAnnuityFactor = (1 - Math.Pow(1 + monthlyRate, -_appSettings.LoanTermMonths)) / monthlyRate;

            var monthlyRepayment = _amount / pvAnnuityFactor;
            var totalRepayment = monthlyRepayment * _appSettings.LoanTermMonths;

            return new Quote(_appSettings, _amount, rate, (decimal) monthlyRepayment, (decimal) totalRepayment);
        }

        private decimal CalculateRate(IList<Lender> qualifyingLenders)
        {
            decimal runningTotalAmount = 0;
            decimal weight = 0;
            for (var index = 0; index < qualifyingLenders.Count; index++)
            {
                var lender = qualifyingLenders[index];

                var remainingAmount = _amount - runningTotalAmount;
                runningTotalAmount += lender.Available;

                weight += index == qualifyingLenders.Count - 1 ? 
                    lender.Rate * remainingAmount :
                    lender.Rate * lender.Available;
            }

            return weight / _amount;
        }

        private IList<Lender> FindQualifyingLenders(IEnumerable<Lender> orderedLenders)
        {
            var sum = 0;
            var lenders = new List<Lender>();
            foreach (var l in orderedLenders)
            {
                sum += l.Available;
                lenders.Add(l);
                if (sum >= _amount)
                {
                    break;
                }
            }

            return lenders;
        }

        private bool IsEnoughMoneyToLend()
        {
            var sumOfLendableAmounts = _lenders.Sum(l => l.Available);
            return sumOfLendableAmounts >= _amount;
        }

        private void ValidateLoanAmount()
        {
            if (!(_amount >= _appSettings.MinBorrowingAmount && 
                _amount <= _appSettings.MaxBorrowingAmount &&
                _amount % _appSettings.BorrowingAmountMultiple == 0))
            {
                throw new ArgumentException(
                    $"The loan amount must be between {_appSettings.MinBorrowingAmount} " +
                    $"and {_appSettings.MaxBorrowingAmount} inclusive and must be a multiple of {_appSettings.BorrowingAmountMultiple}.");
            }
        }
    }
}