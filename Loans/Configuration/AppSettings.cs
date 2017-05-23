using System.Configuration;

namespace Loans.Configuration
{
    public class AppSettings : IAppSettings
    {
        public char Separator => char.Parse(ConfigurationManager.AppSettings["Separator"]);
        public int MinBorrowingAmount => int.Parse(ConfigurationManager.AppSettings["MinBorrowingAmount"]);
        public int MaxBorrowingAmount => int.Parse(ConfigurationManager.AppSettings["MaxBorrowingAmount"]);
        public int BorrowingAmountMultiple => int.Parse(ConfigurationManager.AppSettings["BorrowingAmountMultiple"]);
        public int LoanTermMonths => int.Parse(ConfigurationManager.AppSettings["LoanTermMonths"]);
        public int CsvFileNameIndex => int.Parse(ConfigurationManager.AppSettings["CsvFileNameIndex"]);
        public int CsvFileRateIndex => int.Parse(ConfigurationManager.AppSettings["CsvFileRateIndex"]);
        public int CsvFileAvailabilityIndex => int.Parse(ConfigurationManager.AppSettings["CsvFileAvailabilityIndex"]);
        public int RateRoundngDecimalPlaces => int.Parse(ConfigurationManager.AppSettings["RateRoundngDecimalPlaces"]);
        public int RepaymentsRoundngDecimalPlaces => int.Parse(ConfigurationManager.AppSettings["RepaymentsRoundngDecimalPlaces"]);
    }
}