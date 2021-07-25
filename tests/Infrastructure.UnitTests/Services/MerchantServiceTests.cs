using AutoMapper;
using Infrastructure.Services;
using Infrastructure.UnitTests.Common;
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
        }
    }
}
