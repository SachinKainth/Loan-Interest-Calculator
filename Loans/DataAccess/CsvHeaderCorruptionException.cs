using System;

namespace Loans.DataAccess
{
    public class CsvHeaderCorruptionException : Exception
    {
        public CsvHeaderCorruptionException(string message) : base(message)
        {
        }
    }
}