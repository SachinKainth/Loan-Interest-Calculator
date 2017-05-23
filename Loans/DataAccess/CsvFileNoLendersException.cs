using System;

namespace Loans.DataAccess
{
    public class CsvFileNoLendersException : Exception
    {
        public CsvFileNoLendersException(string message) : base(message)
        {
        }
    }
}