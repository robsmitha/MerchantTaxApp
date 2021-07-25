using Application.Common.Mappings;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class OrderModel : IMapFrom<Order>
    {
        public OrderModel()
        {
            LineItems = new List<LineItemModel>();
        }
        public int Id { get; set; }
        public int MerchantId { get; set; }
        public string MerchantName { get; set; }
        public string MerchantZip { get; set; }
        public int OrderStatusTypeId { get; set; }
        public string OrderStatusTypeName { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public List<LineItemModel> LineItems { get; set; }

        public decimal SubTotal => LineItems.Sum(l => l.ItemPrice * l.Quantity);
        public decimal ShippingTotal => LineItems.Sum(l => l.ItemShipping * l.Quantity);
        public decimal TaxAmount { get; set; }
        public decimal OrderTotal => SubTotal + ShippingTotal + TaxAmount;
        public string OrderDate => string.Format("{0:F}", ModifiedAt);

    }
}
