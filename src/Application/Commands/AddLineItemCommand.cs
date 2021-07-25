using Application.Common.Interfaces;
using Application.Common.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class AddLineItemCommand : IRequest<AddLineItemModel>
    {
        public int MerchantId { get; set; }
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public int NewQty { get; set; }
        public AddLineItemCommand(int merchantId, AddLineItemModel model)
        {
            MerchantId = merchantId;
            ItemId = model.ItemId;
            OrderId = model.OrderId;
            NewQty = model.NewQty;
        }

        public class Validator : AbstractValidator<AddLineItemCommand>
        {
            private readonly IApplicationDbContext _context;
            public Validator(IApplicationDbContext context)
            {
                _context = context;
                RuleFor(v => v.ItemId)
                    .NotEmpty()
                    .MustAsync(BeValidItem)
                        .WithMessage("The item does not exist or is not active.");

                RuleFor(l => l.ItemId).MustAsync(BeLessThanOrEqualToMaxAllowed)
                        .WithMessage("Cannot exceed the maximum allowed for this item.");
            }

            private async Task<bool> BeValidItem(int itemId,
                CancellationToken cancellationToken)
            {
                var item = await _context.Items.FindAsync(itemId);
                return item?.Active == true;
            }

            private async Task<bool> BeLessThanOrEqualToMaxAllowed(AddLineItemCommand args, int itemId,
                CancellationToken cancellationToken)
            {
                var data = from li in _context.LineItems
                           join i in _context.Items on li.ItemId equals i.Id
                           where li.OrderId == args.OrderId && li.ItemId == args.ItemId
                           select new { li, i };

                var linteItem = await data.FirstOrDefaultAsync();
                if (linteItem == null)
                {
                    return true;
                }
                var newQty = data.Count() + args.NewQty;
                return newQty <= linteItem.i.MaxAllowed;

            }
        }
        public class Handler : IRequestHandler<AddLineItemCommand, AddLineItemModel>
        {
            private readonly IMerchantService _merchantService;
            public Handler(IMerchantService merchantService)
            {
                _merchantService = merchantService;
            }

            public async Task<AddLineItemModel> Handle(AddLineItemCommand request, CancellationToken cancellationToken)
            {
                return await _merchantService.AddLineItemAsync(request.MerchantId, request.ItemId, request.NewQty);
            }
        }
    }
}
