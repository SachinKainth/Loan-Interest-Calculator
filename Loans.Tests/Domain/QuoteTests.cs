using System;
using FluentAssertions;
using Loans.Configuration;
using Loans.Domain;
using Moq;
using NUnit.Framework;

namespace Loans.UnitTests.Domain
{
    [TestFixture]
    class QuoteTests
    {
        private Mock<IAppSettings> _appSettingsMock;

        [SetUp]
        public void Setup()
        {
            _appSettingsMock = new Mock<IAppSettings>();
            _appSettingsMock.SetupGet(_ => _.RateRoundngDecimalPlaces).Returns(1);
            _appSettingsMock.SetupGet(_ => _.RepaymentsRoundngDecimalPlaces).Returns(2);
        }

        [Test]
        public void ToString_WhenCalled_FormatsCorrectly()
        {
            var quote = new Quote(_appSettingsMock.Object, 1000, 0.07m, 34.25m, 1232.93m);

            quote.ToString().Should().Be(
                AddNewLine("Requested amount: £1000") +
                AddNewLine("Rate: 7.0%") +
                AddNewLine("Monthly repayment: £34.25") +
                AddNewLine("Total repayment: £1232.93"));
        }

        [Test]
        public void ToString_WhenRounding_UsesToEvenRounding()
        {
            var quote = new Quote(_appSettingsMock.Object, 1000, 0.0345m, 3.145m, 3.145m);

            quote.ToString().Should().Contain("Rate: 3.4%");
            quote.ToString().Should().Contain("Monthly repayment: £3.14");
            quote.ToString().Should().Contain("Total repayment: £3.14");
        }

        private static string AddNewLine(string line)
        {
            return line + Environment.NewLine;
        }
    }
}
