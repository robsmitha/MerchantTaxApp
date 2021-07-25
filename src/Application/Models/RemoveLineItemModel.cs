using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class RemoveLineItemModel
    {
        public int ItemId { get; set; }
        public int OrderId { get; set; }
    }
}
