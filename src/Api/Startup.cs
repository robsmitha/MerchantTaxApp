using Application;
using Application.Common.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Api.Middleware;
using Application.Common.Settings;
using Infrastructure.Services;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddInfrastructure();
            services.AddApplication(Configuration);

            var taxCalculatorSettings = Configuration.GetSection("Configuration")?["TaxCalculatorSettings"];
            switch(taxCalculatorSettings)
            {
                case nameof(ZipTaxSettings):
                    services.Configure<ZipTaxSettings>(Configuration.GetSection(nameof(ZipTaxSettings)));
                    services.AddHttpClient<ITaxCalculator, ZipTaxCalculator>();
                    break;
                case nameof(TaxJarSettings):
                default:
                    services.Configure<TaxJarSettings>(Configuration.GetSection(nameof(TaxJarSettings)));
                    services.AddHttpClient<ITaxCalculator, TaxJarCalculator>();
                    break;
            }
            services.AddTransient<IMerchantService, MerchantService>();
            services.AddTransient<ITaxService, TaxService>();
            

            services.AddControllers();


            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("https://localhost:5001")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApp", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApp v1"));
            }

            app.UseHttpsRedirection();

            app.UseCustomExceptionHandler();

            app.UseRouting();

            app.UseCors("default");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
