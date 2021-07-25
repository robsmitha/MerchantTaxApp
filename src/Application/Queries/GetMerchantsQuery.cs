using Application.Interfaces;
using Application.Models;
using FluentValidation;
using Infrastructure.Context;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetMerchantsQuery : IRequest<List<MerchantModel>>
    {
        public class Handler : IRequestHandler<GetMerchantsQuery, List<MerchantModel>>
        {
            private readonly IMerchantService _merchantService;

            public Handler(IMerchantService merchantService)
            {
                _merchantService = merchantService;
            }


            public async Task<List<MerchantModel>> Handle(GetMerchantsQuery request, CancellationToken cancellationToken)
            {
                return await _merchantService.GetMerchantsAsync();
            }
        }

    }
}
