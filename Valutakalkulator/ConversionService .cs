using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Valutakalkulator.Models;

namespace Valutakalkulator
{
    public class ConversionService 
    {
        private static readonly HttpClient _client = new HttpClient();
        private const string _urlSymbols = "https://api.apilayer.com/fixer/symbols";
        private const string _urlLatest = "https://api.apilayer.com/fixer/latest?symbols={0}&base={1}";
        private const string _urlConvert = "https://api.apilayer.com/fixer/convert?to={0}&from={1}&amount={2}&date={3}";

        public ConversionService()
        {
            _client.DefaultRequestHeaders.Add("apikey", "wUfVCSI2AUbO5WUiUXkdSntSkO5iezPV");
        }

        public async Task ExecuteAsync()
        {
            Console.Write("Enter FraValuta: ");
            var fraValuta = Console.ReadLine();
            var validator = new Validator();

            if (validator.IsCurrencyCodeInvalid(fraValuta, nameof(fraValuta)))
                return;

            Console.Write("Enter TilValuta: ");
            var tilValuta = Console.ReadLine();

            if (validator.IsCurrencyCodeInvalid(tilValuta, nameof(tilValuta)))
                return;

            fraValuta = fraValuta.ToUpper();
            tilValuta = tilValuta.ToUpper();

            var dictionary = await GetCurrencyDictionary();

            if (validator.IsCurrencyUnavailable(dictionary, fraValuta, nameof(fraValuta)))
                return;

            if (validator.IsCurrencyUnavailable(dictionary, tilValuta, nameof(tilValuta)))
                return;

            Console.Write("Enter Belop: ");
            var belop = Console.ReadLine();
            int belopInt;

            if (validator.IsNumberInvalid(belop))
                return;
            else
                belopInt = Convert.ToInt32(belop);

            if (fraValuta.Equals(tilValuta))
            {
                Console.Write($"{belop} {fraValuta} equal {belopInt} {tilValuta}");
                return;
            }

            Console.Write("Enter Dato (valgfri): ");
            var datoString = Console.ReadLine();

            if (validator.IsDateEmpty(datoString))
            {
                var rate = await GetLatestRate(tilValuta, fraValuta);
                Console.Write($"{belop} {fraValuta} equal {belopInt * rate} {tilValuta}");

                return;
            }

            if (validator.IsDateInvalid(datoString))
                return;

            var convertedAmount = await ConvertCurrency(tilValuta, fraValuta, belopInt, datoString);
            Console.Write($"{belop} {fraValuta} equal {convertedAmount} {tilValuta}");
        }

        private async Task<decimal> GetLatestRate(string tilValuta, string fraValuta)
        {
            try
            {
                var response = (await _client.GetAsync(string.Format(_urlLatest, tilValuta, fraValuta))).EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();
                var latestResponse = JsonSerializer.Deserialize<LatestResponse>(responseString);

                return latestResponse.rates[tilValuta];
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return default;
            }
        }

        private async Task<Dictionary<string, string>> GetCurrencyDictionary()
        {
            try
            {
                var response = (await _client.GetAsync(_urlSymbols)).EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();
                var symbolsResponse = JsonSerializer.Deserialize<SymbolsResponse>(responseString);

                return symbolsResponse.symbols;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return null;
            }
        }

        private async Task<decimal> ConvertCurrency(string to, string from, int amount, string date)
        {
            try
            {
                var response = (await _client.GetAsync(string.Format(_urlConvert, to, from, amount, date))).EnsureSuccessStatusCode();
                string responseString = await response.Content.ReadAsStringAsync();
                var convertResponse = JsonSerializer.Deserialize<ConvertResponse>(responseString);

                return convertResponse.result;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                return default;
            }
        }
    }
}