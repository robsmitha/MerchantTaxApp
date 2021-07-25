using Application.Queries;
using Application.UnitTests.Common;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.UnitTests.Queries
{
    public class GetMerchantOrderQueryTests : ServiceMocks
    {
        [Fact]
        public async Task GetMerchantOrderQuery_Handler_ReturnsOrderWithTax()
        {
            var merchant = await _context.Merchants.FirstAsync();
            var item = await _context.Items.FirstAsync(i => i.MerchantId == merchant.Id);
            var orderStatusTypes = await _context.OrderStatusTypes
                .ToDictionaryAsync(o => o.Name.ToUpper(), o => o.Id);
            var order = new Order
            {
                Active = true,
                CreatedAt = DateTime.Now,
                MerchantId = merchant.Id,
                OrderStatusTypeId = orderStatusTypes["PAID"]
            };
            _context.Add(order);
            await _context.SaveChangesAsync();
            _context.Add(new LineItem
            {
                OrderId = order.Id,
                ItemId = item.Id,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();


            var merchantService = new MerchantService(_context, _mapper);
            var taxService = new TaxService(_taxCalculatorMock.Object);
            var handler = new GetMerchantOrderQuery.Handler(merchantService, taxService);
            var query = new GetMerchantOrderQuery(merchant.Id, order.Id);
            var response = await handler.Handle(query, new CancellationToken());
            Assert.True(response.TaxRate != 0);
            Assert.True(response.TaxAmount != 0);
        }

        [Fact]
        public async Task GetMerchantOrderrQuery_Handler_ShouldNotCallTaxCalculatorIfNoLineItems()
        {

            var merchant = await _context.Merchants.FirstAsync();
            var item = await _context.Items.FirstAsync(i => i.MerchantId == merchant.Id);
            var orderStatusTypes = await _context.OrderStatusTypes
                .ToDictionaryAsync(o => o.Name.ToUpper(), o => o.Id);
            var order = new Order
            {
                Active = true,
                CreatedAt = DateTime.Now.AddSeconds(-1),
                MerchantId = merchant.Id,
                OrderStatusTypeId = orderStatusTypes["PAID"],
                ModifiedAt = DateTime.Now
            };
            _context.Add(order);
            await _context.SaveChangesAsync();

            var merchantService = new MerchantService(_context, _mapper);
            var taxService = new TaxService(_taxCalculatorMock.Object);
            var handler = new GetMerchantOrderQuery.Handler(merchantService, taxService);
            var query = new GetMerchantOrderQuery(merchant.Id, order.Id);
            var response = await handler.Handle(query, new CancellationToken());
            _taxCalculatorMock.Verify(service => service.GetTaxRateAsync(It.IsAny<string>()), Times.Never);
            _taxCalculatorMock.Verify(service => service.CalculateSalesTaxAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<float>(), It.IsAny<float>()), Times.Never);
        }

        [Fact]
        public async Task GetMerchantOrderQuery_Validator_MerchantZipShouldNotBeNull()
        {
            var merchant = await _context.Merchants.FirstAsync();
            merchant.Zip = null;
            var orderStatusTypes = await _context.OrderStatusTypes
                .ToDictionaryAsync(o => o.Name.ToUpper(), o => o.Id);
            var order = new Order
            {
                Active = true,
                CreatedAt = DateTime.Now.AddSeconds(-1),
                MerchantId = merchant.Id,
                OrderStatusTypeId = orderStatusTypes["PAID"],
                ModifiedAt = DateTime.Now
            };
            _context.Add(order);
            await _context.SaveChangesAsync();

            var validator = new GetMerchantOrderQuery.Validator(_context);
            var query = new GetMerchantOrderQuery(merchant.Id, order.Id);
            var response = await validator.ValidateAsync(query, new CancellationToken());
            Assert.False(response.IsValid);
        }
    }
}
