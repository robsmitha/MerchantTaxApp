using Application.Common.Interfaces;
using Application.Common.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class RemoveLineItemCommand : IRequest<bool>
    {
        public int MerchantId { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public RemoveLineItemCommand(int merchantId, RemoveLineItemModel model)
        {
            MerchantId = merchantId;
            OrderId = model.OrderId;
            ItemId = model.ItemId;
        }

        public class Validator : AbstractValidator<RemoveLineItemCommand>
        {
            private readonly IApplicationDbContext _context;
            public Validator(IApplicationDbContext context)
            {
                _context = context;

                RuleFor(v => v.ItemId)
                    .NotEmpty()
                    .MustAsync(BeValidItem)
                        .WithMessage("The item does not exist or is not active.");

                RuleFor(v => v.MerchantId)
                    .NotEmpty()
                    .MustAsync(BeOpenOrder)
                        .WithMessage("The order does not exist or is not in open status.");
            }

            private async Task<bool> BeValidItem(int itemId,
                CancellationToken cancellationToken)
            {
                var item = await _context.Items.FindAsync(itemId);
                return item != null && item.Active;
            }

            private async Task<bool> BeOpenOrder(RemoveLineItemCommand args, int merchantId,
            CancellationToken cancellationToken)
            {
                var open = await _context.OrderStatusTypes
                    .FirstAsync(o => o.Name.Equals("Open", StringComparison.InvariantCultureIgnoreCase), cancellationToken: cancellationToken);

                var order = await (from o in _context.Orders
                                   join ost in _context.OrderStatusTypes on o.OrderStatusTypeId equals ost.Id
                                   where o.Id == args.OrderId
                                   && o.MerchantId == args.MerchantId
                                   && o.OrderStatusTypeId == open.Id
                                   select o).FirstOrDefaultAsync(cancellationToken: cancellationToken);

                return order?.Id > 0;
            }
        }

        public class Handler : IRequestHandler<RemoveLineItemCommand, bool>
        {
            private readonly IMerchantService _merchantService;
            public Handler(IMerchantService merchantService)
            {
                _merchantService = merchantService;
            }

            public async Task<bool> Handle(RemoveLineItemCommand request, CancellationToken cancellationToken)
            {
                await _merchantService.RemoveLineItemAsync(request.OrderId, request.ItemId);
                return true;
            }
        }
    }
}
