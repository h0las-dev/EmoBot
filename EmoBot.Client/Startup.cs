using EmoBot.Client.Helpers.Extensions;
using EmoBot.Client.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EmoBot.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc().AddNewtonsoftJson();

            services.AddRabbit(Configuration);
            services.AddTelegramBot(Configuration);
            services.AddCacheService(Configuration);
            services.AddGiphy(Configuration);
            services.AddConverter();

            services.AddSingleton<IUpdateService, UpdateService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ITelegramBotService telegramBotService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            telegramBotService.SetWebhookAsync().Wait();
        }
    }
}
