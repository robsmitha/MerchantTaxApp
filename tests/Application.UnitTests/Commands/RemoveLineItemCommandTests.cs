using Application.Commands;
using Application.Common.Models;
using Application.UnitTests.Common;
using Domain.Entities;
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
    public class RemoveLineItemCommandTests : ServiceMocks
    {
        [Fact]
        public async Task RemoveLineItemCommand_Validator_CannotRemoveLineItemsOnPaidOrder()
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

            var validator = new RemoveLineItemCommand.Validator(_context);
            var query = new RemoveLineItemCommand(merchant.Id, new RemoveLineItemModel
            {
                ItemId = item.Id,
                OrderId = order.Id
            });
            var response = await validator.ValidateAsync(query, new CancellationToken());
            Assert.False(response.IsValid);
        }
    }
}
