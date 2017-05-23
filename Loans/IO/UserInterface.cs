using System;
using Loans.Domain;

namespace Loans.IO
{
    public class UserInterface
    {
        public bool AreInputsValid(string[] args)
        {
            var isValid = args.Length == 2 && !string.IsNullOrWhiteSpace(args[0]) && !string.IsNullOrWhiteSpace(args[1]);

            if (isValid)
            {
                int amount;
                var wasSuccess = int.TryParse(args[1], out amount);
                isValid = wasSuccess;
            }

            return isValid;
        }

        public void DisplayUsageMessage()
        {
            Console.WriteLine($"Usage: {System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName} [market_file] [loan_amount]");
            Environment.Exit(-1);
        }

        public void DisplayQuote(IQuote quote)
        {
            Console.Write(quote.ToString());
            Environment.Exit(-1);
        }

        public void DisplayExceptionMessage(Exception exception)
        {
            Console.WriteLine(exception.Message);
            Environment.Exit(-1);
        }
    }
}