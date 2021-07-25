using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ITaxJarCalculator
    {
        Task<CalculateSalesTaxModel> CalculateSalesTaxAsync(string to_country, string to_zip, string to_state, float shipping, float amount);
        Task<TaxRateModel> GetTaxRateAsync(string zip);
    }
}
