using System;
using System.Collections.Generic;
using FluentAssertions;
using Loans.BusinessLogic;
using Loans.Configuration;
using Loans.Domain;
using Moq;
using NUnit.Framework;

namespace Loans.UnitTests.BusinessLogicTests
{
    [TestFixture]
    class LoanCalculatorTests
    {
        private Mock<IAppSettings> _appSettingsMock;

        [SetUp]
        public void SetUp()
        {
            _appSettingsMock = new Mock<IAppSettings>();
            _appSettingsMock.SetupGet(_ => _.LoanTermMonths).Returns(1);
            _appSettingsMock.SetupGet(_ => _.MinBorrowingAmount).Returns(1000);
            _appSettingsMock.SetupGet(_ => _.MaxBorrowingAmount).Returns(15000);
            _appSettingsMock.SetupGet(_ => _.BorrowingAmountMultiple).Returns(100);
        }

        [TestCase(999)]
        [TestCase(16000)]
        [TestCase(1001)]
        public void Calculate_WhenAmountInvalid_ThrowsException(int amount)
        {
            var loanCalculator = new LoanCalculator(null, _appSettingsMock.Object, amount);
            Action action = () => loanCalculator.Calculate();

            action.ShouldThrow<ArgumentException>().WithMessage(
                "The loan amount must be between 1000 " +
                "and 15000 inclusive and must be a multiple of 100.");
        }

        [TestCase]
        [TestCase(999)]
        [TestCase(100, 200, 300, 300, 99)]
        public void Calculate_WhenNoQualifyingLendersExist_ReturnsUnQuotable(params int[] availableAmounts)
        {
            var lenders = new List<Lender>();
            foreach (var amount in availableAmounts)
            {
                lenders.Add(new Lender ("Lender " + amount, 1, amount));
            }

            var loanCalculator = new LoanCalculator(lenders, _appSettingsMock.Object, 1000);

            var quote = loanCalculator.Calculate();

            quote.Should().BeOfType<UnQuotable>();
        }

        [Test]
        public void Calculate_WhenOnlyOneLenderExistsAndHeCanSupplyAllTheMoney_PicksThatLender()
        {
            var lenders = new List<Lender> { new Lender("Lender 1", 0.07m, 1000) };
            var loanCalculator = new LoanCalculator(lenders, _appSettingsMock.Object, 1000);

            var quote = loanCalculator.Calculate();
            
            VerifyQuote(quote, 0.07m, 1000, 1005.65m, 1005.65m);
        }

        [Test]
        public void Calculate_WhenMultipleLendersExistAndTheyAllCanSupplyExactlyTheWholeAmount_PicksTheOneWithTheLowestRate()
        {
            var lenders = new List<Lender>
            {
                new Lender ("Lender 1", 0.08m, 1000),
                new Lender ("Lender 2", 0.07m, 1000)
            };
            var loanCalculator = new LoanCalculator(lenders, _appSettingsMock.Object, 1000);

            var quote = loanCalculator.Calculate();

            VerifyQuote(quote, 0.07m, 1000, 1005.65m, 1005.65m);
        }

        [Test]
        public void Calculate_WhenMultipleLendersExistAndTheyAllCanSupplyExactlyTheWholeAmountButTheyHaveTheSameRate_PicksOne()
        {
            var lenders = new List<Lender>
            {
                new Lender ("Lender 1", 0.07m, 1000),
                new Lender ("Lender 2", 0.07m, 1000),
                new Lender ("Lender 3", 0.07m, 1000)
            };
            var loanCalculator = new LoanCalculator(lenders, _appSettingsMock.Object, 1000);

            var quote = loanCalculator.Calculate();

            VerifyQuote(quote, 0.07m, 1000, 1005.65m, 1005.65m);
        }

        [TestCase(300, 300, 400)]
        [TestCase(300, 300, 400, 100)]
        public void Calculate_WhenMultipleLendersCanTogetherProvideExactlyTheRightAmount_BorrowsFromAllTheLenders(params int[] availableAmounts)
        {
            var lenders = new List<Lender>();
            for (var index = 0; index < availableAmounts.Length; index++)
            {
                var amount = availableAmounts[index];
                lenders.Add(new Lender("Lender 1" + amount, (index+1)*0.01m, amount));
            }
            var loanCalculator = new LoanCalculator(lenders, _appSettingsMock.Object, 1000);

            var quote = loanCalculator.Calculate();

            VerifyQuote(quote, 0.021m, 1000, 1001.73m, 1001.73m);
        }

        [Test]
        public void Calculate_WhenALenderHasMoreThanTheAmountRequired_BorrowsAmountRequiredFromLender()
        {
            var lenders = new List<Lender>
            {
                new Lender ("Lender 1", 0.01m, 2000)
            };
            var loanCalculator = new LoanCalculator(lenders, _appSettingsMock.Object, 1000);

            var quote = loanCalculator.Calculate();

            VerifyQuote(quote, 0.01m, 1000, 1000.83m, 1000.83m);
        }

        [Test]
        public void Calculate_WhenMultipleLendersCombinedHaveMoreThanTheAmountRequired_BorrowsAmountRequiredFromLenders()
        {
            var lenders = new List<Lender>
            {
                new Lender("Lender 1", 0.01m, 750),
                new Lender("Lender 2", 0.02m, 750)
            };
            var loanCalculator = new LoanCalculator(lenders, _appSettingsMock.Object, 1000);

            var quote = loanCalculator.Calculate();

            VerifyQuote(quote, 0.0125m, 1000, 1001.04m, 1001.04m);
        }
        
        [Test]
        public void Calculate_WhenAllIsWell_ProvidesQuote()
        {
            _appSettingsMock.SetupGet(_ => _.LoanTermMonths).Returns(36);

            var lenders = new List<Lender>
            {
                new Lender("Bob", 0.075m, 640),
                new Lender("Jane", 0.069m, 480),
                new Lender("Fred", 0.071m, 520),
                new Lender("Mary", 0.104m, 170),
                new Lender("John", 0.081m, 320),
                new Lender("Dave", 0.074m, 140),
                new Lender("Angela", 0.071m, 60)
            };

            var loanCalculator = new LoanCalculator(lenders, _appSettingsMock.Object, 1000);

            var quote = loanCalculator.Calculate();

            VerifyQuote(quote, 0.07004m, 1000, 30.78m, 1108.10m);
        }

        private static void VerifyQuote(
            IQuote quote, decimal rate, int amount,
            decimal monthlyRepayment, decimal totalRepayment)
        {
            const decimal amountPrecision = 0.01m;
            const decimal ratePrecision = 0.0001m;

            quote.Rate.Should().BeApproximately(rate, ratePrecision);
            quote.RequestedAmount.Should().Be(amount);
            quote.MonthlyRepayment.Should().BeApproximately(monthlyRepayment, amountPrecision);
            quote.TotalRepayment.Should().BeApproximately(totalRepayment, amountPrecision);
        }
    }
}