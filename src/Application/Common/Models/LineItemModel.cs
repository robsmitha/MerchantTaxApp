using Application.Common.Mappings;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class LineItemModel : IMapFrom<LineItem>
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public decimal ItemPrice { get; set; }
        public decimal ItemShipping { get; set; }
        public int Quantity { get; set; }
        public decimal ItemTotal => ItemPrice * Quantity;
        public string ShippingTotal => ItemShipping.ToString("C");
    }
}
