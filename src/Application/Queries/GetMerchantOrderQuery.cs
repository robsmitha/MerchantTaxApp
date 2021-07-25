using Application.Common.Interfaces;
using Application.Common.Models;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetMerchantOrderQuery : IRequest<OrderModel>
    {
        public int MerchantId { get; set; }
        public int OrderId { get; set; }
        public GetMerchantOrderQuery(int merchantId, int orderId)
        {
            MerchantId = merchantId;
            OrderId = orderId;
        }

        public class Validator : AbstractValidator<GetMerchantOrderQuery>
        {
            private readonly IApplicationDbContext _context;
            public Validator(IApplicationDbContext context)
            {
                _context = context;

                RuleFor(v => v.MerchantId)
                    .NotEmpty()
                    .MustAsync(HaveZipCode)
                        .WithMessage("Merchant must have a zipcode to get order.");
            }

            public async Task<bool> HaveZipCode(int merchantId,
                CancellationToken cancellationToken)
            {
                var merchant = await _context.Merchants.FindAsync(merchantId);
                return !string.IsNullOrEmpty(merchant.Zip);
            }
        }

        public class Handler : IRequestHandler<GetMerchantOrderQuery, OrderModel>
        {
            private readonly IMerchantService _merchantService;
            private readonly ITaxService _taxJarService;

            public Handler(IMerchantService merchantService, ITaxService taxJarService)
            {
                _merchantService = merchantService;
                _taxJarService = taxJarService;
            }

            public async Task<OrderModel> Handle(GetMerchantOrderQuery request, CancellationToken cancellationToken)
            {
                var merchant = await _merchantService.GetMerchantAsync(request.MerchantId);
                var taxRate = await _taxJarService.GetTaxRateAsync(merchant.Zip);
                var order = await _merchantService.GetMerchantOrderAsync(request.MerchantId, request.OrderId);
                var salesTax = await _taxJarService.CalculateSalesTaxAsync("US", merchant.Zip, taxRate.rate.state, (float)order.ShippingTotal, (float)order.SubTotal);
                order.TaxAmount = Convert.ToDecimal(salesTax.tax.amount_to_collect);
                return order;
            }
        }
    }
}
