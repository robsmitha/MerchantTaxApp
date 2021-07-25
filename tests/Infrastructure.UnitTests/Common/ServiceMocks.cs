using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.UnitTests.Common
{
    public class ServiceMocks
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IMapper _mapper;
        protected readonly Mock<ITaxCalculator> _taxCalculatorMock;

        public ServiceMocks()
        {
            _context = GetTestDbContextAsync($"testdb-{Guid.NewGuid()}");
            _mapper = GetMapper();
            _taxCalculatorMock = GetTaxCalculatorMock();
        }

        protected ApplicationDbContext GetTestDbContextAsync(string dbName = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName ?? "DBMemory")
            .Options;
            var context = new ApplicationDbContext(options);
            context.SeedDataAsync().GetAwaiter().GetResult();
            return context;

        }

        protected IMapper GetMapper()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            return mappingConfig.CreateMapper();
        }

        protected Mock<ITaxCalculator> GetTaxCalculatorMock()
        {
            var mock = new Mock<ITaxCalculator>();

            mock.Setup(m => m.GetTaxRateAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new TaxRateModel
                {
                    TaxRate = 0.075M
                }));

            mock.Setup(m => m.CalculateSalesTaxAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<float>(), It.IsAny<float>()))
                .Returns(Task.FromResult(new CalculateSalesTaxModel
                {
                    TaxAmount = 1.25M
                }));

            return mock;
        }
    }
}
