using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Merchant : AuditableEntity
    {
        public string Name { get; set; }
        public string Zip { get; set; }
    }
}
