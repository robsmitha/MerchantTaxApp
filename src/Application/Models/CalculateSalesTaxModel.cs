using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class CalculateSalesTaxModel
    {
        public Tax tax { get; set; }

        public class Jurisdictions
        {
            public string city { get; set; }
            public string country { get; set; }
            public string county { get; set; }
            public string state { get; set; }
        }

        public class Tax
        {
            public double amount_to_collect { get; set; }
            public bool freight_taxable { get; set; }
            public bool has_nexus { get; set; }
            public Jurisdictions jurisdictions { get; set; }
            public double order_total_amount { get; set; }
            public double rate { get; set; }
            public double shipping { get; set; }
            public string tax_source { get; set; }
            public double taxable_amount { get; set; }
        }

    }
}
