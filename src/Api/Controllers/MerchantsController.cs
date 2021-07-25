using Application.Commands;
using Application.Models;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MerchantsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MerchantsController> _logger;

        public MerchantsController(IMediator mediator, ILogger<MerchantsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<MerchantModel>>> GetMerchantsAsync()
        {
            return Ok(await _mediator.Send(new GetMerchantsQuery()));
        }


        [HttpGet("{id}/Items")]
        public async Task<ActionResult<List<MerchantModel>>> GetMerchantItemsAsync(int id)
        {
            return Ok(await _mediator.Send(new GetMerchantItemsQuery(id)));
        }

        [HttpGet("{id}/OpenOrder")]
        public async Task<ActionResult<List<MerchantModel>>> GetMerchantOpenOrderAsync(int id)
        {
            return Ok(await _mediator.Send(new GetMerchantOpenOrderQuery(id)));
        }

        [HttpGet("{id}/Order/{orderId}")]
        public async Task<ActionResult<List<MerchantModel>>> GetMerchantOrderAsync(int id, int orderId)
        {
            return Ok(await _mediator.Send(new GetMerchantOrderQuery(id, orderId)));
        }

        [HttpPost("{id}/AddLineItem")]
        public async Task<ActionResult<AddLineItemModel>> AddLineItemAsync(int id, AddLineItemModel model)
        {
            return Ok(await _mediator.Send(new AddLineItemCommand(id, model)));
        }

        [HttpPost("{id}/RemoveLineItem")]
        public async Task<ActionResult<bool>> RemoveLineItemAsync(int id, RemoveLineItemModel model)
        {
            return Ok(await _mediator.Send(new RemoveLineItemCommand(id, model)));
        }

        [HttpPost("{id}/UpdateOrderToPaid")]
        public async Task<ActionResult<bool>> UpdateOrderToPaidAsync(int id, UpdateOrderToPaidModel model)
        {
            return Ok(await _mediator.Send(new UpdateOrderToPaidCommand(id, model)));
        }
    }
}
