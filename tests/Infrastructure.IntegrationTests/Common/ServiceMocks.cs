using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Application.Common.Settings;
using AutoMapper;
using Infrastructure.Context;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.IntegrationTests.Common
{
    public class ServiceMocks
    {
        protected readonly ApplicationDbContext _context;
        protected readonly IConfiguration _configuration;
        protected readonly TaxJarCalculator _taxJarCalculator;
        protected readonly ZipTaxCalculator _zipTaxCalculator;
        public ServiceMocks()
        {
            _context = GetTestDbContextAsync($"testdb-{Guid.NewGuid()}");
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .Build();
            _taxJarCalculator = new TaxJarCalculator(new HttpClient(), Options.Create(new TaxJarSettings
            {
                BaseUrl = _configuration.GetSection(nameof(TaxJarSettings))[nameof(TaxJarSettings.BaseUrl)],
                Version = _configuration.GetSection(nameof(TaxJarSettings))[nameof(TaxJarSettings.Version)],
                Token = _configuration.GetSection(nameof(TaxJarSettings))[nameof(TaxJarSettings.Token)]
            }));
            _zipTaxCalculator = new ZipTaxCalculator(new HttpClient(), Options.Create(new ZipTaxSettings
            {
                BaseUrl = _configuration.GetSection(nameof(ZipTaxSettings))[nameof(ZipTaxSettings.BaseUrl)],
                Version = _configuration.GetSection(nameof(ZipTaxSettings))[nameof(ZipTaxSettings.Version)],
                Token = _configuration.GetSection(nameof(ZipTaxSettings))[nameof(ZipTaxSettings.Token)]
            }));
        }

        private ApplicationDbContext GetTestDbContextAsync(string dbName = null)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName ?? "DBMemory")
            .Options;
            var context = new ApplicationDbContext(options);
            context.SeedDataAsync().GetAwaiter().GetResult();
            return context;

        }
    }
}
