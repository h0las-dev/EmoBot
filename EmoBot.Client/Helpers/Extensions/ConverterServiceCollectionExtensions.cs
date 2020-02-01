using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmoBot.Client.Services.Converter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmoBot.Client.Helpers.Extensions
{
    public static class ConverterServiceCollectionExtensions
    {
        public static IServiceCollection AddConverter(this IServiceCollection services)
        {
            services.AddSingleton<ITgsConverter, TgsConverter>();
            services.AddSingleton<IWebpConverter, WebpConverter>();
            services.AddSingleton<IConverterService, ConverterService>();

            return services;
        }
    }
}
