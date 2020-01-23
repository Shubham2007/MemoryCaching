using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading;
using static System.Console;

namespace MemoryCaching
{
    class Program
    {
        private const string CacheKey = "availableStocks";
        private static ObjectCache cache = MemoryCache.Default;

        static void Main(string[] args)
        {
            string userChoice = string.Empty, option = string.Empty;

            try
            {
                do
                {
                    PrintOptions();
                    WriteLine("Enter Choice");
                    option = ReadLine();

                    int opt = ValidateOption(option);

                    PerformActionOnSuccess(opt);

                    WriteLine("Continue....(y|n)");
                    userChoice = ReadLine();

                } while (userChoice.ToLower().Trim() == "y");
            }
            catch(CustomException ce)
            {
                // Log
                WriteLine(ce.Message);
            }
            catch(Exception e)
            {
                // Log
                WriteLine("Application has encounterd an error");
            }     

            ReadKey();
        }

        private static void GetStocks()
        {
            // Get stocks
            List<string> stocks = (List<string>)GetAvailableStocks();

            // Print stocks to console
            foreach (string stock in stocks)
            {
                WriteLine(stock);
            }
        }

        private static void PerformActionOnSuccess(int option)
        {
            switch (option)
            {
                case 1: GetStocks();
                    break;

                case 2: AddCache();
                    break;

                case 3: RemoveCache();
                    break;
            }
        }

        private static int ValidateOption(string option)
        {
            bool success = int.TryParse(option, out int opt);

            if (!success || (opt > 3 || opt < 1))
                throw new CustomException("Invalid Choice");

            return opt;
        }

        private static void PrintOptions()
        {
            WriteLine("1 - GetAvailableStocks");
            WriteLine("2 - AddCache");
            WriteLine("3 - RemoveCache");
        }

        /// <summary>
        /// Returns the available stocks from repository
        /// </summary>
        /// <returns></returns>
        private static IEnumerable GetAvailableStocks()
        {
            //Checks if cache has data
            if (cache.Contains(CacheKey))
                return (IEnumerable)cache.Get(CacheKey);

            else
            {
                // Make 3 seconds delay to get data from database 
                Thread.Sleep(3000);

                // Make database call
                IEnumerable availableStocks = GetDefaultStocks();              

                return availableStocks;
            }
        }

        /// <summary>
        /// Get actual data from database
        /// </summary>
        /// <returns></returns>
        private static IEnumerable GetDefaultStocks()
        {
            return new List<string>() { "Pen", "Pencil", "Eraser" };
        }

        /// <summary>
        /// Removes cache from memory
        /// </summary>
        private static void RemoveCache()
        {
            if (!cache.Contains(CacheKey))
            {
                WriteLine("Cache doest not exist");
                return;
            }
                
            cache.Remove(CacheKey);
            WriteLine("Cache deleted");
        }

        /// <summary>
        /// Add caching in memory
        /// </summary>
        /// <param name="data"></param>
        private static void AddCache()
        {
            if (cache.Contains(CacheKey))
            {
                WriteLine("Cache already exist");
                return;
            }
                
            List<string> availableStocks = (List<string>)GetAvailableStocks();
            // Store data in the cache    
            CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
            cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddHours(1.0);
            cache.Add(CacheKey, availableStocks, cacheItemPolicy);

            WriteLine("Cache Added");
        }
    }

    /// <summary>
    /// This class is used to throw catched exceptions
    /// </summary>
    public class CustomException : Exception
    {
        public CustomException() : base()
        { }

        public CustomException(string message) : base(message)
        { }

        public CustomException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}
