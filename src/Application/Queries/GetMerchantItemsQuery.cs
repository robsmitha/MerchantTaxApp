using Application.Interfaces;
using Application.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetMerchantItemsQuery : IRequest<List<ItemModel>>
    {
        public int MerchantId { get; set; }
        public GetMerchantItemsQuery(int merchantId)
        {
            MerchantId = merchantId;
        }

        public class Handler : IRequestHandler<GetMerchantItemsQuery, List<ItemModel>>
        {
            private IMerchantService _merchantService;

            public Handler(IMerchantService merchantService)
            {
                _merchantService = merchantService;
            }

            public async Task<List<ItemModel>> Handle(GetMerchantItemsQuery request, CancellationToken cancellationToken)
            {
                return await _merchantService.GetMerchantItemsAsync(request.MerchantId);
            }
        }
    }
}
