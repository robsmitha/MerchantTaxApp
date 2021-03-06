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
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }

        public List<LineItemModel> LineItems { get; set; }
        public decimal SubTotal => LineItems.Sum(l => l.ItemPrice * l.Quantity);
        public decimal ShippingTotal => LineItems.GroupBy(l => l.ItemId).Sum(g => g.FirstOrDefault()?.ItemShipping ?? 0);
        public decimal OrderTotal => SubTotal + ShippingTotal + TaxAmount;
        public string OrderDate => string.Format("{0:F}", ModifiedAt);

        public string DisplayTaxRate => string.Format("{0:P2}", TaxRate);
        public string DisplayTaxAmount => TaxAmount.ToString("C");
        public string DisplayShippingTotal => ShippingTotal.ToString("C");
        public string DisplaySubTotal => SubTotal.ToString("C");
        public string DisplayOrderTotal => OrderTotal.ToString("C");

    }
}
