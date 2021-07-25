using Application.Common.Settings;
using Application.Interfaces;
using Application.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TaxService : ITaxService
    {
        private readonly ITaxJarCalculator _taxCalculator;
        public TaxService(ITaxJarCalculator taxCalculator)
        {
            _taxCalculator = taxCalculator;
        }

        public async Task<CalculateSalesTaxModel> CalculateSalesTaxAsync(string to_country, string to_zip, string to_state, float shipping, float amount)
        {
            return await _taxCalculator.CalculateSalesTaxAsync(to_country, to_zip, to_state, shipping, amount);
        }

        public async Task<TaxRateModel> GetTaxRateAsync(string zip)
        {
            return await _taxCalculator.GetTaxRateAsync(zip);
        }
    }
}
