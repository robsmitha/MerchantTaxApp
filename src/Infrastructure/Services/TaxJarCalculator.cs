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
    public class TaxJarCalculator : ITaxJarCalculator
    {
        private readonly TaxJarSettings _settings;
        private readonly IExternalService _externalService;
        public TaxJarCalculator(IExternalService externalService, IOptions<TaxJarSettings> options)
        {
            _externalService = externalService;
            _settings = options.Value;
        }

        public async Task<CalculateSalesTaxModel> CalculateSalesTaxAsync(string to_country, string to_zip, string to_state, float shipping, float amount)
        {
            return await _externalService.PostAsync<CalculateSalesTaxModel>(RequestUri("taxes"), JsonConvert.SerializeObject(new
            {
                to_country,
                to_zip,
                to_state,
                shipping,
                amount,
                from_country = to_country,
                from_zip = to_zip,
                from_state = to_state
            }), _settings.Token);
        }

        public async Task<TaxRateModel> GetTaxRateAsync(string zip)
        {
            return await _externalService.SendAsync<TaxRateModel>(RequestUri($"rates/{zip}"), _settings.Token);
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
