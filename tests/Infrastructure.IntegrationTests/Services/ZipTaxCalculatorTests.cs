using Application.Common.Settings;
using Infrastructure.IntegrationTests.Common;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.IntegrationTests.Services
{
    public class ZipTaxCalculatorTests : ServiceMocks
    {
        [Fact]
        public async Task ZipTaxCalculator_GetTaxRateAsync_ReturnsTaxRate()
        {
            var merchant = await _context.Merchants.FirstAsync();
            var taxRate = await _zipTaxCalculator.GetTaxRateAsync(merchant.Zip);
            Assert.True(taxRate.TaxRate > 0);
        }

        [Fact]
        public async Task ZipTaxCalculator_CalculateSalesTaxAsync_ReturnsCalculateSalesTaxModel()
        {
            var merchant = await _context.Merchants.FirstAsync();
            var taxRate = await _zipTaxCalculator.GetTaxRateAsync(merchant.Zip);
            var model = await _taxJarCalculator.CalculateSalesTaxAsync("US", merchant.Zip, taxRate.State, 19.99f, 200f);
            Assert.True(model.TaxAmount > 0);
        }
    }
}
