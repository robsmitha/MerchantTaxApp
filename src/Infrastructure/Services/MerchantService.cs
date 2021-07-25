
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class MerchantService : IMerchantService
    {
        private ApplicationDbContext _context;
        private IMapper _mapper;
        public MerchantService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<MerchantModel>> GetMerchantsAsync()
        {
            var merchants = await _context.Merchants.ToListAsync();
            return _mapper.Map<List<MerchantModel>>(merchants);
        }

        public async Task<MerchantModel> GetMerchantAsync(int merchantId)
        {
            var items = await _context.Merchants.FindAsync(merchantId);
            return _mapper.Map<MerchantModel>(items);
        }

        public async Task<List<ItemModel>> GetMerchantItemsAsync(int merchantId)
        {
            var items = await _context.Items.Where(i => i.MerchantId == merchantId).ToListAsync();
            var model = _mapper.Map<List<ItemModel>>(items);

            OrderModel order;
            try
            {
                order = await GetMerchantOpenOrderAsync(merchantId);
            }
            catch (NotFoundException)
            {
                order = null;
            }
            foreach (var item in model)
            {
                item.MaxAllowedRange = Enumerable.Range(1, item.MaxAllowed).ToArray();
                item.NewQty = 1;
            }
            return model;
        }

        public async Task<OrderModel> GetMerchantOpenOrderAsync(int merchantId)
        {
            var open = await _context.OrderStatusTypes
                .FirstAsync(o => o.Name.Equals("Open", StringComparison.InvariantCultureIgnoreCase));
            var data = from o in _context.Orders.AsEnumerable()
                       join m in _context.Merchants.AsEnumerable() on o.MerchantId equals m.Id
                       join ost in _context.OrderStatusTypes.AsEnumerable() on o.OrderStatusTypeId equals ost.Id
                       join li in _context.LineItems.AsEnumerable() on o.Id equals li.OrderId into tmp_li
                       from li in tmp_li.DefaultIfEmpty()
                       join i in _context.Items.AsEnumerable() on li?.ItemId equals i.Id into tmp_i
                       from i in tmp_i.DefaultIfEmpty()
                       where o.MerchantId == merchantId
                       && o.OrderStatusTypeId == open.Id
                       select new { o, li };

            var rows = data?.ToList();
            if (rows == null || rows.Count == 0)
            {
                throw new NotFoundException($"No open order found for {nameof(merchantId)}: {merchantId}");
            }

            var itemToLineItem = new Dictionary<int, LineItemModel>();
            var total = 0M;
            foreach (var row in rows)
            {
                if (row.li != null)
                {
                    if (!itemToLineItem.ContainsKey(row.li.ItemId))
                        itemToLineItem.Add(row.li.ItemId, _mapper.Map<LineItemModel>(row.li));

                    itemToLineItem[row.li.ItemId].Quantity++;
                    total += row.li.Item.Price;
                }
            }
            var order = rows[0].o;
            var model = _mapper.Map<OrderModel>(order);
            model.LineItems = itemToLineItem.Values.ToList();
            return model;
        }

        public async Task<OrderModel> GetMerchantOrderAsync(int merchantId, int orderId)
        {
            var paid = await _context.OrderStatusTypes
                .FirstAsync(o => o.Name.Equals("Paid", StringComparison.InvariantCultureIgnoreCase));


            var data = from o in _context.Orders.AsEnumerable()
                       join m in _context.Merchants.AsEnumerable() on o.MerchantId equals m.Id
                       join ost in _context.OrderStatusTypes.AsEnumerable() on o.OrderStatusTypeId equals ost.Id
                       join li in _context.LineItems.AsEnumerable() on o.Id equals li.OrderId into tmp_li
                       from li in tmp_li.DefaultIfEmpty()
                       join i in _context.Items.AsEnumerable() on li?.ItemId equals i.Id into tmp_i
                       from i in tmp_i.DefaultIfEmpty()
                       where o.Id == orderId
                       && o.MerchantId == merchantId
                       && o.OrderStatusTypeId == paid.Id
                       select new { o, li };

            var rows = data?.ToList();
            if (rows == null || rows.Count == 0)
            {
                throw new NotFoundException($"{nameof(orderId)}: {orderId} not found or not in paid status.");
            }

            var itemToLineItem = new Dictionary<int, LineItemModel>();
            var total = 0M;
            foreach (var row in rows)
            {
                if (row.li != null)
                {
                    if (!itemToLineItem.ContainsKey(row.li.ItemId))
                        itemToLineItem.Add(row.li.ItemId, _mapper.Map<LineItemModel>(row.li));

                    itemToLineItem[row.li.ItemId].Quantity++;
                    total += row.li.Item.Price;
                }
            }
            var order = rows[0].o;
            var model = _mapper.Map<OrderModel>(order);
            model.LineItems = itemToLineItem.Values.ToList();
            return model;
        }

        public async Task<AddLineItemModel> AddLineItemAsync(int merchantId, int itemId, int newQty)
        {
            var item = await _context.Items.FindAsync(itemId);

            OrderModel order;
            try
            {
                order = await GetMerchantOpenOrderAsync(merchantId);
            }
            catch (NotFoundException)
            {
                var open = await _context.OrderStatusTypes
                    .FirstAsync(o => o.Name.Equals("Open", StringComparison.InvariantCultureIgnoreCase));
                _context.Orders.Add(new Order
                {
                    Active = true,
                    CreatedAt = DateTime.Now,
                    MerchantId = merchantId,
                    OrderStatusTypeId = open.Id
                });
                await _context.SaveChangesAsync();
                order = await GetMerchantOpenOrderAsync(merchantId);
            }

            //handle qty difference
            var itemCount = order.LineItems.Count;
            var totalQuantity = itemCount + newQty;
            var qtyDiff = totalQuantity - itemCount;
            if (qtyDiff == 0)
            {
                //do nothing, the current qty is equal to the NewQty
            }
            else
            {
                if (qtyDiff > 0)
                {
                    for (int i = 0; i < qtyDiff; i++)
                        _context.LineItems.Add(new LineItem
                        {
                            OrderId = order.Id,
                            ItemId = item.Id,
                            CreatedAt = DateTime.Now
                        });
                }
                else if (qtyDiff < 0)
                {
                    qtyDiff = -qtyDiff;

                    var removelineItemIds = qtyDiff < itemCount
                        ? order.LineItems.GetRange(itemCount - qtyDiff, qtyDiff).Select(r => r.Id)
                        : order.LineItems.Select(r => r.Id);

                    _context.LineItems.RemoveRange(_context.LineItems.Where(l => removelineItemIds.Contains(l.Id)));
                }
                await _context.SaveChangesAsync();
            }

            return new AddLineItemModel
            {
                OrderId = order.Id
            };
        }

        public async Task RemoveLineItemAsync(int orderId, int itemId)
        {
            var removeLineItems = await _context.LineItems
                .Where(l => l.OrderId == orderId && l.ItemId == itemId)
                .ToListAsync();
            _context.LineItems.RemoveRange(removeLineItems);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderToPaidAsync(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if(order == null)
            {
                throw new NotFoundException($"{nameof(orderId)}: {orderId} not found.");
            }
            var paid = await _context.OrderStatusTypes
                   .FirstAsync(o => o.Name.Equals("Paid", StringComparison.InvariantCultureIgnoreCase));
            
            order.OrderStatusTypeId = paid.Id;
            order.ModifiedAt = DateTime.Now;
            await _context.SaveChangesAsync();

        }
    }
}
