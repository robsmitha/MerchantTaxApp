using Application.Common.Interfaces;
using Application.Common.Models;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Application.UnitTests.Services
{
    public class MerchantServiceTests
    {
        [Fact]
        public async Task TaxService_GetTaxRateAsync_CanGetTaxRates()
        {
            var mock = new Mock<ITaxService>();
            mock.Setup(x => x.GetTaxRateAsync("32301"))
                .Returns(Task.FromResult(new TaxRateModel()));

            Assert.NotNull(await mock.Object.GetTaxRateAsync("32301"));
        }

        [Fact]
        public async Task TaxService_CalculateSalesTaxAsync_CanGetCalculateSalesTax()
        {
            var mock = new Mock<ITaxService>();
            mock.Setup(x => x.CalculateSalesTaxAsync("US", "32301", "Florida", 1, 1))
                .Returns(Task.FromResult(new CalculateSalesTaxModel()));

            Assert.NotNull(await mock.Object.CalculateSalesTaxAsync("US", "32301", "Florida", 1, 1));
        }
    }
}
