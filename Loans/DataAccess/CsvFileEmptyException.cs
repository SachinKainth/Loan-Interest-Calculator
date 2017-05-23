using System;

namespace Loans.DataAccess
{
    public class CsvFileEmptyException : Exception
    {
        public CsvFileEmptyException(string message) : base(message)
        {
        }
    }
}