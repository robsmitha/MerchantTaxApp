using Application.Common.Settings;
using Infrastructure.Services;
using Infrastructure.UnitTests.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.UnitTests.Services
{
    public class TaxJarCalculatorTests : ServiceMocks
    {
        public async Task Test()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .Build();
            var merchant = await _context.Merchants.FindAsync();
            var client = new HttpClient();
            var taxJarCalculator = new TaxJarCalculator(client, Options.Create(new TaxJarSettings
            {
                BaseUrl = config.GetSection("TaxJarSettings")["BaseUrl"],
                Version = config.GetSection("TaxJarSettings")["Version"],
                Token = config.GetSection("TaxJarSettings")["Token"]
            }));
            await taxJarCalculator.GetTaxRateAsync(merchant.Zip);
        }
    }
}
