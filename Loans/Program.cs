using System;
using System.IO.Abstractions;
using Loans.BusinessLogic;
using Loans.Configuration;
using Loans.DataAccess;
using Loans.IO;

namespace Loans
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var userInterface = new UserInterface();
            if (!userInterface.AreInputsValid(args))
            {
                userInterface.DisplayUsageMessage();
            }

            var file = args[0];
            var amount = int.Parse(args[1]);

            var appSettings = new AppSettings();

            var marketDataDataAccess = new MarketDataDataAccess(new FileSystem(), appSettings, file);

            try
            {
                var marketData = marketDataDataAccess.ReadAllMarketData();
                var loanCalculator = new LoanCalculator(marketData, appSettings, amount);
                var quote = loanCalculator.Calculate();

                userInterface.DisplayQuote(quote);
            }
            catch (Exception e)
            {
                userInterface.DisplayExceptionMessage(e);
            }
        }
    }
}