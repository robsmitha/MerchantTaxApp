using Application.Common.Settings;
using Infrastructure.Services;
using Infrastructure.IntegrationTests.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.IntegrationTests.Services
{
    public class TaxJarCalculatorTests : ServiceMocks
    {
        [Fact]
        public async Task TaxJarCalculator_GetTaxRateAsync_ReturnsTaxRateModel()
        {
            var merchant = await _context.Merchants.FirstAsync();
            var taxRate = await _taxJarCalculator.GetTaxRateAsync(merchant.Zip);
            Assert.True(taxRate.TaxRate > 0);
        }

        [Fact]
        public async Task TaxJarCalculator_CalculateSalesTaxAsync_ReturnsCalculateSalesTaxModel()
        {
            var merchant = await _context.Merchants.FirstAsync();
            var taxRate = await _taxJarCalculator.GetTaxRateAsync(merchant.Zip);
            var model = await _taxJarCalculator.CalculateSalesTaxAsync("US", merchant.Zip, taxRate.State, 19.99f, 200f);
            Assert.True(model.TaxAmount > 0);
        }
    }
}
