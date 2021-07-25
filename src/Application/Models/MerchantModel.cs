using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Models
{
    public class MerchantModel : IMapFrom<Merchant>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Zip { get; set; }
        public decimal TaxRate { get; set; }
    }
}
