using Application.Common.Mappings;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ItemModel : IMapFrom<Item>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Shipping { get; set; }
        public int NewQty { get; set; }
        public int MaxAllowed { get; set; }
        public int[] MaxAllowedRange { get; set; }


    }
}
