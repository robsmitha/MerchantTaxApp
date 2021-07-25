using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Merchant> Merchants { get; set; }
        DbSet<Item> Items { get; set; }
        DbSet<Order> Orders { get; set; }
        DbSet<LineItem> LineItems { get; set; }
        DbSet<OrderStatusType> OrderStatusTypes { get; set; }
    }
}
