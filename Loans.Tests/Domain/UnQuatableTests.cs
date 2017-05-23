using FluentAssertions;
using Loans.Domain;
using NUnit.Framework;

namespace Loans.UnitTests.Domain
{
    [TestFixture]
    class UnQuotableTests
    {
        [Test]
        public void ToString_WhenCalled_ReturnsAMessageInformingUserThatItIsNotPossibleToProvideAQuoteAtThisTime()
        {
            var quote = new UnQuotable();

            quote.ToString().Should().Be("We are very sorry, it has not been possible to provide a quote for that amount at this time.");
        }
    }
}