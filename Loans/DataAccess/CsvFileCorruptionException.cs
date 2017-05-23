using System;

namespace Loans.DataAccess
{
    public class CsvFileCorruptionException : Exception
    {
        public CsvFileCorruptionException(string message) : base(message)
        {
        }
    }
}