using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Application.Common.Behaviors;
using Application.Services;
using Application.Interfaces;
using Application.Common.Settings;
using Microsoft.Extensions.Configuration;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IPipelineBehavior<,>),typeof(ValidationBehavior<,>));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
            

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddAllRequestValidators();

            services.Configure<TaxJarSettings>(configuration.GetSection(nameof(TaxJarSettings)));
            services.AddTransient<IMerchantService, MerchantService>();
            services.AddTransient<ITaxService, TaxService>();
            services.AddTransient<ITaxJarCalculator, TaxJarCalculator>();
            services.AddHttpClient<IExternalService, ExternalService>();

            return services;
        }

        private static IServiceCollection AddAllRequestValidators(
            this IServiceCollection services)
        {
            var validatorType = typeof(IValidator<>);

            var validatorTypes = Assembly.GetExecutingAssembly()
                .GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == validatorType))
                .ToList();

            foreach (var validator in validatorTypes)
            {
                var requestType = validator.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IValidator<>))
                    .Select(i => i.GetGenericArguments()[0])
                    .First();

                var validatorInterface = validatorType
                    .MakeGenericType(requestType);

                services.AddTransient(validatorInterface, validator);
            }

            return services;
        }
    }
}
