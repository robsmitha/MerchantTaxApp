using Application.Common.Customization;
using Application.Common.Interfaces;
using Application.Common.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class UpdateOrderToPaidCommand : IRequest<bool>
    {
        public int MerchantId { get; set; }
        public int OrderId { get; set; }
        public UpdateOrderToPaidCommand(int merchantId, UpdateOrderToPaidModel model)
        {
            MerchantId = merchantId;
            OrderId = model.OrderId;
        }

        public class Validator : AbstractValidator<UpdateOrderToPaidCommand>
        {
            private readonly IApplicationDbContext _context;
            public Validator(IApplicationDbContext context)
            {
                _context = context;

                RuleFor(v => v.OrderId)
                    .NotEmpty()
                    .MustAsync(BeOpenOrder)
                        .WithMessage("The order does not exist or is not in open status.");

                RuleFor(v => v.MerchantId)
                    .NotEmpty()
                    .MustAsync(HaveMerchantZipCode)
                        .WithMessage("The merchant must have a zipcode to update order to paid.");
            }

            private async Task<bool> BeOpenOrder(UpdateOrderToPaidCommand args, int merchantId,
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

            protected async Task<bool> HaveMerchantZipCode(int merchantId,
                CancellationToken cancellationToken)
            {
                var merchant = await _context.Merchants.FindAsync(merchantId);
                return !string.IsNullOrEmpty(merchant.Zip);
            }
        }

        public class Handler : IRequestHandler<UpdateOrderToPaidCommand, bool>
        {
            private readonly IMerchantService _merchantService;
            public Handler(IMerchantService merchantService)
            {
                _merchantService = merchantService;
            }

            public async Task<bool> Handle(UpdateOrderToPaidCommand request, CancellationToken cancellationToken)
            {
                await _merchantService.UpdateOrderToPaidAsync(request.OrderId);
                return true;
            }
        }
    }
}
