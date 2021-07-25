using Domain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Item : AuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int MaxAllowed { get; set; }
        public int MerchantId { get; set; }
        public decimal Shipping { get; set; }

        [ForeignKey("MerchantId")]
        public Merchant Merchant { get; set; }
    }
}
