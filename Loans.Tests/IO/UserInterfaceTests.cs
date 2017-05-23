using FluentAssertions;
using Loans.IO;
using NUnit.Framework;

namespace Loans.UnitTests.IO
{
    [TestFixture]
    class UserInterfaceTests
    {
        private UserInterface _userInterface;

        [SetUp]
        public void Setup()
        {
            _userInterface = new UserInterface();
        }

        [TestCase]
        [TestCase("market.csv")]
        [TestCase("market.csv", "1000", "too many parameters")]
        [TestCase("market.csv", "")]
        [TestCase("", "1000")]
        [TestCase("market.csv", null)]
        [TestCase(null, "1000")]
        [TestCase("market.csv", " ")]
        [TestCase(" ", "1000")]
        [TestCase("market.csv", "text")]
        public void AreInputsValid_InputsInvalid_ReturnsFalse(params string[] args)
        {
            var result = _userInterface.AreInputsValid(args);

            result.Should().BeFalse();
        }

        [Test]
        public void AreInputsValid_InputsValid_ReturnsTrue()
        {
            var result = _userInterface.AreInputsValid(new[] {"market.csv", "1000"});

            result.Should().BeTrue();
        }
    }
}