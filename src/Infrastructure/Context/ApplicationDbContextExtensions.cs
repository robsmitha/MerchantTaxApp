using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Context
{
    public static class ApplicationDbContextExtensions
    {
        public static async Task SeedDataAsync(this ApplicationDbContext context)
        {
            if (!context.Merchants.Any())
            {
                var merchants = new List<Merchant>
                {
                    new()
                    {
                        Name = "Ashley Furniture HomeStore",
                        Zip = "90011"
                    },
                    new()
                    {
                        Name = "Rooms To Go",
                        Zip = "32308"
                    },
                    new()
                    {
                        Name = "Macys Furniture",
                        Zip = "10007"
                    }
                };
                context.AddRange(merchants);
                await context.SaveChangesAsync();
            }

            if (!context.Items.Any())
            {

                foreach (var merchant in context.Merchants)
                {
                    var items = new List<Item>
                    {
                        new()
                        {
                            Name = "Calion Sofa",
                            Description = "In the chicest shade of gray, Calion sofa’s linen-weave upholstery complements so many color schemes and aesthetics. Flared arms, prominent welting and flamestitch-print pillows add just enough panache to this sweet and simple sofa. Supportive seat cushions make for one comfortable landing pad.",
                            Price = 549.99M,
                            MaxAllowed = 2,
                            Shipping = 19.99M
                        },
                        new()
                        {
                            Name = "Tartonelle Accent Chair ",
                            Description = "Fresh and sophisticated. The Tartonelle accent chair with button tufting is a classy addition to your home. Inside-out design with linen weave fabric on the seating area and taupe faux leather on the back. Large nailhead trim ties the look together. Charles of London arms and turned bun feet make this a stylish heirloom piece.",
                            Price = 319.99M,
                            MaxAllowed = 5,
                            Shipping = 9.99M
                        },
                        new()
                        {
                            Name = "Arlenbry Home Office Desk",
                            Description = "Clean-lined and contemporary—but far from stark—the Arlenbry home office desk is a fresh style awakening. Sleek scale makes it a natural fit for smaller spaces. Effortlessly combining modern farmhouse design with urban attitude, the replicated weathered oak grain plays perfectly with the chic yet earthy aesthetic.",
                            Price = 156.99M,
                            MaxAllowed = 3,
                            Shipping = 14.99M
                        },
                        new()
                        {
                            Name = "Crystal Cave Outdoor Loveseat with Table",
                            Description = "Turn your outdoor space into a high-style hideaway with the Crystal Cave 2-piece outdoor loveseat and table set. Clearly for those with an eye for fine design, this sleek, chic outdoor furniture set is quality made to withstand the elements. What looks like traditional wicker is the next best thing: resin wicker woven over rust-proof aluminum for exceptional durability and worry-free living. Acacia wood legs, a tempered glass tabletop and easy-breezy Nuvella®/olefin wrapped cushions enhance its appeal.",
                            Price = 637.49M,
                            MaxAllowed = 3,
                            Shipping = 19.99M
                        },
                        new()
                        {
                            Name = "Callie Accent Mirror",
                            Description = "Reflect your artful eye for design with the Callie accent mirror. A modern twist on the classic quatrefoil, its shapely goldtone metal frame is wonderfully unique.",
                            Price = 43.99M,
                            MaxAllowed = 10,
                            Shipping = 4.99M
                        }
                    };
                    var merchantItems = items.OrderBy(o => new Random().Next()).Take(3).ToList();
                    merchantItems.ForEach(i => i.MerchantId = merchant.Id);
                    context.AddRange(merchantItems);
                }

                await context.SaveChangesAsync();
            }

            if (!context.OrderStatusTypes.Any())
            {
                var orderStatusTypes = new List<OrderStatusType>
                {
                    new()
                    {
                        Name = "Open"
                    },
                    new()
                    {
                        Name = "Paid"
                    }
                };
                context.AddRange(orderStatusTypes);
                await context.SaveChangesAsync();
            }
        }
    }
}
