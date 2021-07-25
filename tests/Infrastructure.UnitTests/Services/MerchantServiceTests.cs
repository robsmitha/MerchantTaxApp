using Application.Common.Models;
using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitTests.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.UnitTests.Services
{
    public class MerchantServiceTests : ServiceMocks
    {
        [Fact]
        public async Task MerchantService_GetMerchantsAsync_ReturnsMerchantModels()
        {
            var merchantService = new MerchantService(_context, _mapper);
            var merchants = await merchantService.GetMerchantsAsync();
            Assert.NotEmpty(merchants);
            Assert.IsType<List<MerchantModel>>(merchants);
        }

        [Fact]
        public async Task MerchantService_GetMerchantItemsAsync_ReturnsItemModels()
        {
            var merchant = await _context.Merchants.FirstAsync();
            var merchantService = new MerchantService(_context, _mapper);
            var items = await merchantService.GetMerchantItemsAsync(merchant.Id);
            Assert.NotEmpty(items);
            Assert.IsType<List<ItemModel>>(items);
        }

        [Fact]
        public async Task MerchantService_GetMerchantAsync_ReturnsMerchantModel()
        {
            var merchant = await _context.Merchants.FirstAsync();
            var merchantService = new MerchantService(_context, _mapper);
            var model = await merchantService.GetMerchantAsync(merchant.Id);
            Assert.NotNull(model);
            Assert.IsType<MerchantModel>(model);
        }
    }
}
