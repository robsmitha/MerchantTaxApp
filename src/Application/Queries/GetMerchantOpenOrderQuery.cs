using Application.Common.Interfaces;
using Application.Common.Models;
using FluentValidation;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetMerchantOpenOrderQuery : IRequest<OrderModel>
    {
        public int MerchantId { get; set; }
        public GetMerchantOpenOrderQuery(int merchantId)
        {
            MerchantId = merchantId;
        }

        public class Validator : AbstractValidator<GetMerchantOpenOrderQuery>
        {
            private readonly IApplicationDbContext _context;
            public Validator(IApplicationDbContext context)
            {
                _context = context;

                RuleFor(v => v.MerchantId)
                    .NotEmpty()
                    .MustAsync(HaveMerchantZipCode)
                        .WithMessage("The merchant must have a zipcode to update order to paid.");
            }

            protected async Task<bool> HaveMerchantZipCode(int merchantId,
                CancellationToken cancellationToken)
            {
                var merchant = await _context.Merchants.FindAsync(merchantId);
                return !string.IsNullOrEmpty(merchant.Zip);
            }
        }

        public class Handler : IRequestHandler<GetMerchantOpenOrderQuery, OrderModel>
        {
            private readonly IMerchantService _merchantService;
            private readonly ITaxService _taxJarService;

            public Handler(IMerchantService merchantService, ITaxService taxJarService)
            {
                _merchantService = merchantService;
                _taxJarService = taxJarService;
            }

            public async Task<OrderModel> Handle(GetMerchantOpenOrderQuery request, CancellationToken cancellationToken)
            {
                var order = await _merchantService.GetMerchantOpenOrderAsync(request.MerchantId);
                if (order.LineItems.Any())
                {
                    var merchant = await _merchantService.GetMerchantAsync(request.MerchantId);
                    var taxRate = await _taxJarService.GetTaxRateAsync(merchant.Zip);
                    var salesTax = await _taxJarService.CalculateSalesTaxAsync("US", merchant.Zip, taxRate.State, (float)order.ShippingTotal, (float)order.SubTotal);
                    order.TaxAmount = Convert.ToDecimal(salesTax.TaxAmount);
                    order.TaxRate = taxRate.TaxRate;
                }
                return order;
            }
        }
    }
}
