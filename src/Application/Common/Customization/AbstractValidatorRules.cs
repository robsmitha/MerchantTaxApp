using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Customization
{
    //public abstract class AbstractValidatorRules<T> : AbstractValidator<T>
    //{
    //    protected readonly ApplicationDbContext _context;
    //    public AbstractValidatorRules(ApplicationDbContext context)
    //    {
    //        _context = context;
    //    }

    //    protected async Task<bool> BeOpenOrder(int orderId,
    //        CancellationToken cancellationToken)
    //    {
    //        var open = await _context.OrderStatusTypes
    //            .FirstAsync(o => o.Name.Equals("Open", StringComparison.InvariantCultureIgnoreCase), cancellationToken: cancellationToken);

    //        var order = await (from o in _context.Orders
    //                           join ost in _context.OrderStatusTypes on o.OrderStatusTypeId equals ost.Id
    //                           where o.Id == orderId
    //                           && o.OrderStatusTypeId == open.Id
    //                           select o).FirstOrDefaultAsync(cancellationToken: cancellationToken);

    //        return order?.Id > 0;
    //    }

    //    protected async Task<bool> HaveMerchantZipCode(int merchantId,
    //        CancellationToken cancellationToken)
    //    {
    //        var merchant = await _context.Merchants.FindAsync(merchantId);
    //        return !string.IsNullOrEmpty(merchant.Zip);
    //    }

    //    protected async Task<bool> BeValidMerchant(int merchantId,
    //        CancellationToken cancellationToken)
    //    {
    //        var merchant = await _context.Merchants.FindAsync(merchantId);
    //        return merchant != null && merchant.Active;
    //    }

    //    protected async Task<bool> BeValidItem(int itemId,
    //        CancellationToken cancellationToken)
    //    {
    //        var item = await _context.Items.FindAsync(itemId);
    //        return item != null && item.Active;
    //    }
    //}
}
