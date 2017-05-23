using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Loans.Configuration;
using Loans.Domain;

namespace Loans.DataAccess
{
    public class MarketDataDataAccess
    {
        private readonly IFileSystem _fileSystem;
        private readonly IAppSettings _appSettings;
        private readonly string _marketCsv;

        public MarketDataDataAccess(IFileSystem fileSystem, IAppSettings appSettings, string marketCsv)
        {
            _fileSystem = fileSystem;
            _appSettings = appSettings;
            _marketCsv = marketCsv;
        }

        public IList<Lender> ReadAllMarketData()
        {
            var marketData = new List<Lender>();

            var rows = ReadAllData();

            Validate(rows);
            
            rows = SkipHeaderRow(rows);

            ExtractMarketData(rows, marketData);

            return marketData;
        }

        private void Validate(IList<string> rows)
        {
            ValidateNonEmptyFile(rows);
            ValidateHeader(rows.First());

            var lenders = SkipHeaderRow(rows);
            ValidateLenders(lenders);
            foreach (var l in lenders)
            {
                var cells = l.Split(_appSettings.Separator);
                ValidateRow(cells, l);
            }
        }

        private void ValidateLenders(IList<string> lenders)
        {
            if (!lenders.Any())
            {
                throw new CsvFileNoLendersException("The CSV file contains no lenders.");
            }
        }

        private void ValidateNonEmptyFile(IList<string> rows)
        {
            if (rows == null) throw new ArgumentNullException(nameof(rows));
            if (rows.Count == 0)
            {
                throw new CsvFileEmptyException("The CSV file is empty.");
            }
        }

        private void ValidateHeader(string header)
        {
            if (header != "Lender,Rate,Available")
            {
                throw new CsvHeaderCorruptionException($"The header of the CSV file is corrupt: '{header}'.");
            }
        }

        private void ValidateRow(string[] cells, string unsplitRow)
        {
            if (cells.Length != 3 || 
                !CorrectNameFormat(cells[_appSettings.CsvFileNameIndex]) || 
                !CorrectRateFormat(cells[_appSettings.CsvFileRateIndex]) ||
                !CorrectAvailabilityFormat(cells[_appSettings.CsvFileAvailabilityIndex]))
            {
                throw new CsvFileCorruptionException($"The CSV file is corrupt at this row: '{unsplitRow}'.");
            }
        }
        
        private void ExtractMarketData(IEnumerable<string> lenders, List<Lender> marketData)
        {
            foreach (var l in lenders)
            {
                var cells = l.Split(_appSettings.Separator);

                var lender = new Lender(
                    cells[_appSettings.CsvFileNameIndex],
                    decimal.Parse(cells[_appSettings.CsvFileRateIndex]),
                    int.Parse(cells[_appSettings.CsvFileAvailabilityIndex]));
 
                marketData.Add(lender);
            }
        }

        private IList<string> ReadAllData()
        {
            var lenders = _fileSystem.File.ReadLines(_marketCsv);
            return lenders.ToList();
        }

        private bool CorrectNameFormat(string cell)
        {
            return !string.IsNullOrWhiteSpace(cell);
        }

        private bool CorrectRateFormat(string cell)
        {
            decimal rate;
            return decimal.TryParse(cell, out rate);
        }

        private bool CorrectAvailabilityFormat(string cell)
        {
            int availability;
            return int.TryParse(cell, out availability);
        }

        private static IList<string> SkipHeaderRow(IEnumerable<string> rows)
        {
            return rows.Skip(1).ToList();
        }
    }
}