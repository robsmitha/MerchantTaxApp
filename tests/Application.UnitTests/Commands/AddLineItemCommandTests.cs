using Application.Commands;
using Application.Common.Models;
using Application.UnitTests.Common;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.UnitTests.Commands
{
    public class AddLineItemCommandTests : ServiceMocks
    {
        [Fact]
        public async Task AddLineItemCommand_Handler_ReturnsOrderId()
        {
            var merchant = await _context.Merchants.FirstAsync();
            var item = await _context.Items.FirstAsync(i => i.MerchantId == merchant.Id);

            var merchantService = new MerchantService(_context, _mapper);
            var handler = new AddLineItemCommand.Handler(merchantService);
            var query = new AddLineItemCommand(merchant.Id, new AddLineItemModel
            {
                ItemId = item.Id,
                MerchantId = item.MerchantId,
                NewQty = 1
            });
            var response = await handler.Handle(query, new CancellationToken());
            Assert.True(response.OrderId != 0);
        }

        [Fact]
        public async Task AddLineItemCommand_Validator_ItemShouldBeActive()
        {
            var merchant = await _context.Merchants.FirstAsync();
            var item = await _context.Items.FirstAsync(i => i.MerchantId == merchant.Id);
            item.Active = false;
            await _context.SaveChangesAsync();
            var validator = new AddLineItemCommand.Validator(_context);
            var query = new AddLineItemCommand(merchant.Id, new AddLineItemModel
            {
                ItemId = item.Id,
                MerchantId = item.MerchantId,
                NewQty = 1
            });
            var response = await validator.ValidateAsync(query, new CancellationToken());
            Assert.False(response.IsValid);
        }

        [Fact]
        public async Task AddLineItemCommand_Validator_CannotExceedMaxAllowed()
        {
            var merchant = await _context.Merchants.FirstAsync();
            var item = await _context.Items.FirstAsync(i => i.MerchantId == merchant.Id);
            item.MaxAllowed = 1;
            await _context.SaveChangesAsync();

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

            var validator = new AddLineItemCommand.Validator(_context);
            var query = new AddLineItemCommand(merchant.Id, new AddLineItemModel
            {
                ItemId = item.Id,
                OrderId = order.Id,
                MerchantId = item.MerchantId,
                NewQty = item.MaxAllowed + 1
            });
            var response = await validator.ValidateAsync(query, new CancellationToken());
            Assert.False(response.IsValid);
        }
    }
}
