using Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMerchantService
    {
        Task<List<MerchantModel>> GetMerchantsAsync();
        Task<MerchantModel> GetMerchantAsync(int merchantId);
        Task<List<ItemModel>> GetMerchantItemsAsync(int merchantId);
        Task<OrderModel> GetMerchantOpenOrderAsync(int merchantId);
        Task<OrderModel> GetMerchantOrderAsync(int merchantId, int orderId);
        Task<AddLineItemModel> AddLineItemAsync(int merchantId, int itemId, int newQty);
        Task RemoveLineItemAsync(int orderId, int itemId);
        Task UpdateOrderToPaidAsync(int orderId);
    }
}
