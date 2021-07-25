using Domain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Order : AuditableEntity
    {
        public int OrderStatusTypeId { get; set; }

        [ForeignKey("OrderStatusTypeId")]
        public OrderStatusType OrderStatusType { get; set; }

        public int MerchantId { get; set; }

        [ForeignKey("MerchantId")]
        public Merchant Merchant { get; set; }
    }
}
