namespace Loans.Configuration
{
    public interface IAppSettings
    {
        int BorrowingAmountMultiple { get; }
        int CsvFileAvailabilityIndex { get; }
        int CsvFileNameIndex { get; }
        int CsvFileRateIndex { get; }
        int LoanTermMonths { get; }
        int MaxBorrowingAmount { get; }
        int MinBorrowingAmount { get; }
        int RateRoundngDecimalPlaces { get; }
        int RepaymentsRoundngDecimalPlaces { get; }
        char Separator { get; }
    }
}