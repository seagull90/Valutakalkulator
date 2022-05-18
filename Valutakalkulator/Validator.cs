using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Valutakalkulator
{
    public class Validator
    {
        private Regex _currencyRegex;
        private const string _validationErrorMessage = "{0}. Session terminates.";

        public Validator()
        {
            _currencyRegex = new Regex(@"^[A-Za-z]{3}$");
        }

        public bool IsCurrencyCodeInvalid(string currencyCode, string inputName)
        {
            if (!_currencyRegex.IsMatch(currencyCode))
            {
                Console.WriteLine(string.Format(_validationErrorMessage, $"Invalid {inputName}: only 3 letters is allowed"));

                return true;
            }

            return false;
        }

        internal bool IsCurrencyUnavailable(Dictionary<string, string> dictionary, string currencyCode, string inputName)
        {
            if (!dictionary.ContainsKey(currencyCode))
            {
                Console.WriteLine(string.Format(_validationErrorMessage, $"Invalid {inputName}: {currencyCode} is not among available currencies"));

                return true;
            }

            return false;
        }

        internal bool IsNumberInvalid(string belop)
        {
            if (!new Regex("[0-9]").IsMatch(belop))
            {
                Console.WriteLine(string.Format(_validationErrorMessage, "Invalid Belop format: only numbers is allowed"));

                return true;
            }

            if (Convert.ToInt32(belop) <= 0)
            {
                Console.WriteLine(string.Format(_validationErrorMessage, "Invalid Belop: must be bigger than zero"));

                return true;
            }

            return false;
        }

        internal bool IsDateInvalid(string datoString)
        {
            Regex datoRegex = new Regex(@"^\d{4}\-(0[1-9]|1[012])\-(0[1-9]|[12][0-9]|3[01])$");

            if (!datoRegex.IsMatch(datoString))
            {
                Console.WriteLine(string.Format(_validationErrorMessage, "Invalid dato: must have YYYY-MM-DD format f.eks 2018-01-01"));

                return true;
            }

            var dateTime = DateTime.Parse(datoString);

            if (dateTime.Date > DateTime.Today)
            {
                Console.WriteLine(string.Format(_validationErrorMessage, "Invalid dato: value cannot be greater than the current date"));

                return true;
            }

            return false;
        }

        internal bool IsDateEmpty(string datoString)
        {
            return string.IsNullOrEmpty(datoString) || string.IsNullOrWhiteSpace(datoString);
        }
    }
}
