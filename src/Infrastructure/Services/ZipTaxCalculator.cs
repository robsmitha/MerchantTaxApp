using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ZipTaxCalculator : ITaxCalculator
    {
        private readonly HttpClient _client;
        private readonly ZipTaxSettings _settings;

        public ZipTaxCalculator(HttpClient client, IOptions<ZipTaxSettings> options)
        {
            _client = client;
            _settings = options.Value;
        }

        public async Task<CalculateSalesTaxModel> CalculateSalesTaxAsync(string to_country, string to_zip, string to_state, float shipping, float amount)
        {
            try
            {
                var uri = RequestUri("request", new Dictionary<string, string>
                {
                    {"postalcode", to_zip }
                });
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    dynamic data = JsonConvert.DeserializeObject(result);
                    var taxRate = new TaxRateModel
                    {
                        TaxRate = data.results[0].taxUse,
                        State = data.results[0].geoState
                    };
                    return new CalculateSalesTaxModel
                    {
                        TaxAmount = Math.Round((decimal)amount * taxRate.TaxRate, 2, MidpointRounding.ToEven)
                    };
                }

                return default;
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }

        public async Task<TaxRateModel> GetTaxRateAsync(string zip)
        {
            try
            {
                var uri = RequestUri("request", new Dictionary<string, string>
                {
                    {"postalcode", zip }
                });
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    dynamic data = JsonConvert.DeserializeObject(result);
                    return new TaxRateModel
                    {
                        TaxRate = data.results[0].taxUse,
                        State = data.results[0].geoState
                    };
                }

                return default;
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }
        private string RequestUri(string endpoint, Dictionary<string, string> @params)
        {
            var requestUri = new StringBuilder();
            requestUri.Append(_settings.BaseUrl);
            requestUri.Append($"/{endpoint}");
            requestUri.Append($"/{_settings.Version}");
            requestUri.Append($"?key={_settings.Token}");
            @params?.Keys.ToList().ForEach(k => requestUri.Append($"&{k}={@params[k]}"));
            return requestUri.ToString();
        }
    }
}
