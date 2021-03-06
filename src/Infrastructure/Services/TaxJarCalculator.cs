using Application.Common.Settings;
using Application.Common.Interfaces;
using Application.Common.Models;
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
    public class TaxJarCalculator : ITaxCalculator
    {
        private readonly HttpClient _client;
        private readonly TaxJarSettings _settings;

        public TaxJarCalculator(HttpClient client, IOptions<TaxJarSettings> options)
        {
            _client = client;
            _settings = options.Value;
        }

        public async Task<CalculateSalesTaxModel> CalculateSalesTaxAsync(string to_country, string to_zip, string to_state, float shipping, float amount)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Token);
                var uri = RequestUri("taxes");
                var json = JsonConvert.SerializeObject(new
                {
                    to_country,
                    to_zip,
                    to_state,
                    shipping,
                    amount,
                    from_country = to_country,
                    from_zip = to_zip,
                    from_state = to_state
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(uri, content);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    dynamic data = JsonConvert.DeserializeObject(result);
                    return new CalculateSalesTaxModel
                    {
                        TaxAmount = data.tax.amount_to_collect
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
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Token);
                var uri = RequestUri($"rates/{zip}");
                var response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    dynamic data = JsonConvert.DeserializeObject(result);
                    return new TaxRateModel
                    {
                        TaxRate = data.rate.combined_rate,
                        State = data.rate.state
                    };
                }

                return default;
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }

        private string RequestUri(string endpoint)
        {
            var requestUri = new StringBuilder();
            requestUri.Append(_settings.BaseUrl);
            requestUri.Append($"/{_settings.Version}");
            requestUri.Append($"/{endpoint}");
            return requestUri.ToString();
        }

    }
}
