using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Loans.Configuration;
using Loans.DataAccess;
using Loans.Domain;
using Moq;
using NUnit.Framework;

namespace Loans.UnitTests.DataAccessTests
{
    [TestFixture]
    internal class MarketDataDataAccessTests
    {
        private List<Lender> _expectedLenders;
        private const string FileName = "market.csv";
        private Mock<IAppSettings> _appSettingsMock;

        [SetUp]
        public void Setup()
        {
            _expectedLenders = new List<Lender>();
            _appSettingsMock = new Mock<IAppSettings>();
            _appSettingsMock.SetupGet(_ => _.Separator).Returns(',');
            _appSettingsMock.SetupGet(_ => _.CsvFileNameIndex).Returns(0);
            _appSettingsMock.SetupGet(_ => _.CsvFileRateIndex).Returns(1);
            _appSettingsMock.SetupGet(_ => _.CsvFileAvailabilityIndex).Returns(2);
        }

        [TestCase(
@"Lender 1, 0.015, 100
Lender 2, 0.05, 1000")]
        [TestCase(
@"Lender 1, 0.015, 100
Lender 2, 0.05, 1000
")]
        public void ReadAllMarketData_WhenDataExists_LoadsAllData(string lenders)
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { FileName, new MockFileData("Lender,Rate,Available" + Environment.NewLine + lenders) }
            });

            _expectedLenders.Add(new Lender("Lender 1", 0.015m, 100));
            _expectedLenders.Add(new Lender("Lender 2", 0.05m, 1000));
            
            var marketDataDataAccess = new MarketDataDataAccess(fileSystem, _appSettingsMock.Object, FileName);
            
            var result = marketDataDataAccess.ReadAllMarketData();

            result.ShouldBeEquivalentTo(_expectedLenders);
        }

        [TestCase("Lender 1,0.1,not integer")]
        [TestCase("Lender 1,not decimal,1000")]
        [TestCase("Lender 1,not decimal,not integer")]
        [TestCase(",0.1,1000")]
        [TestCase(" ,0.1,1000")]
        [TestCase("Lender 1,,1000")]
        [TestCase("Lender 1,0.1,")]
        [TestCase(",,")]
        [TestCase("not enough columns")]
        [TestCase("Lender 1,0.1,1000, too many columns")]
        [TestCase(
@"Lender 1,0.1,1000

")]
        [TestCase(
@"
")]
        public void ReadAllMarketData_WhenDataCorrupt_ThrowsException(string problemLines)
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { FileName, new MockFileData("Lender,Rate,Available" + Environment.NewLine + problemLines) }
            });

            var marketDataDataAccess = new MarketDataDataAccess(fileSystem, _appSettingsMock.Object, FileName);

            Action action = () => marketDataDataAccess.ReadAllMarketData();

            action
                .ShouldThrow<CsvFileCorruptionException>()
                .WithMessage($"The CSV file is corrupt at this row: '{GetLastLine(problemLines)}'.");
        }

        [Test]
        public void ReadAllMarketData_WhenNoLendersExist_ThrowsException()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { FileName, new MockFileData("Lender,Rate,Available") }
            });

            var marketDataDataAccess = new MarketDataDataAccess(fileSystem, _appSettingsMock.Object, FileName);

            Action action = () => marketDataDataAccess.ReadAllMarketData();

            action
                .ShouldThrow<CsvFileNoLendersException>()
                .WithMessage("The CSV file contains no lenders.");
        }

        [TestCase("Corrupt header")]
        [TestCase(
@"
")]
        public void ReadAllMarketData_WhenHeaderCorrupt_ThrowsException(string header)
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { FileName, new MockFileData(header) }
            });

            var marketDataDataAccess = new MarketDataDataAccess(fileSystem, _appSettingsMock.Object, FileName);

            Action action = () => marketDataDataAccess.ReadAllMarketData();

            action
                .ShouldThrow<CsvHeaderCorruptionException>()
                .WithMessage($"The header of the CSV file is corrupt: '{header}'.");
        }

        [Test]
        public void ReadAllMarketData_WhenFileDoesntExist_ThrowsException()
        {
            var fileSystem = new FileSystem();

            var marketDataDataAccess = new MarketDataDataAccess(fileSystem, _appSettingsMock.Object, FileName);

            Action action = () => marketDataDataAccess.ReadAllMarketData();

            action.ShouldThrow<FileNotFoundException>();
        }

        [Test]
        public void ReadAllMarketData_WhenNoData_ThrowsException()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { FileName, new MockFileData("") }
            });

            var marketDataDataAccess = new MarketDataDataAccess(fileSystem, _appSettingsMock.Object, FileName);

            Action action = () => marketDataDataAccess.ReadAllMarketData();

            action.ShouldThrow<CsvFileEmptyException>().WithMessage("The CSV file is empty.");
        }

        private string GetLastLine(string problemLines)
        {
            var index = problemLines.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);
            return index < 0? problemLines : problemLines.Substring(index + Environment.NewLine.Length);
        }
    }
}