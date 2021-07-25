using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OrderStatusType : AuditableEntity
    {
        public string Name { get; set; }
    }
}
