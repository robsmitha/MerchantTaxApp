using Domain.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class LineItem : AuditableEntity
    {
        public int ItemId { get; set; }
        public int OrderId { get; set; }

        [ForeignKey("ItemId")]
        public Item Item { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }
    }
}
