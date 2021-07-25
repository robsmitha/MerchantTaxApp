using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class AddLineItemModel
    {
        public int MerchantId { get; set; }
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public int NewQty { get; set; }
    }
}
